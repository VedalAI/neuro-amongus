from typing import Callable, List, TypeVar

from data.proto import DeadBodyData, Vector2, PositionData, TaskData, TaskType, UsableData, DoorData, VentData, VentDataConnectingVentData, OtherPlayerData

T = TypeVar("T")


def def_vector2():
    return Vector2(x=0, y=0)


def def_positiondata():
    return PositionData(total_distance=-1, next_node_position=def_vector2())


def def_taskdata():
    return TaskData(id=-1, type=TaskType.None_TaskType, consoles_of_interest=[def_positiondata() for _ in range(2)])


def def_usabledata():
    return UsableData(type=0, direction=def_vector2())


def def_deadbodydata():
    return DeadBodyData(parent_id=-1, position=def_vector2())


def def_doordata():
    return DoorData(position=def_positiondata(), is_open=False)


def def_connectingventdata():
    return VentDataConnectingVentData(id=-1, position=def_positiondata())


def def_ventdata():
    return VentData(id=-1, position=def_positiondata(), connecting_vents=[def_connectingventdata() for _ in range(3)])


def def_otherplayerdata():
    return OtherPlayerData(id=-1, last_seen_position=def_positiondata(), last_seen_time=-1, times_saw_vent=0, round_time_visible=0, game_time_visible=0, is_visible=False)


def pad_list(lst: List[T], length: int, padding_value: Callable[[], T]):
    try:
        return lst + [padding_value() if callable(padding_value) else padding_value for _ in range(len(lst), length)]
    except:
        return [padding_value() if callable(padding_value) else padding_value for _ in range(length)]
