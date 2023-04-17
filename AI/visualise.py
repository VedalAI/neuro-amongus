import torch
import math
from util.loader import read_all_recordings


def main():
    device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")

    game_data = read_all_recordings()
    
    x_data = game_data.get_x()
    y_data = game_data.get_y()
    
    angles = []
    
    print(x_data[300])
    print(y_data[300])
    
    for i in range(len(x_data)):
        x = x_data[i]
        path = x[-1][0:2]
        y = y_data[i]
        
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
