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
        internal static readonly string soleId = "uDaKG4";
        internal static readonly string modNamespace = $"MOD_{soleId}";

        Il2CppSystem.Action<ETypeData> callIntoWorld;

        public void Init()
        {
            Config.Init();

            callIntoWorld = (Il2CppSystem.Action<ETypeData>)OnGameStart;
            g.events.On(EGameType.IntoWorld, callIntoWorld);
        }

        public void Destroy()
        {
            g.events.Off(EGameType.IntoWorld, callIntoWorld);
        }

        public static void OnGameStart(ETypeData e)
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

        public static void Log(string s)
        {
            MelonLogger.Msg($"{modNamespace}: {s}");
        }
    }
}
