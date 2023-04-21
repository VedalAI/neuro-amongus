import os
import platform
import shutil
import subprocess
from io import BytesIO
from pathlib import Path
from zipfile import ZipFile

import requests


RELEASE_VERSION = "22.3"


def get_protoc_path():
    bin_dir = Path("lib")
    bin_dir.mkdir(parents=True, exist_ok=True)

    # Define the base URL for downloading the protoc compiler
    base_url = f"https://github.com/protocolbuffers/protobuf/releases/download/v{RELEASE_VERSION}"

    # Determine the appropriate protoc binary for the current operating system
    os_name = platform.system().lower()
    os_arch = "x86_64" if platform.machine().endswith('64') else "x86_32"

    if os_name == "linux":
        binary_name = f"protoc-{RELEASE_VERSION}-linux-{os_arch}"
    elif os_name == "darwin":
        binary_name = f"protoc-{RELEASE_VERSION}-osx-universal_binary"
    elif os_name == "windows":
        binary_name = f"protoc-{RELEASE_VERSION}-{'win64' if os_arch == 'x86_64' else 'win32'}"
    else:
        raise RuntimeError("Unsupported operating system")

    # Set the local path for the protoc binary
    protoc_path = bin_dir / f"protoc-{RELEASE_VERSION}{'.exe' if os_name == 'windows' else ''}"

    # Download and extract the protoc binary if it doesn't exist
    if not protoc_path.is_file():
        response = requests.get(f"{base_url}/{binary_name}.zip", stream=True)
        response.raise_for_status()

        with ZipFile(BytesIO(response.content)) as zf:
            if os_name == "windows":
                protoc_zip_path = f"bin/protoc.exe"
            else:
                protoc_zip_path = f"bin/protoc"

            with zf.open(protoc_zip_path) as src, open(protoc_path, "wb") as dst:
                dst.write(src.read())

        # Make the binary executable (not required on Windows)
        if os_name != "windows":
            protoc_path.chmod(protoc_path.stat().st_mode | 0o111)

    return str(protoc_path)


def compile_proto_files(input_dir, output_file, protoc_path):
    proto_files = []

    for root, _, files in os.walk(input_dir):
        for file in files:
            if file.endswith('.proto'):
                # Add the file to the build
                input_file = os.path.join(root, file)
                proto_files.append(input_file)

    if proto_files:
        # Run the protoc command to compile all .proto files
        output_dir = Path("lib/generated")
        output_dir.mkdir(parents=True, exist_ok=True)
        command = f"{protoc_path} --proto_path={input_dir} --python_betterproto_out={output_dir} {' '.join(proto_files)}"
        subprocess.run(command, shell=True, check=True, cwd=os.path.dirname(os.path.realpath(__file__)))
        shutil.copy2(output_dir / "__init__.py", output_file)
        shutil.rmtree(output_dir)
    else:
        print("No .proto files found in the input directory.")


if __name__ == "__main__":
    compile_proto_files("../Neuro", "data/proto.py", get_protoc_path())
