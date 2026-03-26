using System;
using MOD_Mivopx;
using TaleOfImmortalCheat.Localization;

namespace TaleOfImmortalCheat.UI;

public static class UIRefreshManager
{
	private static bool _isInitialized;

	public static event Action OnLanguageChanged;

	public static void Initialize()
	{
		if (!_isInitialized)
		{
			_isInitialized = true;
			ModMain.Log("UIRefreshManager initialized");
		}
	}

	public static void NotifyLanguageChanged()
	{
		ModMain.Log("Language changed to: " + LocalizationHelper.GetCurrentLanguage());
		UIRefreshManager.OnLanguageChanged?.Invoke();
	}

	public static bool ChangeLanguage(string languageCode)
	{
		if (LocalizationHelper.SetLanguage(languageCode))
		{
			NotifyLanguageChanged();
			return true;
		}
		return false;
	}
}
