using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;
using MelonLoader;
using Newtonsoft.Json;
using MelonLoader.Preferences;

namespace MOD_uDaKG4
{
    [Serializable]
    public class Config
    {
        public int playerViewRange = 5;
    }

    [HarmonyPatch(typeof(WorldMgr), "Init")]
    public class WorldMgrPatchInit
    {
        // Game start
        private static void Postfix(WorldMgr __instance)
        {
            ModMain.OnGameStart();
        }
    }

    public class ModMain
    {
        private TimerCoroutine corUpdate;
        private static HarmonyLib.Harmony harmony;
        public static string soleId = "uDaKG4";
        public static string modNamespace = $"MOD_{soleId}";
        private static Config config;

        public void Init()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
                harmony = null;
            }
            if (harmony == null)
            {
                harmony = new HarmonyLib.Harmony(modNamespace);
            }
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            corUpdate = g.timer.Frame(new Action(OnUpdate), 1, true);

            config = LoadJsonConfig<Config>("config.json");
        }

        public void Destroy()
        {
            g.timer.Stop(corUpdate);

            harmony.UnpatchSelf();
            harmony = null;
        }

        public static void OnGameStart()
        {
            var playerView = g.world.playerUnit.data.dynUnitData.playerView;
            if (playerView != null)
            {
                Log($"Setting playerViewRange={config.playerViewRange}");
                playerView.baseValue = config.playerViewRange;
            }
            else
            {
                Log("Cannot modify view range since playerView is null");
            }
        }

        public void OnUpdate()
        {
        }

        public static void Log(string s)
        {
            MelonLogger.Msg($"{modNamespace}: {s}");
        }

        public static T LoadJsonConfig<T>(string path) where T : new()
        {
            var finalPath = Path.Combine(g.mod.GetModPathRoot(soleId), "ModCode", "dll", path);

            if (!File.Exists(finalPath))
            {
                Log($"Config not found: '{finalPath}'");
                return new T();
            }

            try
            {
                var json = File.ReadAllText(finalPath);

                var obj = JsonConvert.DeserializeObject<T>(json);
                if (obj == null)
                {
                    return new T();
                }
                return obj;
            }
            catch (Exception e)
            {
                Log($"Failed to load config: {e.Message}");
                return new T();
            }
        }
    }
}
