using System;
using System.Collections.Generic;
using System.Reflection;
using EGameTypeData;
using HarmonyLib;
using Il2CppSystem;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// When you manually modify this namespace, you need to go to the module editor to modify the corresponding new namespace, and the assembly also needs to modify the namespace, otherwise the DLL will fail to load! ! !
/// </summary>
namespace MOD_cK2zMO
{
	/// <summary>
	/// This class is the main class of the module
	/// </summary>
	public class ModMain
	{
		private TimerCoroutine corUpdate;
		private static HarmonyLib.Harmony harmony;
		private Il2CppSystem.Action<ETypeData> openUIEndCall;
		private static ModelFile modelFile = new ModelFile();

		internal static ModelFile ModelFile { get => modelFile; set => modelFile = value; }

		/// <summary>
		/// MOD initialization, this function will be called when entering the game
		/// </summary>
		public void Init()
		{
			// If you use the Harmony patch function, you need to manually enable the patch.
			// Start all patches for the current assembly
			if (harmony != null)
			{
				harmony.UnpatchSelf();
				harmony = null;
			}
			if (harmony == null)
			{
				harmony = new HarmonyLib.Harmony("MOD_cK2zMO");
			}
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			openUIEndCall = new System.Action<ETypeData>(OnOpenUIEnd);
			g.events.On(EGameType.OpenUIEnd, openUIEndCall, 0, false);
			corUpdate = g.timer.Frame(new System.Action(OnUpdate), 1, true);
		}

		/// <summary>
		/// When the MOD is destroyed and returns to the main interface, this function will be called and the MOD will be reinitialized.
		/// </summary>
		public void Destroy()
		{
			g.events.Off(EGameType.OpenUIEnd, openUIEndCall);
			g.timer.Stop(corUpdate);
		}

