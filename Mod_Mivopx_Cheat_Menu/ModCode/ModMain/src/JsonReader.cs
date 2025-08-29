using System.Collections.Generic;
using System.IO;
using MOD_Mivopx;
using Newtonsoft.Json;
using TaleOfImmortalCheat.UI;
using TaleOfImmortalCheat.Utilities;

public class JsonReader
{
	public static List<ItemHorse> itemHorses;

	public static List<ItemCloth> itemCloth;

	static JsonReader()
	{
		itemHorses = new List<ItemHorse>();
		itemCloth = new List<ItemCloth>();
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	public static void LoadMountJsonFile()
	{
		string text = FileSearchUtility.FindFile("Mod\\modFQA\\配置修改教程\\配置（只读）Json格式\\ItemHorse.json", "JSON");
		if (text != null)
		{
			itemHorses = JsonConvert.DeserializeObject<List<ItemHorse>>(File.ReadAllText(text));
			ModMain.Log("ItemHorses loaded successfully.");
			return;
		}
		ModMain.LogWarning("ItemHorse.json not found in any known location.");
		throw new FileNotFoundException("Could not locate the ItemHorse.json file.");
	}

	public static void LoadClothJsonFile()
	{
		string text = FileSearchUtility.FindFile("Mod\\modFQA\\配置修改教程\\配置（只读）Json格式\\ClothItem.json", "JSON");
		if (text != null)
		{
			itemCloth = JsonConvert.DeserializeObject<List<ItemCloth>>(File.ReadAllText(text));
			ModMain.Log("Clothes loaded successfully.");
			return;
		}
		ModMain.LogWarning("ClothItem.json not found in any known location.");
		throw new FileNotFoundException("Could not locate the ClothItem.json file.");
	}

	private static void UpdateUITexts()
	{
	}
}
