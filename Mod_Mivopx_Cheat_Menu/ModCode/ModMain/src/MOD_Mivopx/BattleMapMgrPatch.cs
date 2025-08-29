using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleOfImmortalCheat.Localization;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(BattleMapMgr), "BattleStart")]
internal class BattleMapMgrPatch
{
	public static BattleMapMgr battleMgr;

	private static bool _isGodModActive;

	private static bool _isNoCDActive;

	private static bool _isInfPointsActive;

	private static bool _isOneHitActive;

	private const int INFINITE_VALUE = 69696969;

	private const int NOCDVALUE = 10000;

	private static Dictionary<int, int> orig_artifactCDRate;

	private static Dictionary<int, int> orig_itemCDRate;

	private static Dictionary<int, int> orig_skillCDRate;

	private static Dictionary<int, int> orig_stepCDRate;

	private static Dictionary<int, int> orig_ultimateCDRate;

	private static Dictionary<int, int> orig_maxHP;

	private static Dictionary<int, int> orig_maxMP;

	private static Dictionary<int, int> orig_maxSP;

	private static Dictionary<int, int> orig_skillDmg;

	private static Dictionary<int, int> orig_attack;

	private static int CustomID;

	public static bool IsGodModActive
	{
		get
		{
			return _isGodModActive;
		}
		set
		{
			_isGodModActive = value;
			if (battleMgr != null && (battleMgr.isStartBattle || battleMgr.isActiveBattle))
			{
				if (value)
				{
					battleMgr.playerUnitCtrl.AddState(UnitStateType.Invincible, -1f);
				}
				else
				{
					battleMgr.playerUnitCtrl.DelState(UnitStateType.Invincible);
				}
			}
		}
	}

	public static bool IsNoCDActive
	{
		get
		{
			return _isNoCDActive;
		}
		set
		{
			_isNoCDActive = value;
			if (battleMgr != null && (battleMgr.isStartBattle || battleMgr.isActiveBattle))
			{
				if (value)
				{
					BackupValue(orig_artifactCDRate, battleMgr.playerUnitCtrl.data.artifactCDRate.mBaseValue);
					BackupValue(orig_itemCDRate, battleMgr.playerUnitCtrl.data.itemCDRate.mBaseValue);
					BackupValue(orig_skillCDRate, battleMgr.playerUnitCtrl.data.skillCDRate.mBaseValue);
					BackupValue(orig_stepCDRate, battleMgr.playerUnitCtrl.data.stepCDRate.mBaseValue);
					BackupValue(orig_ultimateCDRate, battleMgr.playerUnitCtrl.data.ultimateCDRate.mBaseValue);
					battleMgr.playerUnitCtrl.data.artifactCDRate.mBaseValue = 10000;
					battleMgr.playerUnitCtrl.data.itemCDRate.mBaseValue = 10000;
					battleMgr.playerUnitCtrl.data.skillCDRate.mBaseValue = 10000;
					battleMgr.playerUnitCtrl.data.stepCDRate.mBaseValue = 10000;
					battleMgr.playerUnitCtrl.data.ultimateCDRate.mBaseValue = 10000;
				}
				else
				{
					battleMgr.playerUnitCtrl.data.artifactCDRate.mBaseValue = orig_artifactCDRate[CustomID];
					battleMgr.playerUnitCtrl.data.itemCDRate.mBaseValue = orig_itemCDRate[CustomID];
					battleMgr.playerUnitCtrl.data.skillCDRate.mBaseValue = orig_skillCDRate[CustomID];
					battleMgr.playerUnitCtrl.data.stepCDRate.mBaseValue = orig_stepCDRate[CustomID];
					battleMgr.playerUnitCtrl.data.ultimateCDRate.mBaseValue = orig_ultimateCDRate[CustomID];
				}
			}
		}
	}

