using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Il2CppSystem.Threading;
using MelonLoader;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.Localization;
using TaleOfImmortalCheat.UI;
using TaleOfImmortalCheat.UI.Panels;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.Input;

namespace MOD_Mivopx;

public class ModMain : MelonMod
{
	public const string GUID = "com.kewlpenpen.cheatmod";

	public const string Version = "1.1.8 Pub";

	public const string AuthorModifier = "Updated by Aqua-sama";

	public const string SupportedGameVersion = "v1.2.111.259+";

	public static ModMain Instance;

	private static HarmonyLib.Harmony harmony;

	private static bool hasInitialized;

	private static bool hasInited;

	private static bool isUsingMelon;

	private TimerCoroutine corUpdate;

	public static bool IsRecreatingRewardsNeeded;

	public static bool IsCacheBuilt;

	public static bool IsCacheBeingBuilt;

	private static bool HasCreatedPanels;

	private bool InitOnceWaitingUILog;

	private bool InitOnceLoginUILog;

	private bool InitOnceModMainUILog;

	private bool InitOnceFPS;

	public static bool IsInCombat;

	public bool DeveloperMode;

	public static bool StaticDeveloperMode = true;

	public static bool StaticMainLog = true;

	private static readonly float AddTipDuration = 2f;

	private int frameCount;

	private float elapsedTime;

	private float fps;

	private Text FPSTextComp;

	public static bool ShortcutKey;

	public static readonly string ModSteamID = "3452099943";

	public ModMain()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~ModMain()
	{
		UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
	}

	public override void OnApplicationStart()
	{
		base.OnApplicationStart();
	}

	public override void OnApplicationLateStart()
	{
		base.OnApplicationLateStart();
		Init();
	}

	public void Init()
	{
		if (hasInited)
		{
			return;
		}
		Instance = this;
		hasInited = true;
		Log("Initializing...");
		try
		{
			if (harmony != null)
			{
				harmony.UnpatchSelf();
				harmony = null;
			}
			if (harmony == null)
			{
				harmony = new HarmonyLib.Harmony("MOD_Mivopx");
			}
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			try
			{
				corUpdate = g.timer.Frame((Action)OnUpdateMod, 1, loop: true);
				Log("Initialized via game modding");
			}
			catch
			{
				Log("Initialized via melon loader");
				isUsingMelon = true;
			}
			LocalizationHelper.Initialize();
			UIRefreshManager.Initialize();
			Log("Localization system initialized");
			Universe.Init(1f, LateInit, Log, new UniverseLibConfig
			{
				Disable_EventSystem_Override = false,
				Force_Unlock_Mouse = true,
				Unhollowed_Modules_Folder = Path.Combine("MelonLoader", "Managed")
			});
			if (!DeveloperMode)
			{
				MelonLogger.Warning("Developer mode not enabled.");
				Log("Initialized!");
			}
			else
			{
				Log("Developer mode enabled.");
				Log("Initialized!");
			}
		}
		catch (Exception ex)
		{
			LogError(ex.Message + ":" + ex.StackTrace);
		}
	}

	private static void LateInit()
	{
		UIManager.CreateMain();
		hasInitialized = true;
		Log("Late init!");
	}

	public void Destroy()
	{
		Log("Destroying Cache and flipping booleans for re-initiation.");
		harmony.UnpatchSelf();
		harmony = null;
		g.timer.Stop(corUpdate);
		Cache.Destroy();
		IsCacheBuilt = false;
		IsCacheBeingBuilt = false;
		Game.Destroy();
		hasInited = false;
		hasInitialized = false;
		InitOnceFPS = false;
		FPSTextComp = null;
		InitOnceWaitingUILog = false;
		InitOnceLoginUILog = false;
		InitOnceModMainUILog = false;
	}

