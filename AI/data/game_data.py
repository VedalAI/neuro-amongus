import numpy as np

from data.proto import Frame
from data.proto_defaults import def_taskdata, def_vector2, def_positiondata, pad_list
from data.converter import convert_type


class GameData:
    def __init__(self):
        self.position = def_vector2()
        self.velocity = def_vector2()
        self.tasks = [def_taskdata() for _ in range(10)]
        self.sabotage = def_taskdata()

    def update_frame(self, frame: Frame):
        if frame.local_player:
            self.position = frame.local_player.position
            self.velocity = frame.local_player.velocity

        if frame.tasks:
            self.tasks = pad_list(frame.tasks.tasks, 10, def_taskdata)
            for task in self.tasks:
                task.consoles_of_interest = pad_list(task.consoles_of_interest, 2, def_positiondata)
            self.sabotage = frame.tasks.sabotage if frame.tasks.sabotage else def_taskdata()
            self.sabotage.consoles_of_interest = pad_list(self.sabotage.consoles_of_interest, 2, def_positiondata)

    def get_x(self):
        tasks_data = [convert_type(task) for task in self.tasks]
        sabotage_data = convert_type(self.sabotage)

        return np.hstack([
            *tasks_data,
            sabotage_data
        ])

    def get_y(self):
        velocity_data = convert_type(self.velocity)

        return np.hstack([
            velocity_data
        ])
