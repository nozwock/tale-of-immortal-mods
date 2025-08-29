using HarmonyLib;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx.Patch;

[HarmonyPatch(typeof(UIPlayerInfo), "DestroyUI")]
internal class PlayerInfoDestroyPatch
{
	static PlayerInfoDestroyPatch()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void Postfix(DataWorld __instance)
	{
		ModMain.Log("UIPlayerInfo.DestroyUI");
		Game.UIPlayerInfo = null;
	}

	private static void UpdateUITexts()
	{
	}
}
