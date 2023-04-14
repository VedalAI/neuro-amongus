from model import Model
from dataset import get_x
import torch
import socket
import json
import os

device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")

model = Model().to(device)
model.load_state_dict(torch.load(os.path.dirname(__file__) + "/model.pt"))

model.eval()

# open server on port 6969
server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind(("localhost", 6969))

while True:
    server.listen(1)
    print("Waiting for connection...")
    conn, addr = server.accept()
    with conn:
        print("Connected by", addr)
        while True:
            data = conn.recv(1024)
            if not data:
                break
            frame = json.loads(data.decode())
            if "PlayerRecords" in frame:
                num = len(frame["PlayerRecords"])
                # ensure there are 14 players
                while num < 14:
                    frame["PlayerRecords"][num] = {'Distance': -1, 'RelativeDirection': {'x': 0, 'y': 0}}
                    num += 1
            x = get_x([frame])
            x = torch.tensor(x, dtype=torch.float32, device=device)
            output = model(x).detach().cpu().numpy()[0]
            output = [float(o) for o in output]
            response = { "Direction": {"x": output[0], "y": output[1]},
                         "Report": output[2] > 0.5,
                         "Vent": output[3] > 0.5,
                         "Kill": output[4] > 0.5,
                         "Sabotage": output[5] > 0.5,
                         "Doors": output[6] > 0.5
            }
            print(response)
            conn.sendall(json.dumps(response).encode())
