using HarmonyLib;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(ConfMgr), "startGameTip", MethodType.Getter)]
public class ConfMgrPatch
{
	static ConfMgrPatch()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void Postfix(ConfMgr __instance)
	{
		if ((Game.ConfMgr == null || !Game.ConfMgr.Equals(__instance)) && IsValid(__instance))
		{
			Game.ConfMgr = __instance;
			UIManager.OnConfUpdate();
			ModMain.IsRecreatingRewardsNeeded = true;
			ModMain.Log("Loaded ConfMgr.");
		}
	}

	private static bool IsValid(ConfMgr confMgr)
	{
		if (confMgr != null)
		{
			if (confMgr == null)
			{
				return false;
			}
			ConfLocalText localText = confMgr.localText;
			if (localText == null)
			{
				return false;
			}
			return localText.allText?.Count > 0;
		}
		return false;
	}

	private static void UpdateUITexts()
	{
	}
}
