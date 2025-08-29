using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UnitActionRoleRelation), "PlayerToNPCAction")]
internal class SceneMapPatch_UnitActionRoleRelation
{
	public static UnitActionRoleRelation _instance;

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

	private static void Postfix(UnitActionRoleRelation __instance)
	{
		_instance = __instance;
		if (IsThisActive && _instance != null)
		{
			ModMain.Log("----- ----- ----- ----- -----");
			ModMain.Log("ExploitPanel - UnitActionRoleRelation - Called");
			ModMain.Log("----- ----- ----- ----- -----");
			ModMain.Log("isSetComplete: " + _instance.isSetComplete);
			_instance.isSetComplete = true;
			ModMain.Log("Modified - isSetComplete: " + _instance.isSetComplete);
			ModMain.Log("isNPCReduceIntim: " + _instance.isNPCReduceIntim);
			_instance.isNPCReduceIntim = false;
			ModMain.Log("Modified - isNPCReduceIntim: " + _instance.isNPCReduceIntim);
			ModMain.Log("addIntim: " + _instance.addIntim);
			_instance.addIntim += 69;
			ModMain.Log("Modified - addIntim: " + _instance.addIntim);
			ModMain.Log("isComplete: " + _instance.isComplete);
		}
	}
}
