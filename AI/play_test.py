import os
import socket

import matplotlib.pyplot as plt
import numpy as np
import torch

from data.game import WINDOW_LENGTH
from data.game_state import GameState
from data.proto import Frame, NnOutput, Vector2
from nn.model import LSTMModel


def main():
    device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
    model = LSTMModel().to(device)
    model.load_state_dict(torch.load(os.path.dirname(__file__) + "/model_final_new.pt"))
    model.eval()

    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(("localhost", 6969))

    kill_history = []
    report_history = []
    vent_history = []

    while True:
        # try:
        server.listen(1)
        print("Waiting for connection...")
        conn, addr = server.accept()
        with conn:
            print("Connected by", addr)

            last_game_state = None
            last_velocity = [0, 0]
            header = None
            hidden = None
            x_history = []
            while True:
                data = conn.recv(2048)  # TODO: fix error when disconnecting
                if not data:
                    print("no data")
                    break

                frame = Frame().parse(data)
                if frame.header is not None:
                    header = frame.header

                if last_game_state is not None:
                    last_game_state.data["local_player"]["velocity"] = last_velocity
                state = GameState(frame, last_game_state, header, check_frames=False)
                last_game_state = state

                x = state.get_x()

                is_imposter = x[0] == 1

                # print(x)

                x_history.append(x)
                # max length of 10
                if len(x_history) > WINDOW_LENGTH:
                    x_history.pop(0)

                # pad with zeros if not enough data
                while len(x_history) < WINDOW_LENGTH:
                    x_history.insert(0, np.zeros_like(x_history[0]))

                # print(x_history)
                # print(np.array([x_history]).shape)

                x_history_tensor = torch.tensor(np.array([x_history]), dtype=torch.float32, device=device)
                y, hidden = model(x_history_tensor, hidden)
                y = y.detach().cpu().numpy()[0]
                y = [float(o) for o in y]

                print(y)

                threshold = min(0.5, np.max(y[0:3]) - 0.01)

                print(threshold)

                new_y = [0, 0]
                if y[0] > threshold:
                    new_y[0] += 1
                if y[1] > threshold:
                    new_y[0] -= 1
                if y[2] > threshold:
                    new_y[1] += 1
                if y[3] > threshold:
                    new_y[1] -= 1

                output = NnOutput()
                output.desired_move_direction = Vector2(x=new_y[0], y=new_y[1])
                output.report = y[4] > 0.5
                output.vent = y[5] > 0.5
                output.kill = y[6] > 0.5 and is_imposter

                kill_history.append(y[6])
                # max length of 20
                if len(kill_history) > 20:
                    kill_history.pop(0)

                report_history.append(y[4])
                # max length of 20
                if len(report_history) > 20:
                    report_history.pop(0)

                vent_history.append(y[5])
                # max length of 20
                if len(vent_history) > 20:
                    vent_history.pop(0)

                # plot
                plt.clf()
                # set axis limits
                plt.ylim(0, 1)
                if is_imposter:
                    plt.plot(kill_history, label="kill", color="red")
                plt.plot(report_history, label="report", color="blue")
                plt.plot(vent_history, label="vent", color="green")

                # normalize
                if new_y[0] != 0 and new_y[1] != 0:
                    new_y[0] /= np.sqrt(2)
                    new_y[1] /= np.sqrt(2)
                last_velocity = new_y

                print(new_y)

                conn.sendall(bytes(output))

                plt.pause(0.01)
                #sleep(0.05) # todo: fix
        # except Exception as e:
        #     print(e)


if __name__ == "__main__":
    main()
