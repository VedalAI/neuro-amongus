from setuptools import setup

setup(
    name="neuro_amongus",
    author="VedalAI",
    url="https://github.com/VedalAI/neuro-amongus",
    description="Neuro-sama Among Us AI",
    version="1.0.0-dev",
    install_requires=[
        "setuptools>=45.0",
        "requests>=2.28.2",
        "betterproto[compiler]>=2.0.0b5",
        "torch>=2.0.0",
        "numpy>=1.24.2"
    ],
)