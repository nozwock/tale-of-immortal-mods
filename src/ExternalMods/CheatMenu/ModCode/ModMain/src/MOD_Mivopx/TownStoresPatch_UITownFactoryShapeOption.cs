using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UITownFactoryShapeOption), "Init")]
internal class TownStoresPatch_UITownFactoryShapeOption
{
	public static UITownFactoryShapeOption _UITownFactoryShapeOption;

	private static void Postfix(UITownFactoryShapeOption __instance)
	{
		_UITownFactoryShapeOption = __instance;
	}
}
