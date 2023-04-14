import os
from typing import List

from data.proto import Frame
from data.training_game_data import TrainingGameData


def deserialize_gymbag(file_path: str) -> List[Frame]:
    frames = []

    with open(file_path, "rb") as file:
        data = file.read()
        index = 0

        while index < len(data):
            length = int.from_bytes(data[index : index + 4], "little")
            index += 4

            frame = Frame()
            frame.parse(data[index : index + length])
            index += length

            frames.append(frame)

    return frames


def read_all_recordings() -> TrainingGameData:
    result = TrainingGameData()

    for file in os.listdir("recordings"):
        if file.endswith(".gymbag2"):
            file_path = os.path.join("recordings", file)
            frames = deserialize_gymbag(file_path)
            result.append_frames_new_game(frames)

    return result
