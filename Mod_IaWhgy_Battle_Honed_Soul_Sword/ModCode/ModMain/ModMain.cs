using System;
using System.Collections.Generic;
using EBattleTypeData;
using MelonLoader;
using System.Linq;
using System.Text;

namespace MOD_IaWhgy
{
    public class ModMain
    {
        internal static readonly string modNamespace = typeof(ModMain).Namespace!;

        private int dungeonLevel;
        private int dungeonGrade;
        private int? soulDevourSwordAtkLimit;
        private int bossKillCount;

        private MelonPreferences_Category category;
        private MelonPreferences_Entry<double> mythicalBeastDevourFraction;
        private MelonPreferences_Entry<bool> showNotification;

        public void Init()
        {
            category = MelonPreferences.CreateCategory(modNamespace);
            mythicalBeastDevourFraction = category.CreateEntry(
                "Mythical Beast Devour Reward Fraction",
                0.1,
                description: "How much stats to give for \"consuming\" a Mythical Beast relative to a NPC"
            );
            showNotification = category.CreateEntry(
                "Show Notification",
                true,
                description: "Should notification for added Soul Reaver stats be shown at the end of the battle?"
            );

            g.events.On(EBattleType.BattleStart, (Il2CppSystem.Action<ETypeData>)OnBattleStart);
            g.events.On(EBattleType.BattleEnd, (Il2CppSystem.Action<ETypeData>)OnBattleEnd);
            g.events.On(EBattleType.UnitDie, (Il2CppSystem.Action<ETypeData>)OnUnitDie);
        }

        public void Destroy()
        {
            g.events.Off(EBattleType.BattleStart, (Il2CppSystem.Action<ETypeData>)OnBattleStart);
            g.events.Off(EBattleType.BattleEnd, (Il2CppSystem.Action<ETypeData>)OnBattleEnd);
            g.events.Off(EBattleType.UnitDie, (Il2CppSystem.Action<ETypeData>)OnUnitDie);
        }

        private void OnUnitDie(ETypeData e)
        {
            var edata = e.Cast<UnitDie>();
            if (EffectTool.IsUnitMonstType(edata.unit, (int)MonstType.BOSS))
            {
                bossKillCount += 1;
            }
        }

        private void OnBattleStart(ETypeData e)
        {
            bossKillCount = 0;
            dungeonLevel = g.world.battle.data.dungeonLevel;
            dungeonGrade = (int)Math.Ceiling((double)dungeonLevel / 5);
            if (int.TryParse(g.conf.soulDevourSwordParamer.soulDevourSwordAtkLimit, out var result))
            {
                soulDevourSwordAtkLimit = result;
            }
            else
            {
                soulDevourSwordAtkLimit = null;
            }
        }

        private void OnBattleEnd(ETypeData e)
        {
            var swordMgr = g.world.soulDevourSword;

            var playerUnitId = g.world.playerUnit.data.unitData.unitID;
            if (swordMgr.data.unitId != playerUnitId)
            {
                return;
            }

            if (bossKillCount < 1)
            {
                return;
            }

            var confList_ = g.conf.soulDevourSwordDevour._allConfList;
            var confList = new List<ConfSoulDevourSwordDevourItem>();
            for (int i = 0; i < Math.Min(confList_.Count, dungeonGrade); i++)
            {
                confList.Add(confList_[i]);
            }

            var addAtk = BuildCurve(confList.Select(v => v.atkUp), mythicalBeastDevourFraction.Value)[dungeonGrade - 1] * bossKillCount;
            var addExp = BuildCurve(confList.Select(v => v.increaseExp), mythicalBeastDevourFraction.Value)[dungeonGrade - 1] * bossKillCount;
            Log($"OnBattleEnd: Resolving Soul Reaver's atk: {nameof(addAtk)}={addAtk}, {nameof(addExp)}={addExp}, {nameof(dungeonLevel)}={dungeonLevel}, {nameof(bossKillCount)}={bossKillCount}, {nameof(soulDevourSwordAtkLimit)}={soulDevourSwordAtkLimit}");
            AddSoulSwordAtk(addAtk);

            var expInitial = swordMgr.data.exp;
            swordMgr.AddExp(addExp);
            var expAdded = swordMgr.data.exp - expInitial;

            if (showNotification.Value && (addAtk > 0 || (expAdded > 0)))
            {
                var tip = new StringBuilder("Soul Reaver");
                if (addAtk > 0)
                {
                    tip.AppendFormat(" {0} Atk+", addAtk);
                }
                if (expAdded > 0)
                {
                    tip.AppendFormat(" {0} Exp+", addExp);
                }
                UITipItem.AddTip(tip.ToString(), 2f);
            }
        }

        private void AddSoulSwordAtk(int addAtk)
        {
            var swordMgr = g.world.soulDevourSword;
            var newAtk = Math.Max(swordMgr.data.atk + addAtk, 0);
            if (soulDevourSwordAtkLimit != null)
            {
                newAtk = Math.Min(newAtk, (int)soulDevourSwordAtkLimit);
            }
            else
            {
                Log($"Modifying Soul Reaver's atk stat while soulDevourSwordAtkLimit is null: {nameof(addAtk)}={addAtk}");
            }
            swordMgr.data.atk = newAtk;
        }

        public static List<int> BuildCurve(
            IEnumerable<int> baseValues,
            double fraction)
        {
            var scaled = baseValues.Select(v => (int)Math.Ceiling(v * fraction)).ToList();

            MonotoneFix(scaled);
            return scaled;
        }

        private static void MonotoneFix(List<int> seq)
        {
            for (int i = 1; i < seq.Count; i++)
            {
                if (seq[i] <= seq[i - 1])
                {
                    seq[i] = seq[i - 1] + 1;
                }
            }
        }

        private static void Log(object s)
        {
            MelonLogger.Msg($"{modNamespace}: {s}");
        }
    }
}