	public static void UpdateGameState()
	{
		if (!Game.HasChanged())
		{
			return;
		}
		if (Game.WorldManager.Value == null)
		{
			Log("Update Game State - WorldManager no value");
		}
		else
		{
			Log("Update Game State - WorldManager present");
		}
		try
		{
			State.Load();
			UIManager.OnGameWorldUpdate();
		}
		catch (Exception ex)
		{
			Log("Error updating game state: " + ex.Message + ":" + ex.StackTrace);
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (isUsingMelon && hasInitialized)
		{
			OnUpdateMod();
		}
	}

	private void OnUpdateMod()
	{
		if (Game.ConfMgr == null)
		{
			LogWarning("No ConfMgr found.");
			return;
		}
		if (!IsCacheBuilt && !IsCacheBeingBuilt)
		{
			IsCacheBeingBuilt = true;
			new Thread((Action)delegate
			{
				try
				{
					Cache.Build();
					IsCacheBuilt = true;
					IsCacheBeingBuilt = false;
				}
				catch (Exception ex)
				{
					LogError("Error building cache: " + ex.Message);
					g.timer.Stop(corUpdate);
					Log("The corUpdate timer has been stopped!!");
				}
			}).Start();
		}
		if (!Cache.IsBuilt)
		{
			Log("Waiting for cache...");
			return;
		}
		if (g.ui.GetUI(UIType.Login) != null && !InitOnceLoginUILog)
		{
			LogTip(LocalizationHelper.T("other_msgs_waiting_player_join"), null, 5f);
			InitOnceLoginUILog = true;
		}
		if (IsModMainUIActive() && !InitOnceModMainUILog)
		{
			LogTip(LocalizationHelper.T("other_msgs_player_entered_workshop"), "WARNING", 5f);
			LogTip(LocalizationHelper.T("other_msgs_restart_advised"), "WARNING", 6f);
			InitOnceModMainUILog = true;
		}
		if (!UIManager.UIRoot)
		{
			if (!InitOnceWaitingUILog)
			{
				Log("Preparing to build the cheat menu ui...");
				Log("This might take a moment...");
				InitOnceWaitingUILog = true;
			}
			return;
		}
		if (hasInitialized && !HasCreatedPanels)
		{
			UIManager.Init();
			UIManager.OnLoad();
			HasCreatedPanels = true;
		}
		HandleToggleKeys();
		HandleRewardsRecreation();
		UpdateGameState();
		if (hasInitialized && HasCreatedPanels && ShortcutKey)
		{
			HandleShortcutKeys();
		}
		UpdateFPSDisplay();
	}

	public static void Log(string message)
	{
		if (StaticDeveloperMode)
		{
			if (StaticMainLog)
			{
				Log(message, LogType.Log);
				MelonLogger.Msg(message);
			}
			if (hasInitialized && HasCreatedPanels && UIManager.Panels[PanelType.Debug].IsVisible)
			{
				DebugPanel.Instance?.LogMessage($"[✓] System[{DebugPanel.Instance.GetLogsCount()}]: {message}");
			}
		}
	}

	public static void LogWarning(string message)
	{
		if (StaticMainLog)
		{
			Log(message, LogType.Warning);
			MelonLogger.Warning(message);
		}
		if (hasInitialized && HasCreatedPanels && UIManager.Panels[PanelType.Debug].IsVisible)
		{
			DebugPanel.Instance?.LogMessage($"[ ! ] System[{DebugPanel.Instance.GetLogsCount()}]: {message}", DebugPanel.CustomLogLevel.Warning);
		}
	}

	public static void LogError(string message)
	{
		if (StaticMainLog)
		{
			Log(message, LogType.Error);
			MelonLogger.Error(message);
		}
		if (hasInitialized && HasCreatedPanels && UIManager.Panels[PanelType.Debug].IsVisible)
		{
			DebugPanel.Instance?.LogMessage($"[X] System[{DebugPanel.Instance.GetLogsCount()}]: {message}", DebugPanel.CustomLogLevel.Error);
		}
	}

	private static void Log(string message, LogType level = LogType.Log)
	{
		string text = message?.ToString() ?? "";
		switch (level)
		{
		default:
			Debug.Log(text);
			break;
		case LogType.Warning:
			Debug.LogWarning(text);
			break;
		case LogType.Error:
		case LogType.Exception:
			Debug.LogError(text);
			break;
		}
	}

	public static string GText(string s)
	{
		if (!string.IsNullOrEmpty(GameTool.LS(s)))
		{
			return GameTool.LS(s);
		}
		return s;
	}

	public static void LogTip(string Msg, string LogType = "NORMAL", float? Duration = null)
	{
		float durTime = Duration ?? AddTipDuration;
		string text = ((LogType == "WARNING") ? "<color=yellow>[ ! ]</color> " : ((!(LogType == "ERROR")) ? "<color=green>[✓]</color> " : "<color=red>[X]</color> "));
		UITipItem.AddTip(text + Msg, durTime);
		if (StaticDeveloperMode)
		{
			switch (LogType)
			{
			case "WARNING":
				LogWarning(Msg);
				break;
			case "ERROR":
				LogError(Msg);
				break;
			default:
				Log(Msg);
				break;
			}
		}
	}

	private void HandleToggleKeys()
	{
		if (InputManager.GetKeyDown(KeyCode.F3) || InputManager.GetKeyDown(KeyCode.Backslash) || InputManager.GetKeyDown(KeyCode.Tab))
		{
			UIManager.IsRootVisible = !UIManager.IsRootVisible;
			LogTip(LocalizationHelper.T("other_msgs_cheat_menu_toggled"), null, 1f);
		}
	}

	private void HandleRewardsRecreation()
	{
		if (!IsRecreatingRewardsNeeded)
		{
			return;
		}
		try
		{
			Game.RewardFactory = new RewardFactory();
			Rewards.CreateRewards();
			IsRecreatingRewardsNeeded = false;
		}
		catch (Exception ex)
		{
			Log("Error creating rewards: " + ex.Message + "\n" + ex.StackTrace);
		}
	}

	private bool IsModMainUIActive()
	{
		if (!(g.ui.GetUI(UIType.ModMain) != null) && !(g.ui.GetUI(UIType.ModMainFunction) != null) && !(g.ui.GetUI(UIType.ModMainMgr) != null) && !(g.ui.GetUI(UIType.ModMainProject) != null))
		{
			return g.ui.GetUI(UIType.ModMainShop) != null;
		}
		return true;
	}

	private void HandleShortcutKeys()
	{
		if (!Input.GetKey(KeyCode.LeftControl))
		{
			return;
		}
		(KeyCode, string, Action)[] array = new(KeyCode, string, Action)[5]
		{
			(KeyCode.Alpha1, "Ctrl+1", OtherPanel.UpdateTownMarket),
			(KeyCode.Alpha2, "Ctrl+2", OtherPanel.ForceRest),
			(KeyCode.Alpha3, "Ctrl+3", OtherPanel.TPToDest),
			(KeyCode.Alpha4, "Ctrl+4", OtherPanelMore.IncreaseView),
			(KeyCode.Alpha5, "Ctrl+5", OtherPanelMore.ResetView)
		};
		for (int i = 0; i < array.Length; i++)
		{
			var (key, arg, action) = array[i];
			if (Input.GetKeyDown(key))
			{
				if (IsInCombat)
				{
					LogTip(LocalizationHelper.T("other_msgs_action_unavailable_combat"), null, 0.1f);
					return;
				}
				LogTip(string.Format(LocalizationHelper.T("other_msgs_shortcut_key_pressed"), arg), null, 0.1f);
				action();
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			if (IsInCombat)
			{
				LogTip(LocalizationHelper.T("other_msgs_action_unavailable_combat"), null, 0.1f);
				return;
			}
			LogTip(string.Format(LocalizationHelper.T("other_msgs_shortcut_key_pressed"), "Ctrl+6"), null, 0.1f);
			HandleDramaEvent();
		}
	}

	private void HandleDramaEvent()
	{
		if (DramaTool.OpenDrama(927081281, new DramaData
		{
			unitLeft = Game.WorldManager.Value.playerUnit,
			unitRight = null
		}))
		{
			LogTip(LocalizationHelper.T("other_msgs_cheat_mod_event_triggered"), null, 3f);
		}
		else
		{
			LogTip(LocalizationHelper.T("other_msgs_failed_open_drama_event"), "ERROR", 5f);
		}
	}

	private void UpdateFPSDisplay()
	{
		if (!(MainPanel.mainPanelItem == null))
		{
			frameCount++;
			elapsedTime += Time.deltaTime;
			if (elapsedTime > 1f)
			{
				fps = (float)frameCount / elapsedTime;
				frameCount = 0;
				elapsedTime = 0f;
			}
			if (!InitOnceFPS)
			{
				FPSTextComp = MainPanel.mainPanelItem.GetComponentInChildren<Text>();
				InitOnceFPS = true;
			}
			if (FPSTextComp != null)
			{
				FPSTextComp.resizeTextForBestFit = true;
				FPSTextComp.resizeTextMinSize = 8;
				FPSTextComp.resizeTextMaxSize = 16;
				FPSTextComp.horizontalOverflow = HorizontalWrapMode.Wrap;
				FPSTextComp.verticalOverflow = VerticalWrapMode.Truncate;
				FPSTextComp.text = string.Format("<b><color=#4cd43d>{0}</color></b> <i><color=#ff3030>v{1}</color></i> <b><color=#00FFFF>{2} (FPS: {3})</color></b>", LocalizationHelper.T("panel_mainmenu_title"), "1.1.8 Pub", "v1.2.111.259+", Mathf.RoundToInt(fps));
			}
		}
	}

	private void UpdateUITexts()
	{
		if (FPSTextComp != null)
		{
			FPSTextComp.text = string.Format("<b><color=#4cd43d>{0}</color></b> <i><color=#ff3030>v{1}</color></i> <b><color=#00FFFF>{2} (FPS: {3})</color></b>", LocalizationHelper.T("panel_mainmenu_title"), "1.1.8 Pub", "v1.2.111.259+", Mathf.RoundToInt(fps));
		}
	}
}
