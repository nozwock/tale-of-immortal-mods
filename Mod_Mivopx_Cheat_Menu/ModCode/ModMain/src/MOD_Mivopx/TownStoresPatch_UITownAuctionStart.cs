using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownAuctionStart), "Init")]
internal class TownStoresPatch_UITownAuctionStart
{
	public static UITownAuctionStart _UITownAuctionStart;

	private static void Postfix(UITownAuctionStart __instance)
	{
		_UITownAuctionStart = __instance;
	}
}
