import torch
from torch.utils.data import Dataset

from data.training_game_data import TrainingGameData


class AmongUsDataset(Dataset):
    def __init__(self, game_data: TrainingGameData, device):
        self.x = torch.tensor(game_data.get_x(), dtype=torch.float32, device=device)
        self.y = torch.tensor(game_data.get_y(), dtype=torch.float32, device=device)

    def __len__(self):
        return len(self.x)

    def __getitem__(self, idx):
        return self.x[idx], self.y[idx]
