using System.Text.RegularExpressions;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

public static class Description
{
	public static Regex valuesRegex;

	public static Regex effectsRegex;

	static Description()
	{
		valuesRegex = new Regex("\\&(.*?)\\&", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		effectsRegex = new Regex("\\$(.*?)\\$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	public static string For(string key, bool debug = false)
	{
		if (!Game.ConfMgr.localText.allText.ContainsKey(key))
		{
			return "<NO TRANSLATION FOUND>";
		}
		ConfLocalTextItem confLocalTextItem = Game.ConfMgr.localText.allText[key];
		return ReplaceEffects(ReplaceValues(string.IsNullOrEmpty(confLocalTextItem.en) ? confLocalTextItem.ch : confLocalTextItem.en, debug), debug);
	}

	private static string ReplaceValues(string text, bool debug = false)
	{
		foreach (Match item in valuesRegex.Matches(text))
		{
			if (debug)
			{
				ModMain.Log($"ReplaceValues - Found {item.Groups.Count} groups for match {item.Value}.");
			}
			text = text.Replace(item.Value, "X");
		}
		return text;
	}

	private static string ReplaceEffects(string text, bool debug = false)
	{
		foreach (Match item in effectsRegex.Matches(text))
		{
			if (debug)
			{
				ModMain.Log($"ReplaceEffects Found {item.Groups.Count} groups for match {item.Value}.");
			}
			string text2 = item.Value.Replace("$", "");
			if (!Game.ConfMgr.localText.allText.ContainsKey(text2))
			{
				ModMain.LogWarning("Could not replace effect description for " + text2);
				continue;
			}
			ConfLocalTextItem confLocalTextItem = Game.ConfMgr.localText.allText[text2];
			text = text.Replace(item.Value, string.IsNullOrEmpty(confLocalTextItem.en) ? confLocalTextItem.ch : confLocalTextItem.en);
		}
		return text;
	}

	private static void UpdateUITexts()
	{
	}
}
