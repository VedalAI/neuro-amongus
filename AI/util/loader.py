import json
import os
from pathlib import Path
from typing import List

from data.proto import Frame
from data.training_game_data import TrainingGameData


def deserialize_gymbag(file_path: str) -> List[Frame]:
    frames = []

    with open(file_path, "rb") as file:
        data = file.read()
        index = 0

        while index < len(data):
            length = int.from_bytes(data[index:index+4], "little")
            index += 4

            frame = Frame()
            frame.parse(data[index:index+length])
            index += length

            frames.append(frame)

    return frames


def save_decoded_gymbag(file_path: str, frames: List[Frame]):
    Path(os.path.dirname(file_path)).mkdir(parents=True, exist_ok=True)
    with open(file_path, "w") as file:
        json.dump([f.to_dict() for f in frames], file)


def load_decoded_gymbag(file_path: str) -> List[Frame]:
    with open(file_path, "r") as file:
        return [Frame().from_dict(f) for f in json.load(file)]


def read_all_recordings() -> TrainingGameData:
    result = TrainingGameData()

    for file in os.listdir("recordings"):
        if file.endswith(".gymbag2"):
            file_path = os.path.join("recordings", file)

            decoded_file_path = os.path.join("recordings", "decoded", file + ".json")
            if os.path.exists(decoded_file_path):
                print(f"Loading: {decoded_file_path}")
                frames = load_decoded_gymbag(decoded_file_path)
            else:
                print(f"Parsing: {file_path}")
                frames = deserialize_gymbag(file_path)
                print(f"Saving: {decoded_file_path}")
                save_decoded_gymbag(decoded_file_path, frames)

            print(f"Updating states")
            result.append_frames_new_game(frames)

    return result
