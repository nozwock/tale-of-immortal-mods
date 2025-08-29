using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UIFairyChickenFight), "Init")]
internal class TownStoresPatch_UIFairyChickenFight
{
	public static UIFairyChickenFight _UIFairyChickenFight;

	private static void Postfix(UIFairyChickenFight __instance)
	{
		_UIFairyChickenFight = __instance;
	}
}
