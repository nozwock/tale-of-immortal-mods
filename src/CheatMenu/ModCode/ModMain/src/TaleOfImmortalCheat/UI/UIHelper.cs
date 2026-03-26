using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets;

namespace TaleOfImmortalCheat.UI;

internal static class UIHelper
{
	public static (List<GameObject> contentObjects, List<ButtonRef> tabButtons) CreateTabPanel(GameObject root, string name, Tab[][] tabs, Action<int> onTabChange)
	{
		if (root == null)
		{
			throw new ArgumentNullException("root");
		}
		if (tabs == null || tabs.Length == 0)
		{
			throw new ArgumentException("Tabs array cannot be null or empty", "tabs");
		}
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Name cannot be null or empty", "name");
		}
		List<GameObject> tabObjects = new List<GameObject>();
		List<GameObject> list = new List<GameObject>();
		List<GameObject> list2 = new List<GameObject>();
		List<ButtonRef> list3 = new List<ButtonRef>();
		for (int i = 0; i < tabs.Length; i++)
		{
			GameObject item = UIFactory.CreateHorizontalGroup(root, $"{name}-tabs-group-{i}", forceExpandWidth: true, forceExpandHeight: false, childControlWidth: true, childControlHeight: false, 10, Vector4.zero);
			list2.Add(item);
		}
		for (int j = 0; j < tabs.Length; j++)
		{
			Tab[] array = tabs[j];
			if (array == null)
			{
				continue;
			}
			for (int k = 0; k < array.Length; k++)
			{
				Tab tab = array[k];
				if (tab != null)
				{
					GameObject content;
					AutoSliderScrollbar autoScrollbar;
					GameObject scrollView = UIFactory.CreateScrollView(root, $"{name}-ScrollView-{j}-{k}", out content, out autoScrollbar);
					GameObject gameObject = scrollView;
					int? minWidth = 25;
					int? minHeight = 50;
					int? flexibleHeight = 5000;
					UIFactory.SetLayoutElement(gameObject, minWidth, minHeight, null, flexibleHeight);
					tabObjects.Add(scrollView);
					list.Add(content);
					int currentTabIndex = list.Count - 1;
					ButtonRef buttonRef = UIFactory.CreateButton(list2[j], $"{name}-button-{j}-{k}", tab.Name);
					GameObject gameObject2 = buttonRef.Component.gameObject;
					flexibleHeight = 20;
					int? flexibleWidth = 200;
					UIFactory.SetLayoutElement(gameObject2, null, null, flexibleWidth, flexibleHeight);
					list3.Add(buttonRef);
					buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, (Action)delegate
					{
						ActivateTab(scrollView, tabObjects, onTabChange, currentTabIndex);
					});
					buttonRef.Component.interactable = true;
					bool active = j == 0 && k == 0;
					scrollView.SetActive(active);
				}
			}
		}
		return (contentObjects: list, tabButtons: list3);
	}

	private static void ActivateTab(GameObject targetTab, List<GameObject> allTabs, Action<int> onTabChange, int tabIndex)
	{
		if (targetTab.activeSelf)
		{
			return;
		}
		foreach (GameObject allTab in allTabs)
		{
			allTab.SetActive(value: false);
		}
		targetTab.SetActive(value: true);
		onTabChange?.Invoke(tabIndex);
	}

	public static GameObject CreateTitleBar(GameObject root, Action close, string title, string closeLabel = "X")
	{
		if (root == null)
		{
			throw new ArgumentNullException("root");
		}
		if (string.IsNullOrEmpty(title))
		{
			throw new ArgumentException("Title cannot be null or empty", "title");
		}
		GameObject gameObject = UIFactory.CreateHorizontalGroup(root, "MainTitleBar", forceExpandWidth: true, forceExpandHeight: true, childControlWidth: true, childControlHeight: true, 0, new Vector4(3f, 3f, 15f, 3f));
		int? minWidth = 25;
		int? minHeight = 30;
		int? flexibleHeight = 0;
		UIFactory.SetLayoutElement(gameObject, minWidth, minHeight, null, flexibleHeight);
		Text text = UIFactory.CreateLabel(gameObject, "TitleLabel", title, TextAnchor.MiddleLeft, Color.white, supportRichText: true, 15);
		GameObject gameObject2 = text.gameObject;
		int? minWidth2 = 100;
		flexibleHeight = 1;
		int? flexibleHeight2 = 0;
		UIFactory.SetLayoutElement(gameObject2, minWidth2, null, flexibleHeight, flexibleHeight2);
		Text component = text.gameObject.GetComponent<Text>();
		component.resizeTextForBestFit = true;
		component.resizeTextMinSize = 8;
		component.resizeTextMaxSize = 16;
		component.horizontalOverflow = HorizontalWrapMode.Wrap;
		component.verticalOverflow = VerticalWrapMode.Truncate;
		ButtonRef buttonRef = UIFactory.CreateButton(gameObject, "HideButton", closeLabel);
		buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, close);
		GameObject gameObject3 = buttonRef.Component.gameObject;
		int? minWidth3 = 30;
		flexibleHeight2 = 30;
		flexibleHeight = 0;
		int? flexibleHeight3 = 0;
		UIFactory.SetLayoutElement(gameObject3, minWidth3, null, flexibleHeight, flexibleHeight3, flexibleHeight2);
		StyleCloseButton(buttonRef);
		return gameObject;
	}

	private static void StyleCloseButton(ButtonRef closeButton)
	{
		RuntimeHelper.SetColorBlock(closeButton.Component, new Color(1f, 0.2f, 0.2f), new Color(1f, 0.6f, 0.6f), new Color(0.3f, 0.1f, 0.1f));
		Text buttonText = closeButton.ButtonText;
		buttonText.color = Color.white;
		buttonText.resizeTextForBestFit = true;
		buttonText.resizeTextMinSize = 8;
		buttonText.resizeTextMaxSize = 14;
	}

	public static (GameObject, InputFieldRef) BuildInputField(GameObject root, string name, string placeholder, string qualifiedName = null)
	{
		if (root == null)
		{
			throw new ArgumentNullException("root");
		}
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Name cannot be null or empty", "name");
		}
		string text = qualifiedName ?? name;
		GameObject gameObject = UIFactory.CreateHorizontalGroup(root, text + "-row", forceExpandWidth: false, forceExpandHeight: true, childControlWidth: true, childControlHeight: true, 5, new Vector4(0f, 10f, 0f, 0f));
		UIFactory.SetLayoutElement(UIFactory.CreateLabel(gameObject, text + "-row-label", name + ":", TextAnchor.MiddleRight).gameObject, 90, flexibleWidth: 0, minHeight: 25);
		InputFieldRef inputFieldRef = UIFactory.CreateInputField(gameObject, text + "-row-inputfield", placeholder ?? string.Empty);
		UIFactory.SetLayoutElement(inputFieldRef.Component.gameObject, 80, 25, 0);
		inputFieldRef.Component.characterValidation = InputField.CharacterValidation.Integer;
		return (gameObject, inputFieldRef);
	}

	public static (GameObject, Dropdown) BuildDropdown(GameObject root, string name, Action<int> onChange)
	{
		if (root == null)
		{
			throw new ArgumentNullException("root");
		}
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Name cannot be null or empty", "name");
		}
		Dropdown dropdown;
		GameObject gameObject = UIFactory.CreateDropdown(root, name, out dropdown, null, 14, onChange);
		int? minHeight = 25;
		int? flexibleWidth = 100;
		int? flexibleHeight = 0;
		UIFactory.SetLayoutElement(gameObject, null, minHeight, flexibleWidth, flexibleHeight);
		return (gameObject, dropdown);
	}
}
