using Il2CppSystem.Collections.Generic;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.Localization;

namespace MOD_Mivopx;

internal class SceneMapPatch_100Relation
{
	public static ConfRoleChat TalkFunc = Game.ConfMgr.roleChat;

	public static ConfRoleClose CloseFunc = Game.ConfMgr.roleClose;

	public static ConfRoleCloseDevelop DevelopFunc = Game.ConfMgr.roleCloseDevelop;

	public static ConfRoleAskfor RequestFunc = Game.ConfMgr.roleAskfor;

	public static ConfRoleDrill SparFunc = Game.ConfMgr.roleDrill;

	public static ConfRoleGive GifFunc = Game.ConfMgr.roleGive;

	public static ConfRoleTrains DualCultivationFunc = Game.ConfMgr.roleTrains;

	public static ConfRoleInvite InviteFunc = Game.ConfMgr.roleInvite;

	public static ConfRoleConvoy EscortFunc = Game.ConfMgr.roleConvoy;

	public static ConfRoleDiscovery DebateFunc = Game.ConfMgr.roleDiscovery;

	public static ConfRoleAttack AttackFunc = Game.ConfMgr.roleAttack;

	public static ConfRoleSchoolRecruit RecruitmentFunc = Game.ConfMgr.roleSchoolRecruit;

	public static ConfRoleBattle BattleFunc = Game.ConfMgr.roleBattle;

	public static ConfRoleKill KillFunc = Game.ConfMgr.roleKill;

	public static ConfRoleRelation RelationFunc = Game.ConfMgr.roleRelation;

	public static ConfRoleTeachSkill LearnFunc = Game.ConfMgr.roleTeachSkill;

	private static bool _isThisActive;

