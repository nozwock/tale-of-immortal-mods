using HarmonyLib;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(SceneLogin), "Init")]
public class SceneMapPatchDestroy
{
	static SceneMapPatchDestroy()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void Postfix()
	{
		ModMain.Log("WorldMgr Destroy");
		Game.WorldManager.Value = null;
	}

	private static void UpdateUITexts()
	{
	}
}
