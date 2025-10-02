using MelonLoader;
using EGameTypeData;
using UnityEngine.UI;
using UnityEngine;

namespace MOD_7tLNX4
{
	public class ModMain
	{
		internal static readonly string modNamespace = typeof(ModMain).Namespace!;

		Il2CppSystem.Action<ETypeData> callOpenUIEnd;

		public void Init()
		{
			callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;

			g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
		}

		public void Destroy()
		{
			g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);
		}

		static void FixDragonDoorUpgradeCutSkillText(UIBase _ui)
		{
			var ui = _ui.GetComponent<UIDragonDoorUpgrade>();
			for (int i = 0; i < ui.goRoot.transform.childCount; i++)
			{
				var tItem = ui.goRoot.transform.GetChild(i);
				if (!tItem.Find("bg").gameObject.active)
					continue;
				var skillName = tItem.Find("Name").GetComponent<Text>();
				skillName.horizontalOverflow = HorizontalWrapMode.Overflow;
			}
		}

		static void FixSoulReaverEngDescription(UIBase ui)
		{
			// For some reason devs added the last description string here by mistake.
			// Could just build up the string ourselves but I think this is fine.
			var src = ui.transform.Find(
				"Root/G:srDesc/Viewport/G:goContentDesc/G:goDesc1/G:goDescMask/G:textDesc1_En")?.GetComponent<Text>();

			var dst = ui.transform.Find(
				"Root/G:srDesc/Viewport/G:goContentDesc/G:textDesc_En")?.GetComponent<Text>();

			if (src == null || dst == null)
				return;

			dst.text += "\n" + src.text;
		}

		void OnOpenUIEnd(ETypeData e)
		{
			var edata = e.Cast<OpenUIEnd>();
			if (edata.uiType.uiName == UIType.PropInfoSoulDevourSword.uiName)
			{
				FixSoulReaverEngDescription(edata.ui);
			}
			else if (edata.uiType.uiName == UIType.DragonDoorUpgrade.uiName)
			{
				FixDragonDoorUpgradeCutSkillText(edata.ui);
			}
		}

		internal static void Log(object s)
		{
			MelonLogger.Msg($"{modNamespace}: {s}");
		}
	}
}
