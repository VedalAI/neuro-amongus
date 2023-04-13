"""Install packages as defined in this file into the Python environment."""
from setuptools import setup, find_namespace_packages

# The version of this tool is based on the following steps:
# https://packaging.python.org/guides/single-sourcing-package-version/
VERSION = {}

with open("./src/Neuro_amongus/__init__.py") as fp:
    # pylint: disable=W0122
    exec(fp.read(), VERSION)

setup(
    name="Neuro_amongus",
    author="Vedal",
    url="https://github.com/VedalAI/neuro-amongus",
    description="Neuro-sama Among Us Plugin",
    version=VERSION.get("__version__", "0.0.0"),
    package_dir={"": "src"},
    packages=find_namespace_packages(where="src", exclude=["tests"]),
    install_requires=[
        "setuptools>=45.0",
        "requests>=2.28.2",
        "betterproto[compiler]>=1.2.5",
        "torch>=2.0.0",
        "numpy>=1.24.2"
    ],
    classifiers=[
        "Programming Language :: Python :: 3.0",
        "Topic :: Utilities",
        "Development Status :: 3 - Alpha",
    ],
)