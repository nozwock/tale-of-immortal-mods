using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MOD_Mivopx;
using Microsoft.Win32;

namespace TaleOfImmortalCheat.Utilities;

public static class FileSearchUtility
{
	private class SearchPath
	{
		public string Path { get; set; }

		public string Description { get; set; }
	}

	private static readonly string GameTitle_zhCN = "鬼谷八荒";

	private static readonly string GameTitle_pinyin = "guigubahuang";

	private static readonly string GameTitle_enUS = "Tale of Immortal";

	private static readonly Dictionary<string, string> _filePathCache = new Dictionary<string, string>();

	public static string FindFile(string fileName, string fileType = "file")
	{
		if (_filePathCache.TryGetValue(fileName, out var value))
		{
			if (File.Exists(value))
			{
				ModMain.Log("Found " + fileType + " file in cache: " + value);
				return value;
			}
			_filePathCache.Remove(fileName);
			ModMain.Log("Cached path for " + fileName + " no longer exists, removing from cache");
		}
		ModMain.Log("Looking for " + fileType + " file: " + fileName);
		foreach (SearchPath searchPath in GetSearchPaths(fileName))
		{
			ModMain.Log("Checking " + searchPath.Description + ": " + searchPath.Path);
			if (File.Exists(searchPath.Path))
			{
				ModMain.Log("Found at " + searchPath.Description + ": " + searchPath.Path);
				_filePathCache[fileName] = searchPath.Path;
				return searchPath.Path;
			}
		}
		ModMain.LogWarning(fileType + " file not found: " + fileName + ". Trying comprehensive Steam libraries search...");
		string text = SearchSteamLibraries(fileName);
		if (text != null)
		{
			ModMain.Log(fileName + " found via Steam libraries search at: " + text);
			_filePathCache[fileName] = text;
			return text;
		}
		ModMain.LogWarning(fileType + " file not found in any location: " + fileName);
		return null;
	}

