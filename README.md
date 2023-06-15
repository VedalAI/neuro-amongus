# Neuro-sama Among Us Plugin

The purpose of this plugin is to allow Neuro-sama, the AI VTuber, to play Among Us.

## About

The plan is to first allow this plugin to record data from the game, which will then be used to train a neural network which Neuro will use.

## How to run the plugin

- Install BepInEx [https://docs.reactor.gg/quick_start/install_bepinex](https://docs.reactor.gg/quick_start/install_bepinex)
  - The current release is using [BepInEx-Unity.IL2CPP-win-x86-6.0.0-be.668](https://builds.bepinex.dev/projects/bepinex_be)
- Install Reactor [https://docs.reactor.gg/quick_start/install_reactor](https://docs.reactor.gg/quick_start/install_reactor)
- Ensure you have the .NET 6.0 SDK [https://dotnet.microsoft.com/download/dotnet/6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- Build the plugin using an IDE of your choice, I'm using Visual Studio but JetBrains Rider is a better alternative
- Add the built Neuro.dll to your plugins folder (I'd recommend adding `<OutputPath>PATH_TO_AMONG_US_FOLDER\Among Us\BepInEx\plugins</OutputPath>` to the PropertyGroup section of the [Neuro.csproj](Neuro/Neuro.csproj) file) 
- Run Among Us

## How to run the AI
- Ensure requirements are installed
- Use this for betterproto `pip install git+https://github.com/thrive-labs/python-betterproto.git@add-pickle-methods`

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

## License

[GNU GPLv3](https://choosealicense.com/licenses/gpl-3.0/)

#
<p align="center">This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC.</p>
<p align="center">© Innersloth LLC.</p>
