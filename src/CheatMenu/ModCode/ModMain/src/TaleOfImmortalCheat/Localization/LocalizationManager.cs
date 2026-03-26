using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MOD_Mivopx;

namespace TaleOfImmortalCheat.Localization;

public class LocalizationManager
{
	private const string DEFAULT_LANGUAGE = "en";

	private const string LOCALIZATION_FILE = "AquaLocalText.json";

	private const string INI_SECTION = "Localization";

	private const string INI_LANGUAGE_KEY = "CurrentLanguage";

	private static LocalizationManager _instance;

	private List<LocalizationEntry> _entries;

	private List<string> _availableLanguages;

	private string _currentLanguage;

	private bool _isInitialized;

	public static LocalizationManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LocalizationManager();
			}
			return _instance;
		}
	}

	private LocalizationManager()
	{
		_entries = new List<LocalizationEntry>();
		_availableLanguages = new List<string>();
		_currentLanguage = "en";
		_isInitialized = false;
	}

	public void Initialize()
	{
		if (_isInitialized)
		{
			return;
		}
		try
		{
			_entries = LocalizationParser.LoadFromFile("AquaLocalText.json");
			_availableLanguages = LocalizationParser.ExtractLanguageCodes(_entries);
			LoadLanguagePreference();
			_isInitialized = true;
			ModMain.Log($"Localization system initialized with {_entries.Count} entries and {_availableLanguages.Count} languages");
			ModMain.Log("Current language set to: " + _currentLanguage);
		}
		catch (Exception ex)
		{
			ModMain.LogError("Failed to initialize localization system: " + ex.Message);
			_entries = new List<LocalizationEntry>();
			_availableLanguages = new List<string> { "en" };
			_currentLanguage = "en";
		}
	}

	public string GetText(string key)
	{
		return GetText(key, _currentLanguage);
	}

	public string GetText(string key, string languageCode)
	{
		if (string.IsNullOrEmpty(key))
		{
			return string.Empty;
		}
		if (!_isInitialized)
		{
			ModMain.Log("Localization system not initialized, initializing now for key: " + key);
			Initialize();
		}
		LocalizationEntry localizationEntry = _entries.FirstOrDefault((LocalizationEntry e) => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
		if (localizationEntry == null)
		{
			ModMain.LogWarning("Localization key not found: " + key);
			return key;
		}
		string translation = localizationEntry.GetTranslation(languageCode);
		if (string.IsNullOrEmpty(translation) && languageCode != "en")
		{
			ModMain.Log("Translation not found for " + languageCode + ", trying English fallback");
			translation = localizationEntry.GetTranslation("en");
			if (string.IsNullOrEmpty(translation))
			{
				ModMain.Log("English fallback also missing for key: " + key);
				return key;
			}
		}
		return translation ?? key;
	}

	public bool SetLanguage(string languageCode)
	{
		if (string.IsNullOrEmpty(languageCode))
		{
			return false;
		}
		if (!_isInitialized)
		{
			Initialize();
		}
		if (!_availableLanguages.Contains(languageCode))
		{
			ModMain.LogWarning("Language not available: " + languageCode);
			return false;
		}
		_currentLanguage = languageCode;
		SaveLanguagePreference();
		ModMain.Log("Language set to: " + languageCode);
		return true;
	}

	public List<string> GetAvailableLanguages()
	{
		if (!_isInitialized)
		{
			Initialize();
		}
		return _availableLanguages;
	}

	public string GetCurrentLanguage()
	{
		return _currentLanguage;
	}

	public void ReloadLocalization()
	{
		string currentLanguage = _currentLanguage;
		_entries = LocalizationParser.LoadFromFile("AquaLocalText.json");
		_availableLanguages = LocalizationParser.ExtractLanguageCodes(_entries);
		if (_availableLanguages.Contains(currentLanguage))
		{
			_currentLanguage = currentLanguage;
		}
		else
		{
			_currentLanguage = (_availableLanguages.Contains("en") ? "en" : (_availableLanguages.FirstOrDefault() ?? "en"));
		}
		ModMain.Log($"Localization reloaded with {_entries.Count} entries and {_availableLanguages.Count} languages");
		ModMain.Log("Current language set to: " + _currentLanguage);
	}

	private void LoadLanguagePreference()
	{
		try
		{
			string path = Path.Combine("Mods", "TaleOfImmortalCheat", "ModPrefs.ini");
			if (!File.Exists(path))
			{
				return;
			}
			string[] array = File.ReadAllLines(path);
			bool flag = false;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i].Trim();
				if (text.StartsWith("[") && text.EndsWith("]"))
				{
					flag = text.Substring(1, text.Length - 2).Equals("Localization", StringComparison.OrdinalIgnoreCase);
				}
				else
				{
					if (!flag || !text.StartsWith("CurrentLanguage="))
					{
						continue;
					}
					string[] array3 = text.Split('=');
					if (array3.Length == 2)
					{
						string text2 = array3[1].Trim();
						if (_availableLanguages.Contains(text2))
						{
							_currentLanguage = text2;
							ModMain.Log("Loaded language preference from INI: " + text2);
						}
					}
					break;
				}
			}
		}
		catch (Exception ex)
		{
			ModMain.LogWarning("Failed to load language preference: " + ex.Message);
		}
	}

	private void SaveLanguagePreference()
	{
		try
		{
			string text = Path.Combine("Mods", "TaleOfImmortalCheat");
			string path = Path.Combine(text, "ModPrefs.ini");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			List<string> list = new List<string>();
			bool flag = false;
			bool flag2 = false;
			if (File.Exists(path))
			{
				list = File.ReadAllLines(path).ToList();
				bool flag3 = false;
				for (int i = 0; i < list.Count; i++)
				{
					string text2 = list[i].Trim();
					if (text2.StartsWith("[") && text2.EndsWith("]"))
					{
						flag3 = text2.Substring(1, text2.Length - 2).Equals("Localization", StringComparison.OrdinalIgnoreCase);
						if (flag3)
						{
							flag = true;
						}
					}
					else if (flag3 && text2.StartsWith("CurrentLanguage="))
					{
						list[i] = "CurrentLanguage=" + _currentLanguage;
						flag2 = true;
						break;
					}
				}
			}
			if (!flag)
			{
				list.Add("");
				list.Add("[Localization]");
			}
			if (!flag2)
			{
				int index = list.Count;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].Trim().Equals("[Localization]"))
					{
						index = j + 1;
						break;
					}
				}
				list.Insert(index, "CurrentLanguage=" + _currentLanguage);
			}
			File.WriteAllLines(path, list);
			ModMain.Log("Saved language preference to INI: " + _currentLanguage);
		}
		catch (Exception ex)
		{
			ModMain.LogWarning("Failed to save language preference: " + ex.Message);
		}
	}
}
