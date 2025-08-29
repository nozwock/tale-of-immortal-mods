using System;
using System.Collections.Generic;

namespace TaleOfImmortalCheat.Localization;

[Serializable]
public class LocalizationEntry
{
	[NonSerialized]
	private Dictionary<string, string> _translations;

	public string Id { get; set; }

	public string Key { get; set; }

	public Dictionary<string, string> Translations
	{
		get
		{
			if (_translations == null)
			{
				_translations = new Dictionary<string, string>();
			}
			return _translations;
		}
		set
		{
			_translations = value;
		}
	}

	public LocalizationEntry()
	{
		Id = string.Empty;
		Key = string.Empty;
		_translations = new Dictionary<string, string>();
	}

	public LocalizationEntry(string id, string key)
	{
		Id = id;
		Key = key;
		_translations = new Dictionary<string, string>();
	}

	public string GetTranslation(string languageCode)
	{
		if (string.IsNullOrEmpty(languageCode) || !Translations.ContainsKey(languageCode))
		{
			return null;
		}
		return Translations[languageCode];
	}

	public void SetTranslation(string languageCode, string text)
	{
		if (!string.IsNullOrEmpty(languageCode))
		{
			Translations[languageCode] = text;
		}
	}
}
