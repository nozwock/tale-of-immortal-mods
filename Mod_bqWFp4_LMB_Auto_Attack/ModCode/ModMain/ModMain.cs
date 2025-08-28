using System;
using System.Reflection;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;
using MelonLoader;

namespace MOD_bqWFp4
{
    public class ModMain
    {
        private TimerCoroutine corInBattle;
        private static HarmonyLib.Harmony harmony;

        public static readonly string soleId = "bqWFp4";
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

            g.events.On(EBattleType.BattleStart, (Il2CppSystem.Action<ETypeData>)OnBattleStart);
            g.events.On(EBattleType.BattleEnd, (Il2CppSystem.Action<ETypeData>)OnBattleEnd);
        }

        public void Destroy()
        {
            g.timer.Stop(corInBattle);
            g.events.Off(EBattleType.BattleStart, (Il2CppSystem.Action<ETypeData>)OnBattleStart);
            g.events.Off(EBattleType.BattleEnd, (Il2CppSystem.Action<ETypeData>)OnBattleEnd);

            harmony.UnpatchSelf();
            harmony = null;
        }

        public void OnBattleStart(ETypeData e)
        {
            // Grade Condition
            // Npc type (0-Player), Min Realm, Max Realm
            bool isRebornAndAbove = UnitConditionTool.Condition("grade_0_9_99", new UnitConditionData(g.world.playerUnit, null));

            // Disable the mod if the player is in Reborn or above stage,
            // because of Neather Feather.
            // Ideally I'd like to just check for whether Neather Feather is
            // equipped or not instead here.
            if (!isRebornAndAbove)
            {
                Log("Enabling LMB Auto Attack");
                corInBattle = g.timer.Frame(new Action(InBattle), 1, true);
            }
            else
            {
                Log("Not enabling LMB Auto Attack due to the player being in Reborn or above stage and Neather Feather being available");
            }
        }

        public void OnBattleEnd(ETypeData e)
        {
            g.timer.Stop(corInBattle);
        }

        public void InBattle()
        {
            // isStartBattle or isActiveBattle (it's off when the UI prompt for "Fight" is there)
            if (
                g.world?.battle == null
                || !g.world.battle.isBattle
                || !SceneType.battle.battleMap.isActiveBattle
                || SceneType.battle.battleMap.monstCount == 0
            )
            {
                return;
            }

            // Only cast LMB skill if there's atleast one enemy
            //
            // Before it was being checked using BattleUnitMgr.GetNearUnit and
            // passing it an appropriate list of UnitType, list had to be
            // constructed using .Add since it's an Il2CppSystem List
            // var enemyUnit = SceneType.battle.unit.GetNearUnit(new Vector2(), enemyUnitTypes);

            // Even before the mod used to instead just make the LMB a toggle
            // so that the user doesn't need to hold it constantly
            // if (Input.GetKeyUp(KeyCode.Mouse0))
            // {
            //     attackToggled = !attackToggled;
            // }
            // if (!attackToggled)
            //     return;

            UnitCtrlPlayer playerUnitCtrl = SceneType.battle.battleMap.playerUnitCtrl; // SceneType / SceneBattle / BattleMapMgr
            if (playerUnitCtrl == null)
                return;

            // Fire the basic skill, cursor targeting is handled by the game
            playerUnitCtrl.CreateSkillAttack(MartialType.SkillLeft);
        }

        public static void Log(string s)
        {
            MelonLogger.Msg($"{modNamespace}: {s}");
        }
    }
}
