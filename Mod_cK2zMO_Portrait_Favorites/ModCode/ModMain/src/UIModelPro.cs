using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using UnityEngine.Events;

namespace MOD_cK2zMO
{
	internal class UIModelPro : UIBase
	{
		public Il2CppSystem.Collections.Generic.List<WorldUnitBase> allUnits;
		public int flag = 1;
		public int indexPage = 1;
		public int indexGrade;
		public int indexPageCount = 1;
		public int manxCount = 4;
		public int maxShowCount = 3;
		public Sprite heroBlockSprite;
		public Sprite NormalColorSprite;
		public Sprite SpeicalPngSprite;
		public int selectIndex;
		public int mode;
		public static List<string> fileName;

		private static string charmPrefixLabel = "Charm: ";
		private static string confirmEditPortraitLabel = "Are you sure you want to edit this portrait?";
		private static string confirmRemovePortraitLabel = "Are you sure you want to delete this portrait?";
		private static string confirmApplyPortraitLabel = "Are you sure you want to use this as your character's portrait?";

		public UIModelPro(IntPtr ptr)
			: base(ptr)
		{
		}
		public new void Init()
		{
			Shader shader = Shader.Find("Custom/BackBlur");
			if (shader != null)
			{
				Material material = new Material(shader);
				base.transform.GetChild(0).GetComponent<Image>().material = material;
			}
			base.Init();
		}

