using System;
using UnityEngine.UI;
using EGameTypeData;
using MelonLoader;
using UnityEngine.Events;
using UnityEngine;

namespace MOD_OqiZtz;

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
										talent.state = 0;
									}
									break;
								}
							}

							// FIXME: Talent progress towards activeCost doesn't get preserved anywhere.
							// Will need to keep track of unlocked talents in save file separtaly via: 
							// g.data.obj.SetString(group, key, value)
							// And then modify the ConfArtifactSpriteTalent to make those already unlocked as "No Cost"

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
