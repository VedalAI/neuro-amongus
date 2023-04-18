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
        # return np.vstack([state.get_x() for state in self.states])
        # updated to return sequences of states of length 10
        return np.array([np.vstack([self.states[i + j].get_x() for j in range(10)]) for i in range(len(self.states) - 10)])

    def get_y(self):
        # return np.vstack([state.get_y() for state in self.states])
        # updated to the corresponding y value
        return np.array([self.states[i + 10].get_y() for i in range(len(self.states) - 10)])
