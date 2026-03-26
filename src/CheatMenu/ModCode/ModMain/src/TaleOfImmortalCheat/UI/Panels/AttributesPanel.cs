using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;
using MOD_Mivopx.UI.Panels;
using TaleOfImmortalCheat.Localization;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace TaleOfImmortalCheat.UI.Panels;

public abstract class AttributesPanel : Panel
{
	private static string PanelName = "CheatMenuAttributes";

	private System.Collections.Generic.List<Field> Fields = new System.Collections.Generic.List<Field>();

	private System.Collections.Generic.List<DropdownInput<ConfBattleSkillPrefixValueItem>> skillFields = new System.Collections.Generic.List<DropdownInput<ConfBattleSkillPrefixValueItem>>();

	private System.Collections.Generic.List<DropdownInput<int?>> destinyFields = new System.Collections.Generic.List<DropdownInput<int?>>();

	private System.Collections.Generic.List<DropdownInput<int?>> fatesFields = new System.Collections.Generic.List<DropdownInput<int?>>();

	private Dropdown skillsDropDown;

	private System.Collections.Generic.List<DataUnit.ActionMartialData> allMartialData = new System.Collections.Generic.List<DataUnit.ActionMartialData>();

	private GameObject skillsTab;

	private GameObject prefixesGroup;

	private TooltipPanel tooltip = new TooltipPanel("FatePlanerTooltip");

	private System.Collections.Generic.Dictionary<RectTransform, string> buttonTooltips = new System.Collections.Generic.Dictionary<RectTransform, string>();

	private System.Collections.Generic.List<RectTransform> skillTooltips = new System.Collections.Generic.List<RectTransform>();

	private System.Collections.Generic.List<RectTransform> destiniesTooltips = new System.Collections.Generic.List<RectTransform>();

	private System.Collections.Generic.List<RectTransform> destiniesButtonTooltips = new System.Collections.Generic.List<RectTransform>();

	private System.Collections.Generic.List<RectTransform> fatesTooltips = new System.Collections.Generic.List<RectTransform>();

	private int currentTab;

	private System.Collections.Generic.List<int?> fateOptionsCache = new System.Collections.Generic.List<int?>();

	private Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> fateOptionsDataCache = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();

	private System.Collections.Generic.Dictionary<int, int> fateOptionsPositionCache = new System.Collections.Generic.Dictionary<int, int>();

	private DataUnit.ActionMartialData currentMartialData;

	private static Material transparentMaterial = new Material(Shader.Find("UI/Default"));

	private System.Collections.Generic.List<RectTransform> skillsTabInfoTooltips = new System.Collections.Generic.List<RectTransform>();

	private System.Collections.Generic.List<RectTransform> destiniesTabInfoTooltips = new System.Collections.Generic.List<RectTransform>();

	private System.Collections.Generic.List<RectTransform> fatesTabInfoTooltips = new System.Collections.Generic.List<RectTransform>();

	private Text titleBarText;

	private ButtonRef resetButton;

	private ButtonRef saveButton;

	private ButtonRef saveAllButton;

	private ButtonRef debugSkillButton;

	private ButtonRef refreshCacheButton;

	private ButtonRef runDebugTestButton;

	private ButtonRef addDestinyButton;

	private Tab[] tabsRow1;

	private Tab[] tabsRow2;

	private System.Collections.Generic.List<ButtonRef> tabButtonsRow1 = new System.Collections.Generic.List<ButtonRef>();

	private System.Collections.Generic.List<ButtonRef> tabButtonsRow2 = new System.Collections.Generic.List<ButtonRef>();

	private System.Collections.Generic.Dictionary<string, Text> fieldLabels = new System.Collections.Generic.Dictionary<string, Text>();

	internal abstract AttributesState State { get; }

	internal abstract Action OnUpdate { get; }

	private string Name { get; }

	private bool IsUsingBaseValue { get; }

	public AttributesPanel(string name, bool isUsingBaseValue)
		: base(isStartedVisible: false)
	{
		Name = name;
		IsUsingBaseValue = isUsingBaseValue;
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~AttributesPanel()
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
		foreach (System.Collections.Generic.KeyValuePair<RectTransform, string> buttonTooltip in buttonTooltips)
		{
			if (buttonTooltip.Key != null && !string.IsNullOrEmpty(buttonTooltip.Value))
			{
				Vector3 point = buttonTooltip.Key.InverseTransformPoint(mousePosition);
				if (buttonTooltip.Key.rect.Contains(point))
				{
					string label = (buttonTooltip.Value.StartsWith("tooltip_") ? LocalizationHelper.T(buttonTooltip.Value) : buttonTooltip.Value);
					tooltip.TooltipFor(buttonTooltip.Key, label);
					tooltip.IsVisible = true;
					return result;
				}
			}
		}
		if (currentTab == 5)
		{
			for (int i = 0; i < skillsTabInfoTooltips.Count; i++)
			{
				RectTransform rectTransform = skillsTabInfoTooltips[i];
				Vector3 point2 = rectTransform.InverseTransformPoint(mousePosition);
				if (rectTransform.rect.Contains(point2))
				{
					tooltip.TooltipFor(rectTransform, LocalizationHelper.T("tooltip_attributes_skills_info"));
					tooltip.IsVisible = true;
					return result;
				}
			}
		}
		else if (currentTab == 6)
		{
			for (int j = 0; j < destiniesTabInfoTooltips.Count; j++)
			{
				RectTransform rectTransform2 = destiniesTabInfoTooltips[j];
				Vector3 point3 = rectTransform2.InverseTransformPoint(mousePosition);
				if (rectTransform2.rect.Contains(point3))
				{
					tooltip.TooltipFor(rectTransform2, LocalizationHelper.T("tooltip_attributes_destinies_info"));
					tooltip.IsVisible = true;
					return result;
				}
			}
			for (int k = 0; k < destiniesButtonTooltips.Count; k++)
			{
				RectTransform rectTransform3 = destiniesButtonTooltips[k];
				Vector3 point4 = rectTransform3.InverseTransformPoint(mousePosition);
				if (rectTransform3.rect.Contains(point4))
				{
					tooltip.TooltipFor(rectTransform3, LocalizationHelper.T("tooltip_attributes_destinies_important"));
					tooltip.IsVisible = true;
					return result;
				}
			}
		}
		else if (currentTab == 7)
		{
			for (int l = 0; l < fatesTooltips.Count; l++)
			{
				RectTransform rectTransform4 = fatesTooltips[l];
				Vector3 point5 = rectTransform4.InverseTransformPoint(mousePosition);
				if (!rectTransform4.rect.Contains(point5) || l >= fatesFields.Count)
				{
					continue;
				}
				DropdownInput<int?> dropdownInput = fatesFields[l];
				if (dropdownInput.HasOptions)
				{
					string key = $"role_feature_postnatal_tips{dropdownInput.Value}";
					if (Game.ConfMgr.localText.allText.ContainsKey(key))
					{
						tooltip.TooltipFor(rectTransform4, Description.For(key));
						tooltip.IsVisible = true;
						return result;
					}
				}
			}
			for (int m = 0; m < fatesTabInfoTooltips.Count; m++)
			{
				RectTransform rectTransform5 = fatesTabInfoTooltips[m];
				Vector3 point6 = rectTransform5.InverseTransformPoint(mousePosition);
				if (rectTransform5.rect.Contains(point6))
				{
					tooltip.TooltipFor(rectTransform5, LocalizationHelper.T("tooltip_attributes_fates_info"));
					tooltip.IsVisible = true;
					return result;
				}
			}
		}
		return result;
	}

	public override void OnGameWorldUpdate()
	{
		base.OnGameWorldUpdate();
		if (State != null && State.Unit != null)
		{
			SetInputValues();
		}
		else
		{
			ModMain.LogWarning("Invalid unit state, could not set values.");
		}
	}

	public void SetInputValues()
	{
		if (State.Unit == null)
		{
			return;
		}
		foreach (Field field in Fields)
		{
			field.Reset(State);
		}
		foreach (DropdownInput<ConfBattleSkillPrefixValueItem> skillField in skillFields)
		{
			skillField.Reset(State);
		}
		foreach (DropdownInput<int?> fatesField in fatesFields)
		{
			fatesField.Reset(State);
		}
		foreach (DropdownInput<int?> destinyField in destinyFields)
		{
			destinyField.Reset(State);
		}
		ResetSkillOptions();
	}

	private void OnClickReset()
	{
		if (UIManager.Panels[PanelType.NPCAttributes].IsVisible && !g.ui.GetUI(UIType.NPCInfo))
		{
			ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_reset_failed_npc"), "ERROR", 5f);
		}
		else
		{
			SetInputValues();
		}
	}

	private void OnClickSaveAll()
	{
		if (UIManager.Panels[PanelType.NPCAttributes].IsVisible && !g.ui.GetUI(UIType.NPCInfo))
		{
			ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_save_all_failed_npc"), "ERROR", 5f);
			return;
		}
		foreach (Field field in Fields)
		{
			field.Save(State, null);
		}
		foreach (DropdownInput<ConfBattleSkillPrefixValueItem> skillField in skillFields)
		{
			skillField.Save(State, null);
		}
		foreach (DropdownInput<int?> fatesField in fatesFields)
		{
			fatesField.Save(State, null);
		}
		foreach (DropdownInput<int?> destinyField in destinyFields)
		{
			destinyField.Save(State, null);
		}
		SetInputValues();
		if (OnUpdate != null)
		{
			OnUpdate();
		}
		ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_all_saved"));
	}

