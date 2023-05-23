import os

import torch
from torch.utils.data import DataLoader

from nn.dataset import AmongUsDataset
from nn.model import LSTMModel
from util.loader import read_all_recordings

import numpy as np

def main():
    device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
    model = LSTMModel().to(device)

    # should be cross binary entropy loss for binary/bool outputs TODO: BCEWITHLOGITSLOSS
    #criterion = torch.nn.CrossEntropyLoss()
    criterion = torch.nn.CrossEntropyLoss()
    optimizer = torch.optim.Adam(model.parameters(), lr=0.001)

    games = read_all_recordings()
    # split into train and val
    train_data, val_data = games[:int(len(games) * 0.8)], games[int(len(games) * 0.8):]

    train_dataset = AmongUsDataset(train_data, device)
    val_dataset = AmongUsDataset(val_data, device)

    train_loader = DataLoader(train_dataset, batch_size=32, shuffle=True)
    val_loader = DataLoader(val_dataset, batch_size=32, shuffle=True)

    for epoch in range(50):
        # train
        model.train()
        for x, y in train_loader:
            optimizer.zero_grad()
            y_pred = model(x)
            loss = criterion(y_pred, y)
            loss.backward()
            optimizer.step()
            #print(x[0][-1][2:6])
            #print(y[0])
            output = np.array([0, 0, 0, 0])
        
            if x[0][-1][2:6][0] > 0.5:
                output[0] = 1
            elif x[0][-1][2:6][0] < -0.5:
                output[1] = 1
                
            if x[0][-1][2:6][1] > 0.5:
                output[2] = 1
            elif x[0][-1][2:6][1] < -0.5:
                output[3] = 1
            print(output)
            print(y[0])
            print(y_pred[0])
            print("")

        # validate
        total_loss = 0
        model.eval()
        for x, y in val_loader:
            y_pred = model(x)
            loss = criterion(y_pred, y)
            total_loss += loss.item()
        print(f"Epoch {epoch} loss: {total_loss / len(val_loader)}")

    torch.save(model.state_dict(), os.path.dirname(__file__) + "/model.pt")

if __name__ == "__main__":
    main()
