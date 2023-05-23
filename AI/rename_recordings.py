import os
import uuid

path = "AI/recordings/files/"
new_path = "AI/recordings/"

# recursively rename all files in the recordings folder to a unique name and move them to the recordings folder
for root, dirs, files in os.walk(path):
    for file in files:
        # get the extension of the file
        ext = os.path.splitext(file)[1]

        # create a random name for the file
        random_name = str(uuid.uuid4())

        # rename the file and move it to the recordings folder
        os.rename(os.path.join(root, file), os.path.join(new_path, random_name + ext))
    