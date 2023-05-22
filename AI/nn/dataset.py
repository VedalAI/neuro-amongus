import numpy as np
import torch
from torch.utils.data import Dataset

from data.game import Game

class AmongUsDataset(Dataset):
    def __init__(self, games: list[Game], device):
        self.x = np.array([])
        self.y = np.array([])
        for game in games:
            self.x = np.hstack((self.x, game.get_x()))
            self.y = np.hstack((self.y, game.get_y()))

        self.x = torch.tensor(self.x, dtype=torch.float32, device=device)
        self.y = torch.tensor(self.y, dtype=torch.float32, device=device)

    def __len__(self):
        return len(self.x)

    def __getitem__(self, idx):
        return self.x[idx], self.y[idx]
