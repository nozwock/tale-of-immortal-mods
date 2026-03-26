using System;
using System.Collections.Generic;
using UnityEngine.UI;
using EGameTypeData;
using HarmonyLib;
using MelonLoader;
using UnityEngine.Events;
using UnityEngine;
using UnhollowerRuntimeLib;

namespace MOD_CDK50m;

public class ActiveStateWatcher : MonoBehaviour
{
	// Required for il2cpp
	public ActiveStateWatcher(IntPtr ptr) : base(ptr) { }

	public Action<ActiveStateWatcher> onEnabled;
	public Action<ActiveStateWatcher> onDisabled;

	void OnEnable() => onEnabled?.Invoke(this);
	void OnDisable() => onDisabled?.Invoke(this);
}

public class ModMain
{
	internal static readonly string modNamespace = typeof(ModMain).Namespace!;

	static HarmonyLib.Harmony harmony;

	static bool isSeparateArtifactClicked = false;

	Il2CppSystem.Action<ETypeData> callOpenUIEnd;

	public void Init()
	{
		isSeparateArtifactClicked = false;

		harmony = new HarmonyLib.Harmony(modNamespace);
		harmony.PatchAll();

		foreach (var m in harmony.GetPatchedMethods())
		{
			Log($"Patched: {m.DeclaringType.FullName}::{m.Name}");
		}

		callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
		g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);

		ClassInjector.RegisterTypeInIl2Cpp<ActiveStateWatcher>();
	}

	public void Destroy()
	{
		g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);

		harmony?.UnpatchSelf();
		harmony = null;
	}

	static void RepairArtifacts()
	{
		var props = g.world.playerUnit.data.unitData.propData.allProps;
		var artifacts = new Dictionary<int, int>();
		foreach (var conf in g.conf.artifactShape._allConfList)
		{
			artifacts[conf.id] = conf.durable;
		}

		// Log("Looking for artifacts to repair");
		foreach (var prop in props)
		{
			if (prop == null || prop.propsItem == null) // yes, these can be null
			{
				continue;
			}
			var info = prop.propsItem;
			if (info.type != (int)PropsType.Equip)
			{
				continue;
			}
			if (artifacts.TryGetValue(info.id, out var durability))
			{
				// Log($"Repairing '{g.conf.localText.allText[info.name].en}' {prop.GetValues(4)} -> {durability}");
				prop.SetValues(4, durability);
			}
		}
	}

	void OnOpenUIEnd(ETypeData e)
	{
		var edata = e.Cast<OpenUIEnd>();
		if (edata.ui.name == UIType.Artifact.uiName)
		{
			var ui = edata.ui.GetComponent<UIArtifact>();
			var btn = ui.sprite.btnShapeRemove;
			if (btn.gameObject.GetComponent<ActiveStateWatcher>() == null)
			{
				// Log("Creating new watcher");
				var watcher = btn.gameObject.AddComponent<ActiveStateWatcher>();
				watcher.onEnabled = comp =>
				{
					// Log("Hooking 'Separate Artifact' button");
					var btn = comp.gameObject.GetComponent<Button>();
					var originalCb = btn.onClick;
					btn.onClick = new Button.ButtonClickedEvent();
					btn.onClick.AddListener((UnityAction)delegate
					{
						isSeparateArtifactClicked = true;

						originalCb?.Invoke();
					});
				};
				watcher.onEnabled.Invoke(watcher);
			}
		}
		else if (edata.ui.name == UIType.CheckPopup.uiName)
		{
			if (!isSeparateArtifactClicked)
				return;
			isSeparateArtifactClicked = false;

			// Log("Hooking Popup Ok button");
			var ui = edata.ui.GetComponent<UICheckPopup>();
			ui.btn1.onClick.AddListener((UnityAction)RepairArtifacts);
		}
	}

	// Durability changes during a battle
	[HarmonyPatch(typeof(PropItemArtifact))]
	class Patch_PropItemArtifact
	{
		[HarmonyPrefix, HarmonyPatch("AddDurable")]
		static void AddDurable_Prefix(ref int v)
		{
			if (v < 0)
				v = 0;
		}
	}

	internal static void Log(object s)
	{
		MelonLogger.Msg($"{modNamespace}: {s}");
	}
}
