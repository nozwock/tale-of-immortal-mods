using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownFactoryRefineElixir), "Init")]
internal class TownStoresPatch_UITownFactoryRefineElixir
{
	public static UITownFactoryRefineElixir _instance;

	private static void Postfix(UITownFactoryRefineElixir __instance)
	{
		_instance = __instance;
	}
}
