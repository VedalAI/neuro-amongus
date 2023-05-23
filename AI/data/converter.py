import numpy as np

from data.proto import TaskData, PositionData, Vector2, MapType, RoleType, SystemType, TaskType

import betterproto

def convert_bool(data: bool):
    return [1.0 if data else 0.0]


def convert_dict(data: dict):
    return np.array([convert_type(x) for x in data.values()]) if data else np.array([])


def convert_list(data: list):
    return [convert_type(x) for x in data] if data else np.array([])


# This is needed because decoded frames store enum values as string instead of int
def convert_str(data: str):
    # Check enums MapType, RoleType, SystemType, TaskType for attributes called "data" and return their value
    for cls in [MapType, RoleType, SystemType, TaskType]:
        if hasattr(cls, data):
            return [float(getattr(cls, data).value)]
    raise ValueError(f"Unknown enum value: {data}")


def convert_positiondata(data: PositionData):
    return np.hstack([float(data.total_distance), convert_type(data.next_node_offset)])


def convert_taskdata(data: TaskData):
    return convert_type(data.consoles_of_interest[:2])


def convert_vector2(data: Vector2):
    return [float(data.x), float(data.y)]


def convert_message(data: betterproto.Message):
    #if hasattr(data, "_group_current"):
    #    del data._group_current
    #if hasattr(data, "_unknown_fields"):
    #    del data._unknown_fields
        
    data_dict = {}
    
    #print(data.__repr__())
    
    for field_name in data._betterproto.sorted_field_names:
        #print(field_name)
        data_dict[field_name] = convert_type(data.__getattribute__(field_name), extra_info=field_name)

    return data_dict


def convert_enum(data: betterproto.Enum):
    return int(data)


def convert_type(data, extra_info=None):
    type_mapping = {
        bool: convert_bool,
        dict: convert_dict,
        list: convert_list,
        str: convert_str,
        PositionData: convert_positiondata,
        TaskData: convert_taskdata,
        Vector2: convert_vector2,
        betterproto.Message: convert_message,
        betterproto.Enum: convert_enum,
        int: lambda x: [x],
        float: lambda x: [x],
        np.ndarray: lambda x: convert_list(x.tolist()),
    }
    
    #print(extra_info)

    for cls, func in type_mapping.items():
        if isinstance(data, cls):
            return func(data)
        
    if data is None:
        return None
        
    #print(f"Unknown type: {type(data)}, extra info {extra_info}")

    return data
