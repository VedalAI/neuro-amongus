import json
from os import listdir, path
from pathlib import Path
from typing import List

from betterproto import Casing

from data.proto import Frame
from data.training_game_data import TrainingGameData
from util.attr_dict import AttrDict


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
    Path(path.dirname(file_path)).mkdir(parents=True, exist_ok=True)
    with open(file_path, "w") as file:
        # Snake casing matches the dataclasses, and we need to include default values cuz otherwise they are not included and raise an error when accessing
        json.dump([f.to_dict(casing=Casing.SNAKE, include_default_values=True) for f in frames], file)


def load_decoded_gymbag(file_path: str) -> List[Frame]:
    with open(file_path, "r") as file:
        # we don't convert the loaded dictionaries to actual Frame objects to improve load time
        # instead we wrap it in an AttrDict which allows dict.attribute access by forwarding it to dict["attribute"]
        return [AttrDict(f) for f in json.load(file)]  # type: ignore


def read_all_recordings() -> TrainingGameData:
    print("Loading recordings from disk... (this may take a bit)")

    result = TrainingGameData()

    base_path = Path(path.join(path.dirname(__file__), "..")).resolve()
    recordings_path = path.join(base_path, "recordings")

    for file in listdir(recordings_path):
        if file.endswith(".gymbag2"):
            file_path = path.join(recordings_path, file)

            decoded_file_path = path.join(recordings_path, "decoded", path.splitext(file)[0] + ".frames.json")
            if path.exists(decoded_file_path):
                print(f"Loading: {path.relpath(decoded_file_path, base_path)}")
                frames = load_decoded_gymbag(decoded_file_path)
            else:
                print(f"Parsing: {path.relpath(file_path, base_path)}")
                frames = deserialize_gymbag(file_path)
                print(f"Saving: {path.relpath(decoded_file_path, base_path)}")
                save_decoded_gymbag(decoded_file_path, frames)

            print(f"Updating game data")
            result.append_frames_new_game(frames)

    print("Done loading recordings")

    return result
