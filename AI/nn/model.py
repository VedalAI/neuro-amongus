import torch


class Model(torch.nn.Module):
    def __init__(self):
        super(Model, self).__init__()
        # INPUTS (66)
        # tasks: 10x 2 positiondatas = 10x 2x 3 floats = 60
        # sabotage: 2 positiondatas = 1x 2x 3 floats = 6

        # OUTPUTS (2)
        # velocity: 2 floats = 2

        self.fc1 = torch.nn.Linear(66, 128)
        self.fc2 = torch.nn.Linear(128, 2)

    def forward(self, x):
        x = torch.relu(self.fc1(x))
        x = self.fc2(x)
        return x
