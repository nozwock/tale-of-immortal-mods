using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownFactory), "Init")]
internal class TownStoresPatch_UITownFactory
{
	public static UITownFactory _UITownFactory;

	private static void Postfix(UITownFactory __instance)
	{
		_UITownFactory = __instance;
	}
}
