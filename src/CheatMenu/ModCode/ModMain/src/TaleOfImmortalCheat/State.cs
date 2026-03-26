using System.Collections.Generic;
using System.IO;
using System.Timers;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;
using Newtonsoft.Json;

namespace TaleOfImmortalCheat;

internal class State
{
	public class Npcs
	{
		public System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<int, int>> NPCsFates = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<int, int>>();
	}

	private const string DirName = "Mods\\TaleOfImmortalCheat";

	private static string FileName = "Mods\\TaleOfImmortalCheat\\{0}-cheat-mod.json";

	private static string NpcsFileName = "Mods\\TaleOfImmortalCheat\\{0}-npcs.json";

	public static System.Collections.Generic.Dictionary<int, ConfFateFeatureItem> GradeFates = new System.Collections.Generic.Dictionary<int, ConfFateFeatureItem>();

	public static Npcs NpcsState = new Npcs();

	public static string UnitConfFileName => string.Format(FileName, Game.WorldManager.Value.playerUnit.data.unitData.propertyData.GetName().Replace(" ", "_"));

	public static string NpcsConfFileName => string.Format(NpcsFileName, Game.WorldManager.Value.playerUnit.data.unitData.propertyData.GetName().Replace(" ", "_"));

	public static void Save()
	{
		if (Game.WorldManager.Value == null)
		{
			return;
		}
		System.Collections.Generic.Dictionary<int, int> dictionary = new System.Collections.Generic.Dictionary<int, int>();
		foreach (System.Collections.Generic.KeyValuePair<int, ConfFateFeatureItem> gradeFate in GradeFates)
		{
			dictionary.Add(gradeFate.Key, gradeFate.Value.id);
		}
		if (!Directory.Exists("Mods\\TaleOfImmortalCheat"))
		{
			Directory.CreateDirectory("Mods\\TaleOfImmortalCheat");
		}
		File.WriteAllText(UnitConfFileName, JsonConvert.SerializeObject(dictionary, Formatting.Indented));
		File.WriteAllText(NpcsConfFileName, JsonConvert.SerializeObject(NpcsState, Formatting.Indented));
		ModMain.Log("Saved state - " + UnitConfFileName + " + " + NpcsConfFileName);
	}

	public static void Load()
	{
		if (Game.WorldManager.Value == null || Game.WorldManager.Value.playerUnit?.data?.unitData?.propertyData == null)
		{
			ModMain.Log("Waiting for world manager - skipping state load.");
			return;
		}
		LoadPlayerUnit();
		LoadNpcs();
		ModMain.Log("Loaded state.");
	}

	public static void LoadPlayerUnit()
	{
		if (!File.Exists(UnitConfFileName))
		{
			ModMain.Log("No player unit state to load - skipping.");
			return;
		}
		foreach (System.Collections.Generic.KeyValuePair<int, int> item2 in JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<int, int>>(File.ReadAllText(UnitConfFileName)))
		{
			ConfFateFeatureItem item = Game.ConfMgr.fateFeature.GetItem(item2.Value);
			if (item != null)
			{
				GradeFates[item2.Key] = item;
			}
		}
		ModMain.Log("Loaded player unit state.");
	}

	public static void LoadNpcs()
	{
		if (!File.Exists(NpcsConfFileName))
		{
			ModMain.Log("No npcs state to load - skipping.");
			return;
		}
		NpcsState = JsonConvert.DeserializeObject<Npcs>(File.ReadAllText(NpcsConfFileName));
		foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.Dictionary<int, int>> nPCsFate in NpcsState.NPCsFates)
		{
			WorldUnitBase worldUnitBase = Game.WorldManager.Value.unit.allUnit[nPCsFate.Key];
			foreach (System.Collections.Generic.KeyValuePair<int, int> item in nPCsFate.Value)
			{
				int key = item.Key;
				int value = item.Value;
				Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData> dictionary = worldUnitBase?.data?.unitData?.npcUpGrade ?? new Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData>();
				if (dictionary.ContainsKey(key))
				{
					int luck = dictionary[key].luck;
					worldUnitBase.DestroyLuck(worldUnitBase.GetLuck(luck));
					worldUnitBase.CreateLuck(new DataUnit.LuckData
					{
						createTime = 0,
						duration = -1,
						id = value,
						objData = new DataObjectData()
					});
					ModMain.Log($"Changed fate[{luck}]({key}) -> {value}");
					dictionary[key].luck = value;
				}
				else
				{
					ModMain.Log($"New fate[{key}] -> {value}");
					worldUnitBase.CreateLuck(new DataUnit.LuckData
					{
						createTime = 0,
						duration = -1,
						id = value,
						objData = new DataObjectData()
					});
					dictionary[key] = new DataWorld.World.PlayerLogData.GradeData
					{
						luck = value,
						quality = 1
					};
				}
			}
		}
		ModMain.Log("Loaded npcsstate.");
	}

	public static void ChangeNpcLuck(string unitId, int grade, int luck)
	{
		if (!NpcsState.NPCsFates.TryGetValue(unitId, out var value))
		{
			value = new System.Collections.Generic.Dictionary<int, int>();
			NpcsState.NPCsFates[unitId] = value;
		}
		value[grade] = luck;
		Save();
	}

	public static void TryLoadNpcsWhenReady()
	{
		Timer timer = new Timer(500.0);
		timer.Elapsed += delegate
		{
			if (Game.WorldManager.Value?.unit?.allUnit != null)
			{
				timer.Stop();
				LoadNpcs();
				ModMain.Log("Loaded NPCs after polling for readiness.");
			}
		};
		timer.Start();
	}
}
