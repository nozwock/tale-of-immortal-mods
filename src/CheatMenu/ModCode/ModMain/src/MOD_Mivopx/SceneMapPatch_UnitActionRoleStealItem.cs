using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UnitActionRoleStealItem), "PlayerToNPCAction")]
internal class SceneMapPatch_UnitActionRoleStealItem
{
	public static UnitActionRoleStealItem StealItem;

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
		}
	}

	private static void Postfix(UnitActionRoleStealItem __instance)
	{
		StealItem = __instance;
		if (IsStealActive && StealItem != null && StealItem.isInit)
		{
			ModMain.Log("----- ----- ----- ----- -----");
			ModMain.Log("OtherPanel - _isStealActive - Called");
			ModMain.Log("----- ----- ----- ----- -----");
			ModMain.Log("OtherPanel - _isStealActive - costEnergy: " + StealItem.costEnergy);
			ModMain.Log("OtherPanel - _isStealActive - isStealComplete: " + StealItem.isStealComplete);
			ModMain.Log("OtherPanel - _isStealActive - isStealDiscover: " + StealItem.isStealDiscover);
			ModMain.Log("OtherPanel - _isStealActive - isBattle: " + StealItem.isBattle);
			StealItem.costEnergy = 0;
			StealItem.isStealComplete = true;
			StealItem.isStealDiscover = false;
			StealItem.isBattle = false;
			ModMain.Log("----- ----- ----- ----- -----");
			ModMain.Log("OtherPanel - _isStealActive - Modified - costEnergy: " + StealItem.costEnergy);
			ModMain.Log("OtherPanel - _isStealActive - Modified - isStealComplete: " + StealItem.isStealComplete);
			ModMain.Log("OtherPanel - _isStealActive - Modified - isStealDiscover: " + StealItem.isStealDiscover);
			ModMain.Log("OtherPanel - _isStealActive - Modified - isBattle: " + StealItem.isBattle);
		}
	}
}
