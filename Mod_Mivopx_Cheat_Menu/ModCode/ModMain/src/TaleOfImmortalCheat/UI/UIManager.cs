using System;
using System.Collections.Generic;
using MOD_Mivopx;
using TaleOfImmortalCheat.Localization;
using TaleOfImmortalCheat.UI.Panels;
using UnityEngine;
using UniverseLib.UI;

namespace TaleOfImmortalCheat.UI;

public class UIManager
{
	private static bool arePanelsCreated;

	private static UniverseLib.UI.UIBase uiBase;

	private static bool InitIsRootVisibleOnce;

	public static readonly Dictionary<PanelType, Panel> Panels;

	public static GameObject UIRoot => uiBase?.RootObject;

	public static bool IsRootVisible
	{
		get
		{
			return uiBase?.Enabled ?? false;
		}
		set
		{
			if (uiBase != null && (bool)UIRoot && uiBase.Enabled != value)
			{
				UniversalUI.SetUIActive("com.kewlpenpen.cheatmod", value);
			}
		}
	}

	public static bool IsUIVisible
	{
		get
		{
			return uiBase?.Enabled ?? false;
		}
		set
		{
			if (uiBase != null && (bool)UIRoot && uiBase.Enabled != value)
			{
				UniversalUI.SetUIActive("com.kewlpenpen.cheatmod", value);
			}
		}
	}

	static UIManager()
	{
		Panels = new Dictionary<PanelType, Panel>
		{
			{
				PanelType.Main,
				new MainPanel()
			},
			{
				PanelType.PlayerAttributes,
				new PlayerAttributesPanel()
			},
			{
				PanelType.NPCAttributes,
				new NPCAttributesPanel()
			},
			{
				PanelType.Items,
				new ItemsPanel()
			},
			{
				PanelType.FatePlanner,
				new BuildPlannerPanel()
			},
			{
				PanelType.SearchItem,
				new SearchItemPanel()
			},
			{
				PanelType.RemoveItem,
				new RemoveItemPanel()
			},
			{
				PanelType.SearchSkill,
				new SearchSkillPanel()
			},
			{
				PanelType.Exploit,
				new ExploitPanel()
			},
			{
				PanelType.ExploitMore,
				new ExploitPanelMore()
			},
			{
				PanelType.Other,
				new OtherPanel()
			},
			{
				PanelType.OtherMore,
				new OtherPanelMore()
			},
			{
				PanelType.Debug,
				new DebugPanel()
			},
			{
				PanelType.Language,
				new LanguagePanel()
			}
		};
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	public static void CreateMain()
	{
		if (uiBase != null)
		{
			return;
		}
		try
		{
			ModMain.Log(LocalizationHelper.T("other_msgs_creating_main_ui"));
			uiBase = UniversalUI.RegisterUI("com.kewlpenpen.cheatmod", Update);
			Panels[PanelType.Main].Create();
			ModMain.Log(LocalizationHelper.T("other_msgs_created_main_ui"));
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error creating main ui: " + ex.Message + "; " + ex.StackTrace);
			throw;
		}
	}

	public static void Init()
	{
		if (arePanelsCreated)
		{
			return;
		}
		arePanelsCreated = true;
		ModMain.Log(LocalizationHelper.T("other_msgs_creating_ui_panels"));
		try
		{
			foreach (KeyValuePair<PanelType, Panel> panel in Panels)
			{
				if (panel.Key != PanelType.Main)
				{
					panel.Value.Create();
				}
			}
			ModMain.Log(LocalizationHelper.T("other_msgs_ui_panels_created"));
		}
		catch (Exception ex)
		{
			ModMain.LogError("Creating ui error: " + ex.Message + "; " + ex.StackTrace);
			arePanelsCreated = false;
			throw;
		}
	}

	private static void Update()
	{
		bool allowDrag = true;
		foreach (Panel value in Panels.Values)
		{
			if (value.Update(allowDrag))
			{
				allowDrag = false;
			}
		}
	}

	public static void OnGameWorldUpdate()
	{
		ModMain.Log("UIManager.OnGameWorldUpdate");
		foreach (Panel value in Panels.Values)
		{
			value.OnGameWorldUpdate();
		}
		if (!InitIsRootVisibleOnce)
		{
			IsRootVisible = false;
			IsRootVisible = true;
			InitIsRootVisibleOnce = true;
		}
	}

	internal static void OnLoad()
	{
		if (Panels[PanelType.Main] is MainPanel mainPanel)
		{
			mainPanel.Loaded = true;
		}
		else
		{
			ModMain.LogWarning("Main panel is not of expected type MainPanel");
		}
	}

	public static void OnConfUpdate()
	{
		foreach (Panel value in Panels.Values)
		{
			value.OnConfUpdate();
		}
	}

	public static void CloseSubPanels()
	{
		foreach (KeyValuePair<PanelType, Panel> panel in Panels)
		{
			if (panel.Key != PanelType.Main)
			{
				panel.Value.IsVisible = false;
			}
		}
	}

	private static void UpdateUITexts()
	{
	}
}
