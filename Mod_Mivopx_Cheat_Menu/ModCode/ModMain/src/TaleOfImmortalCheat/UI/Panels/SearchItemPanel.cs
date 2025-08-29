using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.WebPages;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;
using TaleOfImmortalCheat.Localization;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets;

namespace TaleOfImmortalCheat.UI.Panels;

public class SearchItemPanel : Panel
{
	public const string PanelName = "SearchItem";

	private System.Collections.Generic.Dictionary<int, ButtonRef> searchButtons = new System.Collections.Generic.Dictionary<int, ButtonRef>();

	private GameObject searchGroup;

	private GameObject searchScrollView;

	private System.Collections.Generic.Dictionary<int, GameObject> rewardsGroups = new System.Collections.Generic.Dictionary<int, GameObject>();

	private System.Collections.Generic.Dictionary<int, Item> rewards = new System.Collections.Generic.Dictionary<int, Item>();

	private GameObject itemsScrollView;

	private static bool _isCHActive;

	private InputFieldRef searchInput;

	private ButtonRef showAllButton;

	private Toggle toggleShowChineseName;

	private Text toggleShowChineseNameText;

	private ButtonRef rewardButton;

	private ButtonRef clearButton;

	private Text titleBarText;

	public static bool IsCHActive
	{
		get
		{
			return _isCHActive;
		}
		set
		{
			_isCHActive = value;
		}
	}

