from copy import deepcopy

import numpy as np

from data.game_state import GameState
from data.proto import Frame


class Game:
    def __init__(self, frames: list[Frame] = None):
        self.states = []

        game_state = GameState()
        for frame in frames:
            game_state.update_frame(frame)
            self.states.append(game_state)
            game_state = deepcopy(game_state)

    def get_x(self, shuffle=False):
        # return np.vstack([state.get_x() for state in self.states])
        # updated to return sequences of states of length 10
        return np.array([np.vstack([self.states[i + j].get_x() for j in range(10)]) for i in range(len(self.states) - 10)])

    def get_y(self, shuffle=False):
        # return np.vstack([state.get_y() for state in self.states])
        # updated to the corresponding y value
        return np.array([self.states[i + 10].get_y() for i in range(len(self.states) - 10)])
