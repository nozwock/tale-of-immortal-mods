using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownMarketBook), "Init")]
internal class TownStoresPatch_UITownMarketBook
{
	public static UITownMarketBook _UITownMarketBook;

	private static void Postfix(UITownMarketBook __instance)
	{
		_UITownMarketBook = __instance;
	}
}
