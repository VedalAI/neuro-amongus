from util.loader import *

def load_game(file) -> Game:
    if str(file).endswith(".gymbag2"):
        file_path = path.join(RECORDINGS_PATH, file)
        
        # load the game, calculate number of possible frame sequence pairings
        # return decoded file path, number of possible frame sequence pairings
        # in the dataset we can figure out how to load it
        
        decoded_file_path = path.join(RECORDINGS_PATH, "decoded", path.splitext(file)[0] + ".pickle")
        if path.exists(decoded_file_path):
            print(f"Loading: {path.relpath(decoded_file_path, BASE_PATH)}")
            game = load_processed_game(decoded_file_path)
        else:
            print(f"Parsing: {path.relpath(file_path, BASE_PATH)}")
            frames = deserialize_gymbag(file_path)
            if len(frames) == 0:
                return None
            print(f"Updating game data")
            game = Game(frames)
        
        if game is None:
            return None
            
        print("Converting to neural network format...")
        data = (game.get_x(), game.get_y())

        return data
    else:
        return None

result = listdir(RECORDINGS_PATH)
    
result = list(map(load_game, result))