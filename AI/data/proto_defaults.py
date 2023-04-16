from copy import deepcopy
from typing import Callable, List, TypeVar

from data.proto import Vector2, PositionData, TaskData, TaskType

T = TypeVar("T")


def def_vector2():
    return Vector2(x=0, y=0)


def def_positiondata():
    return PositionData(total_distance=-1, next_node_position=def_vector2())


def def_taskdata():
    return TaskData(id=-1, type=TaskType.None_TaskType, consoles_of_interest=[def_positiondata() for _ in range(3)])


def pad_list(lst: List[T], length: int, padding_value: Callable[[], T]):
    return lst + [deepcopy(padding_value())] * (length - len(lst))
