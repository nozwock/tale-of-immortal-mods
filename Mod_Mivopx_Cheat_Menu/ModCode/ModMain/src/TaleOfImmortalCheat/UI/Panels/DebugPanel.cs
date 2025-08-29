using System;
using System.Reflection;
using EBattleTypeData;
using EGameTypeData;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;
using MOD_Mivopx.UI.Panels;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets;

namespace TaleOfImmortalCheat.UI.Panels;

internal class DebugPanel : Panel
{
	public enum CustomLogLevel
	{
		Normal,
		Warning,
		Error
	}

	private const string PanelName = "Debug";

	private int CountLogs;

	private const int MaxLogs = 400;

	private List<string> logMessages = new List<string>();

	private Text logText;

	private ScrollRect scrollRect;

	private GameObject logScrollView;

	private InputField inputField;

	private Dropdown dropdown;

	private TooltipPanel tooltip = new TooltipPanel("ButtonTooltip");

	private Dictionary<RectTransform, string> buttonTooltips = new Dictionary<RectTransform, string>();

	private bool TestingMethod;

	public bool DebugPanelLog;

	private int frameCount;

	private float elapsedTime;

	private float fps;

	private GameObject _currTestLabel;

	public static DebugPanel Instance { get; private set; }

	public DebugPanel()
		: base(isStartedVisible: false)
	{
		Instance = this;
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
		frameCount++;
		elapsedTime += Time.deltaTime;
		if (elapsedTime > 1f)
		{
			fps = (float)frameCount / elapsedTime;
			frameCount = 0;
			elapsedTime = 0f;
		}
		if (TestingMethod && _currTestLabel != null)
		{
			_currTestLabel.GetComponent<Text>().text = "FPS: " + Mathf.RoundToInt(fps);
		}
		Dictionary<RectTransform, string>.Enumerator enumerator = buttonTooltips.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<RectTransform, string> current = enumerator.Current;
			if (current.Key != null && !string.IsNullOrEmpty(current.Value))
			{
				Vector3 point = current.Key.InverseTransformPoint(mousePosition);
				if (current.Key.rect.Contains(point))
				{
					tooltip.TooltipFor(current.Key, current.Value);
					tooltip.IsVisible = true;
					break;
				}
			}
		}
		return result;
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		Debug.Log("Cheat UI - Debug panel");
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel("Debug", uiRoot, out contentHolder);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, false, true, true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.3f, 0.3f);
		component.anchorMax = new Vector2(0.8f, 0.8f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 700f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 765f);
		GameObject item = UIHelper.CreateTitleBar(contentHolder, delegate
		{
			base.IsVisible = false;
		}, "<b>CheatPanel</b> - Debug");
		buttonTooltips.Clear();
		CreateLogPanel(contentHolder);
		CreateToggler(contentHolder);
		tooltip.Create();
		return (panelRoot: gameObject, draggableArea: item);
	}

	private void CreateToggler(GameObject contentHolder)
	{
		UIFactory.CreateToggle(contentHolder, "Log Message", out var toggle, out var text);
		toggle.isOn = false;
		text.text = "Log Message";
		Action<bool> action = delegate(bool v_LogMessage)
		{
			DebugPanelLog = v_LogMessage;
		};
		toggle.onValueChanged.AddListener(action);
		UIFactory.CreateToggle(contentHolder, "Log Active UI Data", out var toggle2, out var text2);
		toggle2.isOn = false;
		text2.text = "Log Active UI Data";
		Action<bool> action2 = delegate(bool v_UIInfo)
		{
			ExploitPatch_UIBase.IsThisActive = v_UIInfo;
		};
		toggle2.onValueChanged.AddListener(action2);
		UIFactory.CreateToggle(contentHolder, "Test Function", out var toggle3, out var text3);
		toggle3.isOn = false;
		text3.text = "Test Function";
		Action<bool> action3 = delegate(bool v_TestMethod)
		{
			if (v_TestMethod)
			{
				TestingMethod = true;
				ModMain.LogTip("Testing Method Enabled.");
			}
			else
			{
				TestingMethod = false;
				ModMain.LogTip("Testing Method Disabled.");
			}
		};
		toggle3.onValueChanged.AddListener(action3);
		buttonTooltips[toggle.gameObject.GetComponent<RectTransform>()] = "(For Developer Use). Toggle Debug Log Message ON/OFF.";
		buttonTooltips[toggle2.gameObject.GetComponent<RectTransform>()] = "(For Developer Use). Log the data of all initiated UI.";
		buttonTooltips[toggle3.gameObject.GetComponent<RectTransform>()] = "(For Developer Use). Test Method.";
	}

	private void CreateLogPanel(GameObject parent)
	{
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel("LogPanel", parent, out contentHolder);
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 500f);
		gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
		CreateCurrTestLabel(contentHolder);
		AutoSliderScrollbar autoScrollbar;
		GameObject gameObject2 = UIFactory.CreateScrollView(gameObject, "LogScrollView", out logScrollView, out autoScrollbar);
		scrollRect = gameObject2.GetComponent<ScrollRect>();
		gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 500f);
		logText = UIFactory.CreateLabel(logScrollView, "LogText", "");
		logText.alignment = TextAnchor.UpperLeft;
		logText.color = Color.cyan;
		logText.supportRichText = true;
		logText.fontSize = 14;
		scrollRect.horizontal = false;
		scrollRect.vertical = true;
		scrollRect.scrollSensitivity = 20f;
		CreateDropdownInput(contentHolder);
		InputFieldRef inputFieldRef = UIFactory.CreateInputField(contentHolder, "InputField", ModMain.Instance.DeveloperMode ? "Please select a class (Game.name)." : "Developer Mode not enabled.");
		inputField = inputFieldRef.Component;
		inputField.placeholder.GetComponent<Text>().text = (ModMain.Instance.DeveloperMode ? "Please select a class (Game.name)." : "Developer Mode not enabled.");
		ButtonRef buttonRef = UIFactory.CreateButton(contentHolder, "LogInput", "Log Input");
		GameObject gameObject3 = buttonRef.Component.gameObject;
		UIFactory.SetLayoutElement(gameObject3, null, 35, null, 0);
		buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, new Action(LogInputField));
		ButtonRef buttonRef2 = UIFactory.CreateButton(contentHolder, "TestFunc", "Test Function");
		GameObject gameObject4 = buttonRef2.Component.gameObject;
		UIFactory.SetLayoutElement(gameObject4, null, 35, null, 0);
		buttonRef2.OnClick = (Action)Delegate.Combine(buttonRef2.OnClick, (Action)delegate
		{
			TestFunction();
		});
		GameObject gameObject5 = UIFactory.CreateButton(contentHolder, "ClearButton", "Clear Logs").Component.gameObject;
		UIFactory.SetLayoutElement(gameObject5, minHeight: 35, flexibleHeight: 0, minWidth: 100);
		gameObject5.GetComponent<Button>().onClick.AddListener(ClearLogs);
		buttonTooltips[inputFieldRef.Component.gameObject.GetComponent<RectTransform>()] = "(For Developer Use). Choose a Game Class. Input a Class Property/Field of (Game.name), then press the Log Input button.";
		buttonTooltips[gameObject3.GetComponent<RectTransform>()] = "(For Developer Use). Used to Debug the Entered Class Property/Field of (Game.name). Logs should be in the 'Debug Tab'.";
		buttonTooltips[gameObject4.GetComponent<RectTransform>()] = "(For Developer Use). This is a test function method, pressing this button will do nothing. Maybe...";
		buttonTooltips[gameObject5.GetComponent<RectTransform>()] = "(For Developer Use). Clear Debug Logs.";
	}

	private void CreateCurrTestLabel(GameObject parent)
	{
		_currTestLabel = UIFactory.CreateLabel(parent, "TestName-label", "Test Label", TextAnchor.MiddleCenter, Color.cyan, supportRichText: true, 15).gameObject;
		_currTestLabel.GetComponent<Text>().text = "Testing Label.";
		UIFactory.SetLayoutElement(_currTestLabel, null, 35, null, 0);
		buttonTooltips[_currTestLabel.gameObject.GetComponent<RectTransform>()] = "Testing Label.";
	}

	public void LogMessage(string message, CustomLogLevel level = CustomLogLevel.Normal)
	{
		if (DebugPanelLog)
		{
			if (logMessages.Count >= 400)
			{
				logMessages.RemoveAt(0);
			}
			logMessages.Add(level switch
			{
				CustomLogLevel.Warning => "<color=yellow>", 
				CustomLogLevel.Error => "<color=red>", 
				_ => "<color=cyan>", 
			} + message + "</color>");
			CountLogs++;
			UpdateLog();
		}
	}

	public int GetLogsCount()
	{
		return CountLogs;
	}

	private void UpdateLog()
	{
		logText.text = string.Join("\n", logMessages.ToArray());
		Canvas.ForceUpdateCanvases();
		ScrollToBottom();
	}

	private void ScrollToBottom()
	{
		scrollRect.verticalNormalizedPosition = 0f;
	}

	private void ClearLogs()
	{
		logMessages.Clear();
		CountLogs = 0;
		UpdateLog();
	}

	private void LogInputField()
	{
		if (!ModMain.Instance.DeveloperMode)
		{
			ModMain.LogWarning("Developer mode not enabled. Aborting process...");
			return;
		}
		string text = inputField.text;
		object obj = null;
		switch (dropdown.value)
		{
		case 0:
			obj = GetFieldOrProperty(Game.ConfMgr, text);
			break;
		case 1:
			obj = GetFieldOrProperty(Game.DataWorld, text);
			break;
		case 2:
			obj = GetFieldOrProperty(Game.WorldManager, text);
			break;
		case 3:
			obj = GetFieldOrProperty(Game.NpcUnit, text);
			break;
		case 4:
			obj = GetFieldOrProperty(Game.UIPlayerInfo, text);
			break;
		}
		if (obj != null)
		{
			LogIt(obj);
		}
		else
		{
			ModMain.LogWarning("Invalid input: " + text);
		}
	}

	private object GetFieldOrProperty(object obj, string name)
	{
		string[] array = name.Split('.');
		foreach (string name2 in array)
		{
			if (obj == null)
			{
				return null;
			}
			Type type = obj.GetType();
			FieldInfo field = type.GetField(name2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				obj = field.GetValue(obj);
				continue;
			}
			PropertyInfo property = type.GetProperty(name2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.CanRead)
			{
				obj = property.GetValue(obj, null);
				continue;
			}
			return null;
		}
		return obj;
	}

	private void LogIt(object obj)
	{
		LogAllClassField.LogAllFieldsAndProperties(obj);
	}

	private void CreateDropdownInput(GameObject parent)
	{
		GameObject gameObject = UIFactory.CreateDropdown(parent, "game-settings", out dropdown, "Select Option", 14, OnDropdownValueChanged, (!ModMain.Instance.DeveloperMode) ? new string[5] { "Developer mode not enabled", "Developer mode not enabled", "Developer mode not enabled", "Developer mode not enabled", "Developer mode not enabled" } : new string[5] { "Game.ConfMgr", "Game.DataWorld", "Game.WorldManager", "Game.NpcUnit", "Game.UIPlayerInfo" });
		UIFactory.SetLayoutElement(gameObject, null, 35, null, 0);
		buttonTooltips[gameObject.GetComponent<RectTransform>()] = "(For Developer Use). Select a 'Game.name' before proceeding to the next step.";
	}

	private void OnDropdownValueChanged(int selectedIndex)
	{
		string[] array = new string[5] { "Enter Class Property/Field to Log (Game.ConfMgr)", "Enter Class Property/Field to Log (Game.DataWorld)", "Enter Class Property/Field to Log (Game.WorldManager)", "Enter Class Property/Field to Log (Game.NpcUnit)", "Enter Class Property/Field to Log (Game.UIPlayerInfo)" };
		if (!ModMain.Instance.DeveloperMode)
		{
			array = new string[5] { "Developer mode not enabled. Aborting process...", "Developer mode not enabled. Aborting process...", "Developer mode not enabled. Aborting process...", "Developer mode not enabled. Aborting process...", "Developer mode not enabled. Aborting process..." };
		}
		inputField.placeholder.GetComponent<Text>().text = array[selectedIndex];
	}

	public void OnOpenUIEnd(ETypeData e)
	{
		if (!(e.Cast<OpenUIEnd>().uiType.uiName == UIType.Build10005.uiName))
		{
			return;
		}
		UIBuild10005 uI = g.ui.GetUI<UIBuild10005>(UIType.Build10005);
		if (!(uI != null))
		{
			return;
		}
		ConfWorldBuilding10005Item building10005Item = uI.building10005Item;
		if (building10005Item == null)
		{
			return;
		}
		ModMain.Log("Initial - Cost: " + building10005Item.cost);
		building10005Item.cost = "0";
		ModMain.Log("Updated - Cost: " + building10005Item.cost);
		PropertyInfo[] properties = building10005Item.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			object value = propertyInfo.GetValue(building10005Item, null);
			if (value != null)
			{
				ModMain.Log($"{propertyInfo.Name}: {value}");
			}
		}
		uI.UpdateUI();
		ModMain.LogTip("No Cost is Active.");
	}

	private void TestFunction()
	{
		if (!ModMain.Instance.DeveloperMode)
		{
			ModMain.LogWarning("Developer mode not enabled. Aborting process...");
			return;
		}
		try
		{
			ModMain.LogTip("No Active Function. Aborting process...", "WARNING");
		}
		catch (Exception ex)
		{
			ModMain.LogTip("An error occurred while trying to Test Function: " + ex.Message + ":" + ex.StackTrace, "WARNING");
		}
	}

	private void TestTool()
	{
		ModMain.LogTip("Random value from 0-9: " + CommonTool.Random(0, 10));
		ModMain.LogTip("Reading internationalized table text 'Prompt': " + GameTool.LS("common_tishi"));
	}

	private void TestEvents()
	{
		Action<ETypeData> action = OnUnitHitDynIntHandler;
		g.events.On(EBattleType.UnitHitDynIntHandler, action, 0);
		g.events.Off(EBattleType.UnitHitDynIntHandler, action);
	}

	private void OnUnitHitDynIntHandler(ETypeData e)
	{
		UnitHitDynIntHandler unitHitDynIntHandler = e.Cast<UnitHitDynIntHandler>();
		ModMain.Log("Unit hit in battle: " + unitHitDynIntHandler.hitUnit.data.name);
		unitHitDynIntHandler.dynV.baseValue = 0;
		unitHitDynIntHandler.dynV.ClearCall();
	}

	private void TestWorld()
	{
		ModMain.LogTip("Player name: " + g.world.playerUnit.data.unitData.propertyData.GetName());
		g.world.battle.IntoBattle(new DataMap.MonstData
		{
			id = 1011,
			level = 5
		});
		g.world.mapEvent.AddGridEvent(g.world.playerUnit.data.unitData.GetPoint(), 6);
		UnitConditionTool.Condition("grade_0_1_1", new UnitConditionData(g.world.playerUnit, null));
		DramaFunctionTool.OptionsFunction("openUI_" + UIType.PlayerInfo.uiName);
		BattleFunctionTool.OptionsFunction("createMonst_7210_0_0_3");
		WorldUnitBase unit = g.world.unit.GetUnit("NPCID_XXXXXX");
		ModMain.Log(unit.data.unitData.propertyData.GetName() ?? "");
		unit.CreateAction(new UnitActionLuckAdd(120));
		unit.CreateAction(new UnitActionRoleTrains(g.world.playerUnit));
		DramaTool.OpenDrama(610011, new DramaData
		{
			unitLeft = g.world.playerUnit,
			unitRight = null
		});
		UICustomDramaDyn uICustomDramaDyn = new UICustomDramaDyn(610011);
		Action action = delegate
		{
		};
		uICustomDramaDyn.SetOptionCall(6100111, action);
		uICustomDramaDyn.dramaData.dialogueOptionsAddText[1001] = "Additional button added";
		uICustomDramaDyn.OpenUI();
	}

	private void TestRes()
	{
		GameObject gameObject = g.res.Load<GameObject>("Effect/Battle/Skill/jueyingjian");
		ModMain.LogTip("This is the Jueying Sword effect: " + gameObject.name);
	}

	private void TestSounds()
	{
		g.sounds.PlayEffect("Battle/jineng/jian/jueyingjian", 1);
		ModMain.LogTip("This is the sound of the Jueying Sword");
	}

	private void TestConf()
	{
		ConfRoleGradeItem gradeItem = g.conf.roleGrade.GetGradeItem(1, 1);
		ModMain.LogTip("This is the configuration for Qi Condensation Early Stage: " + GameTool.LS(gradeItem.gradeName));
	}

	private void TestData()
	{
		ModMain.LogTip("This is the player's name: " + g.data.unit.GetUnit(g.data.world.playerUnitID).propertyData.GetName());
	}

	private void TestUI()
	{
		ModMain.Log("Test UI Called.");
		g.ui.OpenUI(UIType.PlayerInfo);
		ModMain.Log("OpenUI PlayerInfo Initialization Succeeded.");
		UITipItem.AddTip("Prompt message", 5f);
		ModMain.Log("AddTip Prompt message Initialization Succeeded.");
		g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData("Prompt", "This is a prompt box with two buttons", 2, (Action)delegate
		{
			ModMain.Log("Clicked YES");
		}, (Action)delegate
		{
			ModMain.Log("Clicked NO");
		});
		ModMain.Log("OpenUI UICheckPopup Initialization Succeeded.");
		g.ui.OpenUI<UITextInfo>(UIType.TextInfo).InitData("Prompt", "This is text information");
		ModMain.Log("OpenUI UITextInfo Initialization Succeeded.");
		g.ui.OpenUI<UITextInfoLong>(UIType.TextInfoLong).InitData("Prompt", "This is long text information");
		ModMain.Log("OpenUI TextInfoLong Initialization Succeeded.");
		ModMain.Log("Test UI Called Completed.");
	}
}