	public static bool IsInfPointsActive
	{
		get
		{
			return _isInfPointsActive;
		}
		set
		{
			_isInfPointsActive = value;
			if (battleMgr != null && (battleMgr.isStartBattle || battleMgr.isActiveBattle))
			{
				if (value)
				{
					BackupValue(orig_maxHP, battleMgr.playerUnitCtrl.data.maxHP.baseValue);
					BackupValue(orig_maxMP, battleMgr.playerUnitCtrl.data.maxMP.baseValue);
					BackupValue(orig_maxSP, battleMgr.playerUnitCtrl.data.maxSP.baseValue);
					battleMgr.playerUnitCtrl.data.maxHP.baseValue = 69696969;
					battleMgr.playerUnitCtrl.data.maxMP.baseValue = 69696969;
					battleMgr.playerUnitCtrl.data.maxSP.baseValue = 69696969;
					battleMgr.playerUnitCtrl.data.hp = battleMgr.playerUnitCtrl.data.maxHP.value;
					battleMgr.playerUnitCtrl.data.mp = battleMgr.playerUnitCtrl.data.maxMP.value;
					battleMgr.playerUnitCtrl.data.sp = battleMgr.playerUnitCtrl.data.maxSP.value;
				}
				else
				{
					battleMgr.playerUnitCtrl.data.maxHP.baseValue = orig_maxHP[CustomID];
					battleMgr.playerUnitCtrl.data.maxMP.baseValue = orig_maxMP[CustomID];
					battleMgr.playerUnitCtrl.data.maxSP.baseValue = orig_maxSP[CustomID];
					battleMgr.playerUnitCtrl.data.hp = battleMgr.playerUnitCtrl.data.maxHP.value;
					battleMgr.playerUnitCtrl.data.mp = battleMgr.playerUnitCtrl.data.maxMP.value;
					battleMgr.playerUnitCtrl.data.sp = battleMgr.playerUnitCtrl.data.maxSP.value;
				}
			}
		}
	}

	public static bool IsOneHitActive
	{
		get
		{
			return _isOneHitActive;
		}
		set
		{
			_isOneHitActive = value;
			if (battleMgr != null && (battleMgr.isStartBattle || battleMgr.isActiveBattle))
			{
				if (value)
				{
					BackupValue(orig_skillDmg, battleMgr.playerUnitCtrl.data.skillDmg.baseValue);
					BackupValue(orig_attack, battleMgr.playerUnitCtrl.data.attack.baseValue);
					battleMgr.playerUnitCtrl.data.skillDmg.baseValue = 69696969;
					battleMgr.playerUnitCtrl.data.attack.baseValue = 69696969;
				}
				else
				{
					battleMgr.playerUnitCtrl.data.skillDmg.baseValue = orig_skillDmg[CustomID];
					battleMgr.playerUnitCtrl.data.attack.baseValue = orig_attack[CustomID];
				}
			}
		}
	}

