using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using TaleOfImmortalCheat;
using TaleOfImmortalCheat.UI;
using UnityEngine.UI;

namespace MOD_Mivopx;

public static class Cache
{
	private const int NATURE_DESTINY_TYPE = 1;

	public static bool IsBuilt { get; private set; }

	public static System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>> CachedPrefixes { get; private set; }

	public static System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<int, int>> CachedPrefixesPositions { get; private set; }

	public static System.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>> PrefixesOptionsDataCache { get; private set; }

	public static System.Collections.Generic.List<int?> DestiniesOptionsCache { get; private set; }

	public static Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> DestiniesOptionsDataCache { get; private set; }

	public static System.Collections.Generic.Dictionary<int, int> DestiniesOptionsPositionCache { get; private set; }

	static Cache()
	{
		CachedPrefixes = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>>();
		CachedPrefixesPositions = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<int, int>>();
		PrefixesOptionsDataCache = new System.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>>();
		DestiniesOptionsCache = new System.Collections.Generic.List<int?>();
		DestiniesOptionsDataCache = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
		DestiniesOptionsPositionCache = new System.Collections.Generic.Dictionary<int, int>();
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	public static void Build()
	{
		if (IsBuilt)
		{
			return;
		}
		try
		{
			ModMain.Log("Building skill prefix cache...");
			BuildPrefixCache();
			ModMain.Log("Built skill prefix cache.");
			ModMain.Log("Building destinies cache...");
			BuildDestiniesCache();
			ModMain.Log("Built destinies cache.");
			IsBuilt = true;
		}
		catch (Exception ex)
		{
			ModMain.LogError("Failed to build cache: " + ex.Message);
			Destroy();
			throw;
		}
	}

	public static void Destroy()
	{
		CachedPrefixes?.Clear();
		CachedPrefixesPositions?.Clear();
		PrefixesOptionsDataCache?.Clear();
		DestiniesOptionsCache?.Clear();
		DestiniesOptionsDataCache?.Clear();
		DestiniesOptionsPositionCache?.Clear();
		CachedPrefixes = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>>();
		CachedPrefixesPositions = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<int, int>>();
		PrefixesOptionsDataCache = new System.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>>();
		DestiniesOptionsCache = new System.Collections.Generic.List<int?>();
		DestiniesOptionsDataCache = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
		DestiniesOptionsPositionCache = new System.Collections.Generic.Dictionary<int, int>();
		IsBuilt = false;
	}

	private static void BuildPrefixCache()
	{
		Il2CppSystem.Collections.Generic.List<ConfBattleSkillPrefixValueItem> allConfList = Game.ConfMgr.battleSkillPrefixValue._allConfList;
		if (allConfList == null || allConfList.Count == 0)
		{
			ModMain.LogWarning("No battle skill prefix data available");
			return;
		}
		System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> list = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>();
		System.Collections.Generic.Dictionary<int, System.Collections.Generic.HashSet<int>> skillTypeTracker = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.HashSet<int>>();
		System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> list2 = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>();
		Il2CppSystem.Collections.Generic.List<ConfBattleSkillPrefixValueItem>.Enumerator enumerator = allConfList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ConfBattleSkillPrefixValueItem current = enumerator.Current;
			if (Game.ConfMgr.localText.allText.ContainsKey(current.desc))
			{
				list2.Add(current);
			}
			else
			{
				ModMain.Log($"Missing translation for prefix desc: {current.desc} (number: {current.number})");
			}
		}
		if (list2.Count == 0)
		{
			ModMain.LogWarning("No valid translated prefix items found");
			return;
		}
		System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>> dictionary = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>>();
		foreach (ConfBattleSkillPrefixValueItem item in list2)
		{
			if (!dictionary.ContainsKey(item.skillType))
			{
				dictionary[item.skillType] = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>();
			}
			dictionary[item.skillType].Add(item);
		}
		foreach (System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>> item2 in dictionary)
		{
			_ = item2.Key;
			foreach (ConfBattleSkillPrefixValueItem item3 in item2.Value)
			{
				foreach (int item4 in item3.skillID)
				{
					if (item4 == 0)
					{
						list.Add(item3);
					}
					else
					{
						ProcessPrefixItem(item3, item4, skillTypeTracker);
					}
				}
			}
		}
		ProcessZeroSkillItems(list, skillTypeTracker);
		ModMain.Log($"Built prefix cache for {CachedPrefixes.Count} skill combinations");
	}

	private static void ProcessPrefixItem(ConfBattleSkillPrefixValueItem item, int skillId, System.Collections.Generic.Dictionary<int, System.Collections.Generic.HashSet<int>> skillTypeTracker)
	{
		string key = $"{item.skillType}_{skillId}";
		string description = Description.For(item.desc);
		if (!skillTypeTracker.TryGetValue(item.skillType, out var value))
		{
			value = new System.Collections.Generic.HashSet<int>();
			skillTypeTracker[item.skillType] = value;
		}
		value.Add(skillId);
		AddToCaches(key, item, description);
	}

