using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TaleOfImmortalCheat;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UILoadingBar), "Init")]
internal class SceneMapPatch_UILoadingBar
{
	public static UILoadingBar _instance;

	private static void Postfix(UILoadingBar __instance)
	{
		_instance = __instance;
		if (!(_instance != null))
		{
			return;
		}
		ModMain.Log("Loading UI Initializing...");
		if (SceneMapPatch_ConfRoleStealItem.IsStealActive)
		{
			ModMain.Log("Resetting 'Steal Always Succeed' Values.");
			ConfRoleStealItem roleStealItem = Game.ConfMgr.roleStealItem;
			if (roleStealItem != null)
			{
				ModMain.Log("stealCostEnergy: " + roleStealItem.stealCostEnergy);
				roleStealItem._stealCostEnergy.value = 10;
				ModMain.Log("Modified - stealCostEnergy: " + roleStealItem.stealCostEnergy);
				ModMain.Log("stealSucceedMin: " + roleStealItem.stealSucceedMin);
				roleStealItem._stealSucceedMin.value = 10;
				ModMain.Log("Modified - stealSucceedMin: " + roleStealItem.stealSucceedMin);
				ModMain.Log("stealSucceedMax: " + roleStealItem.stealSucceedMax);
				roleStealItem._stealSucceedMax.value = 90;
				ModMain.Log("Modified - stealSucceedMax: " + roleStealItem.stealSucceedMax);
				ModMain.Log("exposeReduceClosePara: " + roleStealItem.exposeReduceClosePara);
				roleStealItem._exposeReduceClosePara.value = 100;
				ModMain.Log("Modified - exposeReduceClosePara: " + roleStealItem.exposeReduceClosePara);
				ModMain.Log("hateAddPara: " + roleStealItem.hateAddPara);
				roleStealItem._hateAddPara.value = 10;
				ModMain.Log("Modified - hateAddPara: " + roleStealItem.hateAddPara);
				ModMain.Log("hateLimited: " + roleStealItem.hateLimited);
				roleStealItem._hateLimited.value = -999;
				ModMain.Log("Modified - hateLimited: " + roleStealItem.hateLimited);
				ModMain.Log("stealMoneyPercentMax: " + roleStealItem.stealMoneyPercentMax);
				roleStealItem._stealMoneyPercentMax.value = 35;
				ModMain.Log("Modified - stealMoneyPercentMax: " + roleStealItem.stealMoneyPercentMax);
				ModMain.Log("stealMoneyPercentMin: " + roleStealItem.stealMoneyPercentMin);
				roleStealItem._stealMoneyPercentMin.value = 20;
				ModMain.Log("Modified - stealMoneyPercentMin: " + roleStealItem.stealMoneyPercentMin);
				ModMain.Log("stealMoneySpMax: " + roleStealItem.stealMoneySpMax);
				roleStealItem._stealMoneySpMax.value = 100;
				ModMain.Log("Modified - stealMoneySpMax: " + roleStealItem.stealMoneySpMax);
				ModMain.Log("stealMoneySpMin: " + roleStealItem.stealMoneySpMin);
				roleStealItem._stealMoneySpMin.value = 70;
				ModMain.Log("Modified - stealMoneySpMin: " + roleStealItem.stealMoneySpMin);
			}
		}
		if (!SceneMapPatch_100Relation.IsThisActive)
		{
			return;
		}
		ModMain.Log("Resetting '100% Intim & No Hate/Stamina/Energy/Day Cost/CD' Values.");
		ConfRoleChat roleChat = Game.ConfMgr.roleChat;
		ConfRoleClose roleClose = Game.ConfMgr.roleClose;
		ConfRoleCloseDevelop roleCloseDevelop = Game.ConfMgr.roleCloseDevelop;
		ConfRoleAskfor roleAskfor = Game.ConfMgr.roleAskfor;
		ConfRoleDrill roleDrill = Game.ConfMgr.roleDrill;
		ConfRoleGive roleGive = Game.ConfMgr.roleGive;
		ConfRoleTrains roleTrains = Game.ConfMgr.roleTrains;
		ConfRoleInvite roleInvite = Game.ConfMgr.roleInvite;
		ConfRoleConvoy roleConvoy = Game.ConfMgr.roleConvoy;
		ConfRoleDiscovery roleDiscovery = Game.ConfMgr.roleDiscovery;
		ConfRoleAttack roleAttack = Game.ConfMgr.roleAttack;
		ConfRoleSchoolRecruit roleSchoolRecruit = Game.ConfMgr.roleSchoolRecruit;
		ConfRoleBattle roleBattle = Game.ConfMgr.roleBattle;
		ConfRoleKill roleKill = Game.ConfMgr.roleKill;
		ConfRoleRelation roleRelation = Game.ConfMgr.roleRelation;
		ConfRoleTeachSkill roleTeachSkill = Game.ConfMgr.roleTeachSkill;
		if (roleChat != null)
		{
			ModMain.Log("closeParaB: " + roleChat.closeParaB);
			roleChat._closeParaB.value = 15;
			ModMain.Log("Modified - closeParaB: " + roleChat.closeParaB);
			ModMain.Log("hateParaD: " + roleChat.hateParaD);
			roleChat._hateParaD.value = 15;
			ModMain.Log("Modified - hateParaD: " + roleChat.hateParaD);
			ModMain.Log("costEnergy: " + roleChat.costEnergy);
			roleChat._costEnergy.value = 5;
			ModMain.Log("Modified - costEnergy: " + roleChat.costEnergy);
			ModMain.Log("hateLimited: " + roleChat.hateLimited);
			roleChat._hateLimited.value = -30;
			ModMain.Log("Modified - hateLimited: " + roleChat.hateLimited);
			ModMain.Log("closeLimited: " + roleChat.closeLimited);
			roleChat._closeLimited.value = 60;
			ModMain.Log("Modified - closeLimited: " + roleChat.closeLimited);
		}
		if (roleClose != null)
		{
			float[] array = new float[11]
			{
				6.25f, 12.5f, 25f, 50f, 100f, 100f, 50f, 25f, 12.5f, 6.25f,
				6.25f
			};
			int[] array2 = new int[11]
			{
				-300, -240, -180, -120, -60, 0, 60, 120, 180, 240,
				300
			};
			float[] array3 = new float[11]
			{
				6.25f, 12.5f, 25f, 50f, 100f, 100f, 50f, 25f, 12.5f, 6.25f,
				6.25f
			};
			float[] array4 = new float[11]
			{
				6.25f, 12.5f, 25f, 50f, 100f, 100f, 50f, 25f, 12.5f, 6.25f,
				6.25f
			};
			float[] array5 = new float[11]
			{
				6.25f, 12.5f, 25f, 50f, 100f, 100f, 50f, 25f, 12.5f, 6.25f,
				6.25f
			};
			int num = 0;
			List<ConfRoleCloseItem>.Enumerator enumerator = roleClose._allConfList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ConfRoleCloseItem current = enumerator.Current;
				if (current != null && num < array.Length)
				{
					current.closeIncrease = array[num];
					current.closeMin = array2[num];
					current.closeReduce = array3[num];
					current.hateIncrease = array4[num];
					current.hateReduce = array5[num];
				}
				num++;
			}
		}
		if (roleCloseDevelop != null)
		{
			ModMain.Log("developValue: " + roleCloseDevelop.developValue);
			roleCloseDevelop._developValue.value = 40;
			ModMain.Log("Modified - developValue: " + roleCloseDevelop.developValue);
			ModMain.Log("closeMin: " + roleCloseDevelop.closeMin);
			roleCloseDevelop._closeMin.value = 60;
			ModMain.Log("Modified - closeMin: " + roleCloseDevelop.closeMin);
		}
		if (roleAskfor != null)
		{
			ModMain.Log("costEnergy: " + roleAskfor.costEnergy);
			roleAskfor._costEnergy.value = 10;
			ModMain.Log("Modified - costEnergy: " + roleAskfor.costEnergy);
			ModMain.Log("closeParaA: " + roleAskfor.closeParaA);
			roleAskfor._closeParaA.value = 8;
			ModMain.Log("Modified - closeParaA: " + roleAskfor.closeParaA);
			ModMain.Log("closeParaB: " + roleAskfor.closeParaB);
			roleAskfor._closeParaB.value = -8;
			ModMain.Log("Modified - closeParaB: " + roleAskfor.closeParaB);
			ModMain.Log("closeParaC: " + roleAskfor.closeParaC);
			roleAskfor._closeParaC.value = -25;
			ModMain.Log("Modified - closeParaC: " + roleAskfor.closeParaC);
			ModMain.Log("closeAddMax: " + roleAskfor.closeAddMax);
			roleAskfor._closeAddMax.value = 120;
			ModMain.Log("Modified - closeAddMax: " + roleAskfor.closeAddMax);
			ModMain.Log("closeReduceMax: " + roleAskfor.closeReduceMax);
			roleAskfor._closeReduceMax.value = 30;
			ModMain.Log("Modified - closeReduceMax: " + roleAskfor.closeReduceMax);
			ModMain.Log("favorParaA: " + roleAskfor.favorParaA);
			roleAskfor._favorParaA.value = 896;
			ModMain.Log("Modified - favorParaA: " + roleAskfor.favorParaA);
			ModMain.Log("favorParaB: " + roleAskfor.favorParaB);
			roleAskfor._favorParaB.value = 160;
			ModMain.Log("Modified - favorParaB: " + roleAskfor.favorParaB);
			ModMain.Log("favorParaC: " + roleAskfor.favorParaC);
			roleAskfor._favorParaC.value = 30;
			ModMain.Log("Modified - favorParaC: " + roleAskfor.favorParaC);
		}
		if (roleDrill != null)
		{
			ModMain.Log("costEnergy: " + roleDrill.costEnergy);
			roleDrill._costEnergy.value = 30;
			ModMain.Log("Modified - costEnergy: " + roleDrill.costEnergy);
			ModMain.Log("addCloseMax: " + roleDrill.addCloseMax);
			roleDrill._addCloseMax.value = 10;
			ModMain.Log("Modified - addCloseMax: " + roleDrill.addCloseMax);
			ModMain.Log("addCloseMin: " + roleDrill.addCloseMin);
			roleDrill._addCloseMin.value = 5;
			ModMain.Log("Modified - addCloseMin: " + roleDrill.addCloseMin);
			ModMain.Log("reduceCloseMax: " + roleDrill.reduceCloseMax);
			roleDrill._reduceCloseMax.value = -3;
			ModMain.Log("Modified - reduceCloseMax: " + roleDrill.reduceCloseMax);
			ModMain.Log("reduceCloseMin: " + roleDrill.reduceCloseMin);
			roleDrill._reduceCloseMin.value = -6;
			ModMain.Log("Modified - reduceCloseMin: " + roleDrill.reduceCloseMin);
		}
		if (roleGive != null)
		{
			ModMain.Log("costEnergy: " + roleGive.costEnergy);
			roleGive._costEnergy.value = 10;
			ModMain.Log("Modified - costEnergy: " + roleGive.costEnergy);
			ModMain.Log("closeParaA: " + roleGive.closeParaA);
			roleGive._closeParaA.value = 15;
			ModMain.Log("Modified - closeParaA: " + roleGive.closeParaA);
			ModMain.Log("closeMax: " + roleGive.closeMax);
			roleGive._closeMax.value = 0;
			ModMain.Log("Modified - closeMax: " + roleGive.closeMax);
			ModMain.Log("closeMin: " + roleGive.closeMin);
			roleGive._closeMin.value = 0;
			ModMain.Log("Modified - closeMin: " + roleGive.closeMin);
			ModMain.Log("closeOnceAddMax: " + roleGive.closeOnceAddMax);
			roleGive._closeOnceAddMax.value = 60;
			ModMain.Log("Modified - closeOnceAddMax: " + roleGive.closeOnceAddMax);
		}
		if (roleTrains != null)
		{
			ModMain.Log("expAddPara: " + roleTrains.expAddPara);
			roleTrains._expAddPara.value = 200;
			ModMain.Log("Modified - expAddPara: " + roleTrains.expAddPara);
			ModMain.Log("closeAddMax: " + roleTrains.closeAddMax);
			roleTrains._closeAddMax.value = 20;
			ModMain.Log("Modified - closeAddMax: " + roleTrains.closeAddMax);
			ModMain.Log("closeAddMin: " + roleTrains.closeAddMin);
			roleTrains._closeAddMin.value = 10;
			ModMain.Log("Modified - closeAddMin: " + roleTrains.closeAddMin);
			ModMain.Log("costDay: " + roleTrains.costDay);
			roleTrains._costDay.value = 10;
			ModMain.Log("Modified - costDay: " + roleTrains.costDay);
			ModMain.Log("trainsCD: " + roleTrains.trainsCD);
			roleTrains._trainsCD.value = 6;
			ModMain.Log("Modified - trainsCD: " + roleTrains.trainsCD);
		}
		if (roleInvite != null)
		{
			ModMain.Log("addLuck: " + roleInvite.addLuck);
			roleInvite._addLuck.value = 140;
			ModMain.Log("Modified - addLuck: " + roleInvite.addLuck);
			ModMain.Log("costEnergy: " + roleInvite.costEnergy);
			roleInvite._costEnergy.value = 10;
			ModMain.Log("Modified - costEnergy: " + roleInvite.costEnergy);
			ModMain.Log("cdTime: " + roleInvite.cdTime);
			roleInvite._cdTime.value = 6;
			ModMain.Log("Modified - cdTime: " + roleInvite.cdTime);
			ModMain.Log("intimateDecline: " + roleInvite.intimateDecline);
			roleInvite._intimateDecline.value = -120;
			ModMain.Log("Modified - intimateDecline: " + roleInvite.intimateDecline);
		}
		if (roleConvoy != null)
		{
			ModMain.Log("costEnergy: " + roleConvoy.costEnergy);
			roleConvoy._costEnergy.value = 10;
			ModMain.Log("Modified - costEnergy: " + roleConvoy.costEnergy);
		}
		if (roleDiscovery != null)
		{
			ModMain.Log("discovery: " + roleDiscovery.discovery.ToString());
			roleDiscovery._discovery.value = "12";
			ModMain.Log("Modified - discovery: " + roleDiscovery.discovery.ToString());
			ModMain.Log("costDay: " + roleDiscovery.costDay.ToString());
			roleDiscovery._costDay.value = "5";
			ModMain.Log("Modified - costDay: " + roleDiscovery.costDay.ToString());
			ModMain.Log("closeAddMin: " + roleDiscovery.closeAddMin.ToString());
			roleDiscovery._closeAddMin.value = "5";
			ModMain.Log("Modified - closeAddMin: " + roleDiscovery.closeAddMin.ToString());
			ModMain.Log("closeAddMax: " + roleDiscovery.closeAddMax.ToString());
			roleDiscovery._closeAddMax.value = "10";
			ModMain.Log("Modified - closeAddMax: " + roleDiscovery.closeAddMax.ToString());
			ModMain.Log("closeReduceMin: " + roleDiscovery.closeReduceMin.ToString());
			roleDiscovery._closeReduceMin.value = "-5";
			ModMain.Log("Modified - closeReduceMin: " + roleDiscovery.closeReduceMin.ToString());
			ModMain.Log("closeReduceMax: " + roleDiscovery.closeReduceMax.ToString());
			roleDiscovery._closeReduceMax.value = "-5";
			ModMain.Log("Modified - closeReduceMax: " + roleDiscovery.closeReduceMax.ToString());
			ModMain.Log("addExpPercent1: " + roleDiscovery.addExpPercent1.ToString());
			roleDiscovery._addExpPercent1.value = "100";
			ModMain.Log("Modified - addExpPercent1: " + roleDiscovery.addExpPercent1.ToString());
			ModMain.Log("addExpPercent2: " + roleDiscovery.addExpPercent2.ToString());
			roleDiscovery._addExpPercent2.value = "35";
			ModMain.Log("Modified - addExpPercent2: " + roleDiscovery.addExpPercent2.ToString());
			ModMain.Log("addExpPercent3: " + roleDiscovery.addExpPercent3.ToString());
			roleDiscovery._addExpPercent3.value = "65";
			ModMain.Log("Modified - addExpPercent3: " + roleDiscovery.addExpPercent3.ToString());
		}
		if (roleAttack != null)
		{
			ModMain.Log("costEnergy: " + roleAttack.costEnergy);
			roleAttack._costEnergy.value = 10;
			ModMain.Log("Modified - costEnergy: " + roleAttack.costEnergy);
		}
		if (roleSchoolRecruit != null)
		{
			ModMain.Log("costEnergy: " + roleSchoolRecruit.energyCost);
			roleSchoolRecruit._energyCost.value = 10;
			ModMain.Log("Modified - costEnergy: " + roleSchoolRecruit.energyCost);
		}
		if (roleBattle != null)
		{
			ModMain.Log("hateAddMin: " + roleBattle.hateAddMin);
			roleBattle._hateAddMin.value = 60;
			ModMain.Log("Modified - hateAddMin: " + roleBattle.hateAddMin);
			ModMain.Log("hateAddMax: " + roleBattle.hateAddMax);
			roleBattle._hateAddMax.value = 60;
			ModMain.Log("Modified - hateAddMax: " + roleBattle.hateAddMax);
			ModMain.Log("hateLimited: " + roleBattle.hateLimited);
			roleBattle._hateLimited.value = -999;
			ModMain.Log("Modified - hateLimited: " + roleBattle.hateLimited);
		}
		if (roleKill != null)
		{
			ModMain.Log("hateAddKill " + roleKill.hateAddKill);
			roleKill._hateAddKill.value = 500;
			ModMain.Log("Modified - hateAddKill " + roleKill.hateAddKill);
			ModMain.Log("reputationReducePara " + roleKill.reputationReducePara);
			roleKill._reputationReducePara.value = -250;
			ModMain.Log("Modified - reputationReducePara " + roleKill.reputationReducePara);
		}
		if (roleRelation != null)
		{
			List<ConfRoleRelationItem>.Enumerator enumerator2 = roleRelation._allConfList.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				ConfRoleRelationItem current2 = enumerator2.Current;
				if (current2 != null)
				{
					ModMain.Log($"addClose: {current2.addClose}");
					current2.addClose = 60;
					ModMain.Log($"Modified - addClose: {current2.addClose}");
					ModMain.Log($"addCloseFamily: {current2.addCloseFamily}");
					current2.addCloseFamily = 30;
					ModMain.Log($"Modified - addCloseFamily: {current2.addCloseFamily}");
					ModMain.Log($"reduceClose: {current2.reduceClose}");
					current2.reduceClose = -30;
					ModMain.Log($"Modified - reduceClose: {current2.reduceClose}");
				}
			}
		}
		if (roleTeachSkill != null)
		{
			ModMain.Log("costDay " + roleTeachSkill.costDay);
			roleTeachSkill._costDay.value = 15;
			ModMain.Log("Modified - costDay " + roleTeachSkill.costDay);
		}
	}
}
