using System;
using System.Collections.Generic;
using System.Linq;
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

internal class BuildPlannerPanel : Panel
{
	private const string PanelName = "FatePlanner";

	private TooltipPanel Tooltip = new TooltipPanel("FatePlanerTooltip");

	private System.Collections.Generic.List<RectTransform> tooltips = new System.Collections.Generic.List<RectTransform>();

	private System.Collections.Generic.List<Dropdown> dropdowns = new System.Collections.Generic.List<Dropdown>();

	private System.Collections.Generic.Dictionary<int, int> fateIndexes = new System.Collections.Generic.Dictionary<int, int>();

	private Text titleBarText;

	private System.Collections.Generic.List<Text> gradeLabels = new System.Collections.Generic.List<Text>();

	public BuildPlannerPanel()
		: base(isStartedVisible: false)
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~BuildPlannerPanel()
	{
		UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel("FatePlanner", uiRoot, out contentHolder);
		int? flexibleHeight = 9999;
		UIFactory.SetLayoutElement(uiRoot, null, null, null, flexibleHeight);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.3f, 0.3f);
		component.anchorMax = new Vector2(0.8f, 0.8f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380f);
		GameObject gameObject2 = UIHelper.CreateTitleBar(contentHolder, delegate
		{
			base.IsVisible = false;
		}, GetTitleBarText());
		titleBarText = gameObject2.GetComponentInChildren<Text>();
		gradeLabels.Clear();
		for (int num = 0; num < Game.UpGradeNames.Count; num++)
		{
			string text = Game.UpGradeNames[num];
			GameObject parent = UIFactory.CreateHorizontalGroup(contentHolder, "FatePlanner-grade-group-$" + text, forceExpandWidth: false, forceExpandHeight: true, childControlWidth: true, childControlHeight: true, 5, new Vector4(0f, 10f, 0f, 0f));
			Text text2 = UIFactory.CreateLabel(parent, "FatePlanner-$" + text + "-label", Game.ConfMgr.localText.allText[text].en + ":", TextAnchor.MiddleRight);
			UIFactory.SetLayoutElement(text2.gameObject, 150, flexibleWidth: 0, minHeight: 25);
			gradeLabels.Add(text2);
			int grade = num + 2;
			Dropdown dropdown;
			GameObject gameObject3 = UIFactory.CreateDropdown(parent, "fates-" + text, out dropdown, "", 14, delegate(int idx)
			{
				int fateId = idx - 1;
				if (fateId > -1)
				{
					if (State.GradeFates.Values.FirstOrDefault((ConfFateFeatureItem f) => f.id == Game.ConfMgr.fateFeature._allConfList[fateId].id) == null)
					{
						State.GradeFates[grade] = Game.ConfMgr.fateFeature._allConfList[fateId];
						ModMain.LogTip(LocalizationHelper.T("panel_buildplanner_status_set_planned_fate").Replace("{0}", grade.ToString()).Replace("{1}", fateId.ToString()));
					}
					else
					{
						ModMain.LogTip(LocalizationHelper.T("panel_buildplanner_status_fate_already_used"));
					}
				}
				else
				{
					State.GradeFates.Remove(grade);
					ModMain.LogTip(LocalizationHelper.T("panel_buildplanner_status_removed_planned_fate").Replace("{0}", grade.ToString()));
				}
				State.Save();
			});
			flexibleHeight = 25;
			int? flexibleHeight2 = 0;
			int? flexibleWidth = 999;
			int? preferredWidth = 400;
			UIFactory.SetLayoutElement(gameObject3, null, flexibleHeight, flexibleWidth, flexibleHeight2, preferredWidth);
			Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> list = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
			ButtonRef buttonRef = UIFactory.CreateButton(parent, "FatePlanner-$" + text + "-desc-button", LocalizationHelper.T("panel_buildplanner_button_description"));
			UIFactory.SetLayoutElement(buttonRef.Component.gameObject, minHeight: 25, flexibleHeight: 0, minWidth: 20);
			tooltips.Add(buttonRef.Component.gameObject.GetComponent<RectTransform>());
			list.Add(new Dropdown.OptionData
			{
				text = LocalizationHelper.T("panel_buildplanner_label_na")
			});
			Il2CppSystem.Collections.Generic.List<ConfFateFeatureItem>.Enumerator enumerator = Game.ConfMgr.fateFeature._allConfList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ConfFateFeatureItem current = enumerator.Current;
				if (Game.ConfMgr.localText.allText.ContainsKey("role_feature_postnatal_name" + current.id))
				{
					list.Add(new Dropdown.OptionData
					{
						text = Description.For("role_feature_postnatal_name" + current.id)
					});
					fateIndexes[current.id] = list.Count - 1;
				}
			}
			dropdown.AddOptions(list);
			dropdowns.Add(dropdown);
		}
		Tooltip.Create();
		return (panelRoot: gameObject, draggableArea: gameObject2);
	}

	public override void OnGameWorldUpdate()
	{
		base.OnGameWorldUpdate();
		if (Game.WorldManager.Value == null)
		{
			return;
		}
		if (State.GradeFates.Count > 0)
		{
			ModMain.Log(LocalizationHelper.T("panel_buildplanner_status_loading_state_entries").Replace("{0}", State.GradeFates.Count.ToString()));
		}
		foreach (System.Collections.Generic.KeyValuePair<int, ConfFateFeatureItem> item in State.GradeFates.ToList())
		{
			int key = item.Key;
			if (fateIndexes.TryGetValue(item.Value.id, out var value))
			{
				int num = key - 2;
				if (num >= 0 && num < dropdowns.Count && dropdowns[num] != null)
				{
					dropdowns[num].value = value;
					dropdowns[num].RefreshShownValue();
					ModMain.Log(LocalizationHelper.T("panel_buildplanner_status_loaded_grade_value").Replace("{0}", item.Key.ToString()).Replace("{1}", value.ToString()));
				}
			}
		}
	}

	public override bool Update(bool allowDragging)
	{
		bool result = base.Update(allowDragging);
		if (base.PanelRoot == null || !base.IsVisible)
		{
			return result;
		}
		Vector3 mousePosition = InputManager.MousePosition;
		for (int i = 0; i < tooltips.Count; i++)
		{
			RectTransform rectTransform = tooltips[i];
			if (!(rectTransform == null))
			{
				Vector3 point = rectTransform.InverseTransformPoint(mousePosition);
				int key = i + 2;
				if (rectTransform.rect.Contains(point) && State.GradeFates.TryGetValue(key, out var value) && Game.ConfMgr?.localText?.allText != null && Game.ConfMgr.localText.allText.ContainsKey($"role_feature_postnatal_tips{value.id}"))
				{
					Tooltip.TooltipFor(rectTransform, Description.For($"role_feature_postnatal_tips{value.id}"));
					Tooltip.IsVisible = true;
					return result;
				}
			}
		}
		Tooltip.IsVisible = false;
		return result;
	}

	private string GetTitleBarText()
	{
		string text = LocalizationHelper.T("common_cheatpanel");
		return "<b>" + text + "</b> - " + LocalizationHelper.T("panel_buildplanner_title");
	}

	private void UpdateUITexts()
	{
		if (titleBarText != null)
		{
			titleBarText.text = GetTitleBarText();
		}
		int num = Math.Min(gradeLabels.Count, Game.UpGradeNames?.Count ?? 0);
		for (int i = 0; i < num; i++)
		{
			if (gradeLabels[i] != null && Game.UpGradeNames != null && i < Game.UpGradeNames.Count)
			{
				string key = Game.UpGradeNames[i];
				if (Game.ConfMgr?.localText?.allText != null && Game.ConfMgr.localText.allText.ContainsKey(key))
				{
					gradeLabels[i].text = Game.ConfMgr.localText.allText[key].en + ":";
				}
			}
		}
		for (int j = 0; j < dropdowns.Count; j++)
		{
			if (dropdowns[j] != null && dropdowns[j].options.Count > 0)
			{
				dropdowns[j].options[0].text = LocalizationHelper.T("panel_buildplanner_label_na");
				if (dropdowns[j].value == 0)
				{
					dropdowns[j].RefreshShownValue();
				}
			}
		}
	}
}
