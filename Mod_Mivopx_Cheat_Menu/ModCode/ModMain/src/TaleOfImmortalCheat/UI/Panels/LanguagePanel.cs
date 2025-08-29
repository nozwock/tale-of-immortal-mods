using System;
using System.Collections.Generic;
using MOD_Mivopx;
using MOD_Mivopx.UI.Panels;
using TaleOfImmortalCheat.Localization;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace TaleOfImmortalCheat.UI.Panels;

public class LanguagePanel : Panel
{
	private const string PanelName = "LanguageSettings";

	private Dropdown languageDropdown;

	private Text currentLanguageLabel;

	private Dictionary<string, string> languageDisplayNames = new Dictionary<string, string>();

	private Text languageLabel;

	private Text titleLabel;

	private ButtonRef applyButton;

	private TooltipPanel tooltip = new TooltipPanel("ButtonTooltip");

	private Dictionary<RectTransform, string> buttonTooltips = new Dictionary<RectTransform, string>();

	public LanguagePanel()
		: base(isStartedVisible: false)
	{
	}

	~LanguagePanel()
	{
		UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
	}

	public override bool Update(bool allowDragging)
	{
		bool result = base.Update(allowDragging);
		if (base.PanelRoot == null || !base.IsVisible)
		{
			return result;
		}
		Vector3 mousePosition = InputManager.MousePosition;
		tooltip.IsVisible = false;
		foreach (KeyValuePair<RectTransform, string> buttonTooltip in buttonTooltips)
		{
			if (buttonTooltip.Key != null && !string.IsNullOrEmpty(buttonTooltip.Value))
			{
				Vector3 point = buttonTooltip.Key.InverseTransformPoint(mousePosition);
				if (buttonTooltip.Key.rect.Contains(point))
				{
					string label = (buttonTooltip.Value.StartsWith("panel_") ? LocalizationHelper.T(buttonTooltip.Value) : buttonTooltip.Value);
					tooltip.TooltipFor(buttonTooltip.Key, label);
					tooltip.IsVisible = true;
					break;
				}
			}
		}
		return result;
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel("LanguageSettings", uiRoot, out contentHolder);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, false, true, true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0.5f);
		component.anchorMax = new Vector2(0.5f, 0.5f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200f);
		GameObject gameObject2 = UIHelper.CreateTitleBar(contentHolder, delegate
		{
			base.IsVisible = false;
		}, GetTitleBarText());
		titleLabel = gameObject2.GetComponentInChildren<Text>();
		GameObject parent = UIFactory.CreateVerticalGroup(contentHolder, "LanguageSection", forceWidth: true, forceHeight: false, childControlWidth: true, childControlHeight: true, 10);
		languageLabel = UIFactory.CreateLabel(parent, "LanguageLabel", LocalizationHelper.T("panel_language_label"), TextAnchor.MiddleCenter);
		GameObject gameObject3 = languageLabel.gameObject;
		int? minHeight = 30;
		UIFactory.SetLayoutElement(gameObject3, null, minHeight);
		currentLanguageLabel = UIFactory.CreateLabel(parent, "CurrentLanguageLabel", GetLanguageDisplayName(LocalizationHelper.GetCurrentLanguage()), TextAnchor.MiddleCenter);
		GameObject gameObject4 = currentLanguageLabel.gameObject;
		minHeight = 30;
		UIFactory.SetLayoutElement(gameObject4, null, minHeight);
		PopulateLanguageDisplayNames();
		List<string> list = new List<string>();
		foreach (string availableLanguage in LocalizationHelper.GetAvailableLanguages())
		{
			list.Add(GetLanguageDisplayName(availableLanguage));
		}
		GameObject gameObject5 = UIFactory.CreateDropdown(parent, "LanguageDropdown", out languageDropdown, "", 14, OnLanguageChanged, list.ToArray());
		minHeight = 30;
		UIFactory.SetLayoutElement(gameObject5, null, minHeight);
		string currentLanguage = LocalizationHelper.GetCurrentLanguage();
		for (int num = 0; num < LocalizationHelper.GetAvailableLanguages().Count; num++)
		{
			if (LocalizationHelper.GetAvailableLanguages()[num] == currentLanguage)
			{
				languageDropdown.value = num;
				break;
			}
		}
		applyButton = UIFactory.CreateButton(parent, "ApplyButton", LocalizationHelper.T("common_applychange"));
		GameObject gameObject6 = applyButton.Component.gameObject;
		minHeight = 40;
		UIFactory.SetLayoutElement(gameObject6, null, minHeight);
		applyButton.OnClick = (Action)Delegate.Combine(applyButton.OnClick, new Action(ApplyLanguageChange));
		buttonTooltips[applyButton.Component.GetComponent<RectTransform>()] = "panel_language_tooltip_apply_button";
		tooltip.Create();
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
		return (panelRoot: gameObject, draggableArea: gameObject2);
	}

	private void PopulateLanguageDisplayNames()
	{
		languageDisplayNames.Clear();
		foreach (string availableLanguage in LocalizationHelper.GetAvailableLanguages())
		{
			string text = LocalizationHelper.GetText("panel_language_name", availableLanguage);
			languageDisplayNames[availableLanguage] = text;
		}
	}

	private string GetLanguageDisplayName(string langCode)
	{
		if (languageDisplayNames.ContainsKey(langCode))
		{
			return languageDisplayNames[langCode];
		}
		string text = LocalizationHelper.GetText("panel_language_name", langCode);
		if (!string.IsNullOrEmpty(text))
		{
			languageDisplayNames[langCode] = text;
			return text;
		}
		return langCode;
	}

	private void OnLanguageChanged(int selectedIndex)
	{
	}

	private void ApplyLanguageChange()
	{
		int value = languageDropdown.value;
		if (value >= 0 && value < LocalizationHelper.GetAvailableLanguages().Count)
		{
			string text = LocalizationHelper.GetAvailableLanguages()[value];
			if (UIRefreshManager.ChangeLanguage(text))
			{
				ModMain.LogTip(LocalizationHelper.T("other_msgs_languagechange") + " " + GetLanguageDisplayName(text));
			}
			else
			{
				ModMain.LogTip(LocalizationHelper.T("other_msgs_failed_change_language"), "ERROR");
			}
		}
	}

	private string GetTitleBarText()
	{
		string text = LocalizationHelper.T("common_cheatpanel");
		return "<b>" + text + "</b> - " + LocalizationHelper.T("panel_language_title");
	}

	private void UpdateUITexts()
	{
		if (titleLabel != null)
		{
			titleLabel.text = GetTitleBarText();
		}
		if (languageLabel != null)
		{
			languageLabel.text = LocalizationHelper.T("panel_language_label");
		}
		if (currentLanguageLabel != null)
		{
			currentLanguageLabel.text = GetLanguageDisplayName(LocalizationHelper.GetCurrentLanguage());
		}
		if (applyButton != null)
		{
			applyButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("common_applychange");
		}
	}
}
