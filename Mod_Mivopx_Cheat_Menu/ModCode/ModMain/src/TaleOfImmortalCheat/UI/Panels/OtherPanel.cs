using System;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;
using MOD_Mivopx.UI.Panels;
using TaleOfImmortalCheat.Localization;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace TaleOfImmortalCheat.UI.Panels;

internal class OtherPanel : Panel
{
	private const string PanelName = "Other";

	private Dropdown branchNameDropdown;

	private string selectedBranchName;

	private Dropdown allBuildSubDropdown;

	private Button enterBuildingButton;

	private List<string> branchNames = new List<string>();

	private GameObject _currSectLabel;

	private TooltipPanel tooltip = new TooltipPanel("ButtonTooltip");

	private Dictionary<RectTransform, string> buttonTooltips = new Dictionary<RectTransform, string>();

	private bool IsAccessSectInit;

	private ButtonRef buttonRef_OtherPanelz;

	private ButtonRef buttonRefUpdateStoreIgnoreTime;

	private ButtonRef buttonRef_WCombine;

	private ButtonRef buttonRef_FCombine;

	private ButtonRef buttonRefRest;

	private ButtonRef buttonRef_ForceBeginAuction;

	private ButtonRef buttonRef_ResetBeginAuction;

	private ButtonRef buttonRef_TPtoDest;

	private ButtonRef buttonRef_OpenStorage;

	private ButtonRef buttonRef_OpenPortal;

	private ButtonRef buttonRef_ClearInventory;

	private ButtonRef buttonRef_ClearStorage;

	private ButtonRef buttonRef_AccessSect;

	private ButtonRef buttonRef_EnterBuilding;

	private Text titleBarText;

	public OtherPanel()
		: base(isStartedVisible: false)
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~OtherPanel()
	{
		UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
	}

