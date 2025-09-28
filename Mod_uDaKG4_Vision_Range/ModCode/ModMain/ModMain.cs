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
    [HarmonyPatch(typeof(WorldMgr), "Init")]
    public class Patch_WorldMgrInit
    {
        // Game start
        private static void Postfix(WorldMgr __instance)
        {
            // Have to switch back to this since the registered callback to
            // event EGameType.IntoWorld doesn't seem to get called reliably
            if (ModMain.harmony != null)
                ModMain.OnGameStart();
        }
    }

    internal class Config
    {
        static bool _isInit = false;

        static MelonPreferences_Category category;
        internal static MelonPreferences_Entry<int> playerViewRange;

        internal static void Init()
        {
            if (_isInit)
                return;

            category = MelonPreferences.CreateCategory(ModMain.modNamespace);
            playerViewRange = category.CreateEntry("playerViewRange", 5);

            _isInit = true;
        }
    }

    public class ModMain
    {
        // private TimerCoroutine corUpdate;
        internal static HarmonyLib.Harmony harmony;
        internal static readonly string soleId = "uDaKG4";
        internal static readonly string modNamespace = $"MOD_{soleId}";

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

            Config.Init();
            // corUpdate = g.timer.Frame(new Action(OnUpdate), 1, true);
        }

        public void Destroy()
        {
            // g.timer.Stop(corUpdate);

            harmony.UnpatchSelf();
            harmony = null;
        }

        public static void OnGameStart()
        {
            var playerView = g.world.playerUnit.data.dynUnitData.playerView;
            if (playerView != null)
            {
                Log($"Setting playerViewRange={Config.playerViewRange.Value}");
                playerView.baseValue = Config.playerViewRange.Value;
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
    }
}
