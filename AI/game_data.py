from proto import Frame, HeaderFrame, LocalPlayerFrame, MapFrame, DeadBodiesFrame, OtherPlayersFrame, TasksFrame, TaskData


# So currently this class just stores the frames themselves, but in theory
# we might want to grab some data from them and store it separately if we
# need to implement any kind of memory.
class GameData:
    def __init__(self):
        self.tasks = [TaskData() for _ in range(10)]
        self.sabotage = TaskData()

    def update_frame(self, frame: Frame):
        if frame.tasks:
            self.tasks = frame.tasks

    def get_x(self):
