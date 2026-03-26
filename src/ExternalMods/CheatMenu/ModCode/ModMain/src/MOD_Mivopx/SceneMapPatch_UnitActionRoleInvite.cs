using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UnitActionRoleInvite), "PlayerToNPCAction")]
internal class SceneMapPatch_UnitActionRoleInvite
{
	public static UnitActionRoleInvite _instance;

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
		}
	}

	private static void Postfix(UnitActionRoleInvite __instance)
	{
		_instance = __instance;
		if (IsThisActive && _instance != null)
		{
			ModMain.Log("----- ----- ----- ----- -----");
			ModMain.Log("ExploitPanel - UnitActionRoleInvite - Called");
			ModMain.Log("----- ----- ----- ----- -----");
			ModMain.Log("isInviteComplete: " + _instance.isInviteComplete);
			_instance.isInviteComplete = true;
			ModMain.Log("Modified - isInviteComplete: " + _instance.isInviteComplete);
			ModMain.Log("costEnergy: " + _instance.costEnergy);
			_instance.costEnergy = 0;
			ModMain.Log("Modified - costEnergy: " + _instance.costEnergy);
			ModMain.Log("forceAgree: " + _instance.forceAgree);
			_instance.forceAgree = true;
			ModMain.Log("Modified - forceAgree: " + _instance.forceAgree);
			ModMain.Log("isNPCReduceIntim: " + _instance.isNPCReduceIntim);
			_instance.isNPCReduceIntim = false;
			ModMain.Log("Modified - isNPCReduceIntim: " + _instance.isNPCReduceIntim);
			ModMain.Log("isComplete: " + _instance.isComplete);
		}
	}
}
