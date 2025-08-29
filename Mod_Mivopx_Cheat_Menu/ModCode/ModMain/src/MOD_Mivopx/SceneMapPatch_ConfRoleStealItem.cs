using TaleOfImmortalCheat;
using TaleOfImmortalCheat.Localization;

namespace MOD_Mivopx;

internal class SceneMapPatch_ConfRoleStealItem
{
	public static ConfRoleStealItem StealItem = Game.ConfMgr.roleStealItem;

	private static bool _isStealActive;

	public static bool IsStealActive
	{
		get
		{
			return _isStealActive;
		}
		set
		{
			_isStealActive = value;
			if (value)
			{
				if (StealItem != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - StealItem - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("stealCostEnergy: " + StealItem.stealCostEnergy);
					StealItem._stealCostEnergy.value = 0;
					ModMain.Log("Modified - stealCostEnergy: " + StealItem.stealCostEnergy);
					ModMain.Log("stealSucceedMin: " + StealItem.stealSucceedMin);
					StealItem._stealSucceedMin.value = 100;
					ModMain.Log("Modified - stealSucceedMin: " + StealItem.stealSucceedMin);
					ModMain.Log("stealSucceedMax: " + StealItem.stealSucceedMax);
					StealItem._stealSucceedMax.value = 100;
					ModMain.Log("Modified - stealSucceedMax: " + StealItem.stealSucceedMax);
					ModMain.Log("exposeReduceClosePara: " + StealItem.exposeReduceClosePara);
					StealItem._exposeReduceClosePara.value = 0;
					ModMain.Log("Modified - exposeReduceClosePara: " + StealItem.exposeReduceClosePara);
					ModMain.Log("hateAddPara: " + StealItem.hateAddPara);
					StealItem._hateAddPara.value = 0;
					ModMain.Log("Modified - hateAddPara: " + StealItem.hateAddPara);
					ModMain.Log("hateLimited: " + StealItem.hateLimited);
					StealItem._hateLimited.value = 0;
					ModMain.Log("Modified - hateLimited: " + StealItem.hateLimited);
					ModMain.Log("stealMoneyPercentMax: " + StealItem.stealMoneyPercentMax);
					StealItem._stealMoneyPercentMax.value = 35;
					ModMain.Log("Modified - stealMoneyPercentMax: " + StealItem.stealMoneyPercentMax);
					ModMain.Log("stealMoneyPercentMin: " + StealItem.stealMoneyPercentMin);
					StealItem._stealMoneyPercentMin.value = 35;
					ModMain.Log("Modified - stealMoneyPercentMin: " + StealItem.stealMoneyPercentMin);
					ModMain.Log("stealMoneySpMax: " + StealItem.stealMoneySpMax);
					StealItem._stealMoneySpMax.value = 100;
					ModMain.Log("Modified - stealMoneySpMax: " + StealItem.stealMoneySpMax);
					ModMain.Log("stealMoneySpMin: " + StealItem.stealMoneySpMin);
					StealItem._stealMoneySpMin.value = 100;
					ModMain.Log("Modified - stealMoneySpMin: " + StealItem.stealMoneySpMin);
				}
				ModMain.LogTip(LocalizationHelper.T("other_msgs_steal_always_succeed_enabled"));
			}
			else
			{
				if (StealItem != null)
				{
					ModMain.Log("stealCostEnergy: " + StealItem.stealCostEnergy);
					StealItem._stealCostEnergy.value = 10;
					ModMain.Log("Modified - stealCostEnergy: " + StealItem.stealCostEnergy);
					ModMain.Log("stealSucceedMin: " + StealItem.stealSucceedMin);
					StealItem._stealSucceedMin.value = 10;
					ModMain.Log("Modified - stealSucceedMin: " + StealItem.stealSucceedMin);
					ModMain.Log("stealSucceedMax: " + StealItem.stealSucceedMax);
					StealItem._stealSucceedMax.value = 90;
					ModMain.Log("Modified - stealSucceedMax: " + StealItem.stealSucceedMax);
					ModMain.Log("exposeReduceClosePara: " + StealItem.exposeReduceClosePara);
					StealItem._exposeReduceClosePara.value = 100;
					ModMain.Log("Modified - exposeReduceClosePara: " + StealItem.exposeReduceClosePara);
					ModMain.Log("hateAddPara: " + StealItem.hateAddPara);
					StealItem._hateAddPara.value = 10;
					ModMain.Log("Modified - hateAddPara: " + StealItem.hateAddPara);
					ModMain.Log("hateLimited: " + StealItem.hateLimited);
					StealItem._hateLimited.value = -999;
					ModMain.Log("Modified - hateLimited: " + StealItem.hateLimited);
					ModMain.Log("stealMoneyPercentMax: " + StealItem.stealMoneyPercentMax);
					StealItem._stealMoneyPercentMax.value = 35;
					ModMain.Log("Modified - stealMoneyPercentMax: " + StealItem.stealMoneyPercentMax);
					ModMain.Log("stealMoneyPercentMin: " + StealItem.stealMoneyPercentMin);
					StealItem._stealMoneyPercentMin.value = 20;
					ModMain.Log("Modified - stealMoneyPercentMin: " + StealItem.stealMoneyPercentMin);
					ModMain.Log("stealMoneySpMax: " + StealItem.stealMoneySpMax);
					StealItem._stealMoneySpMax.value = 100;
					ModMain.Log("Modified - stealMoneySpMax: " + StealItem.stealMoneySpMax);
					ModMain.Log("stealMoneySpMin: " + StealItem.stealMoneySpMin);
					StealItem._stealMoneySpMin.value = 70;
					ModMain.Log("Modified - stealMoneySpMin: " + StealItem.stealMoneySpMin);
				}
				ModMain.LogTip(LocalizationHelper.T("other_msgs_steal_always_succeed_disabled"));
				ModMain.LogTip(LocalizationHelper.T("other_msgs_restored_original_values"), null, 3f);
			}
		}
	}
}
