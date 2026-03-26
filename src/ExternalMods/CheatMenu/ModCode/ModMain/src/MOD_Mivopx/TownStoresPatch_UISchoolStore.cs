using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UISchoolStore), "Init")]
internal class TownStoresPatch_UISchoolStore
{
	public static UISchoolStore _instance;

	private static void Postfix(UISchoolStore __instance)
	{
		_instance = __instance;
	}
}
