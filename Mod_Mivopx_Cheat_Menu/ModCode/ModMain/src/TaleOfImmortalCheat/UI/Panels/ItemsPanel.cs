using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;
using TaleOfImmortalCheat.Localization;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace TaleOfImmortalCheat.UI.Panels;

public class ItemsPanel : Panel
{
	private const string PanelName = "Items";

	private System.Collections.Generic.List<string> grades = new System.Collections.Generic.List<string> { "role_grade_name2", "role_grade_name3", "role_grade_name4", "role_grade_name5", "role_grade_name6", "role_grade_name7", "role_grade_name8", "role_grade_name9", "role_grade_name10" };

	private Text titleBarText;

	private ButtonRef buttonGiveSpiritStones;

	private ButtonRef buttonGiveMounts;

	private ButtonRef buttonGiveSectContribution;

	private ButtonRef buttonGiveSkillMaterials;

	private ButtonRef buttonSearchItem;
	private ButtonRef buttonRemoveItem;

	private ButtonRef buttonSearchSkill;

	private ButtonRef buttonGiveMaterials;

	private ButtonRef buttonGiveEquipments;

	private ButtonRef buttonGiveMaleClothes;

	private ButtonRef buttonGiveFemaleClothes;

	private ButtonRef buttonGiveMissionToken;

	private ButtonRef buttonGiveBaguaJade;

