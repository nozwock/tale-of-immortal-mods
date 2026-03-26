using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UIMapMainPlayerInfo), "Init")]
internal class SceneMapPatch_UIMapMainPlayerInfo
{
	public static UIMapMainPlayerInfo _UIMapMainPlayerInfo;

	private static void Postfix(UIMapMainPlayerInfo __instance)
	{
		_UIMapMainPlayerInfo = __instance;
	}
}
