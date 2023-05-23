import numpy as np
import torch
from torch.utils.data import Dataset

from data.game import Game

class AmongUsDataset(Dataset):
    def __init__(self, games: tuple, device):
        self.x = np.concatenate([x[0] for x in games], dtype=np.float32)
        self.y = np.concatenate([x[1] for x in games], dtype=np.float32)

        self.x = torch.tensor(self.x, dtype=torch.float32, device=device)
        self.y = torch.tensor(self.y, dtype=torch.float32, device=device)

    def __len__(self):
        return len(self.x)

    def __getitem__(self, idx):
        return self.x[idx], self.y[idx]
