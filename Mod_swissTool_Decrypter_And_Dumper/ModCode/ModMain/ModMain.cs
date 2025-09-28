using System;
using System.Reflection;

namespace MOD_swissTool
{
    public class ModMain
    {
		private static HarmonyLib.Harmony harmony;
        public static string valKey = "2a;ad.,&fSf^SX.,:12@D";
        public static string savPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low\\guigugame\\guigubahuang\\Steam\\CacheData";
        public void Init()
        {
            if (harmony != null)
			{
				harmony.UnpatchSelf();
				harmony = null;
			}
			if (harmony == null)
			{
				harmony = new HarmonyLib.Harmony("MOD_swissTool");
			}
			harmony.PatchAll(Assembly.GetExecutingAssembly());
            Console.WriteLine($"Mod encryption key: {GameConf.modEncryPassword}");
            Console.WriteLine($"Cache encryption key: {GameConf.cacheEncryPassword}");
            Console.WriteLine($"Encryption validation key {valKey}");

            Console.WriteLine($"Game saves are stored at:\n {g.cache.cachePath}");
        }

        public void Destroy()
        {
            harmony.UnpatchSelf(); // It is THIS SIMPLE!!!
        }
    }
}
