using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownRefine), "Init")]
internal class TownStoresPatch_UITownRefine
{
	public static UITownRefine _UITownRefine;

	private static void Postfix(UITownRefine __instance)
	{
		_UITownRefine = __instance;
	}
}
