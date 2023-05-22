import numpy as np
import math

from data.converter import convert_type
from data.proto import Frame, SystemType, MapType, RoleType, PositionData, HeaderFrame
from data.proto_defaults import def_taskdata, def_vector2, def_positiondata, def_usabledata, pad_list, def_deadbodydata, def_doordata, def_ventdata, def_otherplayerdata, def_connectingventdata

class ShittyFrameException(Exception):
    pass


class GameState():
    def __init__(self, frame: Frame, last_game_state: "GameState" = None, header: HeaderFrame = None):
        self.frame = frame
        self.frame.header = header

        if not self.frame.local_player:
            raise ShittyFrameException

        if self.frame.local_player.velocity.x == 0 and self.frame.local_player.velocity.y == 0 and not (self.frame.local_player.did_kill or self.frame.local_player.did_report or self.frame.local_player.did_vent): # TODO not killing/sabotaging
            raise ShittyFrameException
        
        if self.frame.local_player.in_vent:
            raise ShittyFrameException

        self.last_velocity = last_game_state.frame.local_player.velocity if last_game_state else def_vector2()
    
        #self.data = convert_type(self.frame)
    
    # def __getstate__(self):
    #     for frame in self.frame:
    #         frame.to_dict(c)
    
    
    def get_x(self):
        self.frame.dead_bodies.dead_bodies = pad_list(self.frame.dead_bodies.dead_bodies, 3, def_deadbodydata)
        self.frame.header.other_impostors = pad_list(self.frame.header.other_impostors, 2, -1)
        self.frame.map.nearby_doors = pad_list(self.frame.map.nearby_doors, 3, def_doordata)
        self.frame.map.nearby_vents = pad_list(self.frame.map.nearby_vents, 3, def_ventdata)
        self.frame.other_players.last_seen_players = pad_list(self.frame.other_players.last_seen_players, 14, def_otherplayerdata)
        self.frame.tasks.tasks = pad_list(self.frame.tasks.tasks, 10, def_taskdata)

        for vent in self.frame.map.nearby_vents:
            vent.connecting_vents = pad_list(vent.connecting_vents, 3, def_connectingventdata)

        for task in self.frame.tasks.tasks:
            task.consoles_of_interest = pad_list(task.consoles_of_interest, 2, def_positiondata)
        
        self.frame.tasks.sabotage = self.frame.tasks.sabotage if self.frame.tasks.sabotage else def_taskdata()
        self.frame.tasks.sabotage.consoles_of_interest = pad_list(self.frame.tasks.sabotage.consoles_of_interest, 2, def_positiondata)
        
        tasks_data = [convert_type(task) for task in self.frame.tasks.tasks]
        # tasks_data_unsorted = tasks_data.copy()
        # sort tasks by distance
        tasks_data = [[math.inf, task[1], task[2], task[0]] if task[0] == -1 else task for task in tasks_data]
        tasks_data.sort(key=lambda x: x[0])
        tasks_data = [[task[1], task[2]] for task in tasks_data]
        sabotage_data = convert_type(self.frame.tasks.sabotage)
        
        velocity_data = convert_type(self.last_velocity)
        
        raycast_data = convert_type(self.frame.local_player.raycast_obstacle_distances)
        
        imposter = convert_type(1 if self.frame.header.role == RoleType.Impostor or self.frame.header.role == RoleType.Shapeshifter else 0)
        print(self.frame.local_player)
        
        kill_cooldown = convert_type(self.frame.local_player.kill_cooldown)
        
        return np.hstack([
            imposter,
            kill_cooldown,
            velocity_data,
            *tasks_data,
            sabotage_data,
            *raycast_data
        ])

    def get_y(self):
        velocity_data = convert_type(self.frame.local_player.velocity)
        
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
            
        report = convert_type(self.frame.local_player.did_report)
        vent = convert_type(self.frame.local_player.did_vent)
        kill = convert_type(self.frame.local_player.did_kill)

        return np.hstack([
            output,
            report,
            vent,
            kill
        ])
