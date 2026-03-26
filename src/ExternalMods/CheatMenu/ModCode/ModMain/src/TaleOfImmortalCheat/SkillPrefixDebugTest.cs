using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;
using TaleOfImmortalCheat.UI;

namespace TaleOfImmortalCheat;

public static class SkillPrefixDebugTest
{
	static SkillPrefixDebugTest()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	public static void TestSkillPrefixCaching()
	{
		ModMain.Log("=== SKILL PREFIX DEBUG TEST ===");
		try
		{
			Il2CppSystem.Collections.Generic.List<ConfBattleSkillPrefixValueItem> allConfList = Game.ConfMgr.battleSkillPrefixValue._allConfList;
			if (allConfList == null || allConfList.Count == 0)
			{
				ModMain.LogError("No battle skill prefix data available");
				return;
			}
			ModMain.Log($"Total prefix items in config: {allConfList.Count}");
			System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>> dictionary = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>>();
			int num = 0;
			Il2CppSystem.Collections.Generic.List<ConfBattleSkillPrefixValueItem>.Enumerator enumerator = allConfList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ConfBattleSkillPrefixValueItem current = enumerator.Current;
				if (!dictionary.ContainsKey(current.skillType))
				{
					dictionary[current.skillType] = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>();
				}
				dictionary[current.skillType].Add(current);
				if (!Game.ConfMgr.localText.allText.ContainsKey(current.desc))
				{
					num++;
				}
			}
			ModMain.Log($"Items missing translations: {num}");
			ModMain.Log($"Skill types found: {dictionary.Count}");
			foreach (System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>> item in dictionary)
			{
				int key = item.Key;
				System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> value = item.Value;
				ModMain.Log($"Skill Type {key}: {value.Count} prefix items");
				System.Collections.Generic.Dictionary<int, int> dictionary2 = new System.Collections.Generic.Dictionary<int, int>();
				foreach (ConfBattleSkillPrefixValueItem item2 in value)
				{
					foreach (int item3 in item2.skillID)
					{
						if (!dictionary2.ContainsKey(item3))
						{
							dictionary2[item3] = 0;
						}
						dictionary2[item3]++;
					}
				}
				ModMain.Log(string.Format("  Skill IDs for type {0}: {1}", key, string.Join(", ", dictionary2.Keys)));
				int num2 = 0;
				foreach (ConfBattleSkillPrefixValueItem item4 in value)
				{
					if (num2 < 3)
					{
						if (Game.ConfMgr.localText.allText.ContainsKey(item4.desc))
						{
							string arg = Description.For(item4.desc);
							ModMain.Log(string.Format("    Example: {0} - {1} (Skills: {2})", item4.number, arg, string.Join(",", item4.skillID)));
							num2++;
						}
						continue;
					}
					break;
				}
			}
			ModMain.Log("=== END SKILL PREFIX DEBUG TEST ===");
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error in skill prefix debug test: " + ex.Message);
		}
	}

	public static void TestSpecificSkill(int skillType, int skillId)
	{
		ModMain.Log($"=== TESTING SPECIFIC SKILL: Type {skillType}, ID {skillId} ===");
		string text = $"{skillType}_{skillId}";
		if (Cache.CachedPrefixes.ContainsKey(text))
		{
			System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> list = Cache.CachedPrefixes[text];
			ModMain.Log($"Found {list.Count} cached prefixes for {text}");
			foreach (ConfBattleSkillPrefixValueItem item in list)
			{
				string arg = (Game.ConfMgr.localText.allText.ContainsKey(item.desc) ? Description.For(item.desc) : ("[NO TRANSLATION: " + item.desc + "]"));
				ModMain.Log($"  {item.number}: {arg}");
			}
		}
		else
		{
			ModMain.Log("No cached prefixes found for " + text);
			Cache.RebuildCacheForSpecificSkill(skillType, skillId);
			if (Cache.CachedPrefixes.ContainsKey(text))
			{
				System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> list2 = Cache.CachedPrefixes[text];
				ModMain.Log($"After rebuild: Found {list2.Count} prefixes for {text}");
			}
			else
			{
				ModMain.Log("Still no prefixes found after rebuild for " + text);
			}
		}
		ModMain.Log("=== END SPECIFIC SKILL TEST ===");
	}

	private static void UpdateUITexts()
	{
	}
}
