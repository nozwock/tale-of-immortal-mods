using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MOD_cK2zMO
{
	internal class ModelFile
	{
		public List<ModelList> ModelList = new List<ModelList>();
		public string pathFile = "ModelFile.json";

		public ModelFile()
		{
			Init();
		}
		public void Init()
		{
			Console.WriteLine("Storage file path:" + pathFile);
			if (File.Exists(pathFile))
			{
				ModelList = JsonConvert.DeserializeObject<List<ModelList>>(File.ReadAllText(pathFile));
				return;
			}
			SaveConf();
		}

		public void SaveConf()
		{
			if (!File.Exists(pathFile))
			{
				new FileStream(pathFile, FileMode.Create, FileAccess.ReadWrite).Close();
			}
			JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
			{
				Formatting = (Formatting)1,
				NullValueHandling = 0
			};
			string text = JsonConvert.SerializeObject(ModelList, jsonSerializerSettings);
			File.WriteAllText(pathFile, text);
			Console.WriteLine("Saved successfully");
		}

	}
}
