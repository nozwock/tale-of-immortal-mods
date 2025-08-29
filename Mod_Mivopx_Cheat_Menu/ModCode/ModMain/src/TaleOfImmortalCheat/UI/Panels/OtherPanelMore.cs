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

public class OtherPanelMore : Panel
{
	public const string PanelName = "OtherMore";

	private TooltipPanel tooltip = new TooltipPanel("ButtonTooltip");

	private Dictionary<RectTransform, string> buttonTooltips = new Dictionary<RectTransform, string>();

	private Text titleBarText;

	private ButtonRef buttonTrainMartial;

	private ButtonRef buttonRepairEye;

	private ButtonRef buttonRepairGourd;

	private ButtonRef buttonIncreaseView;

	private ButtonRef buttonFullView;

	private ButtonRef buttonResetView;

	private ButtonRef buttonOpenCheatMod;

	private ButtonRef buttonCredits;

	public OtherPanelMore()
		: base(isStartedVisible: false)
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~OtherPanelMore()
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

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		Debug.Log("Creating More Other panel");
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel("Other 2", uiRoot, out contentHolder);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, false, true, true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0.5f);
		component.anchorMax = new Vector2(0.5f, 0.5f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 375f);
		GameObject gameObject2 = UIHelper.CreateTitleBar(contentHolder, delegate
		{
			base.IsVisible = false;
		}, GetTitleBarText());
		titleBarText = gameObject2.GetComponentInChildren<Text>();
		buttonTooltips.Clear();
		int num = 342;
		buttonTrainMartial = CreateLocalizedButton(contentHolder, "Other-train-skills", "panel_othermore_button_train_martial", delegate
		{
			try
			{
				Dictionary<string, DataUnit.ActionMartialData>.KeyCollection.Enumerator enumerator = Game.WorldManager.Value.playerUnit.data.unitData.allActionMartial.Keys.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					Game.WorldManager.Value.playerUnit.data.unitData.AddMartialExp(current, 999999);
					ModMain.Log("Training current martial...");
					ModMain.Log("Name: " + Game.WorldManager.Value.playerUnit.data.unitData.GetActionMartial(current).data.propsInfoBase._name);
					ModMain.Log($"Exp: {Game.WorldManager.Value.playerUnit.data.unitData.GetActionMartial(current).exp}");
					ModMain.Log("Trained Exp: 999999");
					ModMain.Log("----- ----- ----- ----- -----");
				}
				ModMain.LogTip(LocalizationHelper.T("status_othermore_training_martial"));
			}
			catch (Exception ex)
			{
				ModMain.LogTip(LocalizationHelper.T("status_othermore_training_martial_error") + ex.Message + ":" + ex.StackTrace, "WARNING");
			}
		}, "tooltip_othermore_train_martial");
		buttonRepairEye = CreateLocalizedButton(contentHolder, "Other2-repair-eye", "panel_othermore_button_repair_eye", RepairEyeProvidence, "tooltip_othermore_repair_eye");
		buttonRepairGourd = CreateLocalizedButton(contentHolder, "Other2-repair-gourd", "panel_othermore_button_repair_gourd", RepairHealMythicalGourd, "tooltip_othermore_repair_gourd");
		buttonIncreaseView = CreateLocalizedButton(contentHolder, "Other2-increase-view", "panel_othermore_button_increase_view", IncreaseView, "tooltip_othermore_increase_view");
		buttonFullView = CreateLocalizedButton(contentHolder, "Other2-full-view", "panel_othermore_button_full_view", FullView, "tooltip_othermore_full_view");
		buttonResetView = CreateLocalizedButton(contentHolder, "Other2-reset-view", "panel_othermore_button_reset_view", ResetView, "tooltip_othermore_reset_view");
		buttonOpenCheatMod = CreateLocalizedButton(contentHolder, "Other2-open-cheat-mod", "panel_othermore_button_open_cheat_mod", OpenCheatMod, "tooltip_othermore_open_cheat_mod");
		buttonCredits = CreateLocalizedButton(contentHolder, "Other2-credits", "panel_othermore_button_credits", Credits, "tooltip_othermore_credits");
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num);
		tooltip.Create();
		return (panelRoot: gameObject, draggableArea: gameObject2);
	}

	internal void RepairEyeProvidence()
	{
		string text = (g.data.world.animaWeapons.Contains(GameAnimaWeapon.PiscesPendant) ? "Bagua Jade" : (g.data.world.animaWeapons.Contains(GameAnimaWeapon.HootinEye) ? "Eye of Providence" : (g.data.world.animaWeapons.Contains(GameAnimaWeapon.DevilDemon) ? "Mythical Gourd" : "NULL")));
		if (text != "Eye of Providence")
		{
			ModMain.LogTip(LocalizationHelper.T("status_othermore_not_using_eye"), "WARNING", 3f);
			ModMain.LogTip(LocalizationHelper.T("status_othermore_current_artifact") + " [" + text + "]", "WARNING", 4f);
			ModMain.LogTip(LocalizationHelper.T("status_othermore_ensure_eye_selected"), "WARNING", 5f);
		}
		else if (Game.DataWorld.Value.data.godEyeData != null)
		{
			DataWorld.World.GodEyeData godEyeData = Game.DataWorld.Value.data.godEyeData;
			if (godEyeData.isDamage)
			{
				ModMain.LogTip(LocalizationHelper.T("status_othermore_repairing") + " " + text + "...", null, 3f);
				ModMain.Log($"IsCanUse: {godEyeData.IsCanUse()}");
				ModMain.Log($"IsCanUpGrade: {godEyeData.IsCanUpGrade()}");
				ModMain.Log($"IsRepair: {godEyeData.IsRepair()}");
				ModMain.Log($"isDamage: {godEyeData.isDamage}");
				godEyeData.isDamage = false;
				ModMain.Log($"Modified - isDamage: {godEyeData.isDamage}");
				if (ExploitPatch_UIGodEye._instance != null)
				{
					ExploitPatch_UIGodEye._instance.UpdateUI();
					ModMain.LogTip(LocalizationHelper.T("status_othermore_updating_eye_ui"), null, 3f);
				}
				ModMain.LogTip(LocalizationHelper.T("status_othermore_repaired"), null, 3f);
			}
			else
			{
				ModMain.LogTip(LocalizationHelper.T("status_othermore_eye_perfect_condition"), "WARNING", 5f);
			}
		}
		else
		{
			ModMain.LogTip(LocalizationHelper.T("other_msgs_god_eye_data_null"), "WARNING");
		}
	}

	internal void RepairHealMythicalGourd()
	{
		string text = (g.data.world.animaWeapons.Contains(GameAnimaWeapon.PiscesPendant) ? "Bagua Jade" : (g.data.world.animaWeapons.Contains(GameAnimaWeapon.HootinEye) ? "Eye of Providence" : (g.data.world.animaWeapons.Contains(GameAnimaWeapon.DevilDemon) ? "Mythical Gourd" : "NULL")));
		if (text != "Mythical Gourd")
		{
			ModMain.LogTip(LocalizationHelper.T("status_othermore_not_using_gourd"), "WARNING", 3f);
			ModMain.LogTip(LocalizationHelper.T("status_othermore_current_artifact") + " [" + text + "]", "WARNING", 4f);
			ModMain.LogTip(LocalizationHelper.T("status_othermore_ensure_gourd_selected"), "WARNING", 5f);
			return;
		}
		if (ExploitPatch_DevilDemonMgr._instance != null)
		{
			DevilDemonMgr instance = ExploitPatch_DevilDemonMgr._instance;
			if (instance.devilDemonData.isDamage)
			{
				ModMain.LogTip(LocalizationHelper.T("status_othermore_repairing") + " " + text + "...", null, 3f);
				ModMain.Log($"Initial - brokenCount: {instance.devilDemonData.brokenCount}");
				if (instance.devilDemonData.brokenCount >= 1)
				{
					instance.devilDemonData.brokenCount = 0;
				}
				ModMain.Log($"Updated - brokenCount: {instance.devilDemonData.brokenCount}");
				ModMain.Log($"Initial - isDamage: {instance.devilDemonData.isDamage}");
				instance.devilDemonData.isDamage = false;
				ModMain.Log($"Updated - isDamage: {instance.devilDemonData.isDamage}");
				ModMain.Log($"Initial - repairMonth: {instance.devilDemonData.repairMonth}");
				if (instance.devilDemonData.repairMonth >= 1)
				{
					instance.devilDemonData.repairMonth = 0;
				}
				ModMain.Log($"Updated - repairMonth: {instance.devilDemonData.repairMonth}");
				ModMain.LogTip(LocalizationHelper.T("status_othermore_repaired"), null, 3f);
			}
			else
			{
				ModMain.LogTip(LocalizationHelper.T("status_othermore_gourd_perfect_condition"), "WARNING", 5f);
			}
		}
		else
		{
			ModMain.LogWarning("DevilDemonMgr returned NULL!");
		}
		if (ExploitPatch_PotMonMgr._instance != null)
		{
			PotMonMgr instance2 = ExploitPatch_PotMonMgr._instance;
			ModMain.LogTip(LocalizationHelper.T("status_othermore_reviving_pets"), null, 4f);
			List<PotMonMgr.PotMonData>.Enumerator enumerator = instance2.potMonDatas.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PotMonMgr.PotMonData current = enumerator.Current;
				if (current != null && current.healthState != PotMonHealthState.Normal)
				{
					current.SetHealthState(PotMonHealthState.Normal);
					ModMain.LogTip(current.fixName + " " + LocalizationHelper.T("status_othermore_pet_healed"), null, 4f);
				}
			}
		}
		else
		{
			ModMain.LogWarning("PotMonMgr returned NULL!");
		}
	}

	internal static void IncreaseView()
	{
		DynInt playerView = Game.WorldManager.Value.playerUnit.data.dynUnitData.playerView;
		if (playerView != null)
		{
			ModMain.LogTip(string.Format("{0}{1}", LocalizationHelper.T("status_othermore_initial_view"), playerView.baseValue), null, 0.5f);
			playerView.baseValue++;
			ModMain.LogTip(string.Format("{0}{1}", LocalizationHelper.T("status_othermore_updated_view"), playerView.baseValue), null, 0.5f);
		}
		else
		{
			ModMain.LogTip(LocalizationHelper.T("status_othermore_playerview_null"), "WARNING");
		}
	}

	internal void FullView()
	{
		DynInt playerView = Game.WorldManager.Value.playerUnit.data.dynUnitData.playerView;
		if (playerView != null)
		{
			ModMain.LogTip(string.Format("{0}{1}", LocalizationHelper.T("status_othermore_initial_view"), playerView.baseValue), null, 0.5f);
			playerView.baseValue = 500;
			ModMain.LogTip(string.Format("{0}{1}", LocalizationHelper.T("status_othermore_updated_view"), playerView.baseValue), null, 0.5f);
		}
		else
		{
			ModMain.LogTip(LocalizationHelper.T("status_othermore_playerview_null"), "WARNING");
		}
	}

	internal static void ResetView()
	{
		DynInt playerView = Game.WorldManager.Value.playerUnit.data.dynUnitData.playerView;
		if (playerView != null)
		{
			ModMain.LogTip(string.Format("{0}{1}", LocalizationHelper.T("status_othermore_initial_view"), playerView.baseValue), null, 0.5f);
			playerView.baseValue = 3;
			ModMain.LogTip(string.Format("{0}{1}", LocalizationHelper.T("status_othermore_updated_view"), playerView.baseValue), null, 0.5f);
		}
		else
		{
			ModMain.LogTip(LocalizationHelper.T("status_othermore_playerview_null"), "WARNING");
		}
	}

	internal static void OpenCheatMod()
	{
		if (DramaTool.OpenDrama(927081281, new DramaData
		{
			unitLeft = Game.WorldManager.Value.playerUnit,
			unitRight = null
		}))
		{
			ModMain.LogTip(LocalizationHelper.T("status_othermore_cheat_mod_triggered"), null, 3f);
		}
		else
		{
			ModMain.LogTip(LocalizationHelper.T("status_othermore_cheat_mod_failed"), "ERROR", 5f);
		}
	}

	internal static void Credits()
	{
		g.ui.OpenUI<UITextInfoLong>(UIType.TextInfoLong).InitData(LocalizationHelper.T("credits_title"), string.Format(LocalizationHelper.T("credits_content"), "1.1.8 Pub"), LocalizationHelper.T("credits_thanks"));
	}

	private string GetTitleBarText()
	{
		string text = LocalizationHelper.T("common_cheatpanel");
		return "<b>" + text + "</b> - " + LocalizationHelper.T("panel_othermore_title");
	}

	private void UpdateUITexts()
	{
		if (titleBarText != null)
		{
			titleBarText.text = GetTitleBarText();
		}
		if (buttonTrainMartial != null)
		{
			buttonTrainMartial.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_othermore_button_train_martial");
		}
		if (buttonRepairEye != null)
		{
			buttonRepairEye.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_othermore_button_repair_eye");
		}
		if (buttonRepairGourd != null)
		{
			buttonRepairGourd.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_othermore_button_repair_gourd");
		}
		if (buttonIncreaseView != null)
		{
			buttonIncreaseView.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_othermore_button_increase_view");
		}
		if (buttonFullView != null)
		{
			buttonFullView.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_othermore_button_full_view");
		}
		if (buttonResetView != null)
		{
			buttonResetView.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_othermore_button_reset_view");
		}
		if (buttonOpenCheatMod != null)
		{
			buttonOpenCheatMod.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_othermore_button_open_cheat_mod");
		}
		if (buttonCredits != null)
		{
			buttonCredits.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_othermore_button_credits");
		}
	}
}
