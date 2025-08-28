using System;
using System.Reflection;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;
using MelonLoader;

namespace MOD_7Wqrm3
{
    // [HarmonyPatch(typeof(ConfGameDifficultyValueEx))]
    // [HarmonyPatch("IsRemoveSave")]
    // public static class Patch_IsRemoveSave
    // {
    //     public static bool Prefix(ref bool __result)
    //     {
    //         ModMain.Log(":IsRemoveSave()");
    //         __result = false; // force it to always return false
    //         return false;     // skip original
    //     }
    // }

    //  // Patch default constructor
    // [HarmonyPatch(typeof(ConfGameDifficultyValueItem))]
    // [HarmonyPatch(MethodType.Constructor)]
    // public static class Patch_ConfGameDifficultyValueItem_Ctor_Default
    // {
    //     public static void Postfix(ConfGameDifficultyValueItem __instance)
    //     {
    //         ModMain.LogObjectFields(__instance);
    //         __instance.rebuildTime = 0;
    //     }
    // }
    // // Patch parameterized constructor
    // [HarmonyPatch(typeof(ConfGameDifficultyValueItem))]
    // [HarmonyPatch(MethodType.Constructor, new Type[]
    // {
    //     typeof(int),    // id
    //     typeof(int),    // refillBlockRequire
    //     typeof(int),    // blockAttribute
    //     typeof(int),    // price
    //     typeof(string), // roleAttr
    //     typeof(int),    // fishJade
    //     typeof(string), // unitAttr
    //     typeof(string), // eliteAttr
    //     typeof(string), // bossAttr
    //     typeof(int),    // totalPoint
    //     typeof(int),    // removeSave
    //     typeof(int),    // rebuildTime
    //     typeof(string), // unlock
    //     typeof(int)     // dropRate
    // })]
    // public static class Patch_ConfGameDifficultyValueItem_Ctor_Param
    // {
    //     public static void Postfix(ConfGameDifficultyValueItem __instance)
    //     {
    //         ModMain.LogObjectFields(__instance);
    //         __instance.rebuildTime = 0;
    //     }
    // }

    [HarmonyPatch(typeof(DataWorld), "InitData")]
    public class DataWorldProxy
    {
        public static DataWorld instance;

        private static void Postfix(DataWorld __instance)
        {
            instance = __instance;
        }
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
    [HarmonyPatch(typeof(DevilDemonMgr), "Init")]
    public class DevilDemonMgrProxy
    {
        public static DevilDemonMgr instance;

        private static void Postfix(DevilDemonMgr __instance)
        {
            instance = __instance;
        }
    }


    public class ModMain
    {
        private TimerCoroutine corUpdate;
        private static HarmonyLib.Harmony harmony;

        public static readonly string soleId = "7Wqrm3";
        public static readonly string modNamespace = $"MOD_{soleId}";

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
            g.events.On(EBattleType.BattleExit, (Il2CppSystem.Action<ETypeData>)OnBattleExit);
        }


        public void Destroy()
        {
            g.timer.Stop(corUpdate);
            g.events.Off(EBattleType.BattleExit, (Il2CppSystem.Action<ETypeData>)OnBattleExit);

            harmony.UnpatchSelf();
            harmony = null;
        }

        public static void OnGameStart()
        {
            FixArtifact();
        }

        public void OnUpdate() { }

        public void OnBattleExit(ETypeData e)
        {
            FixArtifact();
        }

        public static void FixArtifact()
        {
            if (g.data.world.animaWeapons.Contains(GameAnimaWeapon.HootinEye))
            {
                if (DataWorldProxy.instance?.data.godEyeData != null)
                {
                    // g.data.dataWorld.data.godEyeData doesn't seem to contain current data
                    var godEyeData = DataWorldProxy.instance.data.godEyeData;
                    if (godEyeData.isDamage)
                    {
                        Log("Repairing broken Eye of Providence");
                        godEyeData.isDamage = false;
                    }
                }
                else
                {
                    Log("GodEyeData is null");
                }
            }
            else if (g.data.world.animaWeapons.Contains(GameAnimaWeapon.DevilDemon))
            {
                {
                    if (DevilDemonMgrProxy.instance != null)
                    {
                        var instance = DevilDemonMgrProxy.instance;
                        if (instance.devilDemonData.isDamage)
                        {
                            Log("Repairing broken Mythical Gourd");
                            instance.devilDemonData.isDamage = false;
                            instance.devilDemonData.repairMonth = 0;
                        }
                        instance.devilDemonData.brokenCount = 0;
                    }
                    else
                    {
                        Log("DevilDemonMgr is null");
                    }
                }
            }
            else { }
        }

        public static void Log(string s)
        {
            MelonLogger.Msg($"{modNamespace}: {s}");
        }
    }
}
