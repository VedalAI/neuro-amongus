import numpy as np

from data.proto import TaskData, PositionData, MyVector2


def convert_bool(data):
    return [1.0 if data else 0.0]


def convert_dict(data):
    return np.hstack([convert_type(x) for x in data.values()]) if data else np.array([])


def convert_list(data):
    return np.hstack([convert_type(x) for x in data]) if data else np.array([])


def convert_positiondata(data):
    return np.hstack([float(data.total_distance), convert_type(data.next_node_position)])


def convert_taskdata(data):
    return convert_type(data.consoles_of_interest)


def convert_vector2(data):
    return [float(data.x), float(data.y)]


def convert_type(data):
    type_mapping = {
        bool: convert_bool,
        list: convert_list,
        dict: convert_dict,
        PositionData: convert_positiondata,
        TaskData: convert_taskdata,
        MyVector2: convert_vector2,
    }

    for cls, func in type_mapping.items():
        if isinstance(data, cls):
            return func(data)

    return [float(data)]
