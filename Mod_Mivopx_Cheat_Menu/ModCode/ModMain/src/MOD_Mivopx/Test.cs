using Il2CppSystem.Collections.Generic;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

internal class Test
{
	static Test()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	public static void TestMartialArts()
	{
		Dictionary<string, DataUnit.ActionMartialData>.Enumerator enumerator = Game.WorldManager.Value.playerUnit.data.unitData.allActionMartial.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, DataUnit.ActionMartialData> current = enumerator.Current;
			DataProps.PropsData data = current.Value.data;
			ModMain.Log("Count -> " + current.Value.data.propsInfoBase.name);
			List<DataProps.MartialData.Prefix> prefixs = data.To<DataProps.MartialData>().martialInfo.GetPrefixs();
			if (prefixs.Count > 0)
			{
				List<DataProps.MartialData.Prefix>.Enumerator enumerator2 = prefixs.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					DataProps.MartialData.Prefix current2 = enumerator2.Current;
					ModMain.Log($"{current2.prefixValueItem.number}");
				}
			}
			else
			{
				ModMain.Log(current.Key + " -> no prefixes");
			}
		}
	}

	public static void Log(string message)
	{
	}

	public static void PrintNPCLuck()
	{
		WorldUnitBase value = Game.NpcUnit.Value;
		foreach (DataUnit.LuckData item in value.data.unitData.propertyData.bornLuck)
		{
			WorldUnitLuckBase luck = value.GetLuck(item.id);
			if (luck != null)
			{
				value.DestroyLuck(luck);
				value.CreateLuck(new DataUnit.LuckData
				{
					id = item.id,
					createTime = 4,
					duration = -1,
					objData = new DataObjectData()
				});
			}
		}
	}

	public static void TestChangeBornLuck()
	{
		WorldMgr value = Game.WorldManager.Value;
		List<ConfRoleCreateFeatureItem> allConfList = Game.ConfMgr.roleCreateFeature._allConfList;
		DataUnit.PropertyData propertyData = value.playerUnit.data.unitData.propertyData;
		WorldUnitLuckBase luck = value.playerUnit.GetLuck(propertyData.bornLuck[0].id);
		ModMain.Log($"HasLuck: {luck != null}");
		value.playerUnit.DestroyLuck(luck);
		ModMain.Log($"AddLuck Count: {value.playerUnit.data.unitData.propertyData.addLuck.Count}");
		ConfRoleCreateFeatureItem confRoleCreateFeatureItem = allConfList[100];
		value.playerUnit.CreateLuck(new DataUnit.LuckData
		{
			id = confRoleCreateFeatureItem.id,
			createTime = 4,
			duration = -1,
			objData = new DataObjectData()
		});
		propertyData.bornLuck[0].id = confRoleCreateFeatureItem.id;
	}

	public static void TestChangeFateNPC()
	{
		WorldUnitBase value = Game.NpcUnit.Value;
		List<WorldUnitLuckBase>.Enumerator enumerator = value.allLuck.GetEnumerator();
		while (enumerator.MoveNext())
		{
			WorldUnitLuckBase current = enumerator.Current;
			ModMain.Log($"{current.luckConf.id}");
		}
		ModMain.Log(value.data.dynUnitData.gradeID.value.ToString());
		Dictionary<int, DataWorld.World.PlayerLogData.GradeData>.Enumerator enumerator2 = value.data.unitData.npcUpGrade.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			KeyValuePair<int, DataWorld.World.PlayerLogData.GradeData> current2 = enumerator2.Current;
			ModMain.Log($"gradeLog: {current2.Key} -> {current2.Value.luck}:{current2.Value.quality}");
		}
	}

	public static void TestChangeFate()
	{
		WorldUnitBase value = Game.NpcUnit.Value;
		ModMain.Log(value.data.dynUnitData.attack.baseValue.ToString());
		ModMain.Log(value.data.dynUnitData.attack.value.ToString());
		ModMain.Log(value.data.unitData.propertyData.attack.ToString());
		WorldMgr value2 = Game.WorldManager.Value;
		ConfRoleCreateFeatureItem item = Game.ConfMgr.roleCreateFeature.GetItem(700027);
		List<DataUnit.LuckData>.Enumerator enumerator = value2.playerUnit.data.unitData.propertyData.addLuck.GetEnumerator();
		while (enumerator.MoveNext())
		{
			DataUnit.LuckData current = enumerator.Current;
			ModMain.Log($"addLuck: {current.id}");
		}
		value2.playerUnit.data.unitData.propertyData.DelAddLuck(1);
		value2.playerUnit.data.unitData.propertyData.AddAddLuck(new DataUnit.LuckData
		{
			createTime = 0,
			duration = -1,
			id = 1,
			objData = new DataObjectData()
		});
		Dictionary<int, DataWorld.World.PlayerLogData.GradeData>.Enumerator enumerator2 = Game.DataWorld.Value.data.playerLog.upGrade.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			KeyValuePair<int, DataWorld.World.PlayerLogData.GradeData> current2 = enumerator2.Current;
			ModMain.Log($"gradeLog: {current2.Key} -> {current2.Value.luck}:{current2.Value.quality}");
			if (current2.Value.luck == 700028)
			{
				current2.Value.luck = 700027;
				current2.Value.quality = item.level;
			}
		}
	}

	public static void RandomBullshitGo()
	{
		if (Game.ConfMgr.roleGroupSeekPropSub != null)
		{
			_ = Game.ConfMgr.roleGroupSeekPropSub;
		}
		if (Game.ConfMgr.roleConvoy != null)
		{
			_ = Game.ConfMgr.roleConvoy;
		}
		if (Game.ConfMgr.roleTeachSkill != null)
		{
			_ = Game.ConfMgr.roleTeachSkill;
		}
		if (Game.ConfMgr.roleMarry != null)
		{
			_ = Game.ConfMgr.roleMarry;
		}
		if (Game.ConfMgr.roleAttack != null)
		{
			_ = Game.ConfMgr.roleAttack;
		}
		if (Game.ConfMgr.roleDiscovery != null)
		{
			_ = Game.ConfMgr.roleDiscovery;
		}
		if (Game.ConfMgr.rolePlanReve != null)
		{
			_ = Game.ConfMgr.rolePlanReve;
		}
		if (Game.ConfMgr.roleSchoolRecruit != null)
		{
			_ = Game.ConfMgr.roleSchoolRecruit;
		}
		if (Game.ConfMgr.roleBattleChicken != null)
		{
			_ = Game.ConfMgr.roleBattleChicken;
		}
		if (Game.ConfMgr.roleRollDiceBet != null)
		{
			_ = Game.ConfMgr.roleRollDiceBet;
		}
		if (Game.ConfMgr.roleAttributeCoefficient != null)
		{
			_ = Game.ConfMgr.roleAttributeCoefficient;
		}
		if (Game.ConfMgr.battleFormulaCoefficient != null)
		{
			_ = Game.ConfMgr.battleFormulaCoefficient;
		}
		if (Game.ConfMgr.roleBasisProgressValue != null)
		{
			_ = Game.ConfMgr.roleBasisProgressValue;
		}
		if (Game.ConfMgr.roleAttributeLimit != null)
		{
			_ = Game.ConfMgr.roleAttributeLimit;
		}
		if (Game.ConfMgr.roleElixirParam != null)
		{
			_ = Game.ConfMgr.roleElixirParam;
		}
		if (Game.ConfMgr.rivalLoveParam != null)
		{
			_ = Game.ConfMgr.rivalLoveParam;
		}
	}

	public static void RandomShitGo()
	{
		if (Game.ConfMgr.roleChat != null)
		{
			_ = Game.ConfMgr.roleChat;
		}
		if (Game.ConfMgr.roleCloseDevelop != null)
		{
			_ = Game.ConfMgr.roleCloseDevelop;
		}
		if (Game.ConfMgr.roleAskfor != null)
		{
			_ = Game.ConfMgr.roleAskfor;
		}
		if (Game.ConfMgr.roleDrill != null)
		{
			_ = Game.ConfMgr.roleDrill;
		}
		if (Game.ConfMgr.roleGive != null)
		{
			_ = Game.ConfMgr.roleGive;
		}
		if (Game.ConfMgr.roleTrains != null)
		{
			_ = Game.ConfMgr.roleTrains;
		}
		if (Game.ConfMgr.roleInvite != null)
		{
			_ = Game.ConfMgr.roleInvite;
		}
	}

	private static void UpdateUITexts()
	{
	}
}
