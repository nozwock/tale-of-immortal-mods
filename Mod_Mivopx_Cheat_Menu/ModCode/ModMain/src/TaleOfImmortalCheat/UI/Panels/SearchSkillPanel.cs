using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MOD_Mivopx;
using TaleOfImmortalCheat.Localization;
using TaleOfImmortalCheat.Utilities;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets;

namespace TaleOfImmortalCheat.UI.Panels;

public class SearchSkillPanel : Panel
{
	public const string PanelName = "SearchSkill";

	private Dictionary<int, ButtonRef> searchButtons = new Dictionary<int, ButtonRef>();

	private GameObject searchGroup;

	private GameObject searchScrollView;

	private Dictionary<int, GameObject> rewardsGroups = new Dictionary<int, GameObject>();

	private Dictionary<int, Item> rewards = new Dictionary<int, Item>();

	private GameObject itemsScrollView;

	private string iniFilePath;

	private INIReader iniReader;

	private InputFieldRef searchInput;

	private ButtonRef showAllButton;

	private ButtonRef rewardButton;

	private ButtonRef clearButton;

	private Text titleBarText;

	public SearchSkillPanel()
		: base(isStartedVisible: false)
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~SearchSkillPanel()
	{
		UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		Debug.Log("Creating Skills panel");
		iniFilePath = FileSearchUtility.FindFile("ListSkills.ini", "INI");
		if (iniFilePath != null)
		{
			ModMain.Log("ListSkills.ini found at: " + iniFilePath);
			iniReader = new INIReader(iniFilePath);
			GameObject contentHolder;
			GameObject gameObject = UIFactory.CreatePanel("SearchSkill", uiRoot, out contentHolder);
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
			GameObject parent = UIFactory.CreateHorizontalGroup(contentHolder, "SearchSkill-h-group", forceExpandWidth: true, forceExpandHeight: true, childControlWidth: true, childControlHeight: true);
			UIFactory.CreatePanel("SearchSkill-search-panel", parent, out var contentHolder2);
			UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder2, true, false, true, true);
			searchInput = UIFactory.CreateInputField(contentHolder2, "SearchSkill-search", LocalizationHelper.T("panel_searchskill_input_placeholder"));
			searchInput.Component.onEndEdit.AddListener(OnSearch);
			AutoSliderScrollbar autoScrollbar;
			GameObject gameObject3 = UIFactory.CreateScrollView(contentHolder2, "SearchSkill-search-panel-scroll-view", out searchScrollView, out autoScrollbar);
			int? minWidth = 400;
			int? preferredWidth = 400;
			int? flexibleWidth = 0;
			UIFactory.SetLayoutElement(gameObject3, minWidth, null, flexibleWidth, null, preferredWidth);
			showAllButton = CreateLocalizedButton(contentHolder2, "SearchSkill-show-all", "panel_searchskill_button_show_all", delegate
			{
				OnSearch("Show All");
			});
			UIFactory.SetLayoutElement(showAllButton.Component.gameObject, null, 35, null, 0);
			GameObject contentHolder3;
			GameObject parent2 = UIFactory.CreatePanel("SearchSkill-rewards-panel", parent, out contentHolder3);
			UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder3, true, false, true, true);
			UIFactory.CreateScrollView(parent2, "SearchSkill-rewards-panel-items", out itemsScrollView, out var autoScrollbar2);
			GameObject gameObject4 = itemsScrollView;
			int? minWidth2 = 400;
			flexibleWidth = 400;
			preferredWidth = 0;
			UIFactory.SetLayoutElement(gameObject4, minWidth2, null, preferredWidth, null, flexibleWidth);
			autoScrollbar2.Enabled = true;
			UIFactory.CreatePanel("SearchSkill-rewards-panel-reward-button-panel", contentHolder3, out var contentHolder4);
			UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder4, true, false, true, true);
			rewardButton = CreateLocalizedButton(contentHolder4, "SearchSkill-rewards-panel-reward-button-panel-button", "panel_searchskill_button_reward", delegate
			{
				foreach (KeyValuePair<int, Item> reward in rewards)
				{
					if (reward.Value.Id.Equals(1))
					{
						ModMain.LogTip(LocalizationHelper.T("panel_searchskill_status_category_warning"), "WARNING", 5f);
					}
					else
					{
						Game.WorldManager.Value.playerUnit.data.RewardItem(reward.Value.Id, null, reward.Value.Quantity);
					}
				}
				ModMain.Log(string.Format(LocalizationHelper.T("panel_searchskill_status_rewarded_items"), rewards.Count));
			}, Color.gray);
			UIFactory.SetLayoutElement(rewardButton.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 35);
			RuntimeHelper.SetColorBlock(rewardButton.Component, new Color(0.1f, 0.3f, 0.1f), new Color(0.2f, 0.5f, 0.2f), new Color(0.1f, 0.2f, 0.1f), new Color(0.2f, 0.2f, 0.2f));
			clearButton = CreateLocalizedButton(contentHolder4, "SearchSkill-rewards-panel-reward-button-panel-clearbutton", "panel_searchskill_button_clear", delegate
			{
				foreach (int item in new List<int>(rewardsGroups.Keys))
				{
					RemoveItemQuantityGroup(item);
				}
				if (searchGroup != null)
				{
					UnityEngine.Object.Destroy(searchGroup);
					searchGroup = UIFactory.CreateVerticalGroup(searchScrollView, "SearchSkill-search-group", forceWidth: true, forceHeight: false, childControlWidth: true, childControlHeight: false, 1);
				}
				searchButtons.Clear();
				ModMain.Log("Cleared all rewards and reset search.");
			}, Color.gray);
			UIFactory.SetLayoutElement(clearButton.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 35);
			RuntimeHelper.SetColorBlock(clearButton.Component, new Color(1f, 44f / 85f, 0.14901961f), new Color(1f, 0.6313726f, 0.34901962f), new Color(1f, 38f / 51f, 28f / 51f), Color.gray);
			return (panelRoot: gameObject, draggableArea: gameObject2);
		}
		ModMain.LogWarning("ListSkills.ini not found in any location.");
		throw new FileNotFoundException("Could not locate the ListSkills.ini file.");
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
		return "<b>" + text + "</b> - " + LocalizationHelper.T("panel_searchskill_title");
	}

	private void UpdateUITexts()
	{
		if (titleBarText != null)
		{
			titleBarText.text = GetTitleBarText();
		}
		if (searchInput != null)
		{
			searchInput.PlaceholderText.text = LocalizationHelper.T("panel_searchskill_input_placeholder");
		}
		if (showAllButton != null)
		{
			showAllButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_searchskill_button_show_all");
		}
		if (rewardButton != null)
		{
			rewardButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_searchskill_button_reward");
		}
		if (clearButton != null)
		{
			clearButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_searchskill_button_clear");
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
			searchGroup = UIFactory.CreateVerticalGroup(searchScrollView, "SearchSkill-search-group", forceWidth: true, forceHeight: false, childControlWidth: true, childControlHeight: false, 1);
			foreach (Dictionary<string, string> allSection in iniReader.GetAllSections())
			{
				foreach (KeyValuePair<string, string> item in allSection)
				{
					if (value.Equals("Show All", StringComparison.OrdinalIgnoreCase) || Regex.IsMatch(item.Value, value, RegexOptions.IgnoreCase) || (allSection.ContainsKey("1") && Regex.IsMatch(allSection["1"], value, RegexOptions.IgnoreCase)))
					{
						CreateItemButton(item.Value, int.TryParse(item.Key, out var result) ? result : 0, item.Value);
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModMain.LogWarning("Search Error: " + ex.Message + ":" + ex.StackTrace);
		}
		ModMain.Log("Search complete.");
	}

	private void CreateItemButton(string itemName, int itemId, string name)
	{
		ButtonRef btn = UIFactory.CreateButton(searchGroup, "SearchSkill-button-" + itemName, name);
		UIFactory.SetLayoutElement(btn.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 35, flexibleWidth: 999);
		searchButtons[itemId] = btn;
		if (itemId.Equals(1))
		{
			btn.ButtonText.color = Color.cyan;
			btn.ButtonText.text += LocalizationHelper.T("panel_searchskill_label_exclude");
		}
		else
		{
			btn.ButtonText.color = Color.white;
		}
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
		InputFieldRef quantityInput = UIFactory.CreateInputField(gameObject, $"item-quantity-group-{item.Id}-quantity", LocalizationHelper.T("panel_searchskill_input_quantity_placeholder"));
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
