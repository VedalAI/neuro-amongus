from copy import deepcopy

import numpy as np

from data.game_state import GameState
from data.proto import Frame, PositionData
from data.game_state import ShittyFrameException

class Game:
    def __init__(self, frames: list[Frame] = None):
        self.states: list[GameState] = []
        
        state = None
        header = frames[0].header
        for frame in frames:
            try:
                state = GameState(frame, state, header)
                self.states.append(state)
            except ShittyFrameException:
                # print("Shitty frame, skipping")
                continue  

    def get_x(self, shuffle=False):
        # return np.vstack([state.get_x() for state in self.states])
        # updated to return sequences of states of length 10
        return np.array([np.vstack([self.states[i + j].get_x() for j in range(10)]) for i in range(len(self.states) - 10)])

    def get_y(self, shuffle=False):
        # return np.vstack([state.get_y() for state in self.states])
        # updated to the corresponding y value
        return np.array([self.states[i + 10].get_y() for i in range(len(self.states) - 10)])
