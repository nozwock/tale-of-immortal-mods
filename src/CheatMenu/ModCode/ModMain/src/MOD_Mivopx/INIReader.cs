using System.Collections.Generic;
using System.IO;
using MelonLoader;
using TaleOfImmortalCheat.Localization;
using TaleOfImmortalCheat.UI;
using TaleOfImmortalCheat.Utilities;

namespace MOD_Mivopx;

public class INIReader
{
	private List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();

	static INIReader()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	public INIReader(string path)
	{
		string text = FileSearchUtility.FindFile(Path.GetFileName(path), "INI");
		if (text == null)
		{
			ModMain.LogTip(string.Format(LocalizationHelper.T("other_msgs_file_not_found"), Path.GetFileName(path)), "ERROR", 20f);
			ModMain.LogTip(string.Format(LocalizationHelper.T("other_msgs_put_file_at"), path), "ERROR", 21f);
			ModMain.LogTip(LocalizationHelper.T("other_msgs_search_skills_not_work"), "ERROR", 22f);
			MelonLogger.Error("File not found: " + Path.GetFileName(path));
			MelonLogger.Error("Put the required file at: " + path);
			MelonLogger.Error("Otherwise the Search Skills will not work!");
			throw new FileNotFoundException("File not found: " + path);
		}
		Dictionary<string, string> dictionary = null;
		foreach (string item in File.ReadLines(text))
		{
			string text2 = item.Trim();
			if (string.IsNullOrEmpty(text2) || text2.StartsWith("//"))
			{
				continue;
			}
			if (text2.StartsWith("1="))
			{
				if (dictionary != null)
				{
					data.Add(dictionary);
				}
				dictionary = new Dictionary<string, string>();
			}
			string[] array = text2.Split('=', '\u0002');
			if (array.Length == 2 && dictionary != null)
			{
				string key = array[0].Trim();
				string value = array[1].Trim();
				dictionary[key] = value;
			}
		}
		if (dictionary != null)
		{
			data.Add(dictionary);
		}
	}

	public List<Dictionary<string, string>> GetAllSections()
	{
		return data;
	}

	private static void UpdateUITexts()
	{
	}
}
