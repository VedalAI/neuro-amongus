import os
import torch

from nn.model import LSTMModel

device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")

base_model = LSTMModel().to(device)
base_model.load_state_dict(torch.load(os.path.dirname(__file__) + "/model.pt"), strict=False)

actions_model = LSTMModel().to(device)
actions_model.load_state_dict(torch.load(os.path.dirname(__file__) + "/model_finetuned.pt"), strict=False)

# set action params of base model to action params of finetuned model
base_model.actions_fc1.weight = actions_model.actions_fc1.weight
base_model.actions_fc1.bias = actions_model.actions_fc1.bias
base_model.actions_fc2.weight = actions_model.actions_fc2.weight
base_model.actions_fc2.bias = actions_model.actions_fc2.bias
base_model.actions_fc3.weight = actions_model.actions_fc3.weight
base_model.actions_fc3.bias = actions_model.actions_fc3.bias

# save new model
torch.save(base_model.state_dict(), os.path.dirname(__file__) + "/model_final_new.pt")