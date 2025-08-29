using System;
using System.Collections.Generic;
using MOD_Mivopx;
using MOD_Mivopx.UI.Panels;
using TaleOfImmortalCheat.Localization;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace TaleOfImmortalCheat.UI.Panels;

public class MainPanel : Panel
{
	private GameObject LoadingLabel;

	private GameObject WaitingLabel;

	private GameObject ButtonGroup;

	public volatile bool IsReadyPendingValue;

	public volatile bool IsReadyPending;

	public volatile bool Loaded;

	public static GameObject mainPanelItem;

	private bool HasSwitchedLoadingLabels;

	private TooltipPanel mainTooltip = new TooltipPanel("MainMenuButtonTooltip");

	private Dictionary<RectTransform, string> buttonTooltipKeys = new Dictionary<RectTransform, string>();

	private ButtonRef buttonTabPlayer;

	private ButtonRef buttonTabNPC;

	private ButtonRef buttonTabItems;

	private ButtonRef buttonTabFate;

	private ButtonRef buttonTabExploit;

	private ButtonRef buttonTabOther;

	private ButtonRef buttonTabDebug;

	private ButtonRef buttonTabLanguage;

	private Text titleBarText;

	private Text waitingText;

	private Text loadingText;

	public bool IsReady
	{
		get
		{
			if (!(WaitingLabel == null))
			{
				return WaitingLabel.active;
			}
			return false;
		}
		set
		{
			if (!(WaitingLabel == null) && !(ButtonGroup == null))
			{
				if (value)
				{
					WaitingLabel.SetActive(value: false);
					ButtonGroup.SetActive(value: true);
				}
				else
				{
					WaitingLabel.SetActive(value: true);
					ButtonGroup.SetActive(value: false);
				}
			}
		}
	}

