using HarmonyLib;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx.Patch;

[HarmonyPatch(typeof(UIPlayerInfo), "Init")]
internal class PlayerInfoInitPatch
{
	static PlayerInfoInitPatch()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void Postfix(UIPlayerInfo __instance)
	{
		ModMain.Log("UIPlayerInfo.Init");
		Game.UIPlayerInfo = __instance;
	}

	private static void UpdateUITexts()
	{
	}
}