		internal void InitData()
		{
			{
				// Set english text
				base.transform.Find("Root/TextPage/InputField/Placeholder").GetComponent<Text>().text = "Page No.";
				base.transform.Find("Root/ButtonNext/Text").GetComponent<Text>().text = "Next";
				base.transform.Find("Root/ButtonLast/Text").GetComponent<Text>().text = "Previous";
				base.transform.Find("Root/ButtonSelect/Text").GetComponent<Text>().text = "Apply";
				base.transform.Find("Root/ButtonRemove/Text").GetComponent<Text>().text = "Delete";
				base.transform.Find("Root/TextPage/ButtonJump/Text").GetComponent<Text>().text = "Jump";
				base.transform.Find("Root/ButtonSave/Text").GetComponent<Text>().text = "Export";
				base.transform.Find("Root/ButtonChange/Text").GetComponent<Text>().text = "Edit";

				// Hide Export button
				// TODO: Maybe consider adding it back with game's FileTool
				var btnExport = base.transform.Find("Root/ButtonSave").gameObject;
				var btnEdit = base.transform.Find("Root/ButtonChange").gameObject;
				var exportRT = btnExport.GetComponent<RectTransform>();
				var editRT = btnEdit.GetComponent<RectTransform>();
				editRT.anchoredPosition = exportRT.anchoredPosition;
				editRT.sizeDelta = exportRT.sizeDelta;
				btnExport.SetActive(false);

				var textTitle = base.transform.Find("Root/TextTiTle");
				var textComp = textTitle.GetComponent<Text>();
				textComp.text = "Favorites";
				textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
				// Rotate by 90
				textTitle.rotation = Quaternion.Euler(0f, 0f, 90f);
				// Slightly offset to the left
				var pos = textTitle.localPosition;
				pos.x -= 10;
				textTitle.localPosition = pos;

				var textPage = base.transform.Find("Root/TextPage");
				textPage.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
				pos = textPage.localPosition;
				pos.x -= 20;
				textPage.localPosition = pos;

				// Restrict InputField to valid page numbers
				var input = base.transform.Find("Root/TextPage/InputField")
					.GetComponent<UnityEngine.UI.InputField>();
				if (input != null)
				{
					input.contentType = UnityEngine.UI.InputField.ContentType.IntegerNumber;
					input.ForceLabelUpdate();
				}
				input.onEndEdit.AddListener((UnityAction<string>)(val =>
				{
					if (int.TryParse(val, out int page))
					{
						page = Mathf.Clamp(page, indexPage, indexPageCount);
						input.text = page.ToString();
					}
					else
					{
						input.text = indexPage.ToString();
					}
				}));
				// Center the main text
				textComp = input.textComponent;
				if (textComp != null)
					textComp.alignment = TextAnchor.MiddleCenter;

				// Center the placeholder
				var placeholder = input.placeholder as Text;
				if (placeholder != null)
					placeholder.alignment = TextAnchor.MiddleCenter;

			}

			base.transform.gameObject.AddComponent<UIFastClose>();
			this.heroBlockSprite = g.res.Load<Sprite>("Icon/tongyongtouxiang_4");
			this.NormalColorSprite = g.res.Load<Sprite>("Icon/tongyongtouxiang_2");
			this.SpeicalPngSprite = g.res.Load<Sprite>("Icon/Imagebg");
			UIModelPro.fileName = new List<string> { "juanzhoudi", "kaijubg" };
			string text = "BG/" + UIModelPro.fileName[0];
			base.transform.Find("Root/ButtonClose").GetComponent<Image>().sprite = SpriteTool.GetSprite("Common", "tuichu");
			Action delegBTN1 = delegate
			{
				g.ui.CloseUI(new UIType.UITypeBase("UIModelPro", 0), false);
			};
			base.transform.Find("Root/ButtonClose").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegBTN1);
			Sprite sprite = SpriteTool.GetSpriteBigTex("bg/shubg_5");
			if (sprite == null)
			{
				sprite = SpriteTool.GetSpriteBigTex(text);
				if (sprite != null)
				{
					base.transform.Find("bg").GetComponent<Image>().sprite = sprite;
				}
			}
			else
			{
				base.transform.Find("bg").GetComponent<Image>().sprite = sprite;
			}
			if (ModMain.ModelFile.ModelList.Count % this.maxShowCount == 0)
			{
				this.indexPageCount = ModMain.ModelFile.ModelList.Count / this.maxShowCount;
			}
			else
			{
				this.indexPageCount = ModMain.ModelFile.ModelList.Count / this.maxShowCount + 1;
			}
			this.manxCount = ((this.indexPage > ModMain.ModelFile.ModelList.Count / this.maxShowCount) ? (ModMain.ModelFile.ModelList.Count % this.maxShowCount) : this.maxShowCount);
			Action DelegBtnJmp = delegate
			{
				if (base.transform.Find("Root/TextPage/InputField/Text").GetComponent<Text>().text.Length > 0)
				{
					int num = int.Parse(base.transform.Find("Root/TextPage/InputField/Text").GetComponent<Text>().text);
					MelonLogger.Msg("The entered jump page number is：" + num.ToString());
					if (num < 1 || num > this.indexPageCount)
					{
						MelonLogger.Warning("Page shouldn't have been out of index after validation");
						return;
					}
					this.indexPage = num;
					this.UpData();
				}
			};
			base.transform.Find("Root/TextPage/ButtonJump").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DelegBtnJmp);
			Action DelegBtnNext = delegate
			{
				if (this.indexPage < this.indexPageCount)
				{
					this.indexPage++;
					this.UpData();
					return;
				}
				this.indexPage = 1;
				this.UpData();
			};
			base.transform.Find("Root/ButtonNext").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DelegBtnNext);
			Action DelegBtnPrev = delegate
			{
				if (this.indexPage > 1)
				{
					this.indexPage--;
					this.UpData();
					return;
				}
				this.indexPage = this.indexPageCount;
				this.UpData();
			};
			base.transform.Find("Root/ButtonLast").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DelegBtnPrev);
			this.UpData();
		}
		public void SortFun()
		{
		}
		public void SelectModel(int selectIndex)
		{
			if (ModMain.ModelFile.ModelList.Count > selectIndex)
			{
				BattleModelHumanData battleModelHumanData = new BattleModelHumanData
				{
					back = ModMain.ModelFile.ModelList[selectIndex].portraitModel.back,
					body = ModMain.ModelFile.ModelList[selectIndex].portraitModel.body,
					hair = ModMain.ModelFile.ModelList[selectIndex].portraitModel.hair,
					hat = ModMain.ModelFile.ModelList[selectIndex].portraitModel.hat,
					head = ModMain.ModelFile.ModelList[selectIndex].portraitModel.head,
					sex = ModMain.ModelFile.ModelList[selectIndex].portraitModel.sex
				};
				if (this.mode == 0)
				{
					UICreatePlayer ui = g.ui.GetUI<UICreatePlayer>(UIType.CreatePlayer);
					if (ui != null)
					{
						ui.playerData.SetModelData(ModMain.ModelFile.ModelList[selectIndex].portraitModel, battleModelHumanData);
						ui.playerData.dynUnitData.modelData = ModMain.ModelFile.ModelList[selectIndex].portraitModel;
						ui.playerData.dynUnitData.battleModelData = battleModelHumanData;
						ui.uiFacade.portraitModel.data = ModMain.ModelFile.ModelList[selectIndex].portraitModel;
						g.ui.CloseUI(new UIType.UITypeBase("UIModelPro", 0), false);
						MelonLogger.Msg("Start refreshing face pinching data");
						ui.playerData.dynUnitData.beauty.baseValue = g.conf.roleDress.GetBeautyValue(ui.playerData.dynUnitData.modelData);
						ui.uiFacade.UpdateFacadeUI();
						return;
					}
				}
				else if (this.mode == 1)
				{
					UIPlayerInfo ui2 = g.ui.GetUI<UIPlayerInfo>(UIType.PlayerInfo);
					if (ui2 != null)
					{
						g.world.playerUnit.data.SetModelData(ModMain.ModelFile.ModelList[selectIndex].portraitModel, battleModelHumanData);
						g.world.playerUnit.data.dynUnitData.beauty.baseValue = g.conf.roleDress.GetBeautyValue(g.world.playerUnit.data.dynUnitData.modelData);
						ui2.uiProperty.UpdateUI();
						if (SceneType.map != null && SceneType.map.world != null)
						{
							SceneType.map.world.UpdatePlayerModel(true);
						}
						UIMapMain ui3 = g.ui.GetUI<UIMapMain>(UIType.MapMain);
						if (ui3 != null)
						{
							ui3.uiPlayerInfo.ResetUnitModel();
							ui3.uiPlayerInfo.UpdatePlayerInfo();
							ui3.uiPlayerInfo.UpdateUI();
						}
						g.ui.CloseUI(new UIType.UITypeBase("UIModelPro", 0), false);
						return;
					}
				}
				else if (this.mode == 2)
				{
					UINPCInfo ui4 = g.ui.GetUI<UINPCInfo>(UIType.NPCInfo);
					if (ui4 != null)
					{
						ui4.unit.data.SetModelData(ModMain.ModelFile.ModelList[selectIndex].portraitModel, battleModelHumanData);
						ui4.unit.data.dynUnitData.beauty.baseValue = g.conf.roleDress.GetBeautyValue(ui4.unit.data.dynUnitData.modelData);
						ui4.UpdateUI();
						g.ui.CloseUI(new UIType.UITypeBase("UIModelPro", 0), false);
						return;
					}
				}
				else if (this.mode == 3)
				{
					UIModDress ui5 = g.ui.GetUI<UIModDress>(UIType.ModDress);
					if (ui5 != null)
					{
						ui5.valueString = ModMain.GetModDataValueString(ModMain.ModelFile.ModelList[selectIndex].portraitModel);
						ui5.UpdateFacadeUI();
						g.ui.CloseUI(new UIType.UITypeBase("UIModelPro", 0), false);
					}
				}
			}
		}
		public void UpData()
		{
			if (ModMain.ModelFile == null || ModMain.ModelFile.ModelList == null)
			{
				return;
			}
			if (ModMain.ModelFile.ModelList.Count % this.maxShowCount == 0)
			{
				this.indexPageCount = ModMain.ModelFile.ModelList.Count / this.maxShowCount;
			}
			else
			{
				this.indexPageCount = ModMain.ModelFile.ModelList.Count / this.maxShowCount + 1;
			}
			this.manxCount = ((this.indexPage > ModMain.ModelFile.ModelList.Count / this.maxShowCount) ? (ModMain.ModelFile.ModelList.Count % this.maxShowCount) : this.maxShowCount);
			for (int i = 0; i < this.maxShowCount; i++)
			{
				int secondIndex = i + this.maxShowCount * (this.indexPage - 1);
				string text = "Root/Scroll View/Viewport/Content/ModelItemPro" + (i + 1).ToString();
				if (i < this.manxCount)
				{
					PortraitModelData portraitModelDatas = ModMain.ModelFile.ModelList[secondIndex].portraitModel;
					if (portraitModelDatas != null)
					{
						base.transform.Find(text + "/Text1").GetComponent<Text>().text = "#" + Convert.ToString(secondIndex + 1);
						base.transform.Find(text + "/Text2").GetComponent<Text>().text = charmPrefixLabel + g.conf.roleDress.GetBeautyValue(portraitModelDatas).ToString();
						base.transform.Find(text + "/Image").GetComponent<Image>().sprite = this.NormalColorSprite;
						base.transform.Find(text).GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
						Action DelegBtnView = delegate
						{
							if (ModMain.ModelFile.ModelList.Count > secondIndex)
							{
								int num = secondIndex % this.maxShowCount;
								RawImage component3 = this.transform.Find("Root/RawImageModel").GetComponent<RawImage>();
								PortraitModel.CreateTextureInModelData(portraitModelDatas, component3, new Vector2(0f, -5.2f), 1f, false, true, null);
								this.selectIndex = secondIndex;
								for (int j = 0; j < this.maxShowCount; j++)
								{
									string text2 = "Root/Scroll View/Viewport/Content/ModelItemPro" + (j + 1).ToString() + "/ImageBG";
									this.transform.Find(text2).GetComponent<Image>().sprite = this.SpeicalPngSprite;
								}
								if (this.heroBlockSprite != null)
								{
									string text3 = "Root/Scroll View/Viewport/Content/ModelItemPro" + (num + 1).ToString() + "/ImageBG";
									this.transform.Find(text3).GetComponent<Image>().sprite = this.heroBlockSprite;
								}
								else
								{
									MelonLogger.Msg("The border image is empty");
								}
								this.transform.Find("Root/TextShowIndex").GetComponent<Text>().text = "#" + (secondIndex + 1).ToString() + $"\n{charmPrefixLabel}" + g.conf.roleDress.GetBeautyValue(portraitModelDatas).ToString();
							}
						};
						base.transform.Find(text).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DelegBtnView);
						RawImage component = base.transform.Find(text + "/Image/RawImage").GetComponent<RawImage>();
						PortraitModel.CreateTextureInModelData(portraitModelDatas, component, new Vector2(0f, -24.5f), 3.3f, false, true, null);
					}
					else
					{
						base.transform.Find(text + "/Text1").GetComponent<Text>().text = "";
						base.transform.Find(text + "/Text2").GetComponent<Text>().text = "";
						base.transform.Find(text + "/Image").GetComponent<Image>().sprite = this.SpeicalPngSprite;
						base.transform.Find(text + "/Image/RawImage").GetComponent<RawImage>().texture = this.SpeicalPngSprite.texture;
						base.transform.Find(text).GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
					}
				}
				else
				{
					base.transform.Find(text + "/Text1").GetComponent<Text>().text = "";
					base.transform.Find(text + "/Text2").GetComponent<Text>().text = "";
					base.transform.Find(text + "/Image").GetComponent<Image>().sprite = this.SpeicalPngSprite;
					base.transform.Find(text + "/Image/RawImage").GetComponent<RawImage>().texture = this.SpeicalPngSprite.texture;
					base.transform.Find(text).GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
				}
				base.transform.Find("Root/Scroll View/Viewport/Content/ModelItemPro" + (i + 1).ToString() + "/ImageBG").GetComponent<Image>().sprite = this.SpeicalPngSprite;
			}
			if (ModMain.ModelFile.ModelList.Count > this.selectIndex && ModMain.ModelFile.ModelList[this.selectIndex].portraitModel != null)
			{
				RawImage component2 = base.transform.Find("Root/RawImageModel").GetComponent<RawImage>();
				PortraitModel.CreateTextureInModelData(ModMain.ModelFile.ModelList[this.selectIndex].portraitModel, component2, new Vector2(0f, -5.2f), 1f, false, true, null);
				base.transform.Find("Root/TextShowIndex").GetComponent<Text>().text = "#" + (this.selectIndex + 1).ToString() + $"\n{charmPrefixLabel}" + g.conf.roleDress.GetBeautyValue(ModMain.ModelFile.ModelList[this.selectIndex].portraitModel).ToString();
			}
			base.transform.Find("Root/ButtonChange").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			Action DelegBtnTuning = delegate
			{
				Action DelegConfTuning = delegate
				{
					if (ModMain.ModelFile.ModelList.Count > this.selectIndex)
					{
						UIModDress uIModDress = g.ui.OpenUI<UIModDress>(UIType.ModDress);
						if (ModMain.ModelFile.ModelList[this.selectIndex].portraitModel.sex == 1)
						{
							uIModDress.InitData(ModMain.GetModDataValueString(ModMain.ModelFile.ModelList[this.selectIndex].portraitModel), (UnitSexType)1);
						}
						else if (ModMain.ModelFile.ModelList[this.selectIndex].portraitModel.sex == 2)
						{
							uIModDress.InitData(ModMain.GetModDataValueString(ModMain.ModelFile.ModelList[this.selectIndex].portraitModel), (UnitSexType)2);
						}

						Action DelegBtnOK = delegate
						{
							ModMain.ModelFile.ModelList[this.selectIndex].portraitModel = ModMain.GetPortraitModelData(uIModDress.valueString, ModMain.ModelFile.ModelList[this.selectIndex].portraitModel);
							ModMain.ModelFile.SaveConf();
							this.UpData();
						};
						uIModDress.btnOK.onClick.AddListener(DelegBtnOK);
					}
				};
				g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", confirmEditPortraitLabel, 2, DelegConfTuning, null);
			};
			base.transform.Find("Root/ButtonChange").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DelegBtnTuning);
			base.transform.Find("Root/ButtonRemove").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			Action DelegBtnRem = delegate
			{
				Action DelegRemConf = delegate
				{
					if (ModMain.ModelFile.ModelList.Count > this.selectIndex)
					{
						ModMain.ModelFile.ModelList.RemoveAt(this.selectIndex);
						ModMain.ModelFile.SaveConf();
						this.UpData();
					}
				};
				g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", confirmRemovePortraitLabel, 2, DelegRemConf, null);
			};
			base.transform.Find("Root/ButtonRemove").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DelegBtnRem);
			base.transform.Find("Root/ButtonSelect").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			Action DelegBtnConf = delegate
			{
				Action DelegSelConf = delegate
				{
					if (ModMain.ModelFile.ModelList.Count > this.selectIndex)
					{
						MelonLogger.Msg("Number：" + this.selectIndex.ToString());
						this.SelectModel(this.selectIndex);
					}
				};
				g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", confirmApplyPortraitLabel, 2, DelegSelConf, null);
			};
			base.transform.Find("Root/ButtonSelect").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DelegBtnConf);
			base.transform.Find("Root/ButtonSave").GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			Action DelegBtnSav = delegate
			{
				// Action DelegConfSav = delegate
				// {
				// 	if (ModMain.ModelFile.ModelList.Count > this.selectIndex)
				// 	{
				// 		RawImage component4 = base.transform.Find("Root/RawImageModel").GetComponent<RawImage>();
				// 		if (component4 != null)
				// 		{
				// 			Texture mainTexture = component4.mainTexture;
				// 			Texture2D texture2D = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.ARGB32, false);
				// 			RenderTexture active = RenderTexture.active;
				// 			RenderTexture temporary = RenderTexture.GetTemporary(mainTexture.width, mainTexture.height, 32);
				// 			Graphics.Blit(mainTexture, temporary);
				// 			RenderTexture.active = temporary;
				// 			texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
				// 			texture2D.Apply();
				// 			RenderTexture.active = active;
				// 			RenderTexture.ReleaseTemporary(temporary);
				// 			byte[] bytes = ImageConversion.EncodeToPNG(texture2D);
				// 			SaveFileDialog saveFileDialog = new SaveFileDialog();
				// 			saveFileDialog.Filter = "Image Files (*.png)|*.png";
				// 			if (saveFileDialog.ShowDialog() == DialogResult.OK)
				// 			{
				// 				MelonLogger.Msg(saveFileDialog.FileName);
				// 				Action delegWrFile = delegate
				// 												{
				// 													FileTool.WriteByteAsync(saveFileDialog.FileName, bytes, null);
				// 												};
				// 				g.timer.Thread(delegWrFile, null);
				// 			}
				// 		}
				// 	}
				// };
				g.ui.OpenUI<UITextInfo>(UIType.TextInfo).InitData("Notice", "This option is currently disabled");
				// g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", "Are you sure to save the corresponding 3D image?", 2, DelegConfSav, null);
			};
			base.transform.Find("Root/ButtonSave").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DelegBtnSav);
			base.transform.Find("Root/TextPage").GetComponent<Text>().text = string.Concat(new string[]
			{
				$"Page {indexPage}/{indexPageCount}"
			});
		}
	}
}
