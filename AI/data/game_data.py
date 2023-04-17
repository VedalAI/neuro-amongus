import numpy as np
import math

from data.converter import convert_type
from data.proto import Frame
from data.proto_defaults import def_taskdata, def_vector2, def_positiondata, pad_list


class GameData:
    def __init__(self):
        self.velocity = def_vector2()
        self.tasks = [def_taskdata() for _ in range(10)]
        self.sabotage = def_taskdata()
        self.last_velocity = def_vector2()

    def update_frame(self, frame: Frame):
        # if frame.local_player.velocity.x == 0 and frame.local_player.velocity.y == 0:
        #     #print("test ignore")
        #     return
        
        if frame.local_player:
            if self.velocity:
                self.last_velocity = self.velocity
            self.velocity = frame.local_player.velocity

        if frame.tasks:
            self.tasks = pad_list(frame.tasks.tasks, 10, def_taskdata)
            for task in self.tasks:
                task.consoles_of_interest = pad_list(task.consoles_of_interest, 2, def_positiondata)
            self.sabotage = frame.tasks.sabotage if frame.tasks.sabotage else def_taskdata()
            self.sabotage.consoles_of_interest = pad_list(self.sabotage.consoles_of_interest, 2, def_positiondata)

    def get_x(self):
        tasks_data = [convert_type(task) for task in self.tasks]
        tasks_data_unsorted = tasks_data.copy()
        # sort tasks by distance
        tasks_data = [[math.inf, task[1], task[2], task[0]] if task[0] == -1 else task for task in tasks_data]
        tasks_data.sort(key=lambda x: x[0])
        tasks_data = [[task[1], task[2]] for task in tasks_data]
        sabotage_data = convert_type(self.sabotage)
        
        velocity_data = convert_type(self.last_velocity)
        
        return np.hstack([
            velocity_data,
            *tasks_data,
            sabotage_data
        ])

    def get_y(self):
        velocity_data = convert_type(self.velocity)
        
        # up, down, left, right
        output = np.array([0, 0, 0, 0])
        
        if velocity_data[0] > 0.5:
            output[0] = 1
        elif velocity_data[0] < -0.5:
            output[1] = 1
            
        if velocity_data[1] > 0.5:
            output[2] = 1
        elif velocity_data[1] < -0.5:
            output[3] = 1

        return np.hstack([
            output
        ])
