using HarmonyLib;
using TaleOfImmortalCheat.Localization;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(BattleMapMgr), "Destroy")]
internal class BattleMapMgrPatchDestroy
{
	private static void Postfix(BattleMapMgr __instance)
	{
		if (__instance != null)
		{
			BattleMapMgrPatch.ClearDictionary();
			BattleMapMgrPatch.battleMgr = null;
			ModMain.IsInCombat = false;
			if (ModMain.ShortcutKey)
			{
				ModMain.LogTip(LocalizationHelper.T("other_msgs_combat_finished_shortcut_enabled"), null, 1.5f);
			}
		}
	}
}
