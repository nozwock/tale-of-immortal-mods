using System;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;
using TaleOfImmortalCheat.UI.Panels;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UIUpGradeAttr), "InitData")]
public class UIFateFeatureInitDataPatch
{
	static UIFateFeatureInitDataPatch()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void Postfix(UIUpGradeAttr __instance)
	{
		int grade = __instance.gradeItem.grade;
		System.Action action = delegate
		{
			(UIManager.Panels[PanelType.PlayerAttributes] as AttributesPanel)?.SetInputValues();
		};
		__instance.onOkClickCall += (Il2CppSystem.Action)action;
		if (State.GradeFates.TryGetValue(grade, out var fate))
		{
			ConfLocalTextItem arg = Game.ConfMgr.localText.allText[$"role_feature_postnatal_name{fate.id}"];
			int count = __instance.fateLuckList.Count;
			__instance.fateLuckList = new List<int>();
			for (int num = 0; num < count; num++)
			{
				__instance.fateLuckList.Add(fate.id);
			}
			ModMain.Log($"Selected {arg} for grade {grade}.");
			Il2CppSystem.Action onOkClickCall = __instance.onOkClickCall;
			System.Action action2 = delegate
			{
				__instance.selectLuck = fate;
				onOkClickCall.Invoke();
			};
			__instance.onOkClickCall = action2;
		}
		else
		{
			ModMain.Log($"No fate planned for grade {grade}.");
		}
	}

	private static void UpdateUITexts()
	{
	}
}
