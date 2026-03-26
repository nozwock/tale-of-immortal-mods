using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MOD_Mivopx;
using TaleOfImmortalCheat.Utilities;

namespace TaleOfImmortalCheat.Localization;

public static class LocalizationHelper
{
	private static FileSystemWatcher _fileWatcher;

	public static void Initialize()
	{
		ModMain.Log("Initializing LocalizationHelper...");
		LocalizationManager.Instance.Initialize();
		SetupFileWatcher();
		ModMain.Log("LocalizationHelper initialized. Current language: " + GetCurrentLanguage());
		ModMain.Log("Available languages: " + string.Join(", ", GetAvailableLanguages()));
		ModMain.Log("Test translation for 'common_cheatpanel': " + GetText("common_cheatpanel"));
	}

	public static string T(string key)
	{
		return GetText(key);
	}

	public static string GetText(string key)
	{
		return LocalizationManager.Instance.GetText(key);
	}

	public static string GetText(string key, string languageCode)
	{
		return LocalizationManager.Instance.GetText(key, languageCode);
	}

	public static bool SetLanguage(string languageCode)
	{
		return LocalizationManager.Instance.SetLanguage(languageCode);
	}

	public static List<string> GetAvailableLanguages()
	{
		return LocalizationManager.Instance.GetAvailableLanguages();
	}

	public static string GetCurrentLanguage()
	{
		return LocalizationManager.Instance.GetCurrentLanguage();
	}

	private static void SetupFileWatcher()
	{
		try
		{
			if (_fileWatcher != null)
			{
				_fileWatcher.EnableRaisingEvents = false;
				_fileWatcher.Dispose();
				_fileWatcher = null;
			}
			string text = FileSearchUtility.FindFile("AquaLocalText.json", "Localization");
			if (text == null)
			{
				ModMain.LogWarning("Could not find AquaLocalText.json for file watching");
				return;
			}
			string directoryName = Path.GetDirectoryName(text);
			string fileName = Path.GetFileName(text);
			_fileWatcher = new FileSystemWatcher(directoryName)
			{
				Filter = fileName,
				NotifyFilter = (NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size),
				EnableRaisingEvents = true
			};
			_fileWatcher.Changed += OnLocalizationFileChanged;
			_fileWatcher.Created += OnLocalizationFileChanged;
			ModMain.Log("File watcher set up for " + text);
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error setting up file watcher: " + ex.Message);
		}
	}

	private static void OnLocalizationFileChanged(object sender, FileSystemEventArgs e)
	{
		try
		{
			Thread.Sleep(100);
			LocalizationManager.Instance.ReloadLocalization();
			ModMain.Log("Localization file changed, reloaded localization data");
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error handling localization file change: " + ex.Message);
		}
	}
}
