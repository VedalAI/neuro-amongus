import os
from model import Model
from dataset import AmongUsDataset, read_file, FILE_PATH
import torch
from torch.utils.data import DataLoader
from random import shuffle
    
model = Model()

# should be cross binary entropy loss for binary/bool outputs TODO: BCEWITHLOGITSLOSS
criterion = torch.nn.MSELoss()
optimizer = torch.optim.Adam(model.parameters(), lr=0.001)

frames = read_file(FILE_PATH)

# split train and validation
shuffle(frames)
train_frames = frames[:int(len(frames) * 0.8)]
val_frames = frames[int(len(frames) * 0.8):]

train_dataset = AmongUsDataset(train_frames)
val_dataset = AmongUsDataset(val_frames)

train_loader = DataLoader(train_dataset, batch_size=32, shuffle=True)
val_loader = DataLoader(val_dataset, batch_size=32, shuffle=True)

model.train()

for epoch in range(50):
    # train
    model.train()
    for x, y in train_loader:
        optimizer.zero_grad()
        y_pred = model(x)
        loss = criterion(y_pred, y)
        loss.backward()
        optimizer.step()
    
    # validate
    model.eval()
    for x, y in val_loader:
        y_pred = model(x)
        loss = criterion(y_pred, y)
        print(loss)
        
torch.save(model.state_dict(), os.path.dirname(__file__) + "/model.pt")
