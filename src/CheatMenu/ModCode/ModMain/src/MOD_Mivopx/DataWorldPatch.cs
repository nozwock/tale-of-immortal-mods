using HarmonyLib;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(DataWorld), "InitData")]
public class DataWorldPatch
{
	public static DataWorld instance;

	static DataWorldPatch()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void Postfix(DataWorld __instance)
	{
		ModMain.Log("DataWorld.Init");
		Game.DataWorld.Value = __instance;
	}

	private static void UpdateUITexts()
	{
	}
}
