import torch
import torch.autograd as autograd


class Model(torch.nn.Module):
    def __init__(self):
        super(Model, self).__init__()
        # INPUTS (66)
        # tasks: 10x 2 positiondatas = 10x 2x 3 floats = 60
        # sabotage: 2 positiondatas = 1x 2x 3 floats = 6

        # OUTPUTS (2)
        # velocity: 2 floats = 2

        self.fc1 = torch.nn.Linear(58 * 10, 64)

        self.fc2 = torch.nn.Linear(64, 7)

    def forward(self, x):
        # input is batch x 10 x 4 so we need to flatten it
        # x = x[:, -1, :]
        
        x = x.reshape(x.shape[0], -1)

        x = torch.relu(self.fc1(x))

        x = torch.sigmoid(self.fc2(x))

        return x


class LSTMModel(torch.nn.Module):
    def __init__(self, movement_outputs=4):
        super(LSTMModel, self).__init__()
        # INPUTS (66)
        # imposter 1
        # kill cooldown 1
        # velocity 2
        # tasks: 10x 2x 2 positiondatas = 40
        # sabotage: 2 positiondatas = 1x 2x 3 floats = 6
        # raycast_obstacle_distances: 8 floats = 8

        # OUTPUTS (2)
        # velocity: 2 floats = 2

        self.hidden_dim = 128
        self.layers = 2

        # movement
        self.fc1 = torch.nn.Linear(115, self.hidden_dim)
        self.lstm = torch.nn.LSTM(self.hidden_dim, self.hidden_dim, self.layers)
        self.fc2 = torch.nn.Linear(self.hidden_dim, int(self.hidden_dim / 2))
        self.fc3 = torch.nn.Linear(int(self.hidden_dim / 2), movement_outputs)
        
        self.movement_outputs = movement_outputs
        
        # actions
        self.actions_fc1 = torch.nn.Linear(2 + 6 + (3 * 14) + (3 * 3) + (3 * 2) + 8, self.hidden_dim)
        self.actions_fc2 = torch.nn.Linear(self.hidden_dim, int(self.hidden_dim / 2))
        self.actions_fc3 = torch.nn.Linear(int(self.hidden_dim / 2), 3)

    def forward(self, x, hidden=None):
        if hidden is None:
            hidden = self.init_hidden(x.shape[1], x.device)

        # movement
        movement_x = torch.relu(self.fc1(x))
        movement_x, hidden = self.lstm(movement_x, hidden)
        movement_x = movement_x[:, -1, :]
        movement_x = torch.relu(self.fc2(movement_x))
        movement_x = torch.nn.Dropout(p=0.25)(movement_x)
        movement_x = torch.sigmoid(self.fc3(movement_x))
        
        if self.movement_outputs != 4:
            movement_x = movement_x[:, :4]
        
        # actions
        last_frame_x = x[:, -1, :]
        actions_x = torch.cat((
            last_frame_x[:, 0:2],
            last_frame_x[:, 44:],
        ), 1)
            
        actions_x = torch.relu(self.actions_fc1(actions_x))
        actions_x = torch.relu(self.actions_fc2(actions_x))
        actions_x = torch.nn.Dropout(p=0.25)(actions_x)
        actions_x = torch.sigmoid(self.actions_fc3(actions_x))
        
        y = torch.cat((movement_x[:, :4], actions_x), 1)
        
        # y = movement_x

        return y, hidden

    def init_hidden(self, batch_size, device):
        return (autograd.Variable(torch.randn(self.layers, batch_size, self.hidden_dim, device=device)),
                autograd.Variable(torch.randn(self.layers, batch_size, self.hidden_dim, device=device)))
