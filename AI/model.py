import torch

class Model(torch.nn.Module):
    def __init__(self):
        super(Model, self).__init__()
        # INPUTS
        # imposter 1 
        # kill cooldown 1
        # direction to next task 2
        # direction to nearest vent 2
        # direction to nearest body 2
        # can report 1 
        # 14 players 42
        # is emergency task 1
        # in vent 1
        
        # OUTPUTS
        # move direction 2
        # report 1
        # vent 1
        # kill 1
        # sabotage 1
        # doors 1
        
        self.fc1 = torch.nn.Linear(53, 128)
        self.fc2 = torch.nn.Linear(128, 7)

    def forward(self, x):
        x = torch.relu(self.fc1(x))
        x = self.fc2(x)
        return x