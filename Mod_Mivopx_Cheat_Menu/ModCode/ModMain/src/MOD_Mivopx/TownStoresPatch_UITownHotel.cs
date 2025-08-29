using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownHotel), "Init")]
internal class TownStoresPatch_UITownHotel
{
	public static UITownHotel _UITownHotel;

	private static void Postfix(UITownHotel __instance)
	{
		_UITownHotel = __instance;
	}
}
