using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownAuction), "Init")]
internal class TownStoresPatch_UITownAuction
{
	public static UITownAuction _UITownAuction;

	private static void Postfix(UITownAuction __instance)
	{
		_UITownAuction = __instance;
	}
}
