from copy import deepcopy

import numpy as np

from data.game_data import GameData
from data.proto import Frame


class TrainingGameData:
    def __init__(self, states: list[GameData] = None):
        self.states: list[GameData] = states if states else []

    def append_frames_new_game(self, frames: list[Frame]):
        game_data = GameData()
        for frame in frames:
            game_data.update_frame(frame)
            self.states.append(game_data)
            game_data = deepcopy(game_data)

    def shuffle(self):
        np.random.shuffle(self.states)

    def split(self, ratio: float):
        split_index = int(len(self.states) * ratio)
        return TrainingGameData(self.states[:split_index]), TrainingGameData(self.states[split_index:])

    def get_x(self):
        return np.vstack([state.get_x() for state in self.states])

    def get_y(self):
        return np.vstack([state.get_y() for state in self.states])