		/// <summary>
		/// function called every frame
		/// </summary>
		private void OnUpdate()
		{

		}
		public void OnOpenUIEnd(ETypeData e)
		{
			OpenUIEnd openUIEnd = e.Cast<OpenUIEnd>();
			if (openUIEnd.uiType.uiName == UIType.PlayerInfo.uiName)
			{
				PlayerInfoUICMD();
				return;
			}
			if (openUIEnd.uiType.uiName == UIType.NPCInfo.uiName)
			{
				NPCInfoUICMD();
				return;
			}
			if (openUIEnd.uiType.uiName == UIType.CreatePlayer.uiName)
			{
				CreatePlayerUICMD();
				return;
			}
			if (openUIEnd.uiType.uiName == UIType.ModDress.uiName)
			{
				ModDressUICMD();
			}
		}
		public void PlayerInfoUICMD()
		{
			UIPlayerInfo ui = g.ui.GetUI<UIPlayerInfo>(UIType.PlayerInfo);
			if (!(ui != null))
			{
				return;
			}
			string text = "btn_save";
			if (ui.uiProperty.goGroupRoot.transform.Find(text) != null)
			{
				return;
			}
			Button button = UnityEngine.Object.Instantiate(ui.uiSkill.btnMartialTrain, ui.uiProperty.goGroupRoot.transform);
			button.name = text;
			button.transform.localPosition = new Vector3(-600f, -50f);
			Text componentInChildren = button.GetComponentInChildren<Text>();
			Button componentInChildren2 = button.GetComponentInChildren<Button>();
			componentInChildren2.onClick.RemoveAllListeners();
			componentInChildren.text = "Favorites";
			componentInChildren2.onClick.AddListener((System.Action)delegate
			{
				g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("hint", "Are you sure to collect the corresponding portrait data?", 2, (System.Action)delegate
				{
					ModelList modelList = new ModelList
					{
						name = ui.unit.data.unitData.propertyData.GetName(),
						time = System.DateTime.Now.ToString(),
						tips = "",
						portraitModel = g.world.playerUnit.data.unitData.propertyData.modelData
					};
					modelFile.ModelList.Add(modelList);
					modelFile.SaveConf();
					System.Console.WriteLine(modelList.name + "The portrait data is saved successfully.");
				});
			});
			string text2 = "btn_open";
			if (!(ui.uiProperty.goGroupRoot.transform.Find(text2) != null))
			{
				Button button2 = UnityEngine.Object.Instantiate(ui.uiSkill.btnMartialTrain, ui.uiProperty.goGroupRoot.transform);
				button2.name = text2;
				button2.transform.localPosition = new Vector3(-600f, -100f);
				Text componentInChildren3 = button2.GetComponentInChildren<Text>();
				Button componentInChildren4 = button2.GetComponentInChildren<Button>();
				componentInChildren4.onClick.RemoveAllListeners();
				componentInChildren3.text = "Portrait data";
				componentInChildren4.onClick.AddListener((System.Action)delegate
				{
					ClassInjector.RegisterTypeInIl2Cpp<UIModelPro>();
					UIModelPro uIModelPro = g.ui.OpenUI(new UIType.UITypeBase("UIModelPro", UILayer.UI)).gameObject.AddComponent<UIModelPro>();
					uIModelPro.mode = 1;
					uIModelPro.InitData();
				});
			}
		}
		public void NPCInfoUICMD()
		{
			UINPCInfo ui = g.ui.GetUI<UINPCInfo>(UIType.NPCInfo);
			if (!(ui != null))
			{
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(ui.uiUnitInfo.goButton3);
			Transform transform = ui.transform;
			gameObject.transform.SetParent(transform, worldPositionStays: false);
			gameObject.SetActive(value: true);
			gameObject.transform.localPosition = new Vector3(-600f, -150f);
			gameObject.name = "xsmsx_btn_save";
			gameObject.layer = int.MaxValue;
			gameObject.GetComponentInChildren<Text>().text = "Favorites";
			Button componentInChildren = gameObject.GetComponentInChildren<Button>();
			componentInChildren.onClick.RemoveAllListeners();
			componentInChildren.onClick.AddListener((System.Action)delegate
			{
				g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("hint", "Are you sure to collect the corresponding face pinching data?", 2, (System.Action)delegate
				{
					ModelList modelList = new ModelList
					{
						name = ui.unit.data.unitData.propertyData.GetName(),
						time = System.DateTime.Now.ToString(),
						tips = "",
						portraitModel = ui.unit.data.unitData.propertyData.modelData
					};
					modelFile.ModelList.Add(modelList);
					modelFile.SaveConf();
					System.Console.WriteLine(modelList.name + "The portrait data is saved successfully.");
				});
			});
			System.Console.WriteLine("NPC interface collection button added");
			GameObject gameObject2 = UnityEngine.Object.Instantiate(ui.uiUnitInfo.goButton3);
			gameObject2.transform.SetParent(transform, worldPositionStays: false);
			gameObject2.SetActive(value: true);
			gameObject2.transform.localPosition = new Vector3(-443f, -150f);
			gameObject2.name = "xsmsx_btn_open";
			gameObject2.layer = int.MaxValue;
			gameObject2.GetComponentInChildren<Text>().text = "Portrait data";
			Button componentInChildren2 = gameObject2.GetComponentInChildren<Button>();
			componentInChildren2.onClick.RemoveAllListeners();
			componentInChildren2.onClick.AddListener((System.Action)delegate
			{
				ClassInjector.RegisterTypeInIl2Cpp<UIModelPro>();
				UIModelPro uIModelPro = g.ui.OpenUI(new UIType.UITypeBase("UIModelPro", UILayer.UI)).gameObject.AddComponent<UIModelPro>();
				uIModelPro.mode = 2;
				uIModelPro.InitData();
			});
			System.Console.WriteLine("NPC interface portrait data button added");
		}
		public void CreatePlayerUICMD()
		{
			UICreatePlayer uI = g.ui.GetUI<UICreatePlayer>(UIType.CreatePlayer);
			if (!(uI != null))
			{
				return;
			}
			UICreatePlayerFacade __instance = uI.uiFacade;
			if (__instance == null)
			{
				System.Console.WriteLine("ui.uiFacade is empty.");
			}
			else if (__instance.goGroupRoot == null)
			{
				System.Console.WriteLine("ui.uiFacade.goGroupRoot is empty.");
			}
			else
			{
				if (__instance.goGroupRoot.transform.Find("save") != null)
				{
					return;
				}
				Button button = UnityEngine.Object.Instantiate(uI.btnOK, __instance.goGroupRoot.transform);
				button.name = "save";
				button.GetComponentInChildren<Text>().text = "Favorites";
				button.transform.localPosition = new Vector3(-185f, -325f);
				button.onClick.AddListener((System.Action)delegate
				{
					g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("hint", "Are you sure to collect the corresponding portrait data?", 2, (System.Action)delegate
					{
						ModelList modelList = new ModelList();
						if (__instance.portraitModel.data.sex == 1)
						{
							modelList.name = __instance.manName[0] + __instance.manName[1];
						}
						else
						{
							modelList.name = __instance.womanName[0] + __instance.womanName[1];
						}
						modelList.time = System.DateTime.Now.ToString();
						modelList.tips = "";
						modelList.portraitModel = __instance.portraitModel.data.Clone();
						modelFile.ModelList.Add(modelList);
						modelFile.SaveConf();
					});
				});
				Button button2 = UnityEngine.Object.Instantiate(uI.btnOK, __instance.goGroupRoot.transform);
				button2.name = "open";
				button2.GetComponentInChildren<Text>().text = "Portrait";
				button2.transform.localPosition = new Vector3(-25f, -325f);
				button2.onClick.AddListener((System.Action)delegate
				{
					ClassInjector.RegisterTypeInIl2Cpp<UIModelPro>();
					UIModelPro uIModelPro = g.ui.OpenUI(new UIType.UITypeBase("UIModelPro", UILayer.UI)).gameObject.AddComponent<UIModelPro>();
					uIModelPro.mode = 0;
					uIModelPro.InitData();
				});
			}
		}
		public void ModDressUICMD()
		{
			UIModDress ui = g.ui.GetUI<UIModDress>(UIType.ModDress);
			if (!(ui != null))
			{
				return;
			}
			string text = "btn_save";
			if (ui.btnOK.transform.parent.Find(text) != null)
			{
				return;
			}
			Button button = UnityEngine.Object.Instantiate(ui.btnOK, ui.btnOK.transform.parent);
			button.name = text;
			button.transform.localPosition = new Vector3(ui.btnOK.transform.localPosition.x, ui.btnOK.transform.localPosition.y + 150f);
			Text componentInChildren = button.GetComponentInChildren<Text>();
			Button componentInChildren2 = button.GetComponentInChildren<Button>();
			componentInChildren2.onClick.RemoveAllListeners();
			componentInChildren.text = "Favorites";
			componentInChildren2.onClick.AddListener((System.Action)delegate
			{
				g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("hint", "Are you sure to collect the corresponding portrait data?", 2, (System.Action)delegate
				{
					string modelID = ui.GetModelID();
					ModDataValueString modDataValueString = new ModDataValueString();
					modDataValueString.SetString(modelID);
					ModelList modelList = new ModelList
					{
						name = "nameless-" + System.DateTime.Now.TimeOfDay.ToString(),
						time = System.DateTime.Now.ToString(),
						tips = "",
						portraitModel = GetPortraitModelData(modDataValueString)
					};
					modelFile.ModelList.Add(modelList);
					modelFile.SaveConf();
					System.Console.WriteLine(modelList.name + "The portrait data is saved successfully.");
				});
			});
			string text2 = "btn_open";
			if (ui.btnOK.transform.parent.Find(text2) != null)
			{
				return;
			}
			Button button2 = UnityEngine.Object.Instantiate(ui.btnOK, ui.btnOK.transform.parent);
			button2.name = text2;
			button2.transform.localPosition = new Vector3(ui.btnOK.transform.localPosition.x, ui.btnOK.transform.localPosition.y + 100f);
			Text componentInChildren3 = button2.GetComponentInChildren<Text>();
			Button componentInChildren4 = button2.GetComponentInChildren<Button>();
			componentInChildren4.onClick.RemoveAllListeners();
			componentInChildren3.text = "Portrait data";
			componentInChildren4.onClick.AddListener((System.Action)delegate
			{
				if (g.ui.GetUI(new UIType.UITypeBase("UIModelPro", UILayer.UI)) == null)
				{
					ClassInjector.RegisterTypeInIl2Cpp<UIModelPro>();
					UIModelPro uIModelPro = g.ui.OpenUI(new UIType.UITypeBase("UIModelPro", UILayer.UI)).gameObject.AddComponent<UIModelPro>();
					uIModelPro.mode = 3;
					uIModelPro.InitData();
				}
				else
				{
					g.ui.OpenUI<UITextInfo>(UIType.TextInfo).InitData("hint", "You have already opened the portrait database interface, please do not click again.");
				}
			});
			string text3 = "btn_close";
			if (ui.btnOK.transform.parent.Find(text3) != null)
			{
				return;
			}
			Button button3 = UnityEngine.Object.Instantiate(ui.btnOK, ui.btnOK.transform.parent);
			button3.name = text3;
			button3.transform.localPosition = new Vector3(ui.btnOK.transform.localPosition.x, ui.btnOK.transform.localPosition.y + 50f);
			Text componentInChildren5 = button3.GetComponentInChildren<Text>();
			Button componentInChildren6 = button3.GetComponentInChildren<Button>();
			componentInChildren6.onClick.RemoveAllListeners();
			componentInChildren5.text = "Exit editing";
			componentInChildren6.onClick.AddListener((System.Action)delegate
			{
				UIBase uI = g.ui.GetUI(new UIType.UITypeBase("UIModelPro", UILayer.UI));
				if (uI != null)
				{
					UIModelPro component = uI.gameObject.GetComponent<UIModelPro>();
					if (component != null)
					{
						component.UpData();
						System.Console.WriteLine("Exit editing - start refreshing.");
					}
				}
				g.ui.CloseUI(UIType.ModDress);
			});
		}
		public static ModDataValueString GetModDataValueString(PortraitModelData portraitModelData)
		{
			ModDataValueString modDataValueString = new ModDataValueString();
			string @string = string.Join("|", new List<int>
			{
				portraitModelData.sex, portraitModelData.hat, portraitModelData.hair, portraitModelData.hairFront, portraitModelData.head, portraitModelData.eyebrows, portraitModelData.eyes, portraitModelData.nose, portraitModelData.mouth, portraitModelData.body,
				portraitModelData.back, portraitModelData.forehead, portraitModelData.faceFull, portraitModelData.faceLeft, portraitModelData.faceRight
			}.ToArray());
			modDataValueString.SetString(@string);
			return modDataValueString;
		}
		public static PortraitModelData GetPortraitModelData(ModDataValueString modDataValueString, PortraitModelData portraitModelData = null)
		{
			if (portraitModelData == null)
			{
				portraitModelData = new PortraitModelData();
			}
			string[] array = modDataValueString.GetString().Split('|');
			int num = 0;
			portraitModelData.sex = int.Parse(array[num++]);
			portraitModelData.hat = int.Parse(array[num++]);
			portraitModelData.hair = int.Parse(array[num++]);
			portraitModelData.hairFront = int.Parse(array[num++]);
			portraitModelData.head = int.Parse(array[num++]);
			portraitModelData.eyebrows = int.Parse(array[num++]);
			portraitModelData.eyes = int.Parse(array[num++]);
			portraitModelData.nose = int.Parse(array[num++]);
			portraitModelData.mouth = int.Parse(array[num++]);
			portraitModelData.body = int.Parse(array[num++]);
			portraitModelData.back = int.Parse(array[num++]);
			portraitModelData.forehead = int.Parse(array[num++]);
			portraitModelData.faceFull = int.Parse(array[num++]);
			portraitModelData.faceLeft = int.Parse(array[num++]);
			portraitModelData.faceRight = int.Parse(array[num++]);
			return portraitModelData;
		}
	}
}
