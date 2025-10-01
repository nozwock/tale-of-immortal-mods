using System;
using UnityEngine.UI;
using EGameTypeData;
using MelonLoader;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnhollowerBaseLib;

namespace MOD_OqiZtz;

public class ModMain
{
	internal static readonly string modNamespace = typeof(ModMain).Namespace!;

	Il2CppSystem.Action<ETypeData> callSaveData;
	Il2CppSystem.Action<ETypeData> callIntoWorld;
	Il2CppSystem.Action<ETypeData> callOpenUIEnd;

	public static Dictionary<int, HashSet<int>> unlockedSpiritTalents = [];

	public enum SaveObjKey
	{
		UnlockedSpiritTalents
	}

	public void Init()
	{
		callIntoWorld = (Il2CppSystem.Action<ETypeData>)OnIntoWorld;
		callSaveData = (Il2CppSystem.Action<ETypeData>)OnSaveData;
		callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
		g.events.On(EGameType.IntoWorld, callIntoWorld);
		g.events.On(EGameType.SaveData, callSaveData);
		g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
	}

	public void Destroy()
	{
		g.events.Off(EGameType.IntoWorld, callIntoWorld);
		g.events.Off(EGameType.SaveData, callSaveData);
		g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);
	}

	void OnIntoWorld(ETypeData _)
	{
		try
		{
			if (g.data.obj.ContainsKey(modNamespace, SaveObjKey.UnlockedSpiritTalents.ToString()))
			{
				var json = g.data.obj.GetString(modNamespace, SaveObjKey.UnlockedSpiritTalents.ToString());
				unlockedSpiritTalents = JsonConvert.DeserializeObject<Dictionary<int, HashSet<int>>>(json);
			}
			else // Loaded a save that doesn't have it
			{
				unlockedSpiritTalents.Clear();
			}

			SetNoCostTalentForUnlocked();
		}
		catch (Exception ex)
		{
			Log(ex);
		}
	}

	void OnSaveData(ETypeData _)
	{
		try
		{
			var json = JsonConvert.SerializeObject(unlockedSpiritTalents);
			g.data.obj.SetString(modNamespace, SaveObjKey.UnlockedSpiritTalents.ToString(), json);
		}
		catch (Exception ex)
		{
			Log(ex);
		}
	}

	static void SetNoCostTalentForUnlocked(int spriteId = -1)
	{
		HashSet<int> unlockedTalents;
		foreach (var conf in g.conf.artifactSpriteTalent._allConfList)
		{
			if (spriteId != -1 && spriteId != conf.spriteID) // Only modify talent for this spirit
				continue;
			if (unlockedSpiritTalents.TryGetValue(conf.spriteID, out unlockedTalents) && unlockedTalents.Contains(conf.number) && conf.unlock2Type != 17 /*No Cost*/)
			{
				conf.unlock2Type = 17;
				conf.unlock2Value = conf.axisX.ToString();
				conf.unlock2Count = 1;
				conf.unlock3Type = 0;
				conf.unlock3Value = "0";
				conf.unlock3Count = 0;
				conf.unlockDesc = "0";
				conf.unlockDesc = "spriteTalent_unlockDesc100522"; // No Cost
				conf.activeCost = new Il2CppReferenceArray<Il2CppStructArray<int>>(0);
			}
		}

		foreach (var sprite in g.world.playerUnit.data.unitData.artifactSpriteData.sprites)
		{
			if (spriteId != -1 && spriteId != sprite.spriteID)
				continue;
			foreach (var talent in sprite.talents)
			{
				if (unlockedSpiritTalents.TryGetValue(sprite.spriteID, out unlockedTalents) && unlockedTalents.Contains(talent.number))
				{
					// Necessary, otherwise the discrepancy will result in a "No Cost" popup by game on trying to unlock
					talent.unlock2Count = 1;
					talent.unlock3Count = 0;
				}
			}
		}
	}

	void OnOpenUIEnd(ETypeData e)
	{
		try
		{
			var edata = e.Cast<OpenUIEnd>();
			if (edata.ui.name == UIType.Artifact.uiName)
			{
				var ui = edata.ui.GetComponent<UIArtifact>();

				var name = "btnResetTalents";
				var btnResetTalents = ui.talent.goGroupRoot.transform.Find(name)?.GetComponent<Button>();
				if (btnResetTalents != null)
					return;

				btnResetTalents = UnityEngine.Object.Instantiate(ui.sprite.btnGive_En, ui.talent.goGroupRoot.transform);
				btnResetTalents.gameObject.name = name;
				btnResetTalents.gameObject.SetActive(true);

				var txt = btnResetTalents.gameObject.GetComponentInChildren<Text>();
				txt.text = "Reset All";

				var refRect = ui.talent.textTalentNum.GetComponent<RectTransform>();
				var btnRect = btnResetTalents.GetComponent<RectTransform>();
				btnRect.anchorMin = refRect.anchorMin;
				btnRect.anchorMax = refRect.anchorMax;
				btnRect.pivot = refRect.pivot;
				var spacing = 10f;
				btnRect.anchoredPosition = refRect.anchoredPosition + new Vector2(0, btnRect.rect.height + spacing);

				btnResetTalents.onClick.AddListener((UnityAction)delegate
				{
					g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", "Reset all talents?", 2, (Il2CppSystem.Action)delegate
					{
						try
						{
							var ui = btnResetTalents.gameObject.GetComponentInParent<UIArtifact>();

							int i;
							for (i = 0; i < ui.uiSprite.spriteList.Count; i++)
							{
								var sprite = ui.uiSprite.spriteList[i];
								if (sprite.soleID == ui.uiSprite.selSpriteSoleId)
								{
									foreach (var talent in sprite.talents)
									{
										if (talent.state == 1)
										{
											if (!unlockedSpiritTalents.TryGetValue(sprite.spriteID, out var set))
											{
												set = [];
												unlockedSpiritTalents[sprite.spriteID] = set;
											}
											set.Add(talent.number);
										}
										talent.state = 0;
									}
									break;
								}
							}

							SetNoCostTalentForUnlocked(ui.uiSprite.spriteList[i].spriteID);

							// Update UI
							var toggle = ui.uiSprite.goSpriteItemRoot.transform.GetChild(i).GetComponentInChildren<Toggle>();
							toggle.onValueChanged.Invoke(toggle.isOn);
						}
						catch (Exception ex)
						{
							Log(ex);
						}
					});
				});
			}
		}
		catch (Exception ex)
		{
			Log(ex);
		}
	}

	internal static void Log(object s)
	{
		MelonLogger.Msg($"{modNamespace}: {s}");
	}
}