	public ItemsPanel()
		: base(isStartedVisible: false)
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~ItemsPanel()
	{
		UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		Debug.Log("Cheat UI - Creating Items panel");
		GameObject contentHolder;
		GameObject panelGameObject = UIFactory.CreatePanel("Items", uiRoot, out contentHolder);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, false, true, true);
		RectTransform panelComponent = panelGameObject.GetComponent<RectTransform>();
		panelComponent.anchorMin = new Vector2(0.3f, 0.3f);
		panelComponent.anchorMax = new Vector2(0.8f, 0.8f);
		panelComponent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300f);
		var fitter = panelGameObject.AddComponent<ContentSizeFitter>();
		fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		// Who was hardcoding this? like wth? Also, this thing should've been scrollable in the first place. What's up
		// with 'Panel - More' bullshit
		GameObject gameObject2 = UIHelper.CreateTitleBar(contentHolder, delegate
		{
			base.IsVisible = false;
		}, GetTitleBarText());
		titleBarText = gameObject2.GetComponentInChildren<Text>();
		buttonGiveSpiritStones = CreateItemsButton("Items-give-spirit-stones", "panel_items_button_give_spirit_stones", GiveSpiritStones);
		buttonGiveMounts = CreateItemsButton("Items-give-mounts", "panel_items_button_give_mounts", GiveMountMaterials);
		buttonGiveSectContribution = CreateItemsButton("Items-give-sect-contribution", "panel_items_button_give_sect_contribution", GiveSectContribution);
		buttonGiveSkillMaterials = CreateItemsButton("Items-give-skill-materials", "panel_items_button_give_skill_materials", GiveSkillMaterials);
		buttonSearchItem = CreateItemsButton("Items-search-item", "panel_items_button_search_item", OpenSearchItem);
		buttonRemoveItem = CreateItemsButton("Items-remove-item", "panel_items_button_remove_item", OpenRemoveItem);
		buttonSearchSkill = CreateItemsButton("Items-search-skill", "panel_items_button_search_skill", OpenSearchSkill);
		GameObject parent = UIFactory.CreateHorizontalGroup(contentHolder, "Items-grade-materials-group", forceExpandWidth: true, forceExpandHeight: false, childControlWidth: true, childControlHeight: true, 5);
		buttonGiveMaterials = UIFactory.CreateButton(parent, "Items-give-brk-materials", LocalizationHelper.T("panel_items_button_give_materials"));
		UIFactory.SetLayoutElement(buttonGiveMaterials.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 70);
		UIFactory.SetLayoutElement(UIFactory.CreateDropdown(parent, "grade-materials", out var dropdown, null, 14, delegate
		{
		}), null, 35, 0, 0, 100);
		Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> list = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
		foreach (string grade in grades)
		{
			list.Add(new Dropdown.OptionData
			{
				text = Game.ConfMgr.localText.allText[grade].en
			});
		}
		dropdown.AddOptions(list);
		buttonGiveMaterials.OnClick = (Action)Delegate.Combine(buttonGiveMaterials.OnClick, (Action)delegate
		{
			GiveBreakthroughMaterials(dropdown.value);
		});
		buttonGiveEquipments = CreateItemsButton("Items-give-equipments", "panel_items_button_give_equipments", GiveEquipment);
		buttonGiveMaleClothes = CreateItemsButton("Items-give-male-clothes", "panel_items_button_give_male_clothes", GiveMaleClothes);
		buttonGiveFemaleClothes = CreateItemsButton("Items-give-female-clothes", "panel_items_button_give_female_clothes", GiveFemaleClothes);
		buttonGiveMissionToken = CreateItemsButton("Items-give-mission-token", "panel_items_button_give_mission_token", GiveMissionContribution);
		buttonGiveBaguaJade = CreateItemsButton("Items-give-bagua-jade", "panel_items_button_give_bagua_jade", GiveBaguaJade);
		Debug.Log("Cheat UI - Created Items panel");
		return (panelRoot: panelGameObject, draggableArea: gameObject2);
		ButtonRef CreateItemsButton(string name, string textKey, Action onClick, string tooltipKey = null)
		{
			ButtonRef buttonRef = UIFactory.CreateButton(contentHolder, name, LocalizationHelper.T(textKey));
			UIFactory.SetLayoutElement(buttonRef.Component.gameObject, null, 35, null, 0);
			buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, new Action(onClick.Invoke));
			return buttonRef;
		}
	}

	public override void OnGameWorldUpdate()
	{
		base.OnGameWorldUpdate();
	}

	private string GetTitleBarText()
	{
		string text = LocalizationHelper.T("common_cheatpanel");
		return "<b>" + text + "</b> - " + LocalizationHelper.T("panel_items_title");
	}

	private void UpdateUITexts()
	{
		if (titleBarText != null)
		{
			titleBarText.text = GetTitleBarText();
		}
		if (buttonGiveSpiritStones != null)
		{
			buttonGiveSpiritStones.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_spirit_stones");
		}
		if (buttonGiveMounts != null)
		{
			buttonGiveMounts.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_mounts");
		}
		if (buttonGiveSectContribution != null)
		{
			buttonGiveSectContribution.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_sect_contribution");
		}
		if (buttonGiveSkillMaterials != null)
		{
			buttonGiveSkillMaterials.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_skill_materials");
		}
		if (buttonSearchItem != null)
		{
			buttonSearchItem.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_search_item");
		}
		if (buttonRemoveItem != null)
		{
			buttonRemoveItem.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_remove_item");
		}
		if (buttonSearchSkill != null)
		{
			buttonSearchSkill.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_search_skill");
		}
		if (buttonGiveMaterials != null)
		{
			buttonGiveMaterials.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_materials");
		}
		if (buttonGiveEquipments != null)
		{
			buttonGiveEquipments.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_equipments");
		}
		if (buttonGiveMaleClothes != null)
		{
			buttonGiveMaleClothes.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_male_clothes");
		}
		if (buttonGiveFemaleClothes != null)
		{
			buttonGiveFemaleClothes.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_female_clothes");
		}
		if (buttonGiveMissionToken != null)
		{
			buttonGiveMissionToken.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_mission_token");
		}
		if (buttonGiveBaguaJade != null)
		{
			buttonGiveBaguaJade.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_items_button_give_bagua_jade");
		}
	}

	internal void GiveSpiritStones()
	{
		ModMain.Log($"Give {Rewards.SpiritStonesRewardId}");
		if (Game.WorldManager.Value.playerUnit.data.RewardItem(Rewards.SpiritStonesRewardId, null, 0).failedItems.Count != 0)
		{
			ModMain.LogWarning($"Failed to give {Rewards.SpiritStonesRewardId}");
		}
	}

	internal void GiveSectContribution()
	{
		Game.WorldManager.Value.playerUnit.data.RewardItem(Rewards.SectContributionRewardId, null, 0);
	}

	internal void GiveMissionContribution()
	{
		Game.WorldManager.Value.playerUnit.data.RewardItem(Rewards.CityTokenRewardId, null, 0);
	}

	internal void GiveBaguaJade()
	{
		string text = (g.data.world.animaWeapons.Contains(GameAnimaWeapon.PiscesPendant) ? "Bagua Jade" : (g.data.world.animaWeapons.Contains(GameAnimaWeapon.HootinEye) ? "Eye of Providence" : (g.data.world.animaWeapons.Contains(GameAnimaWeapon.DevilDemon) ? "Mythical Gourd" : "NULL")));
		if (text != "Bagua Jade")
		{
			ModMain.LogTip(LocalizationHelper.T("panel_items_error_not_bagua_jade"), "WARNING");
			ModMain.LogTip(string.Format(LocalizationHelper.T("panel_items_error_current_artifact"), text), "WARNING", 4f);
			ModMain.LogTip(LocalizationHelper.T("panel_items_error_ensure_bagua_jade"), "WARNING", 5f);
			return;
		}
		if (Game.DataWorld.Value.data != null)
		{
			Game.DataWorld.Value.data.resurgeCount++;
			ModMain.LogTip(LocalizationHelper.T("panel_items_status_bagua_jade_given"));
		}
		else
		{
			ModMain.LogTip(LocalizationHelper.T("panel_items_error_bagua_jade_failed"), "WARNING");
		}
		if (SceneMapPatch_UIMapMainPlayerInfo._UIMapMainPlayerInfo != null)
		{
			SceneMapPatch_UIMapMainPlayerInfo._UIMapMainPlayerInfo.UpdateUI();
			ModMain.Log("Updating Map Player Info UI...");
			ModMain.Log("Success!");
		}
		else
		{
			ModMain.LogWarning("Updating Map Player Info UI... Failed!");
		}
	}

	internal void GiveBreakthroughMaterials(int grade)
	{
		if (Rewards.GradeRewardIds.TryGetValue(grade + 2, out var value))
		{
			Game.WorldManager.Value.playerUnit.data.RewardItem(value, null, 0);
			ModMain.Log($"Gave rewards({value}) for grade {grade + 2}.");
		}
		else
		{
			ModMain.Log($"No rewards for grade {grade + 2}.");
		}
	}

	internal void GiveSkillMaterials()
	{
		Game.WorldManager.Value.playerUnit.data.RewardItem(Rewards.SkillMaterialsRewardId, null, 0);
	}

	internal void GiveMountMaterials()
	{
		Game.WorldManager.Value.playerUnit.data.RewardItem(Rewards.MountMaterialsRewardId, null, 0);
	}

	internal void GiveEquipment()
	{
		Game.WorldManager.Value.playerUnit.data.RewardItem(Rewards.EquipmentRewardId, null, 0);
	}

	internal void GiveMaleClothes()
	{
		Game.WorldManager.Value.playerUnit.data.RewardItem(Rewards.ClothingMaleRewardId, null, 0);
	}

	internal void GiveFemaleClothes()
	{
		Game.WorldManager.Value.playerUnit.data.RewardItem(Rewards.ClothingFemaleRewardId, null, 0);
	}

	internal void OpenSearchItem()
	{
		UIManager.Panels[PanelType.SearchItem].IsVisible = !UIManager.Panels[PanelType.SearchItem].IsVisible;
	}

	internal void OpenRemoveItem()
	{
		UIManager.Panels[PanelType.RemoveItem].IsVisible = !UIManager.Panels[PanelType.RemoveItem].IsVisible;
	}

	internal void OpenSearchSkill()
	{
		UIManager.Panels[PanelType.SearchSkill].IsVisible = !UIManager.Panels[PanelType.SearchSkill].IsVisible;
	}
}