	private static List<SearchPath> GetSearchPaths(string fileName)
	{
		List<SearchPath> list = new List<SearchPath>();
		try
		{
			list.Add(new SearchPath
			{
				Path = Path.GetFullPath(fileName),
				Description = "direct path"
			});
			list.Add(new SearchPath
			{
				Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName),
				Description = "base directory"
			});
			list.Add(new SearchPath
			{
				Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader", fileName),
				Description = "MelonLoader directory"
			});
			list.Add(new SearchPath
			{
				Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods", fileName),
				Description = "Mods directory"
			});
			string localDirectory = GetLocalDirectory(fileName);
			if (!string.IsNullOrEmpty(localDirectory))
			{
				list.Add(new SearchPath
				{
					Path = localDirectory,
					Description = "local directory (ModExportData)"
				});
			}
			string modFolderDirectory = GetModFolderDirectory(fileName);
			if (!string.IsNullOrEmpty(modFolderDirectory))
			{
				list.Add(new SearchPath
				{
					Path = modFolderDirectory,
					Description = "direct Mod's folder directory"
				});
			}
			string steamWorkshopDirectory = GetSteamWorkshopDirectory(fileName);
			if (!string.IsNullOrEmpty(steamWorkshopDirectory))
			{
				list.Add(new SearchPath
				{
					Path = steamWorkshopDirectory,
					Description = "Steam workshop directory"
				});
			}
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error building search paths: " + ex.Message);
		}
		return list;
	}

	private static string GetLocalDirectory(string fileName)
	{
		try
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ModExportData", "Mod_Mivopx", "ModCode", "dll", fileName);
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error getting local directory: " + ex.Message);
			return null;
		}
	}

	private static string GetModFolderDirectory(string fileName)
	{
		try
		{
			string modPathRoot = g.mod.GetModPathRoot("Mivopx");
			if (!string.IsNullOrEmpty(modPathRoot))
			{
				string text = Path.Combine(modPathRoot, "ModCode", "dll");
				ModMain.Log("Checking mod system path: " + text);
				if (Directory.Exists(text))
				{
					string result = Path.Combine(text, fileName);
					ModMain.Log("Found mod path via mod system: " + text);
					return result;
				}
			}
		}
		catch (Exception ex)
		{
			ModMain.Log("Mod system GetModPathRoot failed: " + ex.Message);
		}
		return null;
	}

	private static string GetSteamWorkshopDirectory(string fileName)
	{
		try
		{
			string modPathRoot = GetModPathRoot();
			if (string.IsNullOrEmpty(modPathRoot))
			{
				return null;
			}
			return Path.Combine(modPathRoot, fileName);
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error getting Steam workshop directory: " + ex.Message);
			return null;
		}
	}

	private static string GetModPathRoot()
	{
		try
		{
			string steamInstallPath = GetSteamInstallPath();
			if (!string.IsNullOrEmpty(steamInstallPath))
			{
				string text = Path.Combine(steamInstallPath, "steamapps", "workshop", "content", "1468810");
				ModMain.Log("Checking fallback workshop path: " + text);
				if (Directory.Exists(text))
				{
					string[] directories = Directory.GetDirectories(text);
					for (int i = 0; i < directories.Length; i++)
					{
						string text2 = Path.Combine(directories[i], "ModCode", "dll");
						if (Directory.Exists(text2))
						{
							ModMain.Log("Found workshop mod path via GetSteamInstallPath fallback: " + text2);
							return text2;
						}
					}
				}
			}
			ModMain.Log("No workshop mod path found in any location");
			return null;
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error getting mod path root: " + ex.Message);
			return null;
		}
	}

	private static string GetSteamInstallPath()
	{
		try
		{
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam"))
			{
				string text = registryKey?.GetValue("SteamPath") as string;
				if (!string.IsNullOrEmpty(text) && Directory.Exists(text))
				{
					ModMain.Log("Found Steam path from registry: " + text);
					return text;
				}
			}
			using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Valve\\Steam"))
			{
				string text2 = registryKey2?.GetValue("InstallPath") as string;
				if (!string.IsNullOrEmpty(text2) && Directory.Exists(text2))
				{
					ModMain.Log("Found Steam path from HKLM registry: " + text2);
					return text2;
				}
			}
			string[] array = new string[5] { "C:\\Program Files (x86)\\Steam", "C:\\Program Files\\Steam", "D:\\Steam", "E:\\Steam", "F:\\Steam" };
			foreach (string text3 in array)
			{
				if (Directory.Exists(text3))
				{
					ModMain.Log("Found Steam at common path: " + text3);
					return text3;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error getting Steam install path: " + ex.Message);
			return null;
		}
	}

	private static string SearchSteamLibraries(string fileName)
	{
		ModMain.Log("Searching Steam libraries for: " + fileName);
		if (File.Exists(fileName))
		{
			ModMain.Log("Found file at direct path: " + Path.GetFullPath(fileName));
			return Path.GetFullPath(fileName);
		}
		string steamInstallPath = GetSteamInstallPath();
		if (string.IsNullOrEmpty(steamInstallPath))
		{
			ModMain.LogWarning("Could not find Steam installation path");
			return null;
		}
		string text = Path.Combine(steamInstallPath, "steamapps", "libraryfolders.vdf");
		List<string> list = new List<string> { Path.Combine(steamInstallPath, "steamapps", "common") };
		ModMain.Log("Checking Steam library folders VDF: " + text);
		if (File.Exists(text))
		{
			ModMain.Log("Found libraryfolders.vdf, parsing additional library paths");
			string[] array = File.ReadAllLines(text);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Trim();
				if (text2.StartsWith("\"path\""))
				{
					string[] array2 = text2.Split('"');
					if (array2.Length >= 4)
					{
						string text3 = array2[3].Replace("\\\\", "\\");
						if (!string.IsNullOrEmpty(text3) && Directory.Exists(text3))
						{
							string text4 = Path.Combine(text3, "steamapps", "common");
							list.Add(text4);
							ModMain.Log("Added library path from VDF: " + text4);
						}
					}
				}
				else
				{
					if (!text2.StartsWith("\"") || !text2.Contains(":\\"))
					{
						continue;
					}
					string text5 = text2.Split('"').FirstOrDefault((string s) => s.Contains(":\\"));
					if (!string.IsNullOrEmpty(text5))
					{
						string text6 = Path.Combine(text5, "steamapps", "common");
						if (Directory.Exists(text6))
						{
							list.Add(text6);
							ModMain.Log("Added legacy library path: " + text6);
						}
					}
				}
			}
		}
		else
		{
			ModMain.LogWarning("libraryfolders.vdf not found, using default Steam path only");
		}
		foreach (string item in list.Distinct())
		{
			try
			{
				ModMain.Log("Searching library: " + item);
				if (!Directory.Exists(item))
				{
					ModMain.Log("Library directory does not exist: " + item);
					continue;
				}
				string text7 = SearchInPrioritizedGameFolders(item, fileName);
				if (text7 != null)
				{
					return text7;
				}
				string[] files = Directory.GetFiles(item, fileName, SearchOption.AllDirectories);
				if (files.Length != 0)
				{
					ModMain.Log($"Found {files.Length} matching file(s) in {item} (fallback search)");
					string[] array = files;
					foreach (string text8 in array)
					{
						ModMain.Log("  - " + text8);
					}
					return files[0];
				}
				ModMain.Log("No matching files found in " + item);
			}
			catch (Exception ex)
			{
				ModMain.LogError("Error searching library " + item + ": " + ex.Message);
			}
		}
		ModMain.LogWarning("File " + fileName + " not found in any Steam library");
		return null;
	}

	private static string SearchInPrioritizedGameFolders(string libraryPath, string fileName)
	{
		string[] array = new string[3] { GameTitle_zhCN, GameTitle_pinyin, GameTitle_enUS };
		foreach (string text in array)
		{
			try
			{
				string text2 = Path.Combine(libraryPath, text);
				ModMain.Log("Checking prioritized game folder: " + text2);
				if (Directory.Exists(text2))
				{
					string[] files = Directory.GetFiles(text2, fileName, SearchOption.AllDirectories);
					if (files.Length != 0)
					{
						ModMain.Log($"Found {files.Length} matching file(s) in prioritized game folder '{text}'");
						string[] array2 = files;
						foreach (string text3 in array2)
						{
							ModMain.Log("  - " + text3);
						}
						return files[0];
					}
					ModMain.Log("No matching files found in prioritized game folder '" + text + "'");
				}
				else
				{
					ModMain.Log("Prioritized game folder does not exist: '" + text + "'");
				}
			}
			catch (Exception ex)
			{
				ModMain.LogError("Error searching prioritized game folder '" + text + "': " + ex.Message);
			}
		}
		return null;
	}

	public static void ClearCache()
	{
		_filePathCache.Clear();
		ModMain.Log("FileSearchUtility cache cleared");
	}

	public static Dictionary<string, string> GetCacheStatus()
	{
		return new Dictionary<string, string>(_filePathCache);
	}
}
