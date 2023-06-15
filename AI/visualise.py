import torch
import math
from util.loader import read_all_recordings


def main():
    device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")

    game_state = read_all_recordings()
    
    x_data = game_state.get_x()
    y_data = game_state.get_y()
    
    angles = []
    
    print(x_data[5])
    print(y_data[5])
    
    for i in range(len(x_data)):
        x = x_data[i]
        path = x[-1][0:2]
        y = y_data[i]
        
        # y contains 4 values, up down left right
        # convert them to a vector2 where eg. up + right would be (0.701, 0.701)
        new_y = [0, 0]
        if y[0] == 1:
            new_y[0] += 1
        if y[1] == 1:
            new_y[0] -= 1
        if y[2] == 1:
            new_y[1] += 1
        if y[3] == 1:
            new_y[1] -= 1
        
        # normalize
        length = math.sqrt(new_y[0] * new_y[0] + new_y[1] * new_y[1])
        if length > 0:
            new_y[0] /= length
            new_y[1] /= length
        
        # get angle between path and y
        dot_product = path[0] * y[0] + path[1] * y[1]
        path_length = (path[0] ** 2 + path[1] ** 2) ** 0.5
        y_length = (y[0] ** 2 + y[1] ** 2) ** 0.5
        angle = math.acos(dot_product / (path_length * y_length))
        
        angles.append(angle)
        
    # print percentage of angles less than 90 degrees
    print(sum([1 for angle in angles if angle < math.pi / 2]) / len(angles))
    
    # print mean angle in degrees
    print(sum(angles) / len(angles) * 180 / math.pi)

    # print median angle in degrees
    angles.sort()
    print(angles[len(angles) // 2] * 180 / math.pi)
        
    

if __name__ == "__main__":
    main()