	private ButtonRef CreateLocalizedButton(GameObject parent, string name, string textKey, Action onClick, string tooltipKey = null)
	{
		ButtonRef buttonRef = UIFactory.CreateButton(parent, name, LocalizationHelper.T(textKey));
		GameObject gameObject = buttonRef.Component.gameObject;
		UIFactory.SetLayoutElement(gameObject, null, 35, null, 0);
		buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, onClick);
		if (!string.IsNullOrEmpty(tooltipKey))
		{
			buttonTooltips[gameObject.GetComponent<RectTransform>()] = tooltipKey;
		}
		return buttonRef;
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
		if (ExploitPatch_UISchool.IsThisActive && ExploitPatch_UISchool._instance != null && IsAccessSectInit)
		{
			UpdateSectLabel();
		}
		Dictionary<RectTransform, string>.Enumerator enumerator = buttonTooltips.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<RectTransform, string> current = enumerator.Current;
			if (current.Key != null && !string.IsNullOrEmpty(current.Value))
			{
				Vector3 point = current.Key.InverseTransformPoint(mousePosition);
				if (current.Key.rect.Contains(point))
				{
					string label = (current.Value.StartsWith("tooltip_") ? LocalizationHelper.T(current.Value) : current.Value);
					tooltip.TooltipFor(current.Key, label);
					tooltip.IsVisible = true;
					break;
				}
			}
		}
		return result;
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		Debug.Log("Cheat UI - Other panel");
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel("Other", uiRoot, out contentHolder);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, false, true, true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0.5f);
		component.anchorMax = new Vector2(0.5f, 0.5f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 375f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 532f);
		GameObject gameObject2 = UIHelper.CreateTitleBar(contentHolder, delegate
		{
			base.IsVisible = false;
		}, GetTitleBarText());
		titleBarText = gameObject2.GetComponentInChildren<Text>();
		buttonTooltips.Clear();
		buttonRef_OtherPanelz = CreateLocalizedButton(contentHolder, "Other-more-panel", "panel_other_button_more_panel", MoreOtherPanel, "tooltip_other_more_panel");
		buttonRefUpdateStoreIgnoreTime = CreateLocalizedButton(contentHolder, "Other-force-refresh-stores", "panel_other_button_force_refresh_stores", UpdateTownMarket, "tooltip_other_force_refresh_stores");
		buttonRef_WCombine = CreateLocalizedButton(contentHolder, "Other-free-workshop-combine", "panel_other_button_free_workshop_combine", FreeWorkshopCombine, "tooltip_other_free_workshop_combine");
		buttonRef_FCombine = CreateLocalizedButton(contentHolder, "Other-free-fruit-combine", "panel_other_button_free_fruit_combine", FreeFruits, "tooltip_other_free_fruit_combine");
		buttonRefRest = CreateLocalizedButton(contentHolder, "Other-force-rest", "panel_other_button_force_rest", ForceRest, "tooltip_other_force_rest");
		buttonRef_ForceBeginAuction = CreateLocalizedButton(contentHolder, "Other-begin-auction", "panel_other_button_begin_auction", delegate
		{
			g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(LocalizationHelper.T("panel_other_popup_begin_auction_title"), LocalizationHelper.T("panel_other_popup_confirm_message"), 2, (Action)delegate
			{
				ForceBeginAuction();
			}, (Action)delegate
			{
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_auction_cancelled"));
			});
		}, "tooltip_other_begin_auction");
		buttonRef_ResetBeginAuction = CreateLocalizedButton(contentHolder, "Other-reset-auction", "panel_other_button_reset_auction", delegate
		{
			g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(LocalizationHelper.T("panel_other_popup_reset_auction_title"), LocalizationHelper.T("panel_other_popup_confirm_message"), 2, (Action)delegate
			{
				ResetBeginAuction();
			}, (Action)delegate
			{
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_reset_auction_cancelled"));
			});
		}, "tooltip_other_reset_auction");
		buttonRef_TPtoDest = CreateLocalizedButton(contentHolder, "Other-teleport-destination", "panel_other_button_teleport_destination", TPToDest, "tooltip_other_teleport_destination");
		buttonRef_OpenStorage = CreateLocalizedButton(contentHolder, "Other-force-open-storage", "panel_other_button_force_open_storage", delegate
		{
			OpenStorage(buttonRef_OpenStorage.Component.gameObject);
		}, "tooltip_other_force_open_storage");
		buttonRef_OpenPortal = CreateLocalizedButton(contentHolder, "Other-force-open-portal", "panel_other_button_force_open_portal", delegate
		{
			OpenPortal(buttonRef_OpenPortal.Component.gameObject);
		}, "tooltip_other_force_open_portal");
		buttonRef_ClearInventory = CreateLocalizedButton(contentHolder, "Other-clear-inventory", "panel_other_button_clear_inventory", delegate
		{
			g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(LocalizationHelper.T("panel_other_popup_clear_inventory_title"), LocalizationHelper.T("panel_other_popup_confirm_message"), 2, (Action)delegate
			{
				DataUnit.UnitDataProps propData = Game.WorldManager.Value.playerUnit.data.unitData.propData;
				if (propData != null)
				{
					for (int num = propData.allProps.Count - 1; num >= 0; num--)
					{
						DataProps.PropsData propsData = propData.allProps[num];
						if (propsData != null && !Game.WorldManager.Value.playerUnit.data.unitData.GetAllEquipPropsSoleID().Contains(propsData.soleID) && propsData.propsID != 10001 && propsData.propsID != 10011 && propsData.propsID != 10041)
						{
							Game.WorldManager.Value.playerUnit.data.CostPropItem(propsData.propsID, propsData.propsCount);
						}
					}
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_inventory_cleared"));
				}
				else
				{
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_inventory_data_null"), "WARNING");
				}
			}, (Action)delegate
			{
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_clear_inventory_cancelled"));
			});
		}, "tooltip_other_clear_inventory");
		buttonRef_ClearStorage = CreateLocalizedButton(contentHolder, "Other-clear-storage", "panel_other_button_clear_storage", delegate
		{
			g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(LocalizationHelper.T("panel_other_popup_clear_storage_title"), LocalizationHelper.T("panel_other_popup_confirm_message"), 2, (Action)delegate
			{
				if (MapBuildTownStorage.GetPlayerStorage() != null)
				{
					MapBuildTownStorage.GetPlayerStorage().data.propData.ClearAllProps();
					MapBuildTownStorage.GetPlayerStorage().data.propDataBook.ClearAllProps();
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_storage_cleared"));
				}
				else
				{
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_storage_data_null"), "WARNING");
				}
			}, (Action)delegate
			{
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_clear_storage_cancelled"));
			});
		}, "tooltip_other_clear_storage");
		buttonRef_AccessSect = CreateLocalizedButton(contentHolder, "Other-force-access-sect", "panel_other_button_force_access_sect", delegate
		{
			LateInitBranchNameDropDown(contentHolder);
			ExpandPanel(component);
			DisableButton(buttonRef_AccessSect);
			IsAccessSectInit = true;
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_force_access_sect_success"));
		}, "tooltip_other_force_access_sect");
		tooltip.Create();
		return (panelRoot: gameObject, draggableArea: gameObject2);
	}

	internal void MoreOtherPanel()
	{
		UIManager.Panels[PanelType.OtherMore].IsVisible = !UIManager.Panels[PanelType.OtherMore].IsVisible;
	}

	private string GetTitleBarText()
	{
		string text = LocalizationHelper.T("common_cheatpanel");
		return "<b>" + text + "</b> - " + LocalizationHelper.T("panel_other_title");
	}

	private void UpdateUITexts()
	{
		if (titleBarText != null)
		{
			titleBarText.text = GetTitleBarText();
		}
		if (buttonRef_OtherPanelz != null)
		{
			buttonRef_OtherPanelz.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_more_panel");
		}
		if (buttonRefUpdateStoreIgnoreTime != null)
		{
			buttonRefUpdateStoreIgnoreTime.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_force_refresh_stores");
		}
		if (buttonRef_WCombine != null)
		{
			buttonRef_WCombine.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_free_workshop_combine");
		}
		if (buttonRef_FCombine != null)
		{
			buttonRef_FCombine.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_free_fruit_combine");
		}
		if (buttonRefRest != null)
		{
			buttonRefRest.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_force_rest");
		}
		if (buttonRef_ForceBeginAuction != null)
		{
			buttonRef_ForceBeginAuction.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_begin_auction");
		}
		if (buttonRef_ResetBeginAuction != null)
		{
			buttonRef_ResetBeginAuction.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_reset_auction");
		}
		if (buttonRef_TPtoDest != null)
		{
			buttonRef_TPtoDest.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_teleport_destination");
		}
		if (buttonRef_OpenStorage != null)
		{
			buttonRef_OpenStorage.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_force_open_storage");
		}
		if (buttonRef_OpenPortal != null)
		{
			buttonRef_OpenPortal.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_force_open_portal");
		}
		if (buttonRef_ClearInventory != null)
		{
			buttonRef_ClearInventory.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_clear_inventory");
		}
		if (buttonRef_ClearStorage != null)
		{
			buttonRef_ClearStorage.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_clear_storage");
		}
		if (buttonRef_AccessSect != null)
		{
			buttonRef_AccessSect.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_force_access_sect");
		}
		if (buttonRef_EnterBuilding != null)
		{
			buttonRef_EnterBuilding.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_other_button_enter_building");
		}
		if (branchNameDropdown != null)
		{
			Text componentInChildren = branchNameDropdown.GetComponentInChildren<Text>();
			if (componentInChildren != null && componentInChildren.text == "Select Branch Name")
			{
				componentInChildren.text = LocalizationHelper.T("panel_other_dropdown_select_branch");
			}
		}
		if (allBuildSubDropdown != null)
		{
			Text componentInChildren2 = allBuildSubDropdown.GetComponentInChildren<Text>();
			if (componentInChildren2 != null && componentInChildren2.text == "Select Build SubType")
			{
				componentInChildren2.text = LocalizationHelper.T("panel_other_dropdown_select_building");
			}
		}
		if (_currSectLabel != null)
		{
			Text component = _currSectLabel.GetComponent<Text>();
			if (component != null && component.text == "Select a Sect Branch.")
			{
				component.text = LocalizationHelper.T("panel_other_label_select_sect_branch");
			}
		}
	}

	private void ExpandPanel(RectTransform component)
	{
		component.anchorMin = new Vector2(0f, 0.3f);
		component.anchorMax = new Vector2(0.3f, 0.3f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 375f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 685f);
		Vector2 anchoredPosition = component.anchoredPosition;
		Vector2 sizeDelta = component.sizeDelta;
		float num = sizeDelta.x / 2f;
		float num2 = sizeDelta.y / 2f;
		float num3 = Screen.width;
		float num4 = Screen.height;
		if (anchoredPosition.x - num < 0f)
		{
			anchoredPosition.x = num;
		}
		if (anchoredPosition.x + num > num3)
		{
			anchoredPosition.x = num3 - num;
		}
		if (anchoredPosition.y - num2 < 0f)
		{
			anchoredPosition.y = num2;
		}
		if (anchoredPosition.y + num2 > num4)
		{
			anchoredPosition.y = num4 - num2;
		}
		component.anchoredPosition = anchoredPosition;
	}

	private void DisableButton(ButtonRef buttonRef)
	{
		buttonRef.Component.interactable = false;
	}

	private void LateInitBranchNameDropDown(GameObject contentHolder)
	{
		if (Game.WorldManager.Value.build.GetBuilds(MapTerrainType.School) != null && Game.WorldManager.Value != null)
		{
			CreateBranchNameDropdown(contentHolder);
			CreateCurrLabelSect(contentHolder);
			CreateAllBuildSubDropdown(contentHolder);
			CreateEnterBuildingButton(contentHolder);
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_ui_expanded"));
		}
		else
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_dropdown_creation_failed"), "WARNING");
		}
	}

	private void CreateBranchNameDropdown(GameObject parent)
	{
		List<MapBuildBase>.Enumerator enumerator = Game.WorldManager.Value.build.GetBuilds(MapTerrainType.School).GetEnumerator();
		while (enumerator.MoveNext())
		{
			MapBuildBase current = enumerator.Current;
			if (current.name != null)
			{
				branchNames.Add(current.name);
			}
		}
		GameObject gameObject = UIFactory.CreateDropdown(parent, "branchNameDropdown", out branchNameDropdown, LocalizationHelper.T("panel_other_dropdown_select_branch"), 14, OnBranchNameDropdownValueChanged, branchNames.ToArray());
		UIFactory.SetLayoutElement(gameObject, null, 35, null, 0);
		buttonTooltips[gameObject.GetComponent<RectTransform>()] = LocalizationHelper.T("tooltip_other_select_branch_name");
	}

	private void OnBranchNameDropdownValueChanged(int selectedIndex)
	{
		selectedBranchName = branchNameDropdown.options[selectedIndex].text;
		ModMain.Log("Selected Branch Name: " + selectedBranchName);
		PopulateAllBuildSubDropdown();
	}

	private void CreateCurrLabelSect(GameObject parent)
	{
		_currSectLabel = UIFactory.CreateLabel(parent, "SectName-label", LocalizationHelper.T("panel_other_label_sect_name"), TextAnchor.MiddleCenter, Color.cyan, supportRichText: false, 15).gameObject;
		_currSectLabel.GetComponent<Text>().text = LocalizationHelper.T("panel_other_label_select_sect_branch");
		UIFactory.SetLayoutElement(_currSectLabel, null, 35, null, 0);
		buttonTooltips[_currSectLabel.gameObject.GetComponent<RectTransform>()] = LocalizationHelper.T("tooltip_other_current_sect_location");
	}

	private void UpdateSectLabel()
	{
		ExploitPatch_UISchool.IsThisActive = false;
		MapBuildSchool school = ExploitPatch_UISchool._instance.school;
		List<MapBuildBase> builds = Game.WorldManager.Value.build.GetBuilds(MapTerrainType.School);
		if (ExploitPatch_UISchool._instance == null || school == null || builds == null)
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_sect_not_visible"), "WARNING");
			return;
		}
		if (ExploitPatch_UISchool._instance.school != null)
		{
			selectedBranchName = ExploitPatch_UISchool._instance.school.name;
			ModMain.LogTip(string.Format(LocalizationHelper.T("panel_other_status_updating_branch_name"), selectedBranchName));
			for (int i = 0; i < branchNameDropdown.options.Count; i++)
			{
				if (branchNameDropdown.options[i].text == selectedBranchName)
				{
					branchNameDropdown.value = i;
					branchNameDropdown.RefreshShownValue();
					break;
				}
			}
		}
		PopulateAllBuildSubDropdown();
	}

	private void CreateAllBuildSubDropdown(GameObject parent)
	{
		GameObject gameObject = UIFactory.CreateDropdown(parent, "allBuildSubDropdown", out allBuildSubDropdown, LocalizationHelper.T("panel_other_dropdown_select_building"), 14, OnAllBuildSubDropdownValueChanged, new string[1] { LocalizationHelper.T("panel_other_dropdown_select_sect_first") });
		UIFactory.SetLayoutElement(gameObject, null, 35, null, 0);
		buttonTooltips[gameObject.GetComponent<RectTransform>()] = LocalizationHelper.T("tooltip_other_select_sect_building");
	}

	private void PopulateAllBuildSubDropdown()
	{
		if (_currSectLabel != null)
		{
			_currSectLabel.GetComponent<Text>().text = LocalizationHelper.T("panel_other_label_current_sect") + branchNameDropdown.options[branchNameDropdown.value].text;
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_current_sect_updated"));
		}
		List<MapBuildBase> builds = Game.WorldManager.Value.build.GetBuilds(MapTerrainType.School);
		if (builds == null)
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_school_null"), "WARNING");
			return;
		}
		List<string> list = new List<string>();
		List<MapBuildBase>.Enumerator enumerator = builds.GetEnumerator();
		while (enumerator.MoveNext())
		{
			MapBuildBase current = enumerator.Current;
			if (current == null || !(current.name == branchNameDropdown.options[branchNameDropdown.value].text))
			{
				continue;
			}
			Dictionary<MapBuildSubType, MapBuildSubBase>.Enumerator enumerator2 = current.allBuildSub.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				KeyValuePair<MapBuildSubType, MapBuildSubBase> current2 = enumerator2.Current;
				if (current2 != null && current2.Value.IsShowBuildUI())
				{
					list.Add(current2.Value.name.ToString());
				}
			}
		}
		allBuildSubDropdown.ClearOptions();
		allBuildSubDropdown.AddOptions(list);
		enterBuildingButton.gameObject.SetActive(value: true);
	}

	private void OnAllBuildSubDropdownValueChanged(int selectedIndex)
	{
		string text = allBuildSubDropdown.options[selectedIndex].text;
		ModMain.LogTip(string.Format(LocalizationHelper.T("panel_other_status_selected_building"), text));
	}

	private void CreateEnterBuildingButton(GameObject parent)
	{
		buttonRef_EnterBuilding = UIFactory.CreateButton(parent, "EnterBuilding", LocalizationHelper.T("panel_other_button_enter_building"));
		GameObject gameObject = buttonRef_EnterBuilding.Component.gameObject;
		UIFactory.SetLayoutElement(gameObject, null, 35, null, 0);
		enterBuildingButton = buttonRef_EnterBuilding.Component;
		enterBuildingButton.gameObject.SetActive(value: false);
		buttonRef_EnterBuilding.OnClick = (Action)Delegate.Combine(buttonRef_EnterBuilding.OnClick, new Action(EnterBuilding));
		buttonTooltips[gameObject.GetComponent<RectTransform>()] = LocalizationHelper.T("tooltip_other_enter_building");
	}

	private void EnterBuilding()
	{
		List<MapBuildBase> builds = Game.WorldManager.Value.build.GetBuilds(MapTerrainType.School);
		if (builds != null)
		{
			string text = branchNameDropdown.options[branchNameDropdown.value].text;
			string text2 = allBuildSubDropdown.options[allBuildSubDropdown.value].text;
			List<MapBuildBase>.Enumerator enumerator = builds.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MapBuildBase current = enumerator.Current;
				if (!(current.name == text) || current == null)
				{
					continue;
				}
				if (ExploitPatch_UISchool._instance == null)
				{
					current.OpenBuild();
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_player_not_inside_sect"), "WARNING", 5f);
				}
				else if (text != ExploitPatch_UISchool._instance.school.name)
				{
					ExploitPatch_UISchool._instance.school.ExitSchool(Game.WorldManager.Value.playerUnit);
					current.OpenBuild();
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_player_different_sect"), "WARNING", 5f);
				}
				MapBuildSchool school = ExploitPatch_UISchool._instance.school;
				ModMain.Log("----- ----- Sect Info: ----- -----");
				ModMain.Log("Instance Name: " + school.name);
				ModMain.Log("Instance Name Origin: " + school.nameOrigin);
				ModMain.Log("Instance Name Main: " + school.mainName);
				ModMain.Log("Sect Name: " + school.schoolData?.school?.name);
				ModMain.Log("Sect Branch Name: " + school.schoolData?.school?.branchName);
				ModMain.Log("Sect Data ID: " + school.buildData?.id);
				ModMain.Log("Sect Data Reputation: " + school.buildData?.reputation.ToString());
				ModMain.Log("Sect Data Total Member: " + school.buildData?.totalMember.ToString());
				ModMain.Log("----- ----- ----- ----- -----");
				Dictionary<MapBuildSubType, MapBuildSubBase>.Enumerator enumerator2 = current.allBuildSub.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					KeyValuePair<MapBuildSubType, MapBuildSubBase> current2 = enumerator2.Current;
					if (current2 != null && current2.Value.IsShowBuildUI() && current2.Value.name.ToString() == text2)
					{
						current2.Value.OpenBuild(ExploitPatch_UISchool._instance.gameObject);
						ModMain.LogTip(string.Format(LocalizationHelper.T("panel_other_status_force_entered_building"), text2));
						ModMain.Log("Name: " + current2.Value.name.ToString());
						ModMain.Log("isShowNameMap: " + current2.Value.mainBuild.isShowNameMap);
						ModMain.Log("IsShowBuildUI: " + current2.Value.IsShowBuildUI());
						ModMain.Log("BuildSubType: " + current2.Key.ToString() + " - BuildSubBase: " + current2.Value.ToString());
						ModMain.Log("----- ----- ----- ----- -----");
						break;
					}
				}
			}
		}
		else
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_entering_building_failed"), "WARNING");
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_sect_not_visible"), "WARNING", 3f);
		}
	}

	internal static void UpdateTownMarket()
	{
		try
		{
			ModMain.Log("----- ----- ----- ----- -----");
			ModMain.Log("----- ----- Update Global Stores ----- -----");
			if (IsAnyStoreVisible())
			{
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_refreshing_current_store"));
			}
			else
			{
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_not_in_store"), "WARNING");
			}
			if (TownStoresPatch_UITownMarketBuy._UITownMarketBuy != null)
			{
				MapBuildTownMarket.UpdateTownMarketIgnoreTime();
				TownStoresPatch_UITownMarketBuy._UITownMarketBuy.UpdateUI(TownStoresPatch_UITownMarketBuy._UITownMarketBuy.curType);
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("Store UI Name: " + TownStoresPatch_UITownMarketBuy._UITownMarketBuy.name.ToString());
				ModMain.Log("Store Name: " + TownStoresPatch_UITownMarketBuy._UITownMarketBuy.townMarket.name.ToString());
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_town_market_refreshed"));
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogWarning("Market is not visible! Please make sure you're inside the Market!");
			}
			if (TownStoresPatch_UITownMarketBook._UITownMarketBook != null)
			{
				TownStoresPatch_UITownMarketBook._UITownMarketBook.townMarketBook.UpdateMartial();
				TownStoresPatch_UITownMarketBook._UITownMarketBook.UpdateUI();
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("Store UI Name: " + TownStoresPatch_UITownMarketBook._UITownMarketBook.name.ToString());
				ModMain.Log("Store Name: " + TownStoresPatch_UITownMarketBook._UITownMarketBook.townMarketBook.name.ToString());
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_manual_pavilion_refreshed"));
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogWarning("Manual Pavilion is not visible! Please make sure you're inside the Manual Pavilion!");
			}
			if (TownStoresPatch_UITownMarketCloth._UITownMarketCloth != null)
			{
				TownStoresPatch_UITownMarketCloth._UITownMarketCloth.townMarketCloth.UpdateCloth();
				g.ui.CloseUI(TownStoresPatch_UITownMarketCloth._UITownMarketCloth, isForceClose: true);
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("Store UI Name: " + TownStoresPatch_UITownMarketCloth._UITownMarketCloth.name.ToString());
				ModMain.Log("Store Name: " + TownStoresPatch_UITownMarketCloth._UITownMarketCloth.townMarketCloth.name.ToString());
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_wardrobe_closing"));
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_wardrobe_refreshed"), null, 3f);
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_open_wardrobe_changes"), null, 4f);
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogWarning("Immortal Wardrobe is not visible! Please make sure you're inside the Immortal Wardrobe!");
			}
			if (TownStoresPatch_UISchoolLibrary._UISchoolLibrary != null)
			{
				TownStoresPatch_UISchoolLibrary._UISchoolLibrary.schoolLibrary.UpdateAllMartial();
				TownStoresPatch_UISchoolLibrary._UISchoolLibrary.UpdateUI();
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("Sect UI Name: " + TownStoresPatch_UISchoolLibrary._UISchoolLibrary.name.ToString());
				ModMain.Log("Sect Name: " + TownStoresPatch_UISchoolLibrary._UISchoolLibrary.schoolLibrary.name.ToString());
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_sect_library_refreshed"));
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogWarning("Manual Library is not visible! Please make sure you're inside the Manual Library!");
			}
			if (TownStoresPatch_UIFairyChickenFight._UIFairyChickenFight != null)
			{
				TownStoresPatch_UIFairyChickenFight._UIFairyChickenFight._build.UpdateStoreIgnoreTime();
				TownStoresPatch_UIFairyChickenFight._UIFairyChickenFight.UpdateStorePropsUI();
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("Store UI Name: " + TownStoresPatch_UIFairyChickenFight._UIFairyChickenFight.name.ToString());
				ModMain.Log("Store Name: " + TownStoresPatch_UIFairyChickenFight._UIFairyChickenFight._build.name.ToString());
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_arena_manuals_refreshed"));
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogWarning("Cockfight Arena is not visible! Please make sure you're inside the Cockfight Arena!");
			}
			if (TownStoresPatch_UISchoolStore._instance != null)
			{
				TownStoresPatch_UISchoolStore._instance.schoolStore.UpdateStock(isForceUpdate: true);
				TownStoresPatch_UISchoolStore._instance.UpdateUI();
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("Store UI Name: " + TownStoresPatch_UISchoolStore._instance.name);
				ModMain.Log("Store Name: " + TownStoresPatch_UISchoolStore._instance.schoolStore.name);
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_treasure_pavilion_refreshed"));
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogWarning("Treasure Pavilion is not visible! Please make sure you're inside the Treasure Pavilion!");
			}
		}
		catch (Exception ex)
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_store_update_error_message").Replace("{0}", ex.Message).Replace("{1}", ex.StackTrace), "WARNING");
		}
	}

	internal static bool IsAnyStoreVisible()
	{
		if (!(TownStoresPatch_UITownMarketBuy._UITownMarketBuy != null) && !(TownStoresPatch_UITownMarketBook._UITownMarketBook != null) && !(TownStoresPatch_UITownMarketCloth._UITownMarketCloth != null) && !(TownStoresPatch_UISchoolLibrary._UISchoolLibrary != null) && !(TownStoresPatch_UIFairyChickenFight._UIFairyChickenFight != null))
		{
			return TownStoresPatch_UISchoolStore._instance != null;
		}
		return true;
	}

	internal void FreeWorkshopCombine()
	{
		try
		{
			if (TownStoresPatch_UITownRefine._UITownRefine != null)
			{
				ModMain.Log("----- ----- Workshop Combine ----- -----");
				ModMain.Log("oneCost: " + TownStoresPatch_UITownRefine._UITownRefine.oneCost);
				TownStoresPatch_UITownRefine._UITownRefine.oneCost = 0;
				ModMain.Log("Modified - oneCost: " + TownStoresPatch_UITownRefine._UITownRefine.oneCost);
				ModMain.Log("curCostId: " + TownStoresPatch_UITownRefine._UITownRefine.curCostId);
				ModMain.Log("refineItem: " + TownStoresPatch_UITownRefine._UITownRefine.refineItem.ToString());
				ModMain.Log("refineItem name: " + TownStoresPatch_UITownRefine._UITownRefine.refineItem.name.ToString());
				ModMain.Log("refineItem probability: " + TownStoresPatch_UITownRefine._UITownRefine.refineItem.probability);
				TownStoresPatch_UITownRefine._UITownRefine.refineItem.probability = 100;
				ModMain.Log("Modified - refineItem probability: " + TownStoresPatch_UITownRefine._UITownRefine.refineItem.probability);
				ModMain.Log("refineMaxNum: " + TownStoresPatch_UITownRefine._UITownRefine.refineMaxNum);
				TownStoresPatch_UITownRefine._UITownRefine.refineMaxNum = 1000;
				ModMain.Log("Modified - refineMaxNum: " + TownStoresPatch_UITownRefine._UITownRefine.refineMaxNum);
				ModMain.Log("moneyCost: " + TownStoresPatch_UITownRefine._UITownRefine.moneyCost);
				TownStoresPatch_UITownRefine._UITownRefine.moneyCost = 0;
				ModMain.Log("Modified - moneyCost: " + TownStoresPatch_UITownRefine._UITownRefine.moneyCost);
				ModMain.Log("isRefineOK: " + TownStoresPatch_UITownRefine._UITownRefine.isRefineOK);
				TownStoresPatch_UITownRefine._UITownRefine.isRefineOK = true;
				ModMain.Log("Modified - isRefineOK: " + TownStoresPatch_UITownRefine._UITownRefine.isRefineOK);
				TownStoresPatch_UITownRefine._UITownRefine.OnOkClick();
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("Store UI Name: " + TownStoresPatch_UITownRefine._UITownRefine.name.ToString());
				ModMain.Log("Store Name: " + TownStoresPatch_UITownRefine._UITownRefine.town.name.ToString());
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_combine_success"));
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_combine_not_visible"), "WARNING");
			}
		}
		catch (Exception ex)
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_combine_error_message").Replace("{0}", ex.Message).Replace("{1}", ex.StackTrace), "WARNING");
		}
	}

	internal void FreeFruits()
	{
		try
		{
			if (ExploitPatch_SectDragonExchangeItem._instance != null)
			{
				UISchoolGetSoul instance = ExploitPatch_SectDragonExchangeItem._instance;
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("OtherPanel - SectDragonExchangeItem - Called");
				ModMain.Log("----- ----- ----- ----- -----");
				if (instance.mainProp != null)
				{
					ModMain.Log("confItem.excreteCount: " + instance.confItem?.excreteCount.ToString());
					instance.confItem.excreteCount = 69;
					ModMain.Log("Modified - confItem.excreteCount: " + instance.confItem?.excreteCount.ToString());
					ModMain.Log("propCount: " + instance.propCount);
					instance.propCount = 2;
					ModMain.Log("Modified - propCount: " + instance.propCount);
					ModMain.Log("confItem.area: " + instance.confItem?.area.ToString());
					ModMain.Log("confItem.eatItemCount: " + instance.confItem?.eatItemCount.ToString());
					instance.OKClick();
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_fruit_tips"));
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_select_another_fruit"));
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_fruit_created"));
				}
				else
				{
					ModMain.LogWarning("----- ----- ----- ----- -----");
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_main_prop_empty"), "WARNING");
				}
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_spirit_not_visible"), "WARNING");
			}
		}
		catch (Exception ex)
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_fruit_error_message").Replace("{0}", ex.Message).Replace("{1}", ex.StackTrace), "WARNING");
		}
	}

	internal static void ForceRest()
	{
		try
		{
			WorldUnitDynData worldUnitDynData = Game.WorldManager?.Value?.playerUnit?.data?.dynUnitData;
			if (worldUnitDynData != null)
			{
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_player_resting_message"));
				ResetAndNormalizeStat(worldUnitDynData.health, worldUnitDynData.healthMax, "Health");
				ResetAndNormalizeStat(worldUnitDynData.hp, worldUnitDynData.hpMax, "Vitality");
				ResetAndNormalizeStat(worldUnitDynData.mp, worldUnitDynData.mpMax, "Energy");
				ResetAndNormalizeStat(worldUnitDynData.sp, worldUnitDynData.spMax, "Focus");
				ResetAndNormalizeStat(worldUnitDynData.energy, worldUnitDynData.energyMax, "Stamina");
				ResetAndNormalizeStat(worldUnitDynData.mood, worldUnitDynData.moodMax, "Mood");
				if (SceneMapPatch_UIMapMainPlayerInfo._UIMapMainPlayerInfo != null)
				{
					SceneMapPatch_UIMapMainPlayerInfo._UIMapMainPlayerInfo.UpdateUI();
					ModMain.Log("Updating Map Player Info UI...");
					ModMain.Log("Success!");
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_player_finished_resting_message"));
				}
				else
				{
					ModMain.LogWarning("Updating Map Player Info UI...");
					ModMain.LogWarning("Failed! It needs a manual reset...");
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_player_finished_resting_message"));
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_open_stats_ui_message"), null, 3f);
				}
			}
			else
			{
				ModMain.LogWarning("Player unit is null. Force Rest failed.");
			}
		}
		catch (Exception ex)
		{
			ModMain.LogWarning("An error occurred: " + ex.Message + ":" + ex.StackTrace);
		}
	}

	private static void ResetAndNormalizeStat(dynamic stat, dynamic statMax, string statName)
	{
		if (stat != null && statMax != null)
		{
			ModMain.Log($"{statName}: {(object)stat.baseValue}");
			ModMain.Log($"{statName} Max: {(object)statMax.baseValue}");
			if (stat.baseValue < statMax.baseValue)
			{
				ModMain.Log(statName + " below Max. Resetting...");
				stat.baseValue = statMax.baseValue;
			}
			if (stat.baseValue >= statMax.baseValue)
			{
				ModMain.Log(statName + " exceeds Max. Incrementing...");
                stat.baseValue += statMax.baseValue;
			}
			ModMain.Log($"{statName} reset to Max: {(object)stat.baseValue}");
		}
		else
		{
			ModMain.LogTip("Player " + statName + " returned NULL!", "WARNING");
		}
	}

	internal static void TPToDest()
	{
		try
		{
			if (SceneMapPatch._SceneMap != null)
			{
				_ = SceneMapPatch._SceneMap.world.curSelectPoint;
				if (SceneMapPatch._SceneMap.world.curSelectPoint.x != -1)
				{
					ModMain.Log("Player Current Location: " + SceneMapPatch._SceneMap.world.playerPoint.ToString());
					ModMain.Log("Selected Target Location: " + SceneMapPatch._SceneMap.world.curSelectPoint.ToString());
					Game.WorldManager.Value.playerUnit.data.unitData.SetPoint(SceneMapPatch._SceneMap.world.curSelectPoint);
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_teleported_to_destination_message").Replace("{0}", SceneMapPatch._SceneMap.world.curSelectPoint.ToString()));
					SceneMapPatch._SceneMap.world.UpdateAllUI();
				}
				else
				{
					ModMain.LogTip(LocalizationHelper.T("panel_other_status_select_target_location_message"), "WARNING", 4f);
				}
			}
			else
			{
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_scene_map_empty_message"), "WARNING");
			}
		}
		catch (Exception ex)
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_teleport_error_message").Replace("{0}", ex.Message).Replace("{1}", ex.StackTrace), "WARNING");
		}
	}

	internal void OpenStorage(GameObject gameObject)
	{
		if (MapBuildTownStorage.GetPlayerStorage() != null)
		{
			ModMain.Log("Checking Storage...");
			ModMain.Log("Checking Player Current Location...");
			ModMain.Log("Finding Storage...");
			MapBuildTownStorage.GetPlayerStorage().OpenBuild(gameObject);
			if (MapBuildTownStorage.GetPlayerStorage().OpenBuild(gameObject))
			{
				ModMain.Log("Storage Found!");
			}
			else
			{
				ModMain.Log("Storage not Found!");
			}
		}
		else
		{
			ModMain.LogWarning("Player Storage returned NULL!");
		}
	}

	internal void OpenPortal(GameObject gameObject)
	{
		if (ExploitPatch_MapBuildTown._instance != null)
		{
			ModMain.Log("Checking Portal...");
			ModMain.Log("Checking Player Current Location...");
			ModMain.Log("Finding Portal...");
			MapBuildTown instance = ExploitPatch_MapBuildTown._instance;
			ModMain.Log("Checking Portal...");
			if (instance.GetBuildSub(MapBuildSubType.TownTransfer) == null)
			{
				ModMain.LogWarning("Portal not found, adding a new Portal...");
				instance.AddBuildSub(MapBuildSubType.TownTransfer);
				ModMain.Log("Re-checking Portal...");
			}
			if (instance.GetBuildSub(MapBuildSubType.TownTransfer) != null)
			{
				instance.GetBuildSub(MapBuildSubType.TownTransfer).OpenBuild(gameObject);
				ModMain.Log("Portal Found!");
			}
			else
			{
				ModMain.Log("There's something wrong with the portal...");
			}
		}
		else
		{
			ModMain.LogWarning("Player Portal returned NULL!");
		}
	}

	internal void ForceBeginAuction()
	{
		try
		{
			if (ExploitPatch_UITownAuction._instance != null && Game.ConfMgr.townAuction != null && Game.ConfMgr.townAuction._auctionInterval != null && Game.ConfMgr.townAuction._announceMonths != null)
			{
				if (Game.ConfMgr.townAuctionDrama != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("askEnergy: " + Game.ConfMgr.townAuctionDrama.askEnergy);
					Game.ConfMgr.townAuctionDrama._askEnergy.value = 0;
					ModMain.Log("Modified - askEnergy: " + Game.ConfMgr.townAuctionDrama.askEnergy);
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("threatEnergy: " + Game.ConfMgr.townAuctionDrama.threatEnergy);
					Game.ConfMgr.townAuctionDrama._threatEnergy.value = 0;
					ModMain.Log("Modified - threatEnergy: " + Game.ConfMgr.townAuctionDrama.threatEnergy);
				}
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("itemCountMin: " + Game.ConfMgr.townAuction.itemCountMin);
				if (Game.ConfMgr.townAuction.itemCountMin < 25)
				{
					Game.ConfMgr.townAuction._itemCountMin.value = 25;
				}
				else if (Game.ConfMgr.townAuction.itemCountMin > 25)
				{
					Game.ConfMgr.townAuction._itemCountMin.value++;
				}
				ModMain.Log("Modified - itemCountMin: " + Game.ConfMgr.townAuction.itemCountMin);
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("itemCount: " + Game.ConfMgr.townAuction.itemCount);
				if (Game.ConfMgr.townAuction.itemCount < 50)
				{
					Game.ConfMgr.townAuction._itemCount.value = 50;
				}
				else if (Game.ConfMgr.townAuction.itemCount > 50)
				{
					Game.ConfMgr.townAuction._itemCount.value++;
				}
				ModMain.Log("Modified - itemCount: " + Game.ConfMgr.townAuction.itemCount);
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("auctionInterval: " + Game.ConfMgr.townAuction.auctionInterval);
				Game.ConfMgr.townAuction._auctionInterval.value = 2;
				ModMain.Log("Modified - auctionInterval: " + Game.ConfMgr.townAuction.auctionInterval);
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("announceMonths: " + Game.ConfMgr.townAuction.announceMonths);
				Game.ConfMgr.townAuction._announceMonths.value = 1;
				ModMain.Log("Modified - announceMonths: " + Game.ConfMgr.townAuction.announceMonths);
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_auction_start_modified_message"));
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_auction_house_not_visible_message"), "WARNING");
			}
		}
		catch (Exception ex)
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_force_begin_auction_error_message").Replace("{0}", ex.Message).Replace("{1}", ex.StackTrace), "WARNING");
		}
	}

	internal void ResetBeginAuction()
	{
		try
		{
			if (ExploitPatch_UITownAuction._instance != null && Game.ConfMgr.townAuction != null && Game.ConfMgr.townAuction._auctionInterval != null && Game.ConfMgr.townAuction._announceMonths != null)
			{
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("auctionInterval: " + Game.ConfMgr.townAuction.auctionInterval);
				Game.ConfMgr.townAuction._auctionInterval.value = 24;
				ModMain.Log("Modified - auctionInterval: " + Game.ConfMgr.townAuction.auctionInterval);
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.Log("announceMonths: " + Game.ConfMgr.townAuction.announceMonths);
				Game.ConfMgr.townAuction._announceMonths.value = 6;
				ModMain.Log("Modified - announceMonths: " + Game.ConfMgr.townAuction.announceMonths);
				ModMain.Log("----- ----- ----- ----- -----");
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_auction_start_restored_message"));
			}
			else
			{
				ModMain.LogWarning("----- ----- ----- ----- -----");
				ModMain.LogTip(LocalizationHelper.T("panel_other_status_auction_house_not_visible_message"), "WARNING");
			}
		}
		catch (Exception ex)
		{
			ModMain.LogTip(LocalizationHelper.T("panel_other_status_reset_begin_auction_error_message").Replace("{0}", ex.Message).Replace("{1}", ex.StackTrace), "WARNING");
		}
	}
}
