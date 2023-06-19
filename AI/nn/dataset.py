import numpy as np
import torch
from torch.utils.data import Dataset
from tqdm import tqdm

from data.game import Game

class AmongUsDataset(Dataset):
    def __init__(self, games: tuple, device, finetune=False):
        self.x = np.concatenate([x[0] for x in games], dtype=np.float32)
        self.y = np.concatenate([x[1] for x in games], dtype=np.float32)

        self.x = torch.tensor(self.x, dtype=torch.float32, device=device)
        self.y = torch.tensor(self.y, dtype=torch.float32, device=device)
        
        if finetune:
            x_curated = []
            y_curated = []
            
            # only include examples where the player is killing
            for i in tqdm(range(len(self.x))):
                if self.y[i][4] != 0 or self.y[i][5] != 0 or self.y[i][6] != 0:
                    #print(i, self.y[i])
                    x_curated.append(self.x[i])
                    y_curated.append(self.y[i])
                    
            example_count = len(x_curated)
            
            for i in range(example_count * 2):
                random_index = np.random.randint(0, len(self.x))
                x_curated.append(self.x[random_index])
                y_curated.append(self.y[random_index])
            
            self.x = torch.stack(x_curated)
            self.y = torch.stack(y_curated)
            
        print("Created dataset of size: ", len(self.x))

    def __len__(self):
        return len(self.x)

    def __getitem__(self, idx):
        return self.x[idx], self.y[idx]