	public SearchItemPanel()
		: base(isStartedVisible: false)
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~SearchItemPanel()
	{
		UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		Debug.Log("Creating Items panel");
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel("SearchItem", uiRoot, out contentHolder);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, false, true, true);
		GameObject gameObject2 = UIHelper.CreateTitleBar(contentHolder, delegate
		{
			base.IsVisible = false;
		}, GetTitleBarText());
		titleBarText = gameObject2.GetComponentInChildren<Text>();
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.3f, 0.3f);
		component.anchorMax = new Vector2(0.8f, 0.8f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 825f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 800f);
		GameObject parent = UIFactory.CreateHorizontalGroup(contentHolder, "SearchItem-h-group", forceExpandWidth: true, forceExpandHeight: true, childControlWidth: true, childControlHeight: true);
		UIFactory.CreatePanel("SearchItem-search-panel", parent, out var contentHolder2);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder2, true, false, true, true);
		searchInput = UIFactory.CreateInputField(contentHolder2, "SearchItem-search", LocalizationHelper.T("panel_searchitem_input_placeholder"));
		searchInput.Component.onEndEdit.AddListener(OnSearch);
		AutoSliderScrollbar autoScrollbar;
		GameObject gameObject3 = UIFactory.CreateScrollView(contentHolder2, "SearchItem-search-panel-scroll-view", out searchScrollView, out autoScrollbar);
		int? minWidth = 400;
		int? preferredWidth = 400;
		int? flexibleWidth = 0;
		UIFactory.SetLayoutElement(gameObject3, minWidth, null, flexibleWidth, null, preferredWidth);
		showAllButton = CreateLocalizedButton(contentHolder2, "SearchItem-show-all", "panel_searchitem_button_show_all", delegate
		{
			OnSearch("Show All");
		});
		UIFactory.SetLayoutElement(showAllButton.Component.gameObject, null, 35, null, 0);
		UIFactory.CreateToggle(contentHolder, LocalizationHelper.T("panel_searchitem_toggle_chinese_name"), out toggleShowChineseName, out toggleShowChineseNameText);
		toggleShowChineseName.isOn = false;
		toggleShowChineseNameText.text = LocalizationHelper.T("panel_searchitem_toggle_chinese_name");
		Action<bool> action = delegate(bool v_ShowChineseName)
		{
			IsCHActive = v_ShowChineseName;
			if (v_ShowChineseName)
			{
				ModMain.LogTip(LocalizationHelper.T("panel_searchitem_status_chinese_name_enabled"));
			}
			else
			{
				ModMain.LogTip(LocalizationHelper.T("panel_searchitem_status_chinese_name_disabled"));
			}
		};
		toggleShowChineseName.onValueChanged.AddListener(action);
		GameObject contentHolder3;
		GameObject parent2 = UIFactory.CreatePanel("SearchItem-rewards-panel", parent, out contentHolder3);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder3, true, false, true, true);
		UIFactory.CreateScrollView(parent2, "SearchItem-rewards-panel-items", out itemsScrollView, out var autoScrollbar2);
		GameObject gameObject4 = itemsScrollView;
		int? minWidth2 = 400;
		flexibleWidth = 400;
		preferredWidth = 0;
		UIFactory.SetLayoutElement(gameObject4, minWidth2, null, preferredWidth, null, flexibleWidth);
		autoScrollbar2.Enabled = true;
		UIFactory.CreatePanel("SearchItem-rewards-panel-reward-button-panel", contentHolder3, out var contentHolder4);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder4, true, false, true, true);
		rewardButton = CreateLocalizedButton(contentHolder4, "SearchItem-rewards-panel-reward-button-panel-button", "panel_searchitem_button_reward", delegate
		{
			int rewardID = Game.RewardFactory.CreateRewards(rewards.Values);
			Game.WorldManager.Value.playerUnit.data.RewardItem(rewardID, null, 0);
		}, Color.gray);
		UIFactory.SetLayoutElement(rewardButton.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 35);
		RuntimeHelper.SetColorBlock(rewardButton.Component, new Color(0.1f, 0.3f, 0.1f), new Color(0.2f, 0.5f, 0.2f), new Color(0.1f, 0.2f, 0.1f), new Color(0.2f, 0.2f, 0.2f));
		clearButton = CreateLocalizedButton(contentHolder4, "SearchItem-rewards-panel-reward-button-panel-clearbutton", "panel_searchitem_button_clear", delegate
		{
			foreach (int item in new System.Collections.Generic.List<int>(rewardsGroups.Keys))
			{
				RemoveItemQuantityGroup(item);
			}
			if (searchGroup != null)
			{
				UnityEngine.Object.Destroy(searchGroup);
				searchGroup = UIFactory.CreateVerticalGroup(searchScrollView, "SearchItem-search-group", forceWidth: true, forceHeight: false, childControlWidth: true, childControlHeight: false, 1);
			}
			searchButtons.Clear();
			ModMain.Log("Cleared all rewards and reset search.");
		}, Color.gray);
		UIFactory.SetLayoutElement(clearButton.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 35);
		RuntimeHelper.SetColorBlock(clearButton.Component, new Color(1f, 44f / 85f, 0.14901961f), new Color(1f, 0.6313726f, 0.34901962f), new Color(1f, 38f / 51f, 28f / 51f), Color.gray);
		return (panelRoot: gameObject, draggableArea: gameObject2);
	}

	private ButtonRef CreateLocalizedButton(GameObject parent, string name, string textKey, Action onClick, Color? color = null)
	{
		ButtonRef buttonRef = UIFactory.CreateButton(parent, name, LocalizationHelper.T(textKey), color);
		buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, onClick);
		return buttonRef;
	}

	private string GetTitleBarText()
	{
		string text = LocalizationHelper.T("common_cheatpanel");
		return "<b>" + text + "</b> - " + LocalizationHelper.T("panel_searchitem_title");
	}

	private void UpdateUITexts()
	{
		if (titleBarText != null)
		{
			titleBarText.text = GetTitleBarText();
		}
		if (searchInput != null)
		{
			searchInput.PlaceholderText.text = LocalizationHelper.T("panel_searchitem_input_placeholder");
		}
		if (showAllButton != null)
		{
			showAllButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_searchitem_button_show_all");
		}
		if (toggleShowChineseNameText != null)
		{
			toggleShowChineseNameText.text = LocalizationHelper.T("panel_searchitem_toggle_chinese_name");
		}
		if (rewardButton != null)
		{
			rewardButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_searchitem_button_reward");
		}
		if (clearButton != null)
		{
			clearButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_searchitem_button_clear");
		}
	}

	private void OnSearch(string value)
	{
		try
		{
			searchButtons.Clear();
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			if (searchGroup != null)
			{
				UnityEngine.Object.Destroy(searchGroup);
			}
			searchGroup = UIFactory.CreateVerticalGroup(searchScrollView, "SearchItem-search-group", forceWidth: true, forceHeight: false, childControlWidth: true, childControlHeight: false, 1);
			Il2CppSystem.Collections.Generic.List<ConfItemPropsItem>.Enumerator enumerator = Game.ConfMgr.itemProps._allConfList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ConfItemPropsItem current = enumerator.Current;
				if (Game.ConfMgr.localText.allText.ContainsKey(current.name))
				{
					ConfLocalTextItem confLocalTextItem = Game.ConfMgr.localText.allText[current.name];
					if (confLocalTextItem.ch == "衣服")
					{
						confLocalTextItem.ch = "衣服 " + current.id;
					}
					if (confLocalTextItem.tc == "衣服")
					{
						confLocalTextItem.tc = "衣服 " + current.id;
					}
					if (confLocalTextItem.en == "Outfit")
					{
						confLocalTextItem.en = "Outfit " + current.id;
					}
					if (confLocalTextItem.kr == "의복")
					{
						confLocalTextItem.kr = "의복 " + current.id;
					}
					if (confLocalTextItem.en == "" || confLocalTextItem.en.IsEmpty())
					{
						confLocalTextItem.en = confLocalTextItem.ch;
					}
					if (value.Equals("Show All", StringComparison.OrdinalIgnoreCase) || Regex.IsMatch(confLocalTextItem.en, value, RegexOptions.IgnoreCase) || Regex.IsMatch(confLocalTextItem.ch, value, RegexOptions.IgnoreCase))
					{
						CreateItemButton(current.name, current.id, IsCHActive ? (confLocalTextItem.en + " (" + confLocalTextItem.ch + ")") : confLocalTextItem.en);
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModMain.LogWarning("Search Error: " + ex.Message + ":" + ex.StackTrace);
		}
		ModMain.Log("Searched complete.");
	}

	private void CreateItemButton(string itemName, int itemId, string name)
	{
		ButtonRef btn = UIFactory.CreateButton(searchGroup, "SearchItem-button-" + itemName, name);
		UIFactory.SetLayoutElement(btn.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 35, flexibleWidth: 999);
		searchButtons[itemId] = btn;
		if (rewards.ContainsKey(itemId))
		{
			RuntimeHelper.SetColorBlock(btn.Component, Color.Lerp(Color.gray, Color.green, 0.3f));
		}
		else
		{
			RuntimeHelper.SetColorBlock(btn.Component, Color.gray);
		}
		btn.OnClick = delegate
		{
			if (rewards.ContainsKey(itemId))
			{
				RuntimeHelper.SetColorBlock(btn.Component, Color.gray);
				RemoveItemQuantityGroup(itemId);
			}
			else
			{
				RuntimeHelper.SetColorBlock(btn.Component, Color.Lerp(Color.gray, Color.green, 0.3f));
				Item item = new Item
				{
					Id = itemId,
					Quantity = 1
				};
				CreateItemQuantityGroup(item, name);
			}
		};
	}

	private void CreateItemQuantityGroup(Item item, string label)
	{
		rewards.Add(item.Id, item);
		GameObject gameObject = UIFactory.CreateHorizontalGroup(itemsScrollView, $"item-quantity-group-{item.Id}", forceExpandWidth: true, forceExpandHeight: true, childControlWidth: true, childControlHeight: true, 0, new Vector4(3f, 3f, 15f, 3f));
		int? minWidth = 25;
		int? minHeight = 30;
		int? flexibleHeight = 0;
		UIFactory.SetLayoutElement(gameObject, minWidth, minHeight, null, flexibleHeight);
		ButtonRef buttonRef = UIFactory.CreateButton(gameObject, "HideButton", "X");
		buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, (Action)delegate
		{
			RemoveItemQuantityGroup(item.Id);
		});
		GameObject gameObject2 = buttonRef.Component.gameObject;
		int? minWidth2 = 25;
		flexibleHeight = 0;
		int? preferredWidth = 25;
		UIFactory.SetLayoutElement(gameObject2, minWidth2, null, flexibleHeight, null, preferredWidth);
		RuntimeHelper.SetColorBlock(buttonRef.Component, new Color(1f, 0.2f, 0.2f), new Color(1f, 0.6f, 0.6f), new Color(0.3f, 0.1f, 0.1f));
		Text buttonText = buttonRef.ButtonText;
		buttonText.color = Color.white;
		buttonText.resizeTextForBestFit = true;
		buttonText.resizeTextMinSize = 14;
		buttonText.resizeTextMaxSize = 14;
		GameObject gameObject3 = UIFactory.CreateLabel(gameObject, $"item-quantity-group-{item.Id}-label", label, TextAnchor.MiddleLeft, default(Color), supportRichText: false, 15).gameObject;
		preferredWidth = 5000;
		UIFactory.SetLayoutElement(gameObject3, null, null, preferredWidth);
		InputFieldRef quantityInput = UIFactory.CreateInputField(gameObject, $"item-quantity-group-{item.Id}-quantity", LocalizationHelper.T("panel_searchitem_input_quantity_placeholder"));
		GameObject gameObject4 = quantityInput.Component.gameObject;
		int? minWidth3 = 100;
		preferredWidth = 0;
		flexibleHeight = 100;
		UIFactory.SetLayoutElement(gameObject4, minWidth3, null, preferredWidth, null, flexibleHeight);
		quantityInput.Text = item.Quantity.ToString();
		quantityInput.Component.onEndEdit.AddListener(delegate(string value)
		{
			if (int.TryParse(value, out var result))
			{
				rewards[item.Id] = new Item
				{
					Id = item.Id,
					Quantity = result
				};
			}
			else
			{
				quantityInput.Text = rewards[item.Id].Quantity.ToString();
			}
		});
		rewardsGroups.Add(item.Id, gameObject);
	}

	private void RemoveItemQuantityGroup(int itemId)
	{
		rewards.Remove(itemId);
		UnityEngine.Object.Destroy(rewardsGroups[itemId]);
		rewardsGroups.Remove(itemId);
		if (searchButtons.ContainsKey(itemId))
		{
			RuntimeHelper.SetColorBlock(searchButtons[itemId].Component, Color.gray);
		}
	}
}