	static BattleMapMgrPatch()
	{
		orig_artifactCDRate = new Dictionary<int, int>();
		orig_itemCDRate = new Dictionary<int, int>();
		orig_skillCDRate = new Dictionary<int, int>();
		orig_stepCDRate = new Dictionary<int, int>();
		orig_ultimateCDRate = new Dictionary<int, int>();
		orig_maxHP = new Dictionary<int, int>();
		orig_maxMP = new Dictionary<int, int>();
		orig_maxSP = new Dictionary<int, int>();
		orig_skillDmg = new Dictionary<int, int>();
		orig_attack = new Dictionary<int, int>();
		CustomID = 0;
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void BackupValue(Dictionary<int, int> dictionary, int baseValue)
	{
		if (!dictionary.ContainsKey(CustomID))
		{
			dictionary[CustomID] = baseValue;
		}
	}

	public static void ClearDictionary()
	{
		orig_artifactCDRate.Clear();
		orig_itemCDRate.Clear();
		orig_skillCDRate.Clear();
		orig_stepCDRate.Clear();
		orig_ultimateCDRate.Clear();
		orig_maxHP.Clear();
		orig_maxMP.Clear();
		orig_maxSP.Clear();
		orig_skillDmg.Clear();
		orig_attack.Clear();
	}

	private static int GenerateCustomID()
	{
		return Environment.TickCount & 0x7FFFFFFF;
	}

	public static void InitializeCustomID()
	{
		CustomID = GenerateCustomID();
	}

	private static void Postfix(BattleMapMgr __instance)
	{
		battleMgr = __instance;
		if (battleMgr != null && (battleMgr.isStartBattle || battleMgr.isActiveBattle))
		{
			if (IsGodModActive)
			{
				battleMgr.playerUnitCtrl.AddState(UnitStateType.Invincible, -1f);
				ModMain.LogTip(LocalizationHelper.T("other_msgs_god_mode_active"));
			}
			if (IsNoCDActive)
			{
				BackupValue(orig_artifactCDRate, battleMgr.playerUnitCtrl.data.artifactCDRate.mBaseValue);
				BackupValue(orig_itemCDRate, battleMgr.playerUnitCtrl.data.itemCDRate.mBaseValue);
				BackupValue(orig_skillCDRate, battleMgr.playerUnitCtrl.data.skillCDRate.mBaseValue);
				BackupValue(orig_stepCDRate, battleMgr.playerUnitCtrl.data.stepCDRate.mBaseValue);
				BackupValue(orig_ultimateCDRate, battleMgr.playerUnitCtrl.data.ultimateCDRate.mBaseValue);
				battleMgr.playerUnitCtrl.data.artifactCDRate.mBaseValue = 10000;
				battleMgr.playerUnitCtrl.data.itemCDRate.mBaseValue = 10000;
				battleMgr.playerUnitCtrl.data.skillCDRate.mBaseValue = 10000;
				battleMgr.playerUnitCtrl.data.stepCDRate.mBaseValue = 10000;
				battleMgr.playerUnitCtrl.data.ultimateCDRate.mBaseValue = 10000;
				ModMain.LogTip(LocalizationHelper.T("other_msgs_fast_no_cooldown_active"));
			}
			if (IsInfPointsActive)
			{
				BackupValue(orig_maxHP, battleMgr.playerUnitCtrl.data.maxHP.baseValue);
				BackupValue(orig_maxMP, battleMgr.playerUnitCtrl.data.maxMP.baseValue);
				BackupValue(orig_maxSP, battleMgr.playerUnitCtrl.data.maxSP.baseValue);
				battleMgr.playerUnitCtrl.data.maxHP.baseValue = 69696969;
				battleMgr.playerUnitCtrl.data.maxMP.baseValue = 69696969;
				battleMgr.playerUnitCtrl.data.maxSP.baseValue = 69696969;
				battleMgr.playerUnitCtrl.data.hp = battleMgr.playerUnitCtrl.data.maxHP.value;
				battleMgr.playerUnitCtrl.data.mp = battleMgr.playerUnitCtrl.data.maxMP.value;
				battleMgr.playerUnitCtrl.data.sp = battleMgr.playerUnitCtrl.data.maxSP.value;
				ModMain.LogTip(LocalizationHelper.T("other_msgs_infinite_vitality_active"));
			}
			if (IsOneHitActive)
			{
				BackupValue(orig_skillDmg, battleMgr.playerUnitCtrl.data.skillDmg.baseValue);
				BackupValue(orig_attack, battleMgr.playerUnitCtrl.data.attack.baseValue);
				battleMgr.playerUnitCtrl.data.skillDmg.baseValue = 69696969;
				battleMgr.playerUnitCtrl.data.attack.baseValue = 69696969;
				ModMain.LogTip(LocalizationHelper.T("other_msgs_one_hit_kill_active"));
			}
		}
	}

	private static void UpdateUITexts()
	{
	}
}
