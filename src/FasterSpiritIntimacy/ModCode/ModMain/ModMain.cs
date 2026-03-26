using System;
using UnityEngine.UI;
using EGameTypeData;
using MelonLoader;
using UnityEngine.Events;
using UnityEngine;
using UnhollowerRuntimeLib;

namespace MOD_Ug5YpO;

public class ActiveStateWatcher : MonoBehaviour
{
	// Required for il2cpp
	public ActiveStateWatcher(IntPtr ptr) : base(ptr) { }

	public Action<ActiveStateWatcher> onEnabled;
	public Action<ActiveStateWatcher> onDisabled;

	void OnEnable() => onEnabled?.Invoke(this);
	void OnDisable() => onDisabled?.Invoke(this);
}

internal class Config
{
	static bool _isInit = false;

	static MelonPreferences_Category categoryBase;
	internal static MelonPreferences_Entry<int> giftIntimacyMult;

	internal static void Init()
	{
		if (_isInit)
			return;

		categoryBase = MelonPreferences.CreateCategory($"{ModMain.modNamespace} Faster Spirit Intimacy");
		giftIntimacyMult = categoryBase.CreateEntry("Gift Intimacy Multiplier", 2);

		_isInit = true;
	}
}

public class ModMain
{
	internal static readonly string modNamespace = typeof(ModMain).Namespace!;

	Il2CppSystem.Action<ETypeData> callOpenUIEnd;

	class State
	{
		internal static DataUnit.ArtifactSpriteData.Sprite selectedSprite;
		internal static int initialIntimacy = -1;
		internal static bool isGiftClicked = false;

		internal static void Reset()
		{
			isGiftClicked = false;
			initialIntimacy = -1;
			selectedSprite = null;
		}
	}

	public void Init()
	{
		State.Reset();
		Config.Init();

		callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
		g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);

		ClassInjector.RegisterTypeInIl2Cpp<ActiveStateWatcher>();
	}

	public void Destroy()
	{
		g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);
	}

	void OnOpenUIEnd(ETypeData e)
	{
		var edata = e.Cast<OpenUIEnd>();
		if (edata.ui.name == UIType.Artifact.uiName)
		{
			var ui = edata.ui.GetComponent<UIArtifact>();
			var btn = ui.sprite.btnGive.IsActive() ? ui.sprite.btnGive : ui.sprite.btnGive_En;
			if (btn.gameObject.GetComponent<ActiveStateWatcher>() != null)
				return;
			var watcher = btn.gameObject.AddComponent<ActiveStateWatcher>();
			watcher.onEnabled = comp =>
			{
				var btn = comp.gameObject.GetComponent<Button>();
				var originalCb = btn.onClick;
				btn.onClick = new Button.ButtonClickedEvent();
				btn.onClick.AddListener((UnityAction)delegate
				{
					try
					{
						State.isGiftClicked = true;

						var pred = (DataUnit.ArtifactSpriteData.Sprite it) => it.soleID == ui.uiSprite.selSpriteSoleId;
						State.selectedSprite = ui.uiSprite.spriteList.Find(pred);
						State.initialIntimacy = State.selectedSprite != null ? State.selectedSprite.intimacy : -1;
					}
					catch (Exception e)
					{
						Log(e);
					}

					originalCb?.Invoke();
				});
			};
			watcher.onDisabled = _ => State.Reset();
			watcher.onEnabled.Invoke(watcher);
		}
		else if (edata.ui.name == UIType.PropSelect.uiName)
		{
			if (!State.isGiftClicked)
				return;
			State.isGiftClicked = false;

			var ui = edata.ui.GetComponent<UIPropSelect>();
			ui.btnOK.onClick.AddListener((UnityAction)delegate
			{
				if (State.selectedSprite == null)
				{
					Log("Cannot increase spirit intimacy due to selectedSprite being null");
					return;
				}

				var scaledIntimacy = Mathf.Clamp(
					State.initialIntimacy
					+ (State.selectedSprite.intimacy - State.initialIntimacy)
					* Config.giftIntimacyMult.Value,
					0,
					g.conf.artifactSpriteClose._closeMax.value);
				// Log($"Intimacy: {State.selectedSprite.intimacy} -> {scaledIntimacy}");
				State.selectedSprite.intimacy = scaledIntimacy;
			});
		}
	}

	internal static void Log(object s)
	{
		MelonLogger.Msg($"{modNamespace}: {s}");
	}
}