using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MOD_Mivopx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaleOfImmortalCheat.Utilities;

namespace TaleOfImmortalCheat.Localization;

public static class LocalizationParser
{
	private const string DEFAULT_LANGUAGE = "en";

	public static List<LocalizationEntry> LoadFromFile(string filePath)
	{
		try
		{
			string text = FileSearchUtility.FindFile(filePath, "Localization");
			if (text == null)
			{
				ModMain.LogWarning("Localization file not found: " + filePath + ". File search completed with no results.");
				ModMain.LogTip(filePath + " not found in any location. Make sure you have AquaLocalText.json in your game folder.", "ERROR", 20f);
				return new List<LocalizationEntry>();
			}
			return ParseJsonContent(File.ReadAllText(text));
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error loading localization file: " + ex.Message);
			return new List<LocalizationEntry>();
		}
	}

	public static List<LocalizationEntry> ParseJsonContent(string jsonContent)
	{
		try
		{
			List<LocalizationEntry> list = new List<LocalizationEntry>();
			JsonLoadSettings settings = new JsonLoadSettings
			{
				CommentHandling = CommentHandling.Ignore
			};
			foreach (JObject item in JArray.Parse(jsonContent, settings).Cast<JObject>())
			{
				LocalizationEntry localizationEntry = new LocalizationEntry
				{
					Id = (item["id"]?.ToString() ?? string.Empty),
					Key = (item["key"]?.ToString() ?? string.Empty)
				};
				foreach (JProperty item2 in item.Properties())
				{
					if (item2.Name != "id" && item2.Name != "key")
					{
						localizationEntry.SetTranslation(item2.Name, item2.Value.ToString());
					}
				}
				list.Add(localizationEntry);
			}
			ModMain.Log($"Successfully parsed {list.Count} localization entries");
			return list;
		}
		catch (JsonException ex)
		{
			ModMain.LogError("Error parsing localization JSON: " + ex.Message);
			return new List<LocalizationEntry>();
		}
	}

	public static List<string> ExtractLanguageCodes(List<LocalizationEntry> entries)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (LocalizationEntry entry in entries)
		{
			foreach (string key in entry.Translations.Keys)
			{
				hashSet.Add(key);
			}
		}
		List<string> list = hashSet.ToList();
		if (list.Contains("en"))
		{
			list.Remove("en");
			list.Insert(0, "en");
		}
		return list;
	}
}
