using MelonLoader;
using UnityEngine.Events;

namespace MOD_VOYaCr;

public class ModMain
{
	internal static HarmonyLib.Harmony harmony;
	internal static readonly string modNamespace = typeof(ModMain).Namespace!;

	Il2CppSystem.Action<ETypeData> callOpenUIEnd;

	public void Init()
	{
		harmony = new HarmonyLib.Harmony(modNamespace);
		harmony.PatchAll();

		callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;

		g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
	}

	public void Destroy()
	{
		g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);

		harmony?.UnpatchSelf();
		harmony = null;
	}

	void OnOpenUIEnd(ETypeData e)
	{
		static void ReplenishTao()
		{
			var dynData = g.world.playerUnit.data.dynUnitData;
			dynData.dp.baseValue = dynData.dpMax.baseValue;
		}

		var edata = e.Cast<EGameTypeData.OpenUIEnd>();
		if (edata.ui.name == UIType.TownHotel.uiName)
		{
			var ui = edata.ui.GetComponent<UITownHotel>();
			ui.btnOK.onClick.AddListener((UnityAction)ReplenishTao);
		}
		else if (edata.ui.name == UIType.SchoolHospital.uiName)
		{
			var ui = edata.ui.GetComponent<UISchoolHospital>();
			ui.btnOK.onClick.AddListener((UnityAction)ReplenishTao);
		}
	}

	internal static void Log(string s)
	{
		MelonLogger.Msg($"{modNamespace}: {s}");
	}
}
