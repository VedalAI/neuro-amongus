import numpy as np

from data.proto import TaskData, PositionData, Vector2, MapType, RoleType, SystemType, TaskType


def convert_bool(data):
    return [1.0 if data else 0.0]


def convert_dict(data):
    return np.hstack([convert_type(x) for x in data.values()]) if data else np.array([])


def convert_list(data):
    return np.hstack([convert_type(x) for x in data]) if data else np.array([])


# This is needed because decoded frames store enum values as string instead of int
def convert_str(data):
    # Check enums MapType, RoleType, SystemType, TaskType for attributes called "data" and return their value
    for cls in [MapType, RoleType, SystemType, TaskType]:
        if hasattr(cls, data):
            return [float(getattr(cls, data).value)]
    raise ValueError(f"Unknown enum value: {data}")


def convert_positiondata(data):
    return np.hstack([float(data.total_distance), convert_type(data.next_node_offset)])


def convert_taskdata(data):
    return convert_type(data.consoles_of_interest[:2])


def convert_vector2(data):
    return [float(data.x), float(data.y)]


def convert_type(data):
    type_mapping = {
        bool: convert_bool,
        dict: convert_dict,
        list: convert_list,
        str: convert_str,
        PositionData: convert_positiondata,
        TaskData: convert_taskdata,
        Vector2: convert_vector2,
    }

    for cls, func in type_mapping.items():
        if isinstance(data, cls):
            return func(data)

    return [float(data)]
