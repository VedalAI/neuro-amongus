from copy import deepcopy

from data.proto import MyVector2, PositionData, TaskData, TaskDataTaskType


def def_vector2():
    return MyVector2(x=0, y=0)


def def_positiondata():
    return PositionData(total_distance=-1, next_node_position=def_vector2())


def def_taskdata():
    return TaskData(id=-1, type=TaskDataTaskType.Unset, consoles_of_interest=[def_positiondata() for _ in range(3)])


def pad_list(lst, length, padding_value):
    return lst + [deepcopy(padding_value())] * (length - len(lst))
