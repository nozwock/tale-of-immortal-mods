using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UISchoolLibrary), "Init")]
internal class TownStoresPatch_UISchoolLibrary
{
	public static UISchoolLibrary _UISchoolLibrary;

	private static void Postfix(UISchoolLibrary __instance)
	{
		_UISchoolLibrary = __instance;
	}
}
