import socket

import torch

from data.game_data import GameData
from data.proto import Frame, NnOutput, Vector2
from nn.model import Model


def main():
    device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
    model = Model().to(device)
    model.load_state_dict(torch.load("model.pt"))
    model.eval()

    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(("localhost", 6969))

    while True:
        server.listen(1)
        print("Waiting for connection...")
        conn, addr = server.accept()
        with conn:
            print("Connected by", addr)

            game_data = GameData()
            while True:
                data = conn.recv(1024)
                if not data:
                    print("no data")
                    break

                frame = Frame.FromString(data)
                game_data.update_frame(frame)

                x = game_data.get_x()
                x = torch.tensor(x, dtype=torch.float32, device=device)
                output = model(x).detach().cpu().numpy()
                output = [float(o) for o in output]

                result = NnOutput()
                result.desired_move_direction = Vector2(x=output[0], y=output[1])

                conn.sendall(bytes(result))


if __name__ == "__main__":
    main()
