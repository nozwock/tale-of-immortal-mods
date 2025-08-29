using HarmonyLib;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(WorldMgr), "Init")]
public class WorldMgrPatchInit
{
	static WorldMgrPatchInit()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void Postfix(WorldMgr __instance)
	{
		ModMain.Log("WorldMgr Init");
		Game.WorldManager.Value = __instance;
	}

	private static void UpdateUITexts()
	{
	}
}
