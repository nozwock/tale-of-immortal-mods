using HarmonyLib;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UINPCInfo), "InitData")]
public class UINPCInfoPatchInitData
{
	public static UINPCInfo instance;

	static UINPCInfoPatchInitData()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void Postfix(UINPCInfo __instance)
	{
		ModMain.Log("UINPCInfo InitData");
		instance = __instance;
		Game.NpcUnit.Value = __instance.unit;
	}

	public static void Update()
	{
		try
		{
			instance?.UpdateUI();
		}
		catch
		{
			ModMain.Log("Failed to update NPC ui.");
		}
	}

	private static void UpdateUITexts()
	{
	}
}
