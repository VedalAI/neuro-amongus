import json

import numpy as np
from torch.utils.data import Dataset
import torch

def convert_type(data):
    if(type(data) == list):
        return np.hstack(tuple([convert_type(x) for x in data]))
    elif(type(data) == dict):
        return np.hstack(tuple([convert_type(x) for x in data.values()]))
    elif(type(data) == bool):
        return [float(1 if data else 0)]
    else:
        return [float(data)]
    
def get_x(frames):
    return np.array([np.hstack((convert_type(frame['IsImposter']),
                    convert_type(frame['KillCooldown']),
                    convert_type(frame['DirectionToNearestTask']),
                    convert_type(frame['IsEmergencyTask']),
                    convert_type(frame['DirectionToNearestVent']),
                    convert_type(frame['DirectionToNearestBody']),
                    convert_type(frame['CanReport']),
                    convert_type(frame['PlayerRecords']),
                    convert_type(frame['InVent']))) for frame in frames])

def get_y(frames):
    return np.array([np.hstack((convert_type(frame['Direction']),
                    convert_type(frame['Report']),
                    convert_type(frame['Vent']),
                    convert_type(frame['Kill']),
                    convert_type(frame['Sabotage']),
                    convert_type(frame['Doors']))) for frame in frames])

class AmongUsDataset(Dataset):
    def __init__(self, frames):
        processed_frames = []
        for frame in frames:
            if "PlayerRecords" in frame:
                num = len(frame["PlayerRecords"])
                # ensure there are 14 players
                while num < 14:
                    frame["PlayerRecords"][num] = {'Distance': -1, 'RelativeDirection': {'x': 0, 'y': 0}}
                    num += 1
            processed_frames.append(frame)
        
        self.x = get_x(frames)
        self.y = get_y(frames)
        
        self.x = torch.tensor(self.x, dtype=torch.float32)
        self.y = torch.tensor(self.y, dtype=torch.float32)
        
    def __len__(self):
        return len(self.x)
    
    def __getitem__(self, idx):
        return self.x[idx], self.y[idx]

FILE_PATH = "V:\\SteamLibrary\\steamapps\\common\\Among Us\\BepInEx\\plugins\\output.json"

def read_file(path):
    with open(path, "r") as file:
        data = json.load(file)
        return data