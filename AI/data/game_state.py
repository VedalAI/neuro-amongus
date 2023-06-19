import numpy as np
import math

from data.converter import convert_type
from data.proto import Frame, SystemType, MapType, RoleType, PositionData, HeaderFrame
from data.proto_defaults import def_taskdata, def_vector2, def_positiondata, def_usabledata, pad_list, def_deadbodydata, def_doordata, def_ventdata, def_otherplayerdata, def_connectingventdata

class ShittyFrameException(Exception):
    pass


class GameState():
    def __init__(self, frame: Frame, last_game_state: "GameState" = None, header: HeaderFrame = None, check_frames: bool = True, use_last_velocity: bool = True):
        #self.data = frame
        #self.data["header = header
        
        #print(frame.local_player.did_report)
        
        if check_frames:
            if not frame.local_player:
                raise ShittyFrameException

            if frame.local_player.velocity.x == 0 and frame.local_player.velocity.y == 0 and not (frame.local_player.did_kill or frame.local_player.did_report or frame.local_player.did_vent):
                raise ShittyFrameException
            
            if frame.local_player.in_vent:
                raise ShittyFrameException
        
        self.data = convert_type(frame)
        self.header = convert_type(header)
        
        # if last_game_state:
        #     if frame.local_player.kill_cooldown > last_game_state.data["local_player"]["kill_cooldown"][0]:
        #         last_game_state.data["local_player"]["did_kill"] = [1.0]
        
        if use_last_velocity:
            self.last_velocity = last_game_state.data["local_player"]["velocity"] if last_game_state else convert_type(def_vector2())
        else:
            self.last_velocity = self.data["local_player"]["velocity"]
        
    def get_x(self):
        self.data["dead_bodies"]["dead_bodies"] = pad_list(self.data["dead_bodies"]["dead_bodies"], 3, def_deadbodydata)
        self.header["other_impostors"] = pad_list(self.header["other_impostors"], 2, -1)
        self.data["map"]["nearby_doors"] = pad_list(self.data["map"]["nearby_doors"], 3, def_doordata)
        self.data["map"]["nearby_vents"] = pad_list(self.data["map"]["nearby_vents"], 3, def_ventdata)
        self.data["other_players"]["last_seen_players"] = pad_list(self.data["other_players"]["last_seen_players"], 14, def_otherplayerdata)
        
        self.data["tasks"]["tasks"] = np.array([pad_list(t, 2, def_positiondata) for t in self.data["tasks"]["tasks"]])
        
        self.data["tasks"]["tasks"] = pad_list(self.data["tasks"]["tasks"], 10, def_taskdata)

        for vent in self.data["map"]["nearby_vents"]:
            vent["connecting_vents"] = pad_list(vent["connecting_vents"], 3, def_connectingventdata)

        self.data["tasks"]["sabotage"] = self.data["tasks"]["sabotage"] if self.data["tasks"]["sabotage"] is not None else convert_type(def_taskdata())
        #self.data["tasks"]["sabotage"] = pad_list(self.data["tasks"]["sabotage"]["consoles_of_interest"], 2, def_positiondata)
        
        self.data["tasks"]["sabotage"] = pad_list(self.data["tasks"]["sabotage"], 2, def_positiondata)
        
        other_players_data = self.data["other_players"]["last_seen_players"]
        other_players_data = [[d["is_visible"][0], d["last_seen_position"][0], d["last_seen_position"][1]] for d in other_players_data]
        
        player_position = self.data["local_player"]["position"]
        
        dead_bodies_data = self.data["dead_bodies"]["dead_bodies"]
        dead_bodies_data = [[convert_type(d["parent_id"][0] > -1)[0], d["position"][0] - player_position[0], d["position"][1] - player_position[1]] for d in dead_bodies_data]
        
        vent_data = self.data["map"]["nearby_vents"]
        vent_data = [[d["position"][0], d["position"][1]] for d in vent_data]
        
        # tasks_data = [convert_type(task) for task in self.data["tasks.tasks]
        tasks_data = self.data["tasks"]["tasks"]
        # tasks_data_unsorted = tasks_data["copy()
        # sort tasks by distance
        
        #if len(tasks_data) != 10:
        #    print(len(tasks_data))
        
        tasks_data = np.vstack(tasks_data)
        
        #print(np.array(tasks_data).shape)
        
        #for task in tasks_data:
        #    task = pad_list(task, 1, def_taskdata())
            
        if len(tasks_data[0]) != 3:
            tasks_data = [task[0] for task in tasks_data]
        
        tasks_data = [[math.inf, task[1], task[2], task[0]] if task[0] == -1 else task for task in tasks_data]
        tasks_data.sort(key=lambda x: x[0])
        tasks_data = [[task[1], task[2]] for task in tasks_data]
        
        #print(len(tasks_data))
        #print(len(self.data["tasks"]["sabotage"]))
        
        if len(self.data["tasks"]["sabotage"]) != 2:
            print(len(self.data["tasks"]["sabotage"]))
        
        x = np.hstack([
            self.header["is_impostor"],
            self.data["local_player"]["kill_cooldown"],
            *convert_type(self.last_velocity),
            *tasks_data,
            *self.data["tasks"]["sabotage"],
            *other_players_data,
            *dead_bodies_data,
            *vent_data,
            *self.data["local_player"]["raycast_obstacle_distances"]
        ])
        return x

    def get_y(self):
        velocity_data = convert_type(self.data["local_player"]["velocity"])
        
        if isinstance(velocity_data[0], list):
            velocity_data = [velocity_data[0][0], velocity_data[1][0]]
        
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
            output,
            self.data["local_player"]["did_report"],
            self.data["local_player"]["did_vent"],
            self.data["local_player"]["did_kill"]
        ])
