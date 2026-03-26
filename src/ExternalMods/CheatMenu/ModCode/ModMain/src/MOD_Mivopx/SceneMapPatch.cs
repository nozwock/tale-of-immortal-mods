using HarmonyLib;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(SceneMap), "Init")]
internal class SceneMapPatch
{
	public static SceneMap _SceneMap;

	static SceneMapPatch()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void Postfix(SceneMap __instance)
	{
		_SceneMap = __instance;
	}

	private static void UpdateUITexts()
	{
	}
}
