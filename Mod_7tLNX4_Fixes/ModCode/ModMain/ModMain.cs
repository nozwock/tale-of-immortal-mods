using System;
using System.Collections.Generic;
using System.Reflection;
using EGameTypeData;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_7tLNX4
{
	public class ModMain
	{
		internal static readonly string modNamespace = typeof(ModMain).Namespace!;
		static readonly HarmonyLib.Harmony harmony = new(modNamespace);

		Il2CppSystem.Action<ETypeData> callLoadScene;
		Il2CppSystem.Action<ETypeData> callOpenUIEnd;

		public void Init()
		{
			harmony.PatchAll();
			foreach (var m in harmony.GetPatchedMethods())
			{
				Log($"Patched: {m.DeclaringType.FullName}::{m.Name}");
			}

			callLoadScene = (Il2CppSystem.Action<ETypeData>)OnLoadScene;
			callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;

			g.events.On(EGameType.LoadScene, callLoadScene);
			g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
		}

		public void Destroy()
		{
			g.events.Off(EGameType.LoadScene, callLoadScene);
			g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);

			harmony.UnpatchSelf();
		}

		void OnLoadScene(ETypeData e)
		{
			var edata = e.Cast<LoadScene>();
			var sceneName = edata.sceneType.sceneName;

			if (sceneName == SceneType.map?.sceneName)
			{
				g.timer.Frame((Action)FixPlayerMount, 1);
			}
		}

		// These don't work:
		// EMapType.PlayerEquipCloth
		// UIMapMainPlayerInfo.OnPlayerEquipCloth
		// There's UnitActionNpcEquipItem.EquipClothing but that's for NPC
		//
		// UnitActionEquipEquip seems to be for "Functional" equips, not martial/combat ones
		//
		// For Mount equips, can also hook btnEquip of UIPropInfoHorse
		[HarmonyPatch]
		class Patch_UnitActionEquip
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				yield return AccessTools.Method(typeof(UnitActionEquipEquip), nameof(UnitActionEquipEquip.OnCreate));
				yield return AccessTools.Method(typeof(UnitActionEquipUnequip), nameof(UnitActionEquipUnequip.OnCreate));
			}

			static void Postfix(UnitActionBase __instance)
			{
				if (__instance.unit.data.unitData.unitID != g.world.playerUnit.data.unitData.unitID)
					return;

				g.timer.Frame((Action)FixPlayerMount, 1);

				// Cloth className 304 type 3
				// propData.GetProps(__instance.propsSoleID)
			}
		}

		static void FixDragonDoorUpgradeCutSkillText(UIBase _ui)
		{
			var ui = _ui.GetComponent<UIDragonDoorUpgrade>();
			for (int i = 0; i < ui.goRoot.transform.childCount; i++)
			{
				var tItem = ui.goRoot.transform.GetChild(i);
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

		static void FixPlayerMount()
		{
			static Transform FindChild(Transform tf, string substr)
			{
				if (tf == null)
					return null;

				for (int i = 0; i < tf.childCount; i++)
				{
					var child = tf.GetChild(i);
					if (child.name.Contains(substr))
						return child;
				}
				return null;
			}

			var root = GameObject.Find("MapWorld/UnitRoot")?.transform;
			var unitRoot = FindChild(root, "Horse/")?.Find("Posi/Top")?.GetChild(0)?.Find("Root");
			if (unitRoot == null)
			{
				return;
			}

			g.timer.Frame((Action)delegate
			{
				unitRoot.Find("Foot2").gameObject.SetActive(true);
				unitRoot.Find("Body/Foot").gameObject.SetActive(false);
				unitRoot.gameObject.SetActive(false);
				unitRoot.gameObject.SetActive(true);
			}, 1);
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
