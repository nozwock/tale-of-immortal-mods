using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownMarketBuy), "Init")]
internal class TownStoresPatch_UITownMarketBuy
{
	public static UITownMarketBuy _UITownMarketBuy;

	private static void Postfix(UITownMarketBuy __instance)
	{
		_UITownMarketBuy = __instance;
	}
}
