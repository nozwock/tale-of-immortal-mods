using HarmonyLib;
using TaleOfImmortalCheat.Localization;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(BattleMapMgr), "Init")]
internal class BattleMapMgrPatchInit
{
	private static void Postfix(BattleMapMgr __instance)
	{
		if (__instance != null)
		{
			BattleMapMgrPatch.InitializeCustomID();
			ModMain.IsInCombat = true;
			if (ModMain.ShortcutKey)
			{
				ModMain.LogTip(LocalizationHelper.T("other_msgs_combat_started_shortcut_disabled"), null, 1.5f);
			}
		}
	}
}
