using System;
using System.Reflection;
using MelonLoader;

namespace MOD_uDaKG4
{
    public class ModMain
    {
        private TimerCoroutine corUpdate;
        private static HarmonyLib.Harmony harmony;
        public static string soleId = "uDaKG4";
        public static string modNamespace = $"MOD_{soleId}";

        private MelonPreferences_Category category;
        private MelonPreferences_Entry<int> playerViewRange;

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

            category = MelonPreferences.CreateCategory(modNamespace);
            playerViewRange = category.CreateEntry("playerViewRange", 5);

            g.events.On(EGameType.IntoWorld, (Il2CppSystem.Action<ETypeData>)OnGameStart);
            corUpdate = g.timer.Frame(new Action(OnUpdate), 1, true);
        }

        public void Destroy()
        {
            g.timer.Stop(corUpdate);
            g.events.Off(EGameType.IntoWorld, (Il2CppSystem.Action<ETypeData>)OnGameStart);

            harmony.UnpatchSelf();
            harmony = null;
        }

        public void OnGameStart(ETypeData e)
        {
            var playerView = g.world.playerUnit.data.dynUnitData.playerView;
            if (playerView != null)
            {
                Log($"Setting playerViewRange={playerViewRange.Value}");
                playerView.baseValue = playerViewRange.Value;
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
