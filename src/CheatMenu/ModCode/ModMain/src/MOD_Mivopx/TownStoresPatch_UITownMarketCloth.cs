using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownMarketCloth), "Init")]
internal class TownStoresPatch_UITownMarketCloth
{
	public static UITownMarketCloth _UITownMarketCloth;

	private static void Postfix(UITownMarketCloth __instance)
	{
		_UITownMarketCloth = __instance;
	}
}
