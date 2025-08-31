using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using UnityEngine.Events;
using UnhollowerRuntimeLib;

namespace MOD_cK2zMO
{
	internal class UIModelPro : UIBase
	{
		public Il2CppSystem.Collections.Generic.List<WorldUnitBase> allUnits;
		public int flag = 1;
		public int indexPage = 1;
		public int indexGrade;
		public int indexPageCount = 1;
		public int maxShowCount = 3;
		public Sprite heroBlockSprite;
		public Sprite NormalColorSprite;
		public Sprite SpeicalPngSprite;
		public int selectIndex;
		public int mode;
		public static List<string> fileName;

		private static string charmPrefixLabel = "Charm: ";
		private static string confirmRemovePortraitLabel = "Are you sure you want to delete this portrait?";
		private static string confirmApplyPortraitLabel = "Are you sure you want to use this as your character's portrait?";
		private static string confirmEditCompletePortraitLabel = "Are you sure you want to overwrite portrait #{0} with the currently set one?";

		public UIModelPro(IntPtr ptr)
			: base(ptr)
		{
		}

		public static void OpenUI(int mode)
		{
			if (g.ui.GetUI(new UIType.UITypeBase("UIModelPro", UILayer.UI)) != null)
			{
				g.ui.CloseUI(new UIType.UITypeBase("UIModelPro", UILayer.UI));
			}
			ClassInjector.RegisterTypeInIl2Cpp<UIModelPro>();
			var uIModelPro = g.ui.OpenUI(new UIType.UITypeBase("UIModelPro", UILayer.UI)).gameObject.AddComponent<UIModelPro>();
			uIModelPro.mode = mode;
			uIModelPro.InitData();
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
			// NOTE: Since OpenUI for UIModelPro just brings it into focus and doesn't reset it or something, the InitData after that will
			// keep applying over the previous state, and that means the offsetting of textTitle and some other components will
			// compound over each other.
			// So, for this reason, wherever there's a button that spawns this UI, always close the UI if it's open and
			// only then reopen it again to make sure the state is reset.
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

				// Disable "Apply" when the UI is opened from ModDress, as it only adds to the confusion with both
				// "Edit" and "Apply" doing the same thing in this context
				if (mode == 3)
				{
					var btnApply = base.transform.Find("Root/ButtonSelect");
					btnApply.GetComponent<Button>().interactable = false;
					btnApply.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f, 1f); // gray;
				}
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
		public void SelectModel(int selectIndex, int? mode_ = null)
		{
			mode_ ??= mode;
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
				if (mode_ == 0)
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
				else if (mode_ == 1)
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
				else if (mode_ == 2)
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
				else if (mode_ == 3)
				{
					UIModDress ui = g.ui.GetUI<UIModDress>(UIType.ModDress);
					if (ui != null)
					{
						// Just setting valuestring doesn't work, valuestring is mostly for the copy-paste character
						// feature anyways, it's unused if we just set it like that without the callback of that
						// InputField (OnEdit*)
						ui.valueString = ModMain.GetModDataValueString(ModMain.ModelFile.ModelList[selectIndex].portraitModel);
						ui.playerData.SetModelData(ModMain.ModelFile.ModelList[selectIndex].portraitModel, battleModelHumanData);
						ui.playerData.dynUnitData.modelData = ModMain.ModelFile.ModelList[selectIndex].portraitModel;
						ui.playerData.dynUnitData.battleModelData = battleModelHumanData;
						ui.UpdateFacadeUI();
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
			if (ModMain.ModelFile.ModelList.Count == 0)
			{
				g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", "There are no favorited portraits to show.", 1);
				g.ui.CloseUI(new UIType.UITypeBase("UIModelPro", 0), false);
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
			// They're named "index" but they're just counts
			if (indexPage > indexPageCount)
			{
				indexPage = indexPageCount;
			}
			for (int i = 0; i < this.maxShowCount; i++)
			{
				int secondIndex = i + this.maxShowCount * (this.indexPage - 1);
				string text = "Root/Scroll View/Viewport/Content/ModelItemPro" + (i + 1).ToString();
				if (ModMain.ModelFile.ModelList.Count > secondIndex) // if index is valid
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
				if (ModMain.ModelFile.ModelList.Count > this.selectIndex)
				{
					// TODO: Change the confirmation dialog text for "Apply" based on the context, it has to be
					// different for when applying to player, npc or in the character creator... or make the
					// confirmation message generic so it works for all
					//
					// TODO: restrict gender when applying portrait if that isn't the case already, with a preference
					// that can switch off this guard if the user wants
					//
					// TODO: Add tooltips to buttons
					// FIXME: Jump input field seems to keep getting reset to current page

					// Instead of closing and reopening ModDress if already opened, just update the model data and bring
					// the UI into focus. Note that in case of UIModDress OpenUI calls create new instances of the UI
					// instead of reusing existing ones. I'm not sure why different UI classes show different behaviour.
					var ui = g.ui.GetUI<UIModDress>(UIType.ModDress);
					if (ui != null)
					{
						SelectModel(selectIndex, 3);
						ui.transform.SetAsLastSibling();
						ui.GetComponentInParent<Canvas>().sortingOrder = 200;
						return;
					}

					ui = g.ui.OpenUI<UIModDress>(UIType.ModDress);

					ModifyModDressUI(ui);

					// Set portrait model state and init UI
					if (ModMain.ModelFile.ModelList[this.selectIndex].portraitModel.sex == 1)
					{
						ui.InitData(ModMain.GetModDataValueString(ModMain.ModelFile.ModelList[this.selectIndex].portraitModel), (UnitSexType)1);
					}
					else if (ModMain.ModelFile.ModelList[this.selectIndex].portraitModel.sex == 2)
					{
						ui.InitData(ModMain.GetModDataValueString(ModMain.ModelFile.ModelList[this.selectIndex].portraitModel), (UnitSexType)2);
					}

					// Overwrite "Edit Complete" callback
					ui.btnOK.onClick.RemoveAllListeners();
					ui.btnOK.onClick.AddListener((Action)delegate
					{
						g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", string.Format(confirmEditCompletePortraitLabel, selectIndex + 1), 2, (Action)delegate
						{
							g.ui.CloseUI(UIType.ModDress);
							ModMain.ModelFile.ModelList[this.selectIndex].portraitModel = ModMain.GetPortraitModelData(ui.valueString, ModMain.ModelFile.ModelList[this.selectIndex].portraitModel);
							ModMain.ModelFile.SaveConf();
							this.UpData();
						});
					});

					static void ModifyModDressUI(UIModDress ui)
					{
						if (ui.btnOK.transform.parent.Find(ModMain.btnSavePortraitName) == null)
						{
							var btnSavePortrait = UnityEngine.Object.Instantiate(ui.btnOK, ui.btnOK.transform.parent);
							btnSavePortrait.name = ModMain.btnSavePortraitName;
							btnSavePortrait.transform.localPosition = new Vector3(ui.btnOK.transform.localPosition.x, ui.btnOK.transform.localPosition.y + 100f);
							btnSavePortrait.GetComponentInChildren<Text>().text = ModMain.btnAddToFavoriteLabel;
							var btn = btnSavePortrait.GetComponentInChildren<Button>();
							btn.onClick.RemoveAllListeners();
							btn.onClick.AddListener((System.Action)delegate
							{
								g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Notice", ModMain.confirmAddToFavoriteLabel, 2, (System.Action)delegate
								{
									string modelID = ui.GetModelID();
									var modDataValueString = new ModDataValueString();
									modDataValueString.SetString(modelID);
									var modelList = new ModelList
									{
										name = "nameless-" + System.DateTime.Now.TimeOfDay.ToString(),
										time = System.DateTime.Now.ToString(),
										tips = "",
										portraitModel = ModMain.GetPortraitModelData(modDataValueString)
									};
									ModMain.ModelFile.ModelList.Add(modelList);
									ModMain.ModelFile.SaveConf();
									MelonLogger.Msg(modelList.name + "The portrait data is saved successfully.");
								});
							});
						}
						if (ui.btnOK.transform.parent.Find(ModMain.btnViewFavoritesName) == null)
						{
							var btnViewFavorites = UnityEngine.Object.Instantiate(ui.btnOK, ui.btnOK.transform.parent);
							btnViewFavorites.name = ModMain.btnViewFavoritesName;
							btnViewFavorites.transform.localPosition = new Vector3(ui.btnOK.transform.localPosition.x, ui.btnOK.transform.localPosition.y + 50f);
							btnViewFavorites.GetComponentInChildren<Text>().text = ModMain.btnOpenFavoriteUILabel;
							var btn = btnViewFavorites.GetComponentInChildren<Button>();
							btn.onClick.RemoveAllListeners();
							btn.onClick.AddListener((System.Action)delegate
							{
								UIModelPro.OpenUI(3);
							});
						}
						string btnCloseName = "btnClose";
						if (ui.btnOK.transform.parent.Find(btnCloseName) == null)
						{
							var btnCloseGo = new GameObject
							{
								name = btnCloseName
							};
							btnCloseGo.transform.SetParent(ui.transform.Find("Root"), false);
							var rt = btnCloseGo.AddComponent<RectTransform>();
							rt.anchoredPosition = new Vector2(750f, 380f);
							rt.sizeDelta = new Vector2(60f, 60f); ;
							var img = btnCloseGo.AddComponent<Image>();
							img.sprite = SpriteTool.GetSprite("Common", "tuichu");
							img.preserveAspect = true;
							var btn = btnCloseGo.AddComponent<Button>();
							btn.targetGraphic = img;
							btn.onClick.AddListener((System.Action)delegate
							{
								UIBase ui = g.ui.GetUI(new UIType.UITypeBase("UIModelPro", UILayer.UI));
								if (ui != null)
								{
									UIModelPro component = ui.gameObject.GetComponent<UIModelPro>();
									if (component != null)
									{
										component.UpData();
										MelonLogger.Msg("Exit editing - start refreshing.");
									}
								}
								g.ui.CloseUI(UIType.ModDress);
							});
						}
					}
				}
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
						if (ModMain.ModelFile.ModelList.Count <= selectIndex)
						{
							selectIndex -= Math.Abs(ModMain.ModelFile.ModelList.Count - 1 - selectIndex);
						}
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
