global using static MOD_2UTwDC.Prelude;
using System.Linq;
using MelonLoader;

namespace MOD_2UTwDC;

internal class Prelude
{
    internal static readonly string modNamespace = typeof(ModMain).Namespace!;
    internal static readonly string soleId = modNamespace.Split('_').LastOrDefault();

    /// <summary>
    /// Get localized string for a key with mod's `soleId` prepended to it.
    /// </summary>
    internal static string LS(string key)
    {
        return GameTool.LS($"{soleId}.{key}");
    }

    internal static void Log(object obj)
    {
        MelonLogger.Msg($"{modNamespace}: {obj}");
    }
}