import torch


class Model(torch.nn.Module):
    def __init__(self):
        super(Model, self).__init__()
        # INPUTS (101)
        # position: 2 floats = 2
        # tasks: 10x 3 positiondatas = 10x 3x 3 floats = 90
        # sabotage: 3 positiondatas = 1x 3x 3 floats = 9

        # OUTPUTS (2)
        # velocity: 2 floats = 2

        self.fc1 = torch.nn.Linear(101, 256)
        self.fc2 = torch.nn.Linear(256, 2)

    def forward(self, x):
        x = torch.relu(self.fc1(x))
        x = self.fc2(x)
        return x
