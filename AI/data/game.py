from copy import deepcopy

import numpy as np

from data.game_state import GameState
from data.proto import Frame, PositionData
from data.game_state import ShittyFrameException

WINDOW_LENGTH = 50

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
        # updated to return sequences of states of length WINDOW_LENGTH
        state_xs = [state.get_x() for state in self.states]
        # for i in range(len(self.states)):
        #     if self.states[i].data["local_player"]["did_report"][0] == 1:
        #         print(self.states[i - 1])
        #         print(self.states[i])
        #         print(self.states[i + 1])
        #         print("report", i)
        #     elif self.states[i].data["local_player"]["did_kill"][0] == 1:
        #         print(self.states[i - 1])
        #         print(self.states[i])
        #         print(self.states[i + 1])
        #         print("kill", i)
                
        return np.array([np.vstack([state_xs[i + j] for j in range(WINDOW_LENGTH)]) for i in range(len(self.states) - WINDOW_LENGTH)])

    def get_y(self, shuffle=False):
        # return np.vstack([state.get_y() for state in self.states])
        # updated to the corresponding y value
        state_ys = [state.get_y() for state in self.states]
        return np.array([state_ys[i + WINDOW_LENGTH] for i in range(len(self.states) - WINDOW_LENGTH)])