	private static void ProcessZeroSkillItems(System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> zeroSkillItems, System.Collections.Generic.Dictionary<int, System.Collections.Generic.HashSet<int>> skillTypeTracker)
	{
		foreach (ConfBattleSkillPrefixValueItem zeroSkillItem in zeroSkillItems)
		{
			if (!skillTypeTracker.TryGetValue(zeroSkillItem.skillType, out var value))
			{
				continue;
			}
			string description = Description.For(zeroSkillItem.desc);
			foreach (int item in value)
			{
				AddToCaches($"{zeroSkillItem.skillType}_{item}", zeroSkillItem, description);
			}
		}
	}

	private static void AddToCaches(string key, ConfBattleSkillPrefixValueItem item, string description)
	{
		Dropdown.OptionData item2 = new Dropdown.OptionData
		{
			text = description
		};
		if (CachedPrefixes.TryGetValue(key, out var value))
		{
			if (!CachedPrefixesPositions[key].ContainsKey(item.number))
			{
				value.Add(item);
				CachedPrefixesPositions[key][item.number] = value.Count - 1;
				PrefixesOptionsDataCache[key].Add(item2);
			}
		}
		else
		{
			CachedPrefixes[key] = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> { item };
			CachedPrefixesPositions[key] = new System.Collections.Generic.Dictionary<int, int> { [item.number] = 0 };
			Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> list = new Il2CppSystem.Collections.Generic.List<Dropdown.OptionData>();
			list.Add(item2);
			PrefixesOptionsDataCache[key] = list;
		}
	}

	public static void RebuildCacheForSpecificSkill(int skillType, int skillId)
	{
		string text = $"{skillType}_{skillId}";
		ModMain.Log("Rebuilding cache specifically for key: " + text);
		if (CachedPrefixes.ContainsKey(text))
		{
			CachedPrefixes.Remove(text);
			CachedPrefixesPositions.Remove(text);
			PrefixesOptionsDataCache.Remove(text);
		}
		Il2CppSystem.Collections.Generic.List<ConfBattleSkillPrefixValueItem> allConfList = Game.ConfMgr.battleSkillPrefixValue._allConfList;
		if (allConfList == null || allConfList.Count == 0)
		{
			ModMain.LogWarning("No battle skill prefix data available for specific rebuild");
			return;
		}
		System.Collections.Generic.List<ConfBattleSkillPrefixValueItem> list = new System.Collections.Generic.List<ConfBattleSkillPrefixValueItem>();
		Il2CppSystem.Collections.Generic.List<ConfBattleSkillPrefixValueItem>.Enumerator enumerator = allConfList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ConfBattleSkillPrefixValueItem current = enumerator.Current;
			if (current.skillType != skillType || !Game.ConfMgr.localText.allText.ContainsKey(current.desc))
			{
				continue;
			}
			bool flag = false;
			foreach (int item in current.skillID)
			{
				if (item == skillId || item == 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				list.Add(current);
			}
		}
		ModMain.Log($"Found {list.Count} applicable prefix items for {text}");
		list.Sort((ConfBattleSkillPrefixValueItem a, ConfBattleSkillPrefixValueItem b) => a.number.CompareTo(b.number));
		foreach (ConfBattleSkillPrefixValueItem item2 in list)
		{
			string text2 = Description.For(item2.desc);
			AddToCaches(text, item2, text2);
			ModMain.Log($"Added prefix {item2.number}: {text2}");
		}
		ModMain.Log($"Completed rebuild for {text} with {list.Count} items");
	}

	private static void BuildDestiniesCache()
	{
		Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem> allConfList = Game.ConfMgr.roleCreateFeature._allConfList;
		if (allConfList == null || allConfList.Count == 0)
		{
			ModMain.LogWarning("No role create feature data available");
			return;
		}
		DestiniesOptionsCache.Add(null);
		DestiniesOptionsDataCache.Add(new Dropdown.OptionData
		{
			text = "N/A"
		});
		DestiniesOptionsPositionCache[-1] = 0;
		System.Collections.Generic.List<ConfRoleCreateFeatureItem> list = new System.Collections.Generic.List<ConfRoleCreateFeatureItem>();
		Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem>.Enumerator enumerator = allConfList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ConfRoleCreateFeatureItem current = enumerator.Current;
			if (current.type == 1)
			{
				list.Add(current);
			}
		}
		foreach (ConfRoleCreateFeatureItem item in list)
		{
			DestiniesOptionsCache.Add(item.id);
			DestiniesOptionsDataCache.Add(new Dropdown.OptionData
			{
				text = ModMain.GText(item.name)
			});
			DestiniesOptionsPositionCache[item.id] = DestiniesOptionsDataCache.Count - 1;
		}
		ModMain.Log($"Loaded {list.Count} nature destinies");
	}

	private static void UpdateUITexts()
	{
	}
}
