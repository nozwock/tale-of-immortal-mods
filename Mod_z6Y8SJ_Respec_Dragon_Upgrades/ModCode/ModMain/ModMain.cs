using MelonLoader;
using EGameTypeData;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace MOD_z6Y8SJ;

public class ModMain
{
	internal static readonly string modNamespace = typeof(ModMain).Namespace!;

	Il2CppSystem.Action<ETypeData> callOpenUIEnd;

	public void Init()
	{
		callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
		g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
	}

	public void Destroy()
	{
		g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);
	}

	void OnOpenUIEnd(ETypeData e)
	{
		var edata = e.Cast<OpenUIEnd>();
		if (edata.uiType.uiName == UIType.DragonDoorUpgrade.uiName)
		{
			var ui = edata.ui.GetComponent<UIDragonDoorUpgrade>();

			var name = "btnRespecUpgrades";
			// var root = ui.transform.Find("Root");
			var btnCenteredRoot = ui.btnUpgrade.transform.parent.transform;
			var btnRespecUpgrades = btnCenteredRoot.Find(name)?.GetComponent<Button>();
			if (btnRespecUpgrades != null)
				return;

			btnRespecUpgrades = UnityEngine.Object.Instantiate(ui.btnUpgrade, btnCenteredRoot);
			btnRespecUpgrades.gameObject.name = name;
			btnRespecUpgrades.gameObject.SetActive(true);

			var txt = btnRespecUpgrades.gameObject.GetComponentInChildren<Text>();
			txt.text = "Respec";

			var refRect = ui.btnUpgrade.GetComponent<RectTransform>();
			var btnRect = btnRespecUpgrades.GetComponent<RectTransform>();
			// btnRect.anchorMin = refRect.anchorMin;
			// btnRect.anchorMax = refRect.anchorMax;
			// btnRect.pivot = refRect.pivot;
			var spacing = 5f;
			btnRect.anchoredPosition = new Vector2(-(btnRect.rect.width / 2 + spacing), 0);
			refRect.anchoredPosition = new Vector2(+refRect.rect.width / 2 + spacing, 0);

			btnRespecUpgrades.onClick.AddListener((UnityAction)delegate
			{
				var curLevel = g.data.buildSchool.dragonDoor.poolLevel;
				if (curLevel <= 0)
				{
					g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", "There are no upgrades available to respec.", 1);
				}
				g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", "Respec Greenscale Spring upgrades?", 2, (Il2CppSystem.Action)RespecGreenscaleUpgrades);
			});
		}
	}

	static void RespecGreenscaleUpgrades()
	{
		static void OnSelect(int level, int buff)
		{
			foreach (var item in g.data.buildSchool.dragonDoor.unitsBuffs)
			{
				if (item.Key == g.world.playerUnit.data.unitData.unitID)
				{
					item.Value[level][0] = buff;
					break;
				}
			}
			ShowSelectGreenscaleUpgrade(level + 1, OnSelect, OnFinish);
		}

		static void OnFinish()
		{
			g.ui.CloseUI(UIType.DragonDoorUpgrade);
			g.ui.OpenUI(UIType.DragonDoorUpgrade);
		}

		ShowSelectGreenscaleUpgrade(1, OnSelect, OnFinish);
	}

	static void ShowSelectGreenscaleUpgrade(int level, Action<int, int> onSelect = null, Action onError = null)
	{
		// Picked this set of actions from here:
		// UIDragonDoorUpgrade.<>c__DisplayClass5_1$$<UpgradeEffect>b__2

		var curLevel = g.data.buildSchool.dragonDoor.poolLevel;
		if (level <= 0 || level > curLevel)
		{
			onError?.Invoke();
			return;
		}

		List<int> buffs = [];
		var confDragon = g.conf.schoolDragonDoor.GetItem(level);
		if (confDragon.dragonBuff.Count >= 1)
		{
			buffs.AddRange(confDragon.dragonBuff[0]);
		}
		if (confDragon.playerBuff.Count >= 1)
		{
			buffs.AddRange(confDragon.playerBuff[0]);
		}

		Il2CppSystem.Collections.Generic.List<DataStruct<string, string>> effectInfo = new();

		var valueData = new BattleSkillValueData(g.world.playerUnit);
		foreach (var buff in buffs)
		{
			var effect = g.conf.schoolDragonDoorBuff.GetEffect(buff);
			var confEffect = g.conf.battleEffect.GetItem(effect);
			var richDesc = UIMartialInfoTool.GetDescRichText(GameTool.LS(confEffect.desc), valueData, 2);
			effectInfo.Add(new(GameTool.LS(confEffect.name), richDesc));
		}

		var ui = g.ui.OpenUI<UIDragonDoorSelectEffect>(UIType.DragonDoorSelectEffect);
		ui.InitData(effectInfo, (Action<int>)delegate (int idx)
		{
			onSelect?.Invoke(level, buffs[idx]);
		});
	}

	internal static void Log(object s)
	{
		MelonLogger.Msg($"{modNamespace}: {s}");
	}
}