	public static bool IsThisActive
	{
		get
		{
			return _isThisActive;
		}
		set
		{
			_isThisActive = value;
			if (value)
			{
				if (TalkFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - TalkFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("closeParaB: " + TalkFunc.closeParaB);
					TalkFunc._closeParaB.value = 100;
					ModMain.Log("Modified - closeParaB: " + TalkFunc.closeParaB);
					ModMain.Log("hateParaD: " + TalkFunc.hateParaD);
					TalkFunc._hateParaD.value = -100;
					ModMain.Log("Modified - hateParaD: " + TalkFunc.hateParaD);
					ModMain.Log("costEnergy: " + TalkFunc.costEnergy);
					TalkFunc._costEnergy.value = 0;
					ModMain.Log("Modified - costEnergy: " + TalkFunc.costEnergy);
					ModMain.Log("hateLimited: " + TalkFunc.hateLimited);
					TalkFunc._hateLimited.value = 0;
					ModMain.Log("Modified - hateLimited: " + TalkFunc.hateLimited);
					ModMain.Log("closeLimited: " + TalkFunc.closeLimited);
					TalkFunc._closeLimited.value = 100;
					ModMain.Log("Modified - closeLimited: " + TalkFunc.closeLimited);
				}
				if (CloseFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - CloseFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					float[] array = new float[11]
					{
						100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f,
						100f
					};
					int[] array2 = new int[11]
					{
						0, 0, 0, 0, 0, 0, 300, 300, 300, 300,
						300
					};
					float[] array3 = new float[11];
					float[] array4 = new float[11];
					float[] array5 = new float[11];
					int num = 0;
					List<ConfRoleCloseItem>.Enumerator enumerator = CloseFunc._allConfList.GetEnumerator();
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
				if (DevelopFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - DevelopFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("developValue: " + DevelopFunc.developValue);
					DevelopFunc._developValue.value = 100;
					ModMain.Log("Modified - developValue: " + DevelopFunc.developValue);
					ModMain.Log("closeMin: " + DevelopFunc.closeMin);
					DevelopFunc._closeMin.value = 100;
					ModMain.Log("Modified - closeMin: " + DevelopFunc.closeMin);
				}
				if (RequestFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - RequestFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("costEnergy: " + RequestFunc.costEnergy);
					RequestFunc._costEnergy.value = 0;
					ModMain.Log("Modified - costEnergy: " + RequestFunc.costEnergy);
					ModMain.Log("closeParaA: " + RequestFunc.closeParaA);
					RequestFunc._closeParaA.value = 8;
					ModMain.Log("Modified - closeParaA: " + RequestFunc.closeParaA);
					ModMain.Log("closeParaB: " + RequestFunc.closeParaB);
					RequestFunc._closeParaB.value = 8;
					ModMain.Log("Modified - closeParaB: " + RequestFunc.closeParaB);
					ModMain.Log("closeParaC: " + RequestFunc.closeParaC);
					RequestFunc._closeParaC.value = 8;
					ModMain.Log("Modified - closeParaC: " + RequestFunc.closeParaC);
					ModMain.Log("closeAddMax: " + RequestFunc.closeAddMax);
					RequestFunc._closeAddMax.value = 200;
					ModMain.Log("Modified - closeAddMax: " + RequestFunc.closeAddMax);
					ModMain.Log("closeReduceMax: " + RequestFunc.closeReduceMax);
					RequestFunc._closeReduceMax.value = 0;
					ModMain.Log("Modified - closeReduceMax: " + RequestFunc.closeReduceMax);
					ModMain.Log("favorParaA: " + RequestFunc.favorParaA);
					RequestFunc._favorParaA.value = 896;
					ModMain.Log("Modified - favorParaA: " + RequestFunc.favorParaA);
					ModMain.Log("favorParaB: " + RequestFunc.favorParaB);
					RequestFunc._favorParaB.value = 896;
					ModMain.Log("Modified - favorParaB: " + RequestFunc.favorParaB);
					ModMain.Log("favorParaC: " + RequestFunc.favorParaC);
					RequestFunc._favorParaC.value = 896;
					ModMain.Log("Modified - favorParaC: " + RequestFunc.favorParaC);
				}
				if (SparFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - SparFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("costEnergy: " + SparFunc.costEnergy);
					SparFunc._costEnergy.value = 0;
					ModMain.Log("Modified - costEnergy: " + SparFunc.costEnergy);
					ModMain.Log("addCloseMax: " + SparFunc.addCloseMax);
					SparFunc._addCloseMax.value = 100;
					ModMain.Log("Modified - addCloseMax: " + SparFunc.addCloseMax);
					ModMain.Log("addCloseMin: " + SparFunc.addCloseMin);
					SparFunc._addCloseMin.value = 100;
					ModMain.Log("Modified - addCloseMin: " + SparFunc.addCloseMin);
					ModMain.Log("reduceCloseMax: " + SparFunc.reduceCloseMax);
					SparFunc._reduceCloseMax.value = 0;
					ModMain.Log("Modified - reduceCloseMax: " + SparFunc.reduceCloseMax);
					ModMain.Log("reduceCloseMin: " + SparFunc.reduceCloseMin);
					SparFunc._reduceCloseMin.value = 0;
					ModMain.Log("Modified - reduceCloseMin: " + SparFunc.reduceCloseMin);
				}
				if (GifFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - GifFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("costEnergy: " + GifFunc.costEnergy);
					GifFunc._costEnergy.value = 0;
					ModMain.Log("Modified - costEnergy: " + GifFunc.costEnergy);
					ModMain.Log("closeParaA: " + GifFunc.closeParaA);
					GifFunc._closeParaA.value = 100;
					ModMain.Log("Modified - closeParaA: " + GifFunc.closeParaA);
					ModMain.Log("closeMax: " + GifFunc.closeMax);
					GifFunc._closeMax.value = 100;
					ModMain.Log("Modified - closeMax: " + GifFunc.closeMax);
					ModMain.Log("closeMin: " + GifFunc.closeMin);
					GifFunc._closeMin.value = 100;
					ModMain.Log("Modified - closeMin: " + GifFunc.closeMin);
					ModMain.Log("closeOnceAddMax: " + GifFunc.closeOnceAddMax);
					GifFunc._closeOnceAddMax.value = 100;
					ModMain.Log("Modified - closeOnceAddMax: " + GifFunc.closeOnceAddMax);
				}
				if (DualCultivationFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - DualCultivationFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("expAddPara: " + DualCultivationFunc.expAddPara);
					DualCultivationFunc._expAddPara.value = 500;
					ModMain.Log("Modified - expAddPara: " + DualCultivationFunc.expAddPara);
					ModMain.Log("closeAddMax: " + DualCultivationFunc.closeAddMax);
					DualCultivationFunc._closeAddMax.value = 100;
					ModMain.Log("Modified - closeAddMax: " + DualCultivationFunc.closeAddMax);
					ModMain.Log("closeAddMin: " + DualCultivationFunc.closeAddMin);
					DualCultivationFunc._closeAddMin.value = 100;
					ModMain.Log("Modified - closeAddMin: " + DualCultivationFunc.closeAddMin);
					ModMain.Log("costDay: " + DualCultivationFunc.costDay);
					DualCultivationFunc._costDay.value = 0;
					ModMain.Log("Modified - costDay: " + DualCultivationFunc.costDay);
					ModMain.Log("trainsCD: " + DualCultivationFunc.trainsCD);
					DualCultivationFunc._trainsCD.value = 0;
					ModMain.Log("Modified - trainsCD: " + DualCultivationFunc.trainsCD);
				}
				if (InviteFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - InviteFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("addLuck: " + InviteFunc.addLuck);
					InviteFunc._addLuck.value = 200;
					ModMain.Log("Modified - addLuck: " + InviteFunc.addLuck);
					ModMain.Log("costEnergy: " + InviteFunc.costEnergy);
					InviteFunc._costEnergy.value = 0;
					ModMain.Log("Modified - costEnergy: " + InviteFunc.costEnergy);
					ModMain.Log("cdTime: " + InviteFunc.cdTime);
					InviteFunc._cdTime.value = 0;
					ModMain.Log("Modified - cdTime: " + InviteFunc.cdTime);
					ModMain.Log("intimateDecline: " + InviteFunc.intimateDecline);
					InviteFunc._intimateDecline.value = 0;
					ModMain.Log("Modified - intimateDecline: " + InviteFunc.intimateDecline);
				}
				if (EscortFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - EscortFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("costEnergy: " + EscortFunc.costEnergy);
					EscortFunc._costEnergy.value = 0;
					ModMain.Log("Modified - costEnergy: " + EscortFunc.costEnergy);
				}
				if (DebateFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - DebateFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("discovery: " + DebateFunc.discovery.ToString());
					DebateFunc._discovery.value = "0";
					ModMain.Log("Modified - discovery: " + DebateFunc.discovery.ToString());
					ModMain.Log("costDay: " + DebateFunc.costDay.ToString());
					DebateFunc._costDay.value = "0";
					ModMain.Log("Modified - costDay: " + DebateFunc.costDay.ToString());
					ModMain.Log("closeAddMin: " + DebateFunc.closeAddMin.ToString());
					DebateFunc._closeAddMin.value = "100";
					ModMain.Log("Modified - closeAddMin: " + DebateFunc.closeAddMin.ToString());
					ModMain.Log("closeAddMax: " + DebateFunc.closeAddMax.ToString());
					DebateFunc._closeAddMax.value = "100";
					ModMain.Log("Modified - closeAddMax: " + DebateFunc.closeAddMax.ToString());
					ModMain.Log("closeReduceMin: " + DebateFunc.closeReduceMin.ToString());
					DebateFunc._closeReduceMin.value = "0";
					ModMain.Log("Modified - closeReduceMin: " + DebateFunc.closeReduceMin.ToString());
					ModMain.Log("closeReduceMax: " + DebateFunc.closeReduceMax.ToString());
					DebateFunc._closeReduceMax.value = "0";
					ModMain.Log("Modified - closeReduceMax: " + DebateFunc.closeReduceMax.ToString());
					ModMain.Log("addExpPercent1: " + DebateFunc.addExpPercent1.ToString());
					DebateFunc._addExpPercent1.value = "100";
					ModMain.Log("Modified - addExpPercent1: " + DebateFunc.addExpPercent1.ToString());
					ModMain.Log("addExpPercent2: " + DebateFunc.addExpPercent2.ToString());
					DebateFunc._addExpPercent2.value = "100";
					ModMain.Log("Modified - addExpPercent2: " + DebateFunc.addExpPercent2.ToString());
					ModMain.Log("addExpPercent3: " + DebateFunc.addExpPercent3.ToString());
					DebateFunc._addExpPercent3.value = "100";
					ModMain.Log("Modified - addExpPercent3: " + DebateFunc.addExpPercent3.ToString());
				}
				if (AttackFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - AttackFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("costEnergy: " + AttackFunc.costEnergy);
					AttackFunc._costEnergy.value = 0;
					ModMain.Log("Modified - costEnergy: " + AttackFunc.costEnergy);
				}
				if (RecruitmentFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - RecruitmentFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("costEnergy: " + RecruitmentFunc.energyCost);
					RecruitmentFunc._energyCost.value = 0;
					ModMain.Log("Modified - costEnergy: " + RecruitmentFunc.energyCost);
				}
				if (BattleFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - BattleFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("hateAddMin: " + BattleFunc.hateAddMin);
					BattleFunc._hateAddMin.value = 0;
					ModMain.Log("Modified - hateAddMin: " + BattleFunc.hateAddMin);
					ModMain.Log("hateAddMax: " + BattleFunc.hateAddMax);
					BattleFunc._hateAddMax.value = 0;
					ModMain.Log("Modified - hateAddMax: " + BattleFunc.hateAddMax);
					ModMain.Log("hateLimited: " + BattleFunc.hateLimited);
					BattleFunc._hateLimited.value = 0;
					ModMain.Log("Modified - hateLimited: " + BattleFunc.hateLimited);
				}
				if (KillFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - KillFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("hateAddKill " + KillFunc.hateAddKill);
					KillFunc._hateAddKill.value = 1;
					ModMain.Log("Modified - hateAddKill " + KillFunc.hateAddKill);
					ModMain.Log("reputationReducePara " + KillFunc.reputationReducePara);
					KillFunc._reputationReducePara.value = -1;
					ModMain.Log("Modified - reputationReducePara " + KillFunc.reputationReducePara);
				}
				if (RelationFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - RelationFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					List<ConfRoleRelationItem>.Enumerator enumerator2 = RelationFunc._allConfList.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						ConfRoleRelationItem current2 = enumerator2.Current;
						if (current2 != null)
						{
							ModMain.Log($"addClose: {current2.addClose}");
							current2.addClose = 100;
							ModMain.Log($"Modified - addClose: {current2.addClose}");
							ModMain.Log($"addCloseFamily: {current2.addCloseFamily}");
							current2.addCloseFamily = 100;
							ModMain.Log($"Modified - addCloseFamily: {current2.addCloseFamily}");
							ModMain.Log($"reduceClose: {current2.reduceClose}");
							current2.reduceClose = 0;
							ModMain.Log($"Modified - reduceClose: {current2.reduceClose}");
						}
					}
				}
				if (LearnFunc != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - LearnFunc - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("costDay " + LearnFunc.costDay);
					LearnFunc._costDay.value = 0;
					ModMain.Log("Modified - costDay " + LearnFunc.costDay);
				}
				ModMain.LogTip(LocalizationHelper.T("other_msgs_100_intim_no_cost_enabled"));
				return;
			}
			if (TalkFunc != null)
			{
				ModMain.Log("closeParaB: " + TalkFunc.closeParaB);
				TalkFunc._closeParaB.value = 15;
				ModMain.Log("Modified - closeParaB: " + TalkFunc.closeParaB);
				ModMain.Log("hateParaD: " + TalkFunc.hateParaD);
				TalkFunc._hateParaD.value = 15;
				ModMain.Log("Modified - hateParaD: " + TalkFunc.hateParaD);
				ModMain.Log("costEnergy: " + TalkFunc.costEnergy);
				TalkFunc._costEnergy.value = 5;
				ModMain.Log("Modified - costEnergy: " + TalkFunc.costEnergy);
				ModMain.Log("hateLimited: " + TalkFunc.hateLimited);
				TalkFunc._hateLimited.value = -30;
				ModMain.Log("Modified - hateLimited: " + TalkFunc.hateLimited);
				ModMain.Log("closeLimited: " + TalkFunc.closeLimited);
				TalkFunc._closeLimited.value = 60;
				ModMain.Log("Modified - closeLimited: " + TalkFunc.closeLimited);
			}
			if (CloseFunc != null)
			{
				float[] array6 = new float[11]
				{
					6.25f, 12.5f, 25f, 50f, 100f, 100f, 50f, 25f, 12.5f, 6.25f,
					6.25f
				};
				int[] array7 = new int[11]
				{
					-300, -240, -180, -120, -60, 0, 60, 120, 180, 240,
					300
				};
				float[] array8 = new float[11]
				{
					6.25f, 12.5f, 25f, 50f, 100f, 100f, 50f, 25f, 12.5f, 6.25f,
					6.25f
				};
				float[] array9 = new float[11]
				{
					6.25f, 12.5f, 25f, 50f, 100f, 100f, 50f, 25f, 12.5f, 6.25f,
					6.25f
				};
				float[] array10 = new float[11]
				{
					6.25f, 12.5f, 25f, 50f, 100f, 100f, 50f, 25f, 12.5f, 6.25f,
					6.25f
				};
				int num2 = 0;
				List<ConfRoleCloseItem>.Enumerator enumerator = CloseFunc._allConfList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ConfRoleCloseItem current3 = enumerator.Current;
					if (current3 != null && num2 < array6.Length)
					{
						current3.closeIncrease = array6[num2];
						current3.closeMin = array7[num2];
						current3.closeReduce = array8[num2];
						current3.hateIncrease = array9[num2];
						current3.hateReduce = array10[num2];
					}
					num2++;
				}
			}
			if (DevelopFunc != null)
			{
				ModMain.Log("developValue: " + DevelopFunc.developValue);
				DevelopFunc._developValue.value = 40;
				ModMain.Log("Modified - developValue: " + DevelopFunc.developValue);
				ModMain.Log("closeMin: " + DevelopFunc.closeMin);
				DevelopFunc._closeMin.value = 60;
				ModMain.Log("Modified - closeMin: " + DevelopFunc.closeMin);
			}
			if (RequestFunc != null)
			{
				ModMain.Log("costEnergy: " + RequestFunc.costEnergy);
				RequestFunc._costEnergy.value = 10;
				ModMain.Log("Modified - costEnergy: " + RequestFunc.costEnergy);
				ModMain.Log("closeParaA: " + RequestFunc.closeParaA);
				RequestFunc._closeParaA.value = 8;
				ModMain.Log("Modified - closeParaA: " + RequestFunc.closeParaA);
				ModMain.Log("closeParaB: " + RequestFunc.closeParaB);
				RequestFunc._closeParaB.value = -8;
				ModMain.Log("Modified - closeParaB: " + RequestFunc.closeParaB);
				ModMain.Log("closeParaC: " + RequestFunc.closeParaC);
				RequestFunc._closeParaC.value = -25;
				ModMain.Log("Modified - closeParaC: " + RequestFunc.closeParaC);
				ModMain.Log("closeAddMax: " + RequestFunc.closeAddMax);
				RequestFunc._closeAddMax.value = 120;
				ModMain.Log("Modified - closeAddMax: " + RequestFunc.closeAddMax);
				ModMain.Log("closeReduceMax: " + RequestFunc.closeReduceMax);
				RequestFunc._closeReduceMax.value = 30;
				ModMain.Log("Modified - closeReduceMax: " + RequestFunc.closeReduceMax);
				ModMain.Log("favorParaA: " + RequestFunc.favorParaA);
				RequestFunc._favorParaA.value = 896;
				ModMain.Log("Modified - favorParaA: " + RequestFunc.favorParaA);
				ModMain.Log("favorParaB: " + RequestFunc.favorParaB);
				RequestFunc._favorParaB.value = 160;
				ModMain.Log("Modified - favorParaB: " + RequestFunc.favorParaB);
				ModMain.Log("favorParaC: " + RequestFunc.favorParaC);
				RequestFunc._favorParaC.value = 30;
				ModMain.Log("Modified - favorParaC: " + RequestFunc.favorParaC);
			}
			if (SparFunc != null)
			{
				ModMain.Log("costEnergy: " + SparFunc.costEnergy);
				SparFunc._costEnergy.value = 30;
				ModMain.Log("Modified - costEnergy: " + SparFunc.costEnergy);
				ModMain.Log("addCloseMax: " + SparFunc.addCloseMax);
				SparFunc._addCloseMax.value = 10;
				ModMain.Log("Modified - addCloseMax: " + SparFunc.addCloseMax);
				ModMain.Log("addCloseMin: " + SparFunc.addCloseMin);
				SparFunc._addCloseMin.value = 5;
				ModMain.Log("Modified - addCloseMin: " + SparFunc.addCloseMin);
				ModMain.Log("reduceCloseMax: " + SparFunc.reduceCloseMax);
				SparFunc._reduceCloseMax.value = -3;
				ModMain.Log("Modified - reduceCloseMax: " + SparFunc.reduceCloseMax);
				ModMain.Log("reduceCloseMin: " + SparFunc.reduceCloseMin);
				SparFunc._reduceCloseMin.value = -6;
				ModMain.Log("Modified - reduceCloseMin: " + SparFunc.reduceCloseMin);
			}
			if (GifFunc != null)
			{
				ModMain.Log("costEnergy: " + GifFunc.costEnergy);
				GifFunc._costEnergy.value = 10;
				ModMain.Log("Modified - costEnergy: " + GifFunc.costEnergy);
				ModMain.Log("closeParaA: " + GifFunc.closeParaA);
				GifFunc._closeParaA.value = 15;
				ModMain.Log("Modified - closeParaA: " + GifFunc.closeParaA);
				ModMain.Log("closeMax: " + GifFunc.closeMax);
				GifFunc._closeMax.value = 0;
				ModMain.Log("Modified - closeMax: " + GifFunc.closeMax);
				ModMain.Log("closeMin: " + GifFunc.closeMin);
				GifFunc._closeMin.value = 0;
				ModMain.Log("Modified - closeMin: " + GifFunc.closeMin);
				ModMain.Log("closeOnceAddMax: " + GifFunc.closeOnceAddMax);
				GifFunc._closeOnceAddMax.value = 60;
				ModMain.Log("Modified - closeOnceAddMax: " + GifFunc.closeOnceAddMax);
			}
			if (DualCultivationFunc != null)
			{
				ModMain.Log("expAddPara: " + DualCultivationFunc.expAddPara);
				DualCultivationFunc._expAddPara.value = 200;
				ModMain.Log("Modified - expAddPara: " + DualCultivationFunc.expAddPara);
				ModMain.Log("closeAddMax: " + DualCultivationFunc.closeAddMax);
				DualCultivationFunc._closeAddMax.value = 20;
				ModMain.Log("Modified - closeAddMax: " + DualCultivationFunc.closeAddMax);
				ModMain.Log("closeAddMin: " + DualCultivationFunc.closeAddMin);
				DualCultivationFunc._closeAddMin.value = 10;
				ModMain.Log("Modified - closeAddMin: " + DualCultivationFunc.closeAddMin);
				ModMain.Log("costDay: " + DualCultivationFunc.costDay);
				DualCultivationFunc._costDay.value = 10;
				ModMain.Log("Modified - costDay: " + DualCultivationFunc.costDay);
				ModMain.Log("trainsCD: " + DualCultivationFunc.trainsCD);
				DualCultivationFunc._trainsCD.value = 6;
				ModMain.Log("Modified - trainsCD: " + DualCultivationFunc.trainsCD);
			}
			if (InviteFunc != null)
			{
				ModMain.Log("addLuck: " + InviteFunc.addLuck);
				InviteFunc._addLuck.value = 140;
				ModMain.Log("Modified - addLuck: " + InviteFunc.addLuck);
				ModMain.Log("costEnergy: " + InviteFunc.costEnergy);
				InviteFunc._costEnergy.value = 10;
				ModMain.Log("Modified - costEnergy: " + InviteFunc.costEnergy);
				ModMain.Log("cdTime: " + InviteFunc.cdTime);
				InviteFunc._cdTime.value = 6;
				ModMain.Log("Modified - cdTime: " + InviteFunc.cdTime);
				ModMain.Log("intimateDecline: " + InviteFunc.intimateDecline);
				InviteFunc._intimateDecline.value = -120;
				ModMain.Log("Modified - intimateDecline: " + InviteFunc.intimateDecline);
			}
			if (EscortFunc != null)
			{
				ModMain.Log("costEnergy: " + EscortFunc.costEnergy);
				EscortFunc._costEnergy.value = 10;
				ModMain.Log("Modified - costEnergy: " + EscortFunc.costEnergy);
			}
			if (DebateFunc != null)
			{
				ModMain.Log("discovery: " + DebateFunc.discovery.ToString());
				DebateFunc._discovery.value = "12";
				ModMain.Log("Modified - discovery: " + DebateFunc.discovery.ToString());
				ModMain.Log("costDay: " + DebateFunc.costDay.ToString());
				DebateFunc._costDay.value = "5";
				ModMain.Log("Modified - costDay: " + DebateFunc.costDay.ToString());
				ModMain.Log("closeAddMin: " + DebateFunc.closeAddMin.ToString());
				DebateFunc._closeAddMin.value = "5";
				ModMain.Log("Modified - closeAddMin: " + DebateFunc.closeAddMin.ToString());
				ModMain.Log("closeAddMax: " + DebateFunc.closeAddMax.ToString());
				DebateFunc._closeAddMax.value = "10";
				ModMain.Log("Modified - closeAddMax: " + DebateFunc.closeAddMax.ToString());
				ModMain.Log("closeReduceMin: " + DebateFunc.closeReduceMin.ToString());
				DebateFunc._closeReduceMin.value = "-5";
				ModMain.Log("Modified - closeReduceMin: " + DebateFunc.closeReduceMin.ToString());
				ModMain.Log("closeReduceMax: " + DebateFunc.closeReduceMax.ToString());
				DebateFunc._closeReduceMax.value = "-5";
				ModMain.Log("Modified - closeReduceMax: " + DebateFunc.closeReduceMax.ToString());
				ModMain.Log("addExpPercent1: " + DebateFunc.addExpPercent1.ToString());
				DebateFunc._addExpPercent1.value = "100";
				ModMain.Log("Modified - addExpPercent1: " + DebateFunc.addExpPercent1.ToString());
				ModMain.Log("addExpPercent2: " + DebateFunc.addExpPercent2.ToString());
				DebateFunc._addExpPercent2.value = "35";
				ModMain.Log("Modified - addExpPercent2: " + DebateFunc.addExpPercent2.ToString());
				ModMain.Log("addExpPercent3: " + DebateFunc.addExpPercent3.ToString());
				DebateFunc._addExpPercent3.value = "65";
				ModMain.Log("Modified - addExpPercent3: " + DebateFunc.addExpPercent3.ToString());
			}
			if (AttackFunc != null)
			{
				ModMain.Log("costEnergy: " + AttackFunc.costEnergy);
				AttackFunc._costEnergy.value = 10;
				ModMain.Log("Modified - costEnergy: " + AttackFunc.costEnergy);
			}
			if (RecruitmentFunc != null)
			{
				ModMain.Log("costEnergy: " + RecruitmentFunc.energyCost);
				RecruitmentFunc._energyCost.value = 10;
				ModMain.Log("Modified - costEnergy: " + RecruitmentFunc.energyCost);
			}
			if (BattleFunc != null)
			{
				ModMain.Log("hateAddMin: " + BattleFunc.hateAddMin);
				BattleFunc._hateAddMin.value = 60;
				ModMain.Log("Modified - hateAddMin: " + BattleFunc.hateAddMin);
				ModMain.Log("hateAddMax: " + BattleFunc.hateAddMax);
				BattleFunc._hateAddMax.value = 60;
				ModMain.Log("Modified - hateAddMax: " + BattleFunc.hateAddMax);
				ModMain.Log("hateLimited: " + BattleFunc.hateLimited);
				BattleFunc._hateLimited.value = -999;
				ModMain.Log("Modified - hateLimited: " + BattleFunc.hateLimited);
			}
			if (KillFunc != null)
			{
				ModMain.Log("hateAddKill " + KillFunc.hateAddKill);
				KillFunc._hateAddKill.value = 500;
				ModMain.Log("Modified - hateAddKill " + KillFunc.hateAddKill);
				ModMain.Log("reputationReducePara " + KillFunc.reputationReducePara);
				KillFunc._reputationReducePara.value = -250;
				ModMain.Log("Modified - reputationReducePara " + KillFunc.reputationReducePara);
			}
			if (RelationFunc != null)
			{
				List<ConfRoleRelationItem>.Enumerator enumerator2 = RelationFunc._allConfList.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					ConfRoleRelationItem current4 = enumerator2.Current;
					if (current4 != null)
					{
						ModMain.Log($"addClose: {current4.addClose}");
						current4.addClose = 60;
						ModMain.Log($"Modified - addClose: {current4.addClose}");
						ModMain.Log($"addCloseFamily: {current4.addCloseFamily}");
						current4.addCloseFamily = 30;
						ModMain.Log($"Modified - addCloseFamily: {current4.addCloseFamily}");
						ModMain.Log($"reduceClose: {current4.reduceClose}");
						current4.reduceClose = -30;
						ModMain.Log($"Modified - reduceClose: {current4.reduceClose}");
					}
				}
			}
			if (LearnFunc != null)
			{
				ModMain.Log("costDay " + LearnFunc.costDay);
				LearnFunc._costDay.value = 15;
				ModMain.Log("Modified - costDay " + LearnFunc.costDay);
			}
			ModMain.LogTip(LocalizationHelper.T("other_msgs_100_intim_no_cost_disabled"));
			ModMain.LogTip(LocalizationHelper.T("other_msgs_restored_original_values"), null, 3f);
		}
	}
}
