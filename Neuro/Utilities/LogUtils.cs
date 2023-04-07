using System.IO;
using System.Runtime.CompilerServices;

namespace Neuro.Utilities;

public static class LogUtils
{
    public static void WarnDoubleSingletonInstance([CallerFilePath] string file = null)
    {
        Warning($"Tried to create an instance of {Path.GetFileNameWithoutExtension(file)} when it already exists");
    }
}
