using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using EBattleTypeData;
using MelonLoader;
using System.Linq;
using System.Text;
using EGameTypeData;

namespace MOD_IaWhgy
{
    internal class Config
    {
        static bool _isInit = false;

        static MelonPreferences_Category category;
        internal static MelonPreferences_Entry<double> mythicalBeastDevourFraction;
        internal static MelonPreferences_Entry<bool> showNotification;
        internal static MelonPreferences_Entry<string> devourDeadlineMode;

        internal static DevourDeadlineMode CurrentDevourDeadlineMode =>
            Enum.TryParse<DevourDeadlineMode>(devourDeadlineMode.Value, out var parsed) ? parsed : DevourDeadlineMode.ResetOnBeastDevour;

        internal enum DevourDeadlineMode
        {
            [Description("Disable deadline system completely")]
            Disabled,
            [Description("Reset deadline when devouring beasts")]
            ResetOnBeastDevour,
            [Description("Leave deadline system unchanged")]
            Unchanged
        }

        internal static void Init()
        {
            if (_isInit)
                return;

            category = MelonPreferences.CreateCategory(ModMain.modNamespace);
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
            devourDeadlineMode = category.CreateEntry(
                "Devour Deadline Mode",
                DevourDeadlineMode.ResetOnBeastDevour.ToString(),
                description: ModMain.GetEnumDescription<DevourDeadlineMode>()
            );

            _isInit = true;
        }
    }

    public class ModMain
    {
        internal static readonly string modNamespace = typeof(ModMain).Namespace!;

        Il2CppSystem.Action<ETypeData> callBattleStart;
        Il2CppSystem.Action<ETypeData> callBattleEnd;
        Il2CppSystem.Action<ETypeData> callUnitDie;
        Il2CppSystem.Action<ETypeData> callOpenUIEnd;

        private int dungeonLevel;
        private int dungeonGrade;
        private int? soulDevourSwordAtkLimit;
        private int bossKillCount;
        private int atkUp;
        private int expUp;

        private int newMonth;

        private int CurrentMonth
        {
            get
            {
                var date = GameTool.DayToDate(g.data.world.worldDay);
                return (date[0] - 1) * 12 + date[1];
            }
        }

        public void Init()
        {
            Config.Init();

            callBattleStart = (Il2CppSystem.Action<ETypeData>)OnBattleStart;
            callBattleEnd = (Il2CppSystem.Action<ETypeData>)OnBattleEnd;
            callUnitDie = (Il2CppSystem.Action<ETypeData>)OnUnitDie;
            callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;

            g.events.On(EBattleType.BattleStart, callBattleStart);
            g.events.On(EBattleType.BattleEnd, callBattleEnd);
            g.events.On(EBattleType.UnitDie, callUnitDie);
            g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
        }

        public void Destroy()
        {
            g.events.Off(EBattleType.BattleStart, callBattleStart);
            g.events.Off(EBattleType.BattleEnd, callBattleEnd);
            g.events.Off(EBattleType.UnitDie, callUnitDie);
            g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);
        }

        private void OnOpenUIEnd(ETypeData e)
        {
            var edata = e.Cast<OpenUIEnd>();
            if (edata.uiType.uiName == UIType.LoadingBar.uiName) // The circular loading UI on month skip
            {
                if (newMonth != CurrentMonth)
                {
                    OnSkipMonth();
                    newMonth = CurrentMonth;
                }
            }
        }

        private void OnSkipMonth()
        {
            if (Config.CurrentDevourDeadlineMode == Config.DevourDeadlineMode.Disabled)
            {
                var swordMgr = g.world.soulDevourSword;

                var playerUnitId = g.world.playerUnit.data.unitData.unitID;
                if (swordMgr.data.unitId != playerUnitId)
                {
                    return;
                }

                SetSoulSwordDevourMonth(CurrentMonth); // Keep resetting deadline every month
            }
        }

        private void OnUnitDie(ETypeData e)
        {
            var edata = e.Cast<UnitDie>();
            if (
                // FIXME:
                // Is true for NPC enemies in sect wars for some reason. Checking against edata.unit.data.unitType for
                // UnitType.Monst doesn't work either.
                EffectTool.IsUnitMonstType(edata.unit, (int)MonstType.BOSS)
            )
            {
                bossKillCount += 1;

                var swordMgr = g.world.soulDevourSword;

                var playerUnitId = g.world.playerUnit.data.unitData.unitID;
                if (swordMgr.data.unitId != playerUnitId)
                {
                    return;
                }

                Log($"OnBattleEnd: Resolving Soul Reaver's stats: {nameof(atkUp)}={atkUp}, {nameof(expUp)}={expUp}, {nameof(dungeonLevel)}={dungeonLevel}, {nameof(soulDevourSwordAtkLimit)}={soulDevourSwordAtkLimit}");
                AddSoulSwordAtk(atkUp);

                var expInitial = swordMgr.data.exp;
                swordMgr.AddExp(expUp);
                var expAdded = swordMgr.data.exp - expInitial;

                if (Config.showNotification.Value && (atkUp > 0 || (expAdded > 0)))
                {
                    var tip = new StringBuilder("Soul Reaver");
                    if (atkUp > 0)
                    {
                        tip.AppendFormat(" {0} Atk+", atkUp);
                    }
                    if (expAdded > 0)
                    {
                        tip.AppendFormat(" {0} Exp+", expUp);
                    }
                    UITipItem.AddTip(tip.ToString(), 2f);
                }
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

            var confList_ = g.conf.soulDevourSwordDevour._allConfList;
            var confList = new List<ConfSoulDevourSwordDevourItem>();
            for (int i = 0; i < Math.Min(confList_.Count, dungeonGrade); i++)
            {
                confList.Add(confList_[i]);
            }
            atkUp = BuildCurve(confList.Select(v => v.atkUp), Config.mythicalBeastDevourFraction.Value)[dungeonGrade - 1];
            expUp = BuildCurve(confList.Select(v => v.increaseExp), Config.mythicalBeastDevourFraction.Value)[dungeonGrade - 1];
        }

        private void OnBattleEnd(ETypeData e)
        {
            var swordMgr = g.world.soulDevourSword;

            var playerUnitId = g.world.playerUnit.data.unitData.unitID;
            if (swordMgr.data.unitId != playerUnitId || bossKillCount < 1)
            {
                return;
            }

            if (Config.CurrentDevourDeadlineMode == Config.DevourDeadlineMode.ResetOnBeastDevour)
            {
                SetSoulSwordDevourMonth(CurrentMonth);
            }
        }

        /// <summary>
        /// Preserves cooldown state.
        /// </summary>
        private static void SetSoulSwordDevourMonth(int month)
        {
            var swordMgr = g.world.soulDevourSword;
            // Keeping the cooldown as-is
            swordMgr.data.nextDevourMonth = month - 1 + Math.Abs(swordMgr.data.nextDevourMonth - swordMgr.data.lastDevourMonth);
            // Assuming month here is starting with 0
            swordMgr.data.lastDevourMonth = month - 1;
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

        public static string GetEnumDescription<T>(string sep = "\n") where T : Enum
        {
            return string.Join(sep,
                Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Select(v =>
                    {
                        var memInfo = typeof(T).GetMember(v.ToString()).FirstOrDefault();
                        var attr = memInfo?
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .Cast<DescriptionAttribute>()
                            .FirstOrDefault();

                        return attr != null
                            ? $"{v} ({attr.Description})"
                            : v.ToString();
                    }));
        }

        private static void Log(object s)
        {
            MelonLogger.Msg($"{modNamespace}: {s}");
        }
    }
}