	private void OnClickSave()
	{
		if (UIManager.Panels[PanelType.NPCAttributes].IsVisible && !g.ui.GetUI(UIType.NPCInfo))
		{
			ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_save_failed_npc"), "ERROR", 5f);
			return;
		}
		foreach (Field field in Fields)
		{
			field.Save(State, currentTab);
		}
		foreach (DropdownInput<ConfBattleSkillPrefixValueItem> skillField in skillFields)
		{
			skillField.Save(State, currentTab);
		}
		foreach (DropdownInput<int?> fatesField in fatesFields)
		{
			fatesField.Save(State, currentTab);
		}
		foreach (DropdownInput<int?> destinyField in destinyFields)
		{
			destinyField.Save(State, currentTab);
		}
		SetInputValues();
		if (OnUpdate != null)
		{
			OnUpdate();
		}
		ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_saved"));
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel(PanelName + "-" + Name, uiRoot, out contentHolder);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, false, true, true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.3f, 0.3f);
		component.anchorMax = new Vector2(0.8f, 0.8f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 550f);
		GameObject gameObject2 = UIHelper.CreateTitleBar(contentHolder, delegate
		{
			base.IsVisible = false;
		}, GetTitleBarText());
		titleBarText = gameObject2.GetComponentInChildren<Text>();
		tabsRow1 = new Tab[4]
		{
			new Tab(LocalizationHelper.T("panel_attributes_tab_general")),
			new Tab(LocalizationHelper.T("panel_attributes_tab_combat")),
			new Tab(LocalizationHelper.T("panel_attributes_tab_martial_arts")),
			new Tab(LocalizationHelper.T("panel_attributes_tab_spiritual_root"))
		};
		tabsRow2 = new Tab[4]
		{
			new Tab(LocalizationHelper.T("panel_attributes_tab_artisanship")),
			new Tab(LocalizationHelper.T("panel_attributes_tab_skills")),
			new Tab(LocalizationHelper.T("panel_attributes_tab_destinies")),
			new Tab(LocalizationHelper.T("panel_attributes_tab_fates"))
		};
		var (list, list2) = UIHelper.CreateTabPanel(contentHolder, PanelName + "-" + Name + "-tabs", new Tab[2][] { tabsRow1, tabsRow2 }, delegate(int tabIndex)
		{
			currentTab = tabIndex;
		});
		buttonTooltips.Clear();
		destiniesTooltips.Clear();
		destiniesButtonTooltips.Clear();
		fatesTooltips.Clear();
		skillsTabInfoTooltips.Clear();
		destiniesTabInfoTooltips.Clear();
		fatesTabInfoTooltips.Clear();
		BuildButtons(contentHolder);
		BuildGeneralUI(list[0]);
		BuildCombatUI(list[1]);
		BuildMartialArtsUI(list[2]);
		BuildSpiritualRootsUI(list[3]);
		BuildArtisanShipUI(list[4]);
		BuildSkillsUI(list[5]);
		BuildDestiniesUI(list[6]);
		BuildFatesUI(list[7]);
		tabButtonsRow1.Clear();
		tabButtonsRow2.Clear();
		for (int num = 0; num < 4 && num < list2.Count; num++)
		{
			tabButtonsRow1.Add(list2[num]);
		}
		for (int num2 = 4; num2 < 8 && num2 < list2.Count; num2++)
		{
			tabButtonsRow2.Add(list2[num2]);
		}
		tooltip.Create();
		return (panelRoot: gameObject, draggableArea: gameObject2);
	}

	private ButtonRef CreateLocalizedButton(GameObject parent, string name, string textKey, Action onClick, string tooltipKey = null)
	{
		ButtonRef buttonRef = UIFactory.CreateButton(parent, name, LocalizationHelper.T(textKey));
		buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, onClick);
		return buttonRef;
	}

	private void BuildButtons(GameObject root)
	{
		resetButton = CreateLocalizedButton(root, PanelName + "-" + Name + "-reset-button", "panel_attributes_button_reset", OnClickReset);
		GameObject gameObject = resetButton.Component.gameObject;
		int? minHeight = 35;
		int? flexibleHeight = 0;
		UIFactory.SetLayoutElement(gameObject, null, minHeight, null, flexibleHeight);
		RuntimeHelper.SetColorBlock(resetButton.Component, new Color(1f, 44f / 85f, 0.14901961f), new Color(1f, 0.6313726f, 0.34901962f), new Color(1f, 38f / 51f, 28f / 51f), Color.gray);
		saveButton = CreateLocalizedButton(root, PanelName + "-" + Name + "-save-button", "panel_attributes_button_save", OnClickSave);
		GameObject gameObject2 = saveButton.Component.gameObject;
		flexibleHeight = 35;
		minHeight = 0;
		UIFactory.SetLayoutElement(gameObject2, null, flexibleHeight, null, minHeight);
		RuntimeHelper.SetColorBlock(saveButton.Component, new Color(0.1f, 0.3f, 0.1f), new Color(0.2f, 0.5f, 0.2f), new Color(0.1f, 0.2f, 0.1f), new Color(0.2f, 0.2f, 0.2f));
		saveAllButton = CreateLocalizedButton(root, PanelName + "-" + Name + "-save-all-button", "panel_attributes_button_save_all", OnClickSaveAll);
		GameObject gameObject3 = saveAllButton.Component.gameObject;
		minHeight = 35;
		flexibleHeight = 0;
		UIFactory.SetLayoutElement(gameObject3, null, minHeight, null, flexibleHeight);
		RuntimeHelper.SetColorBlock(saveAllButton.Component, new Color(0.1f, 0.3f, 0.1f), new Color(0.2f, 0.5f, 0.2f), new Color(0.1f, 0.2f, 0.1f), new Color(0.2f, 0.2f, 0.2f));
	}

	private void BuildFatesUI(GameObject gameObject)
	{
		fatesTabInfoTooltips.Clear();
		GameObject gameObject2 = UIFactory.CreateHorizontalGroup(gameObject, "FatesTabInfoGroup", forceExpandWidth: false, forceExpandHeight: false, childControlWidth: false, childControlHeight: false);
		HorizontalLayoutGroup component = gameObject2.GetComponent<HorizontalLayoutGroup>();
		if (component != null)
		{
			component.childAlignment = TextAnchor.MiddleCenter;
		}
		ButtonRef buttonRef = UIFactory.CreateButton(gameObject2, "FatesTabInfoButton", "?");
		buttonRef.ButtonText.color = Color.yellow;
		RectTransform component2 = buttonRef.Component.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0.5f, 0.5f);
		component2.anchorMax = new Vector2(0.5f, 0.5f);
		component2.pivot = new Vector2(0.5f, 0.5f);
		component2.anchoredPosition = Vector2.zero;
		component2.sizeDelta = new Vector2(36f, 36f);
		fatesTabInfoTooltips.Add(buttonRef.Component.gameObject.GetComponent<RectTransform>());
		if (Game.UpGradeNames == null || Game.UpGradeNames.Count == 0)
		{
			GameObject gameObject3 = new GameObject("FatesTabPlaceholder");
			gameObject3.transform.SetParent(gameObject.transform, worldPositionStays: false);
			gameObject3.AddComponent<LayoutElement>().minHeight = 60f;
		}
		fateOptionsCache.Add(null);
		fateOptionsDataCache.Add(new Dropdown.OptionData
		{
			text = "N/A"
		});
		Il2CppSystem.Collections.Generic.List<ConfFateFeatureItem>.Enumerator enumerator = Game.ConfMgr.fateFeature._allConfList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ConfFateFeatureItem current = enumerator.Current;
			if (Game.ConfMgr.localText.allText.ContainsKey($"role_feature_postnatal_name{current.id}"))
			{
				fateOptionsCache.Add(current.id);
				fateOptionsDataCache.Add(new Dropdown.OptionData
				{
					text = ((!string.IsNullOrEmpty(Game.ConfMgr.localText.allText[$"role_feature_postnatal_name{current.id}"].en)) ? Game.ConfMgr.localText.allText[$"role_feature_postnatal_name{current.id}"].en : Game.ConfMgr.localText.allText[$"role_feature_postnatal_name{current.id}"].ch)
				});
				fateOptionsPositionCache[current.id] = fateOptionsDataCache.Count - 1;
			}
		}
		for (int i = 0; i < Game.UpGradeNames.Count; i++)
		{
			int gradeIndex = i + 2;
			GameObject parent = UIFactory.CreateHorizontalGroup(gameObject, FieldName(PanelName, $"Fate-${gradeIndex}-HGroup"), forceExpandWidth: false, forceExpandHeight: true, childControlWidth: true, childControlHeight: true, 5, new Vector4(0f, 10f, 0f, 0f));
			Dropdown dropdown;
			GameObject gameObject4 = UIFactory.CreateDropdown(parent, FieldName(Name, $"Fate-{gradeIndex}-Dropdown"), out dropdown, "", 14, delegate
			{
			});
			int? minHeight = 25;
			int? flexibleHeight = 0;
			int? flexibleWidth = 999;
			int? preferredWidth = 400;
			UIFactory.SetLayoutElement(gameObject4, null, minHeight, flexibleWidth, flexibleHeight, preferredWidth);
			ButtonRef buttonRef2 = UIFactory.CreateButton(parent, FieldName(PanelName, $"Fate-{gradeIndex}-DescButton"), "?");
			UIFactory.SetLayoutElement(buttonRef2.Component.gameObject, minHeight: 25, flexibleHeight: 0, minWidth: 20);
			fatesTooltips.Add(buttonRef2.Component.gameObject.GetComponent<RectTransform>());
			DropdownInput<int?> item = new DropdownInput<int?>(dropdown, delegate(AttributesState state)
			{
				if (state.Unit.data.dynUnitData.GetGrade() < gradeIndex)
				{
					return (int?)null;
				}
				if (!state.GradeLog.ContainsKey(gradeIndex))
				{
					ModMain.Log($"No grade log found for grade {gradeIndex}");
					return 0;
				}
				if (!fateOptionsPositionCache.ContainsKey(state.GradeLog[gradeIndex].luck))
				{
					ModMain.Log($"Luck {fateOptionsPositionCache} not found in cache");
					return 0;
				}
				return fateOptionsPositionCache[state.GradeLog[gradeIndex].luck];
			}, delegate(AttributesState state, int? luckId)
			{
				if (luckId.HasValue && (!state.GradeLog.ContainsKey(gradeIndex) || state.GradeLog[gradeIndex].luck != luckId.Value))
				{
					if (state.ChangeLuck != null)
					{
						state.ChangeLuck(state.Unit.data.unitData.unitID, gradeIndex, luckId.Value);
					}
					if (state.GradeLog.ContainsKey(gradeIndex))
					{
						int luck = state.GradeLog[gradeIndex].luck;
						state.Unit.DestroyLuck(state.Unit.GetLuck(luck));
						state.Unit.CreateLuck(new DataUnit.LuckData
						{
							createTime = 0,
							duration = -1,
							id = luckId.Value,
							objData = new DataObjectData()
						});
						ModMain.Log($"Changed fate[{luck}]({gradeIndex}) -> {luckId}");
						state.GradeLog[gradeIndex].luck = luckId.Value;
					}
					else
					{
						ModMain.Log($"New fate[{gradeIndex}] -> {luckId}");
						state.Unit.CreateLuck(new DataUnit.LuckData
						{
							createTime = 0,
							duration = -1,
							id = luckId.Value,
							objData = new DataObjectData()
						});
						state.GradeLog[gradeIndex] = new DataWorld.World.PlayerLogData.GradeData
						{
							luck = luckId.Value,
							quality = 1
						};
					}
				}
			}, fateOptionsDataCache, fateOptionsCache, 7);
			fatesFields.Add(item);
		}
	}

	private void BuildDestiniesUI(GameObject gameObject)
	{
		destiniesTabInfoTooltips.Clear();
		GameObject gameObject2 = UIFactory.CreateHorizontalGroup(gameObject, "DestiniesTabInfoGroup", forceExpandWidth: false, forceExpandHeight: false, childControlWidth: false, childControlHeight: false);
		HorizontalLayoutGroup component = gameObject2.GetComponent<HorizontalLayoutGroup>();
		if (component != null)
		{
			component.childAlignment = TextAnchor.MiddleCenter;
		}
		ButtonRef buttonRef = UIFactory.CreateButton(gameObject2, "DestiniesTabInfoButton", "?");
		buttonRef.ButtonText.color = Color.yellow;
		RectTransform component2 = buttonRef.Component.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0.5f, 0.5f);
		component2.anchorMax = new Vector2(0.5f, 0.5f);
		component2.pivot = new Vector2(0.5f, 0.5f);
		component2.anchoredPosition = Vector2.zero;
		component2.sizeDelta = new Vector2(36f, 36f);
		destiniesTabInfoTooltips.Add(buttonRef.Component.gameObject.GetComponent<RectTransform>());
		InitialDestinyField(gameObject);
		CreateAddDestinyButton(gameObject);
	}

	private void InitialDestinyField(GameObject gameObject)
	{
		for (int i = 0; i < 3; i++)
		{
			GameObject parent = UIFactory.CreateHorizontalGroup(gameObject, FieldName(PanelName, $"Born-Luck-{i}-HGroup"), forceExpandWidth: false, forceExpandHeight: true, childControlWidth: true, childControlHeight: true, 5, new Vector4(0f, 10f, 0f, 0f));
			Dropdown dropdown;
			GameObject gameObject2 = UIFactory.CreateDropdown(parent, FieldName(Name, $"Born-Luck-{i}-Dropdown"), out dropdown, "", 14, delegate
			{
			});
			int? minHeight = 25;
			int? flexibleWidth = 999;
			int? flexibleHeight = 0;
			int? preferredWidth = 400;
			UIFactory.SetLayoutElement(gameObject2, null, minHeight, flexibleWidth, flexibleHeight, preferredWidth);
			ButtonRef buttonRef = UIFactory.CreateButton(parent, FieldName(PanelName, $"Born-Luck-{i}-DescButton"), "?");
			UIFactory.SetLayoutElement(buttonRef.Component.gameObject, minHeight: 25, flexibleHeight: 0, minWidth: 20);
			destiniesTooltips.Add(buttonRef.Component.gameObject.GetComponent<RectTransform>());
			int luckIndex = i;
			destinyFields.Add(new DropdownInput<int?>(dropdown, delegate(AttributesState state)
			{
				DataUnit.LuckData[] bornLuckArray = GetBornLuckArray(state);
				if (bornLuckArray == null)
				{
					ModMain.LogWarning("Destiny UI - No bornLuck array found.");
					return (int?)null;
				}
				if (!IsIndexValid(luckIndex, bornLuckArray))
				{
					ModMain.LogWarning($"Destiny UI - Invalid index {luckIndex} for bornLuck array.");
					return 0;
				}
				int id = bornLuckArray[luckIndex].id;
				if (!MOD_Mivopx.Cache.DestiniesOptionsPositionCache.ContainsKey(id))
				{
					ModMain.LogWarning($"Destiny UI - Missing destiniesOptionsPositionCache for bornLuck[{luckIndex}] -> {id}.");
					return 0;
				}
				return MOD_Mivopx.Cache.DestiniesOptionsPositionCache[id];
			}, delegate(AttributesState state, int? luckId)
			{
				if (!luckId.HasValue || luckId == -1)
				{
					ModMain.Log("Destiny UI - Luck ID not specified (N/A).");
				}
				else
				{
					DataUnit.LuckData[] bornLuckArray = GetBornLuckArray(state);
					if (bornLuckArray == null || !IsIndexValid(luckIndex, bornLuckArray))
					{
						ModMain.LogWarning($"Cannot update luck. Index {luckIndex} is invalid.");
					}
					else
					{
						int id = bornLuckArray[luckIndex].id;
						if (luckId != id)
						{
							WorldUnitLuckBase luck = state.Unit.GetLuck(id);
							if (luck != null)
							{
								state.Unit.DestroyLuck(luck);
								ModMain.Log($"Destiny UI - Removed old luck[{luckIndex}]({id}).");
							}
							state.Unit.CreateLuck(new DataUnit.LuckData
							{
								createTime = 0,
								duration = -1,
								id = luckId.Value,
								objData = new DataObjectData()
							});
							bornLuckArray[luckIndex].id = luckId.Value;
							if (state.ChangeLuck != null)
							{
								state.ChangeLuck(state.Unit.data.unitData.unitID, luckIndex, luckId.Value);
							}
						}
					}
				}
			}, MOD_Mivopx.Cache.DestiniesOptionsDataCache, MOD_Mivopx.Cache.DestiniesOptionsCache, 6));
		}
	}

	private DataUnit.LuckData[] GetBornLuckArray(AttributesState state)
	{
		if (state == null)
		{
			ModMain.LogWarning("State is null. Cannot retrieve bornLuck array.");
			return null;
		}
		if (state.Unit == null)
		{
			ModMain.LogWarning("Unit is null in state. Cannot retrieve bornLuck array.");
			return null;
		}
		if (state.Unit.data?.unitData?.propertyData == null)
		{
			ModMain.LogWarning("PropertyData is null in unit data. Cannot retrieve bornLuck array.");
			return null;
		}
		return state.Unit.data.unitData.propertyData.bornLuck;
	}

	private void CreateAddDestinyButton(GameObject gameObject)
	{
		addDestinyButton = CreateLocalizedButton(gameObject, "Add-Born-Luck-Button", "panel_attributes_button_add_destiny", delegate
		{
			if (UIManager.Panels[PanelType.PlayerAttributes].IsVisible && g.ui.GetUI(UIType.PlayerInfo) != null)
			{
				ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_close_player_ui_warning"), "WARNING", 5f);
				ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_prevent_save_corruption"), "WARNING", 6f);
			}
			else if (UIManager.Panels[PanelType.NPCAttributes].IsVisible && g.ui.GetUI(UIType.NPCInfo) == null)
			{
				ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_cannot_add_destiny_npc_closed"), "ERROR", 5f);
			}
			else
			{
				AttributesState state = State;
				if (state == null)
				{
					ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_cannot_add_destiny_instance_null"), "WARNING", 5f);
				}
				else
				{
					ModMain.Log("instanceState initialized.");
					if (destiniesTooltips.Count >= 9)
					{
						ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_destiny_limit_exceeded"), "WARNING", 1f);
					}
					AddNewBornLuck(gameObject, state);
				}
			}
		});
		addDestinyButton.ButtonText.color = Color.cyan;
		GameObject gameObject2 = addDestinyButton.Component.gameObject;
		UIFactory.SetLayoutElement(gameObject2, null, 35, null, 0);
		destiniesButtonTooltips.Add(addDestinyButton.Component.gameObject.GetComponent<RectTransform>());
		int? num = 30;
		int? flexibleHeight = 0;
		int? minWidth = 30;
		int? minHeight = num;
		int? preferredWidth = 30;
		UIFactory.SetLayoutElement(gameObject2, minWidth, minHeight, null, flexibleHeight, preferredWidth);
	}

	private void UpdateAttPanel()
	{
		PanelType[] array = new PanelType[2]
		{
			PanelType.PlayerAttributes,
			PanelType.NPCAttributes
		};
		foreach (PanelType key in array)
		{
			Panel panel = UIManager.Panels[key];
			if (panel.IsVisible)
			{
				panel.IsVisible = false;
				panel.IsVisible = true;
				(panel as AttributesPanel)?.SetInputValues();
				break;
			}
		}
	}

	private bool IsIndexValid(int index, DataUnit.LuckData[] array)
	{
		return index < array.Length;
	}

	private void AddDefaultLuck(System.Collections.Generic.List<DataUnit.LuckData> luckList, int id)
	{
		luckList.Add(new DataUnit.LuckData
		{
			createTime = 0,
			duration = -1,
			id = id,
			objData = new DataObjectData()
		});
	}

	private void AddNewBornLuck(GameObject gameObject, AttributesState state)
	{
		DataUnit.LuckData[] bornLuckArray = GetBornLuckArray(state);
		if (bornLuckArray == null)
		{
			ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_destiny_array_null"), "WARNING", 3f);
			return;
		}
		System.Collections.Generic.List<DataUnit.LuckData> list = bornLuckArray.ToList();
		int count = destiniesTooltips.Count;
		if (IsIndexValid(count, bornLuckArray))
		{
			ModMain.Log($"An existing destiny was found at index {count}. Using the existing destiny.");
			int num = bornLuckArray.Count();
			if (num <= 3)
			{
				return;
			}
			ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_existing_destinies_detected"));
			for (int i = 0; i < num; i++)
			{
				if (i >= count || i >= destinyFields.Count)
				{
					CreateNewDestinyFieldUI(gameObject, i, list, state);
				}
			}
			UpdateAttPanel();
			ModMain.Log($"Successfully added/updated Destiny with index {count}.");
			ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_destiny_added_successfully"), null, 3f);
			ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_save_changes_reopen_ui"), null, 3f);
		}
		else if (count == bornLuckArray.Count())
		{
			ModMain.LogTip($"No existing destiny was found. Adding a new destiny for index: {count}", null, 3f);
			AddDefaultLuck(list, 1304);
			state.Unit.data.unitData.propertyData.bornLuck = list.ToArray();
			CreateNewDestinyFieldUI(gameObject, count, list, state);
			UpdateAttPanel();
			OnClickSave();
			ModMain.Log($"Successfully added/updated Destiny with index {count}.");
			ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_destiny_added_successfully"), null, 3f);
			ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_save_changes_reopen_ui"), null, 3f);
		}
	}

	private void CreateNewDestinyFieldUI(GameObject gameObject, int luckIndex, System.Collections.Generic.List<DataUnit.LuckData> luckList, AttributesState state)
	{
		GameObject parent = UIFactory.CreateHorizontalGroup(gameObject, FieldName(PanelName, $"Born-Luck-{luckIndex}-HGroup"), forceExpandWidth: false, forceExpandHeight: true, childControlWidth: true, childControlHeight: true, 5, new Vector4(0f, 10f, 0f, 0f));
		UIFactory.SetLayoutElement(UIFactory.CreateDropdown(parent, FieldName(Name, $"Born-Luck-{luckIndex}-Dropdown"), out var dropdown, "", 14, delegate
		{
		}), null, 25, 999, 0, 400);
		ButtonRef buttonRef = UIFactory.CreateButton(parent, FieldName(PanelName, $"Born-Luck-{luckIndex}-DescButton"), "?");
		UIFactory.SetLayoutElement(buttonRef.Component.gameObject, minHeight: 25, flexibleHeight: 0, minWidth: 20);
		ButtonRef buttonRef2 = UIFactory.CreateButton(parent, FieldName(PanelName, $"Born-Luck-{luckIndex}-RemoveButton"), "X");
		UIFactory.SetLayoutElement(buttonRef2.Component.gameObject, minHeight: 25, flexibleHeight: 0, minWidth: 20);
		buttonRef2.OnClick = (Action)Delegate.Combine(buttonRef2.OnClick, (Action)delegate
		{
			if (luckIndex == destinyFields.Count - 1)
			{
				if (luckIndex < luckList.Count && luckIndex < destiniesTooltips.Count && luckIndex < destinyFields.Count)
				{
					luckList.RemoveAt(luckIndex);
					if (luckIndex < state.Unit.data.unitData.propertyData.bornLuck.Count())
					{
						int id = state.Unit.data.unitData.propertyData.bornLuck[luckIndex].id;
						WorldUnitLuckBase luck = state.Unit.GetLuck(id);
						state.Unit.DestroyLuck(luck);
					}
					state.Unit.data.unitData.propertyData.bornLuck = luckList.ToArray();
					RectTransform rectTransform = destiniesTooltips[luckIndex];
					if (rectTransform != null)
					{
						UnityEngine.Object.DestroyImmediate(rectTransform.gameObject);
					}
					destiniesTooltips.RemoveAt(luckIndex);
					destinyFields.RemoveAt(luckIndex);
					UnityEngine.Object.DestroyImmediate(parent);
					ModMain.Log($"Removed destiny, tooltip, and dropdown at index {luckIndex}.");
					ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_destiny_removed_refresh_ui"), null, 3f);
					OnClickSave();
				}
				else
				{
					ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_cannot_remove_destiny_out_of_range"), "WARNING", 3f);
				}
			}
			else
			{
				ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_cannot_remove_destiny_not_last"), "WARNING", 3f);
			}
		});
		destiniesTooltips.Add(buttonRef.Component.gameObject.GetComponent<RectTransform>());
		destinyFields.Add(new DropdownInput<int?>(dropdown, delegate(AttributesState statez)
		{
			DataUnit.LuckData[] bornLuckArray = GetBornLuckArray(statez);
			if (bornLuckArray == null)
			{
				ModMain.LogWarning("Destiny UI - No bornLuck array found.");
				return (int?)null;
			}
			if (IsIndexValid(luckIndex, bornLuckArray))
			{
				int id = bornLuckArray[luckIndex].id;
				if (!MOD_Mivopx.Cache.DestiniesOptionsPositionCache.ContainsKey(id))
				{
					ModMain.LogWarning($"Destiny UI - Missing cache for bornLuck[{luckIndex}] with ID {id}.");
					return 0;
				}
				return MOD_Mivopx.Cache.DestiniesOptionsPositionCache[id];
			}
			return 0;
		}, delegate(AttributesState statez, int? luckId)
		{
			if (!luckId.HasValue || luckId == -1)
			{
				ModMain.Log("Destiny UI - Luck ID not specified.");
			}
			else
			{
				DataUnit.LuckData[] bornLuckArray = GetBornLuckArray(statez);
				if (bornLuckArray == null || !IsIndexValid(luckIndex, bornLuckArray))
				{
					ModMain.LogWarning($"Cannot update luck. Index {luckIndex} is invalid.");
				}
				else
				{
					int id = bornLuckArray[luckIndex].id;
					if (luckId != id)
					{
						WorldUnitLuckBase luck = state.Unit.GetLuck(id);
						if (luck != null)
						{
							state.Unit.DestroyLuck(luck);
							ModMain.Log($"Removed old luck [{id}].");
						}
						state.Unit.CreateLuck(new DataUnit.LuckData
						{
							createTime = 0,
							duration = -1,
							id = luckId.Value,
							objData = new DataObjectData()
						});
						bornLuckArray[luckIndex].id = luckId.Value;
						ModMain.Log($"Updated bornLuck[{luckIndex}] with new ID {luckId.Value}.");
						if (statez.ChangeLuck != null)
						{
							statez.ChangeLuck(statez.Unit.data.unitData.unitID, luckIndex, luckId.Value);
						}
					}
				}
			}
		}, MOD_Mivopx.Cache.DestiniesOptionsDataCache, MOD_Mivopx.Cache.DestiniesOptionsCache, 6));
		ApplyTransparencyToChildren(parent);
	}

	private void BuildSkillsUI(GameObject gameObject)
	{
		skillsTabInfoTooltips.Clear();
		GameObject gameObject2 = UIFactory.CreateHorizontalGroup(gameObject, "SkillsTabInfoGroup", forceExpandWidth: false, forceExpandHeight: false, childControlWidth: false, childControlHeight: false);
		HorizontalLayoutGroup component = gameObject2.GetComponent<HorizontalLayoutGroup>();
		if (component != null)
		{
			component.childAlignment = TextAnchor.MiddleCenter;
		}
		ButtonRef buttonRef = UIFactory.CreateButton(gameObject2, "SkillsTabInfoButton", "?");
		buttonRef.ButtonText.color = Color.yellow;
		RectTransform component2 = buttonRef.Component.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0.5f, 0.5f);
		component2.anchorMax = new Vector2(0.5f, 0.5f);
		component2.pivot = new Vector2(0.5f, 0.5f);
		component2.anchoredPosition = Vector2.zero;
		component2.sizeDelta = new Vector2(36f, 36f);
		skillsTabInfoTooltips.Add(buttonRef.Component.gameObject.GetComponent<RectTransform>());
		skillsTab = gameObject;
		Image component3 = skillsTab.GetComponent<Image>();
		if (component3 != null)
		{
			component3.color = new Color(0f, 0f, 0f, 0f);
		}
		GameObject gameObject3 = UIFactory.CreateDropdown(gameObject, FieldName(Name, "Skills"), out skillsDropDown, "", 14, delegate(int idx)
		{
			if (allMartialData.Count <= idx)
			{
				ModMain.LogError("Invalid skill index.");
			}
			else
			{
				currentMartialData = allMartialData[idx];
				ModMain.Log($"skill index = {idx}");
				LoadSkillPrefixes(allMartialData[idx]);
			}
		});
		int? minHeight = 25;
		int? flexibleHeight = 0;
		int? flexibleWidth = 999;
		int? preferredWidth = 400;
		UIFactory.SetLayoutElement(gameObject3, null, minHeight, flexibleWidth, flexibleHeight, preferredWidth);
		GameObject parent = UIFactory.CreateHorizontalGroup(gameObject, "SkillButtons", forceExpandWidth: false, forceExpandHeight: false, childControlWidth: true, childControlHeight: true, 5);
		debugSkillButton = CreateLocalizedButton(parent, "Debug-Skill-Button", "panel_attributes_button_debug_skill", delegate
		{
			if (UIManager.Panels[PanelType.NPCAttributes].IsVisible && !g.ui.GetUI(UIType.NPCInfo))
			{
				ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_cannot_do_npc"), "ERROR", 5f);
			}
			else if (currentMartialData != null)
			{
				DebugCurrentSkillPrefixes(currentMartialData);
			}
			else
			{
				ModMain.LogWarning("No skill selected for debugging");
			}
		});
		debugSkillButton.ButtonText.color = Color.yellow;
		GameObject gameObject4 = debugSkillButton.Component.gameObject;
		int? minHeight2 = 30;
		int? flexibleHeight2 = 0;
		int? flexibleWidth2 = 1;
		UIFactory.SetLayoutElement(gameObject4, null, minHeight2, flexibleWidth2, flexibleHeight2);
		refreshCacheButton = CreateLocalizedButton(parent, "Refresh-Cache-Button", "panel_attributes_button_refresh_cache", delegate
		{
			if (UIManager.Panels[PanelType.NPCAttributes].IsVisible && !g.ui.GetUI(UIType.NPCInfo))
			{
				ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_cannot_do_npc"), "ERROR", 5f);
			}
			else
			{
				ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_forcing_cache_rebuild"));
				MOD_Mivopx.Cache.Destroy();
				MOD_Mivopx.Cache.Build();
				if (currentMartialData != null)
				{
					LoadSkillPrefixes(currentMartialData);
					ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_cache_refreshed_skill_reloaded"));
				}
				else
				{
					ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_cache_refreshed"));
				}
			}
		});
		refreshCacheButton.ButtonText.color = Color.cyan;
		GameObject gameObject5 = refreshCacheButton.Component.gameObject;
		flexibleWidth2 = 30;
		flexibleHeight2 = 0;
		minHeight2 = 1;
		UIFactory.SetLayoutElement(gameObject5, null, flexibleWidth2, minHeight2, flexibleHeight2);
		runDebugTestButton = CreateLocalizedButton(parent, "Test-Prefixes-Button", "panel_attributes_button_run_debug_test", delegate
		{
			if (UIManager.Panels[PanelType.NPCAttributes].IsVisible && !g.ui.GetUI(UIType.NPCInfo))
			{
				ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_cannot_do_npc"), "ERROR", 5f);
			}
			else
			{
				SkillPrefixDebugTest.TestSkillPrefixCaching();
				if (currentMartialData != null)
				{
					DataProps.MartialData martialData = currentMartialData.data.To<DataProps.MartialData>();
					SkillPrefixDebugTest.TestSpecificSkill((int)martialData.martialType, martialData.baseID);
				}
				ModMain.LogTip(LocalizationHelper.T("panel_attributes_status_debug_test_completed"));
			}
		});
		runDebugTestButton.ButtonText.color = Color.magenta;
		GameObject gameObject6 = runDebugTestButton.Component.gameObject;
		minHeight2 = 30;
		flexibleHeight2 = 0;
		flexibleWidth2 = 1;
		UIFactory.SetLayoutElement(gameObject6, null, minHeight2, flexibleWidth2, flexibleHeight2);
	}

	private void ResetSkillOptions()
	{
		if (skillsDropDown == null)
		{
			return;
		}
		allMartialData.Clear();
		UnityEngine.Object.Destroy(prefixesGroup);
		skillsDropDown.ClearOptions();
		Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> list = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
		int num = 0;
		int value = 0;
		Il2CppSystem.Collections.Generic.Dictionary<string, DataUnit.ActionMartialData>.ValueCollection.Enumerator enumerator = State.Unit.data.unitData.allActionMartial.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			DataUnit.ActionMartialData current = enumerator.Current;
			try
			{
				list.Add(new Dropdown.OptionData
				{
					text = current.data.propsInfoBase.name
				});
				allMartialData.Add(current);
			}
			catch (Exception ex)
			{
				ModMain.LogError("Failed to load skill: " + ex.Message);
				continue;
			}
			if (currentMartialData != null)
			{
				DataProps.MartialData martialData = currentMartialData.data.To<DataProps.MartialData>();
				DataProps.MartialData martialData2 = current.data.To<DataProps.MartialData>();
				Il2CppSystem.Collections.Generic.List<DataProps.MartialData.Prefix> prefixs = martialData.martialInfo.GetPrefixs();
				Il2CppSystem.Collections.Generic.List<DataProps.MartialData.Prefix> prefixs2 = martialData2.martialInfo.GetPrefixs();
				if (martialData.baseID == martialData2.baseID && martialData2.martialType == martialData.martialType && prefixs.Count == prefixs2.Count)
				{
					bool flag = true;
					for (int i = 0; i < prefixs.Count; i++)
					{
						if (prefixs[i].prefixValueItem.number != prefixs2[i].prefixValueItem.number)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						ModMain.Log($"found skill index = {num}");
						value = num;
					}
				}
			}
			num++;
		}
		skillsDropDown.AddOptions(list);
		skillsDropDown.value = value;
		if (currentMartialData == null)
		{
			LoadSkillPrefixes(allMartialData[0]);
		}
		else
		{
			LoadSkillPrefixes(currentMartialData);
		}
	}

	private void LoadSkillPrefixes(DataUnit.ActionMartialData actionMartialData)
	{
		UnityEngine.Object.Destroy(prefixesGroup);
		skillTooltips.Clear();
		skillFields.Clear();
		prefixesGroup = UIFactory.CreateVerticalGroup(skillsTab, PanelName + "-search-group", forceWidth: true, forceHeight: false, childControlWidth: true, childControlHeight: true, 5, new Vector4(0f, 10f, 0f, 0f));
		if (prefixesGroup != null)
		{
			Image component = prefixesGroup.GetComponent<Image>();
			if (component != null)
			{
				component.color = new Color(0f, 0f, 0f, 0f);
			}
		}
		DataProps.MartialData martialData = actionMartialData.data.To<DataProps.MartialData>();
		Il2CppSystem.Collections.Generic.List<DataProps.MartialData.Prefix>.Enumerator enumerator = martialData.martialInfo.GetPrefixs().GetEnumerator();
		while (enumerator.MoveNext())
		{
			DataProps.MartialData.Prefix current = enumerator.Current;
			DataProps.MartialData.Prefix prefixToUpdate = current;
			string k = $"{(int)martialData.martialType}_{martialData.baseID}";
			if (!MOD_Mivopx.Cache.CachedPrefixes.ContainsKey(k))
			{
				ModMain.LogWarning("No skill prefixes found for key " + k + ", attempting to rebuild cache for this skill");
				MOD_Mivopx.Cache.RebuildCacheForSpecificSkill((int)martialData.martialType, martialData.baseID);
			}
			if (!MOD_Mivopx.Cache.CachedPrefixes.ContainsKey(k))
			{
				ModMain.LogWarning("Still no skill prefixes found for key " + k + " after rebuild attempt");
				continue;
			}
			GameObject parent = UIFactory.CreateHorizontalGroup(prefixesGroup, FieldName(PanelName, $"Skills-Prefix-{prefixToUpdate.index}-HGroup"), forceExpandWidth: true, forceExpandHeight: false, childControlWidth: true, childControlHeight: false, 5, new Vector4(0f, 10f, 0f, 0f));
			Dropdown dropdown;
			GameObject gameObject = UIFactory.CreateDropdown(parent, FieldName(Name, $"Skills-Prefix-{prefixToUpdate.index}"), out dropdown, "", 14, delegate
			{
			});
			int? minHeight = 25;
			int? flexibleHeight = 0;
			int? flexibleWidth = 999;
			int? preferredWidth = 400;
			UIFactory.SetLayoutElement(gameObject, null, minHeight, flexibleWidth, flexibleHeight, preferredWidth);
			ButtonRef buttonRef = UIFactory.CreateButton(parent, FieldName(PanelName, $"BornSkills-Prefix-{prefixToUpdate.index}"), "?");
			UIFactory.SetLayoutElement(buttonRef.Component.gameObject, minHeight: 25, flexibleHeight: 0, minWidth: 20);
			skillTooltips.Add(buttonRef.Component.gameObject.GetComponent<RectTransform>());
			DropdownInput<ConfBattleSkillPrefixValueItem> dropdownInput = new DropdownInput<ConfBattleSkillPrefixValueItem>(dropdown, delegate
			{
				if (!MOD_Mivopx.Cache.CachedPrefixes.ContainsKey(k))
				{
					ModMain.LogWarning("No prefixes cached for '" + k + "'");
					return (int?)null;
				}
				int number = prefixToUpdate.prefixValueItem.number;
				if (!MOD_Mivopx.Cache.CachedPrefixesPositions[k].ContainsKey(number))
				{
					ModMain.Log(string.Format("Prefix number {0} not found in cache for key {1}. Available numbers: {2}", number, k, string.Join(", ", MOD_Mivopx.Cache.CachedPrefixesPositions[k].Keys)));
					return 0;
				}
				return MOD_Mivopx.Cache.CachedPrefixesPositions[k][number];
			}, delegate(AttributesState state, ConfBattleSkillPrefixValueItem setPrefix)
			{
				if (prefixToUpdate.prefixValueItem.number != setPrefix.number)
				{
					ModMain.Log($"Updated martial art with index={prefixToUpdate.index};number={setPrefix.number};desc={Description.For(setPrefix.desc)}");
					actionMartialData.data.To<DataProps.MartialData>().SetPrefixID(prefixToUpdate.index, setPrefix.number);
				}
			}, MOD_Mivopx.Cache.PrefixesOptionsDataCache[k], MOD_Mivopx.Cache.CachedPrefixes[k], 5);
			dropdownInput.Reset(State);
			skillFields.Add(dropdownInput);
		}
		ApplyTransparencyToChildren(prefixesGroup);
	}

	private void ApplyTransparencyToChildren(GameObject parent)
	{
		Image[] array = parent.GetComponentsInChildren<Image>(includeInactive: true);
		if (transparentMaterial == null)
		{
			transparentMaterial = new Material(Shader.Find("UI/Default"));
		}
		if (array != null && transparentMaterial != null)
		{
			Image[] array2 = array;
			foreach (Image image in array2)
			{
				if (image != null && !image.GetComponent<Button>() && !image.GetComponent<Toggle>() && !image.GetComponent<Scrollbar>() && image.gameObject.name != "Checkmark" && !image.gameObject.name.Contains("Tooltip"))
				{
					image.material = transparentMaterial;
					image.color = new Color(0f, 0f, 0f, 0.3f);
				}
			}
		}
		else
		{
			ModMain.LogWarning("Failed to apply transparency to children at AttributesPanel[AddNewBornLuck].");
		}
	}

	private void RebuildPrefixCacheForSkill(int martialType, int baseID)
	{
		string text = $"{martialType}_{baseID}";
		Il2CppSystem.Collections.Generic.List<ConfBattleSkillPrefixValueItem> allConfList = Game.ConfMgr.battleSkillPrefixValue._allConfList;
		if (allConfList == null || allConfList.Count == 0)
		{
			ModMain.LogWarning("No battle skill prefix data available for rebuild");
			return;
		}
		System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> list = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>();
		System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> list2 = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>();
		System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> list3 = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>();
		ModMain.Log($"Rebuilding cache for skill type {martialType}, base ID {baseID}");
		ModMain.Log($"Total prefix items in config: {allConfList.Count}");
		Il2CppSystem.Collections.Generic.List<ConfBattleSkillPrefixValueItem>.Enumerator enumerator = allConfList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ConfBattleSkillPrefixValueItem current = enumerator.Current;
			if (current.skillType != martialType)
			{
				continue;
			}
			list3.Add(current);
			if (!Game.ConfMgr.localText.allText.ContainsKey(current.desc))
			{
				ModMain.Log($"Missing translation for prefix {current.number}: {current.desc}");
				continue;
			}
			bool flag = false;
			foreach (int item2 in current.skillID)
			{
				if (item2 == baseID)
				{
					flag = true;
					ModMain.Log($"Found specific prefix for skill: {current.number} - {Description.For(current.desc)}");
					break;
				}
				if (item2 == 0)
				{
					list2.Add(current);
					ModMain.Log($"Found universal prefix: {current.number} - {Description.For(current.desc)}");
					break;
				}
			}
			if (flag)
			{
				list.Add(current);
			}
		}
		list.AddRange(list2);
		ModMain.Log($"Found {list3.Count} items matching skill type {martialType}");
		ModMain.Log($"Found {list.Count} valid prefix items for key {text}");
		ModMain.Log($"Found {list2.Count} universal prefix items");
		if (list.Count == 0)
		{
			ModMain.LogWarning($"No valid prefix items found for skill type {martialType}, base ID {baseID}");
			System.Collections.Generic.HashSet<int> hashSet = new System.Collections.Generic.HashSet<int>();
			foreach (ConfBattleSkillPrefixValueItem item3 in list3)
			{
				foreach (int item4 in item3.skillID)
				{
					hashSet.Add(item4);
				}
			}
			ModMain.Log(string.Format("Available skill IDs for type {0}: {1}", martialType, string.Join(", ", hashSet)));
			return;
		}
		MOD_Mivopx.Cache.CachedPrefixes[text] = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>();
		MOD_Mivopx.Cache.CachedPrefixesPositions[text] = new System.Collections.Generic.Dictionary<int, int>();
		MOD_Mivopx.Cache.PrefixesOptionsDataCache[text] = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
		foreach (ConfBattleSkillPrefixValueItem item5 in list)
		{
			string text2 = Description.For(item5.desc);
			Dropdown.OptionData item = new Dropdown.OptionData
			{
				text = text2
			};
			MOD_Mivopx.Cache.CachedPrefixes[text].Add(item5);
			MOD_Mivopx.Cache.CachedPrefixesPositions[text][item5.number] = MOD_Mivopx.Cache.CachedPrefixes[text].Count - 1;
			MOD_Mivopx.Cache.PrefixesOptionsDataCache[text].Add(item);
			ModMain.Log($"Added to cache: {item5.number} - {text2}");
		}
		ModMain.Log($"Successfully rebuilt prefix cache for key {text} with {list.Count} items");
	}

	private void DebugCurrentSkillPrefixes(DataUnit.ActionMartialData actionMartialData)
	{
		DataProps.MartialData martialData = actionMartialData.data.To<DataProps.MartialData>();
		ModMain.Log("=== DEBUG: Current Skill Info ===");
		ModMain.Log("Skill Name: " + martialData.data.propsInfoBase.name);
		ModMain.Log($"Martial Type: {martialData.martialType} ({(int)martialData.martialType})");
		ModMain.Log($"Base ID: {martialData.baseID}");
		ModMain.Log($"Cache Key: {(int)martialData.martialType}_{martialData.baseID}");
		Il2CppSystem.Collections.Generic.List<DataProps.MartialData.Prefix> prefixs = martialData.martialInfo.GetPrefixs();
		ModMain.Log($"Current Prefixes Count: {prefixs.Count}");
		for (int i = 0; i < prefixs.Count; i++)
		{
			DataProps.MartialData.Prefix prefix = prefixs[i];
			ModMain.Log($"Prefix {i}: Index={prefix.index}, Number={prefix.prefixValueItem.number}, Desc={prefix.prefixValueItem.desc}");
			if (Game.ConfMgr.localText.allText.ContainsKey(prefix.prefixValueItem.desc))
			{
				ModMain.Log("  Translation: " + Description.For(prefix.prefixValueItem.desc));
			}
			else
			{
				ModMain.Log("  No translation found for: " + prefix.prefixValueItem.desc);
			}
		}
		ModMain.Log("=== END DEBUG ===");
	}

	private void BuildGeneralUI(GameObject tab)
	{
        static System.Collections.Generic.List<(int Id, string Label)> BuildTraitOptions(int start, int end)
		{
			System.Collections.Generic.List<(int Id, string Label)> list = [];
			for (int i = start; i <= end; i++)
			{
				string key = "role_character_name" + i;
				string label = key;
				if (Game.ConfMgr.localText.allText.ContainsKey(key)) {
					// TODO: handle localization? LanguageType
					label = Game.ConfMgr.localText.allText[key].en;
				}
				list.Add((i, label));
			}
			return list;
		}

		var name = "InternalTrait";
		var groupGo = UIFactory.CreateVerticalGroup(tab, $"{name}-row", false, false, true, true, spacing: 5, padding: new Vector4(5f, 10f, 10f, 10f));
		var labelText = UIFactory.CreateLabel(groupGo, $"{name}-row-label", LocalizationHelper.T("panel_attributes_field_internal_trait"), TextAnchor.MiddleLeft);
		UIFactory.SetLayoutElement(labelText.gameObject);
		fieldLabels[FieldName(Name, name)] = labelText.GetComponent<Text>();

		var dropdownGo = AddDynDropdown(groupGo, name, (AttributesState data) => data.Unit.data.dynUnitData.inTrait, 0, BuildTraitOptions(1, 7));
		UIFactory.SetLayoutElement(dropdownGo);

		name = "ExternalTrait";
		groupGo = UIFactory.CreateVerticalGroup(tab, $"{name}-row", false, false, true, true, spacing: 5, padding: new Vector4(0f, 10f, 10f, 10f));
		labelText = UIFactory.CreateLabel(groupGo, $"{name}-row-label", LocalizationHelper.T("panel_attributes_field_external_trait"), TextAnchor.MiddleLeft);
		UIFactory.SetLayoutElement(labelText.gameObject);
		fieldLabels[FieldName(Name, name)] = labelText.GetComponent<Text>();

		dropdownGo = AddDynDropdown(groupGo, $"{name}1", (AttributesState data) => data.Unit.data.dynUnitData.outTrait1, 0, BuildTraitOptions(8, 19));
		UIFactory.SetLayoutElement(dropdownGo);
		dropdownGo = AddDynDropdown(groupGo, $"{name}2", (AttributesState data) => data.Unit.data.dynUnitData.outTrait2, 0, BuildTraitOptions(8, 19));
		UIFactory.SetLayoutElement(dropdownGo);

		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_age"), "", FieldName(Name, "Age"), (AttributesState data) => data.Unit.data.dynUnitData.age, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_lifespan"), "", FieldName(Name, "Lifespan"), (AttributesState data) => data.Unit.data.dynUnitData.life, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_charisma"), "", FieldName(Name, "Charisma"), (AttributesState data) => data.Unit.data.dynUnitData.beauty, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_exp"), "", FieldName(Name, "Exp"), (AttributesState data) => data.Unit.data.dynUnitData.exp, 0);
		AddTextInput(tab, LocalizationHelper.T("panel_attributes_field_intim_to_player"), "", FieldName(Name, "Intim"), (AttributesState state) => state.Unit.data.unitData.relationData.intimToPlayerUnit.ToString(), delegate(AttributesState state, string text)
		{
			state.Unit.data.unitData.relationData.intimToPlayerUnit = float.Parse(text);
		}, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_reputation"), "", FieldName(Name, "Reputation"), (AttributesState data) => data.Unit.data.dynUnitData.reputation, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_insight"), "", FieldName(Name, "Insight"), (AttributesState data) => data.Unit.data.dynUnitData.talent, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_health"), "", FieldName(Name, "Health"), (AttributesState data) => data.Unit.data.dynUnitData.health, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_max_health"), "", FieldName(Name, "MaxHealth"), (AttributesState data) => data.Unit.data.dynUnitData.healthMax, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_vitality"), "", FieldName(Name, "Vitality"), (AttributesState data) => data.Unit.data.dynUnitData.hp, 0, isClamped: false);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_max_vitality"), "", FieldName(Name, "MaxVitality"), (AttributesState data) => data.Unit.data.dynUnitData.hpMax, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_energy"), "", FieldName(Name, "Energy"), (AttributesState data) => data.Unit.data.dynUnitData.mp, 0, isClamped: false);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_max_energy"), "", FieldName(Name, "MaxEnergy"), (AttributesState data) => data.Unit.data.dynUnitData.mpMax, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_focus"), "", FieldName(Name, "Focus"), (AttributesState data) => data.Unit.data.dynUnitData.sp, 0, isClamped: false);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_max_focus"), "", FieldName(Name, "MaxFocus"), (AttributesState data) => data.Unit.data.dynUnitData.spMax, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_luck"), "", FieldName(Name, "Luck"), (AttributesState data) => data.Unit.data.dynUnitData.luck, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_stamina"), "", FieldName(Name, "Stamina"), (AttributesState data) => data.Unit.data.dynUnitData.energy, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_max_stamina"), "", FieldName(Name, "MaxStamina"), (AttributesState data) => data.Unit.data.dynUnitData.energyMax, 0);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_skill_points"), "", FieldName(Name, "SkillPoints"), (AttributesState data) => data.Unit.data.dynUnitData.abilityPoint, 0);
	}

	private void BuildCombatUI(GameObject tab)
	{
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_atk"), "", FieldName(Name, "Atk"), (AttributesState data) => data.Unit.data.dynUnitData.attack, 1);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_def"), "", FieldName(Name, "Def"), (AttributesState data) => data.Unit.data.dynUnitData.defense, 1);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_travel_speed"), "", FieldName(Name, "TravelSpeed"), (AttributesState data) => data.Unit.data.dynUnitData.footSpeed, 1);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_crit"), "", FieldName(Name, "Crit"), (AttributesState data) => data.Unit.data.dynUnitData.crit, 1);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_crid_dmg"), "", FieldName(Name, "CridDmg"), (AttributesState data) => data.Unit.data.dynUnitData.critValue, 1);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_cris_res"), "", FieldName(Name, "CrisRes"), (AttributesState data) => data.Unit.data.dynUnitData.guard, 1);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_crid_dr"), "", FieldName(Name, "CridDr"), (AttributesState data) => data.Unit.data.dynUnitData.guardValue, 1);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_agi"), "", FieldName(Name, "Agi"), (AttributesState data) => data.Unit.data.dynUnitData.moveSpeed, 1);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_spiritual_res"), "", FieldName(Name, "SpiritualRes"), (AttributesState data) => data.Unit.data.dynUnitData.magicFree, 1);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_martial_res"), "", FieldName(Name, "MartialRes"), (AttributesState data) => data.Unit.data.dynUnitData.phycicalFree, 1);
	}

	private void BuildMartialArtsUI(GameObject tab)
	{
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_sword"), "", FieldName(Name, "Sword"), (AttributesState data) => data.Unit.data.dynUnitData.basisSword, 2);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_spear"), "", FieldName(Name, "Spear"), (AttributesState data) => data.Unit.data.dynUnitData.basisSpear, 2);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_blade"), "", FieldName(Name, "Blade"), (AttributesState data) => data.Unit.data.dynUnitData.basisBlade, 2);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_fist"), "", FieldName(Name, "Fist"), (AttributesState data) => data.Unit.data.dynUnitData.basisFist, 2);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_palm"), "", FieldName(Name, "Palm"), (AttributesState data) => data.Unit.data.dynUnitData.basisPalm, 2);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_finger"), "", FieldName(Name, "Finger"), (AttributesState data) => data.Unit.data.dynUnitData.basisFinger, 2);
	}

	private void BuildSpiritualRootsUI(GameObject tab)
	{
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_fire"), "", FieldName(Name, "Fire"), (AttributesState data) => data.Unit.data.dynUnitData.basisFire, 3);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_water"), "", FieldName(Name, "Water"), (AttributesState data) => data.Unit.data.dynUnitData.basisFroze, 3);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_lightning"), "", FieldName(Name, "Lightning"), (AttributesState data) => data.Unit.data.dynUnitData.basisThunder, 3);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_wind"), "", FieldName(Name, "Wind"), (AttributesState data) => data.Unit.data.dynUnitData.basisWind, 3);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_earth"), "", FieldName(Name, "Earth"), (AttributesState data) => data.Unit.data.dynUnitData.basisEarth, 3);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_wood"), "", FieldName(Name, "Wood"), (AttributesState data) => data.Unit.data.dynUnitData.basisWood, 3);
	}

	private void BuildArtisanShipUI(GameObject tab)
	{
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_alchemy"), "", FieldName(Name, "Alchemy"), (AttributesState data) => data.Unit.data.dynUnitData.refineElixir, 4);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_forge"), "", FieldName(Name, "Forge"), (AttributesState data) => data.Unit.data.dynUnitData.refineWeapon, 4);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_fengshui"), "", FieldName(Name, "Fengshui"), (AttributesState data) => data.Unit.data.dynUnitData.geomancy, 4);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_talisman"), "", FieldName(Name, "Talisman"), (AttributesState data) => data.Unit.data.dynUnitData.symbol, 4);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_herbology"), "", FieldName(Name, "Herbology"), (AttributesState data) => data.Unit.data.dynUnitData.herbal, 4);
		AddDynInput(tab, LocalizationHelper.T("panel_attributes_field_mining"), "", FieldName(Name, "Mining"), (AttributesState data) => data.Unit.data.dynUnitData.mine, 4);
	}

	private void AddDynInput(GameObject tab, string name, string placeholder, string fieldName, Func<AttributesState, DynInt> accessor, int tabIndex, bool isClamped = true)
	{
		(GameObject, InputFieldRef) tuple = UIHelper.BuildInputField(tab, name, placeholder, fieldName);
		GameObject item = tuple.Item1;
		InputFieldRef item2 = tuple.Item2;
		Text text = item.transform.Find(fieldName + "-row-label")?.GetComponent<Text>();
		if (text != null)
		{
			fieldLabels[fieldName] = text;
		}
		Fields.Add(new DynInputField(item2, accessor, IsUsingBaseValue, tabIndex, isClamped));
	}

	private GameObject AddDynDropdown(GameObject tab, string name, Func<AttributesState, DynInt> accessor, int tabIndex, System.Collections.Generic.List<(int Id, string Label)> options, bool isClamped = true)
	{
		// Build dropdown UI
		(GameObject, Dropdown) tuple = UIHelper.BuildDropdown(tab, name, _ => { });
		Dropdown dropdown = tuple.Item2;

		// Populate with provided options
		foreach (var (_, Label) in options)
		{
			dropdown.options.Add(new Dropdown.OptionData(Label));
		}

		// Add field wrapper
		Fields.Add(new DynDropdownField(dropdown, accessor, IsUsingBaseValue, tabIndex, options.First().Id, isClamped));
		return tuple.Item1;
	}

	private void AddTextInput(GameObject tab, string name, string placeholder, string fieldName, Func<AttributesState, string> accessor, Action<AttributesState, string> save, int tabIndex)
	{
		(GameObject, InputFieldRef) tuple = UIHelper.BuildInputField(tab, name, placeholder, fieldName);
		GameObject item = tuple.Item1;
		InputFieldRef item2 = tuple.Item2;
		Text text = item.transform.Find(fieldName + "-row-label")?.GetComponent<Text>();
		if (text != null)
		{
			fieldLabels[fieldName] = text;
		}
		Fields.Add(new TextInputField(item2, accessor, save, tabIndex));
	}

	private string GetTitleBarText()
	{
		return "<b>" + LocalizationHelper.T("common_cheatpanel") + "</b> - " + Name;
	}

	private void UpdateUITexts()
	{
		if (titleBarText != null)
		{
			titleBarText.text = GetTitleBarText();
		}
		if (resetButton != null)
		{
			resetButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_attributes_button_reset");
		}
		if (saveButton != null)
		{
			saveButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_attributes_button_save");
		}
		if (saveAllButton != null)
		{
			saveAllButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_attributes_button_save_all");
		}
		if (debugSkillButton != null)
		{
			debugSkillButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_attributes_button_debug_skill");
		}
		if (refreshCacheButton != null)
		{
			refreshCacheButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_attributes_button_refresh_cache");
		}
		if (runDebugTestButton != null)
		{
			runDebugTestButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_attributes_button_run_debug_test");
		}
		if (addDestinyButton != null)
		{
			addDestinyButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_attributes_button_add_destiny");
		}
		if (tabButtonsRow1.Count >= 4)
		{
			tabButtonsRow1[0].ButtonText.text = LocalizationHelper.T("panel_attributes_tab_general");
			tabButtonsRow1[1].ButtonText.text = LocalizationHelper.T("panel_attributes_tab_combat");
			tabButtonsRow1[2].ButtonText.text = LocalizationHelper.T("panel_attributes_tab_martial_arts");
			tabButtonsRow1[3].ButtonText.text = LocalizationHelper.T("panel_attributes_tab_spiritual_root");
		}
		if (tabButtonsRow2.Count >= 4)
		{
			tabButtonsRow2[0].ButtonText.text = LocalizationHelper.T("panel_attributes_tab_artisanship");
			tabButtonsRow2[1].ButtonText.text = LocalizationHelper.T("panel_attributes_tab_skills");
			tabButtonsRow2[2].ButtonText.text = LocalizationHelper.T("panel_attributes_tab_destinies");
			tabButtonsRow2[3].ButtonText.text = LocalizationHelper.T("panel_attributes_tab_fates");
		}
		UpdateFieldLabels();
	}

	private void UpdateFieldLabels()
	{
		foreach (System.Collections.Generic.KeyValuePair<string, string> item in new System.Collections.Generic.Dictionary<string, string>
		{
			{
				FieldName(Name, "InternalTrait"),
				"panel_attributes_field_internal_trait"
			},
			{
				FieldName(Name, "ExternalTrait"),
				"panel_attributes_field_external_trait"
			},
			{
				FieldName(Name, "Age"),
				"panel_attributes_field_age"
			},
			{
				FieldName(Name, "Lifespan"),
				"panel_attributes_field_lifespan"
			},
			{
				FieldName(Name, "Charisma"),
				"panel_attributes_field_charisma"
			},
			{
				FieldName(Name, "Exp"),
				"panel_attributes_field_exp"
			},
			{
				FieldName(Name, "Intim"),
				"panel_attributes_field_intim_to_player"
			},
			{
				FieldName(Name, "Reputation"),
				"panel_attributes_field_reputation"
			},
			{
				FieldName(Name, "Insight"),
				"panel_attributes_field_insight"
			},
			{
				FieldName(Name, "Health"),
				"panel_attributes_field_health"
			},
			{
				FieldName(Name, "MaxHealth"),
				"panel_attributes_field_max_health"
			},
			{
				FieldName(Name, "Vitality"),
				"panel_attributes_field_vitality"
			},
			{
				FieldName(Name, "MaxVitality"),
				"panel_attributes_field_max_vitality"
			},
			{
				FieldName(Name, "Energy"),
				"panel_attributes_field_energy"
			},
			{
				FieldName(Name, "MaxEnergy"),
				"panel_attributes_field_max_energy"
			},
			{
				FieldName(Name, "Focus"),
				"panel_attributes_field_focus"
			},
			{
				FieldName(Name, "MaxFocus"),
				"panel_attributes_field_max_focus"
			},
			{
				FieldName(Name, "Luck"),
				"panel_attributes_field_luck"
			},
			{
				FieldName(Name, "Stamina"),
				"panel_attributes_field_stamina"
			},
			{
				FieldName(Name, "MaxStamina"),
				"panel_attributes_field_max_stamina"
			},
			{
				FieldName(Name, "SkillPoints"),
				"panel_attributes_field_skill_points"
			},
			{
				FieldName(Name, "Atk"),
				"panel_attributes_field_atk"
			},
			{
				FieldName(Name, "Def"),
				"panel_attributes_field_def"
			},
			{
				FieldName(Name, "TravelSpeed"),
				"panel_attributes_field_travel_speed"
			},
			{
				FieldName(Name, "Crit"),
				"panel_attributes_field_crit"
			},
			{
				FieldName(Name, "CridDmg"),
				"panel_attributes_field_crid_dmg"
			},
			{
				FieldName(Name, "CrisRes"),
				"panel_attributes_field_cris_res"
			},
			{
				FieldName(Name, "CridDr"),
				"panel_attributes_field_crid_dr"
			},
			{
				FieldName(Name, "Agi"),
				"panel_attributes_field_agi"
			},
			{
				FieldName(Name, "SpiritualRes"),
				"panel_attributes_field_spiritual_res"
			},
			{
				FieldName(Name, "MartialRes"),
				"panel_attributes_field_martial_res"
			},
			{
				FieldName(Name, "Sword"),
				"panel_attributes_field_sword"
			},
			{
				FieldName(Name, "Spear"),
				"panel_attributes_field_spear"
			},
			{
				FieldName(Name, "Blade"),
				"panel_attributes_field_blade"
			},
			{
				FieldName(Name, "Fist"),
				"panel_attributes_field_fist"
			},
			{
				FieldName(Name, "Palm"),
				"panel_attributes_field_palm"
			},
			{
				FieldName(Name, "Finger"),
				"panel_attributes_field_finger"
			},
			{
				FieldName(Name, "Fire"),
				"panel_attributes_field_fire"
			},
			{
				FieldName(Name, "Water"),
				"panel_attributes_field_water"
			},
			{
				FieldName(Name, "Lightning"),
				"panel_attributes_field_lightning"
			},
			{
				FieldName(Name, "Wind"),
				"panel_attributes_field_wind"
			},
			{
				FieldName(Name, "Earth"),
				"panel_attributes_field_earth"
			},
			{
				FieldName(Name, "Wood"),
				"panel_attributes_field_wood"
			},
			{
				FieldName(Name, "Alchemy"),
				"panel_attributes_field_alchemy"
			},
			{
				FieldName(Name, "Forge"),
				"panel_attributes_field_forge"
			},
			{
				FieldName(Name, "Fengshui"),
				"panel_attributes_field_fengshui"
			},
			{
				FieldName(Name, "Talisman"),
				"panel_attributes_field_talisman"
			},
			{
				FieldName(Name, "Herbology"),
				"panel_attributes_field_herbology"
			},
			{
				FieldName(Name, "Mining"),
				"panel_attributes_field_mining"
			}
		})
		{
			if (fieldLabels.TryGetValue(item.Key, out var value) && value != null)
			{
				value.text = LocalizationHelper.T(item.Value) + ":";
			}
		}
	}

	private static string FieldName(string name, string field)
	{
		return PanelName + "-" + name + "-" + field;
	}
}