	public MainPanel()
		: base(isStartedVisible: true)
	{
		HeightCompensation = 40;
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~MainPanel()
	{
		UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel("CheatMainMenu", uiRoot, out contentHolder);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, true, true, true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 1f);
		component.anchorMax = new Vector2(0.5f, 1f);
		component.pivot = new Vector2(0.5f, 1f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 460f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80f);
		GameObject gameObject2 = UIHelper.CreateTitleBar(contentHolder, delegate
		{
			UIManager.IsRootVisible = false;
		}, GetTitleBarText(), "F3");
		titleBarText = gameObject2.GetComponentInChildren<Text>();
		ButtonGroup = UIFactory.CreateHorizontalGroup(contentHolder, "CheatMainMenu-buttons-holder", forceExpandWidth: true, forceExpandHeight: true, childControlWidth: true, childControlHeight: true, 5);
		ButtonGroup.SetActive(value: false);
		buttonTooltipKeys.Clear();
		buttonTabPlayer = CreateMainButton("CheatMainMenu-player", LocalizationHelper.T("panel_player_mainmenu_title"), delegate
		{
			UIManager.Panels[PanelType.PlayerAttributes].IsVisible = !UIManager.Panels[PanelType.PlayerAttributes].IsVisible;
			(UIManager.Panels[PanelType.PlayerAttributes] as AttributesPanel)?.SetInputValues();
		});
		buttonTabNPC = CreateMainButton("CheatMainMenu-npc", LocalizationHelper.T("panel_npc_mainmenu_title"), delegate
		{
			if (!g.ui.GetUI(UIType.NPCInfo))
			{
				ModMain.LogTip(LocalizationHelper.T("other_msgs_npc_info_panel_not_found"), "ERROR");
			}
			else
			{
				UIManager.Panels[PanelType.NPCAttributes].IsVisible = !UIManager.Panels[PanelType.NPCAttributes].IsVisible;
				(UIManager.Panels[PanelType.NPCAttributes] as AttributesPanel)?.SetInputValues();
			}
		}, "tooltip_mainmenu_npc");
		buttonTabItems = CreateMainButton("CheatMainMenu-items", LocalizationHelper.T("panel_items_mainmenu_title"), delegate
		{
			UIManager.Panels[PanelType.Items].IsVisible = !UIManager.Panels[PanelType.Items].IsVisible;
		});
		buttonTabFate = CreateMainButton("CheatMainMenu-fate", LocalizationHelper.T("panel_fate_mainmenu_title"), delegate
		{
			UIManager.Panels[PanelType.FatePlanner].IsVisible = !UIManager.Panels[PanelType.FatePlanner].IsVisible;
		}, "tooltip_mainmenu_fate");
		buttonTabExploit = CreateMainButton("CheatMainMenu-exploit", LocalizationHelper.T("panel_exploit_mainmenu_title"), delegate
		{
			UIManager.Panels[PanelType.Exploit].IsVisible = !UIManager.Panels[PanelType.Exploit].IsVisible;
		});
		buttonTabOther = CreateMainButton("CheatMainMenu-other", LocalizationHelper.T("panel_other_mainmenu_title"), delegate
		{
			UIManager.Panels[PanelType.Other].IsVisible = !UIManager.Panels[PanelType.Other].IsVisible;
		});
		if (ModMain.Instance.DeveloperMode)
		{
			buttonTabDebug = CreateMainButton("CheatMainMenu-debug", LocalizationHelper.T("panel_debug_mainmenu_title"), delegate
			{
				UIManager.Panels[PanelType.Debug].IsVisible = !UIManager.Panels[PanelType.Debug].IsVisible;
			});
		}
		buttonTabLanguage = CreateMainButton("CheatMainMenu-language", LocalizationHelper.T("panel_language_mainmenu_title"), delegate
		{
			UIManager.Panels[PanelType.Language].IsVisible = !UIManager.Panels[PanelType.Language].IsVisible;
		}, "tooltip_mainmenu_language");
		waitingText = UIFactory.CreateLabel(contentHolder, "CheatMainMenu-waiting-label", "<b>" + LocalizationHelper.T("status_waiting_save_load") + "</b>", TextAnchor.MiddleCenter);
		WaitingLabel = waitingText.gameObject;
		UIFactory.SetLayoutElement(WaitingLabel, 90, flexibleWidth: 0, minHeight: 25);
		WaitingLabel.SetActive(value: false);
		loadingText = UIFactory.CreateLabel(contentHolder, "CheatMainMenu-loading-label", "<b>" + LocalizationHelper.T("status_loading") + "</b>", TextAnchor.MiddleCenter);
		LoadingLabel = loadingText.gameObject;
		UIFactory.SetLayoutElement(LoadingLabel, 90, flexibleWidth: 0, minHeight: 25);
		mainPanelItem = gameObject2;
		mainTooltip.Create();
		return (panelRoot: gameObject, draggableArea: gameObject2);
		ButtonRef CreateMainButton(string name, string textz, Action onClick, string tooltipKey = null)
		{
			ButtonRef buttonRef = UIFactory.CreateButton(ButtonGroup, name, textz);
			GameObject gameObject3 = buttonRef.Component.gameObject;
			UIFactory.SetLayoutElement(gameObject3, null, 35, null, 0);
			buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, new Action(onClick.Invoke));
			if (!string.IsNullOrEmpty(tooltipKey))
			{
				buttonTooltipKeys[gameObject3.GetComponent<RectTransform>()] = tooltipKey;
			}
			return buttonRef;
		}
	}

	public override void OnGameWorldUpdate()
	{
		base.OnGameWorldUpdate();
		if (Game.WorldManager.Value == null)
		{
			if (Loaded)
			{
				ModMain.Log("MainPanel is ready && not loaded.");
				IsReady = false;
				UIManager.CloseSubPanels();
			}
			else
			{
				ModMain.Log("MainPanel is not ready && loaded.");
				IsReadyPendingValue = false;
				IsReadyPending = true;
			}
		}
		else if (Loaded)
		{
			ModMain.Log("MainPanel is ready && loaded.");
			IsReady = true;
		}
		else
		{
			ModMain.Log("MainPanel is not ready && not loaded.");
			IsReadyPendingValue = true;
			IsReadyPending = true;
		}
	}

	public override bool Update(bool allowDrag)
	{
		bool result = base.Update(allowDrag);
		if (Loaded)
		{
			if (!HasSwitchedLoadingLabels)
			{
				LoadingLabel.SetActive(value: false);
				WaitingLabel.SetActive(value: true);
				HasSwitchedLoadingLabels = true;
			}
			if (IsReadyPending)
			{
				IsReady = IsReadyPendingValue;
				IsReadyPending = false;
			}
		}
		Vector3 mousePosition = Input.mousePosition;
		bool flag = false;
		foreach (KeyValuePair<RectTransform, string> buttonTooltipKey in buttonTooltipKeys)
		{
			RectTransform key = buttonTooltipKey.Key;
			if (!(key == null))
			{
				Vector3 point = key.InverseTransformPoint(mousePosition);
				if (key.rect.Contains(point))
				{
					string label = LocalizationHelper.T(buttonTooltipKey.Value);
					mainTooltip.TooltipFor(key, label);
					mainTooltip.IsVisible = true;
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			mainTooltip.IsVisible = false;
		}
		return result;
	}

	private string GetTitleBarText()
	{
		string text = LocalizationHelper.T("panel_mainmenu_title");
		return "<b><color=#4cd43d>" + text + $"</color></b> <i><color=#ff3030>{ModMain.Version}</color></i> <b><color=#00FFFF>v1.2.111.259+ (FPS: ...)</color></b>";
	}

	private void UpdateUITexts()
	{
		if (buttonTabPlayer != null)
		{
			buttonTabPlayer.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_player_mainmenu_title");
		}
		if (buttonTabNPC != null)
		{
			buttonTabNPC.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_npc_mainmenu_title");
		}
		if (buttonTabItems != null)
		{
			buttonTabItems.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_mainmenu_title");
		}
		if (buttonTabFate != null)
		{
			buttonTabFate.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_fate_mainmenu_title");
		}
		if (buttonTabExploit != null)
		{
			buttonTabExploit.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_exploit_mainmenu_title");
		}
		if (buttonTabOther != null)
		{
			buttonTabOther.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_mainmenu_title");
		}
		if (buttonTabDebug != null)
		{
			buttonTabDebug.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_debug_mainmenu_title");
		}
		if (buttonTabLanguage != null)
		{
			buttonTabLanguage.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_language_mainmenu_title");
		}
	}
}
