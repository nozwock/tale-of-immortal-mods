using MelonLoader;
using EGameTypeData;
using System.Collections.Generic;

namespace MOD_h3anho;

internal class Config
{
	static bool _isInit = false;

	static MelonPreferences_Category categoryBase;
	internal static MelonPreferences_Entry<int> logEntryLimit;
	// internal static MelonPreferences_Entry<int> keepSkillLogTime;

	internal static void Init()
	{
		if (_isInit)
			return;

		categoryBase = MelonPreferences.CreateCategory($"{ModMain.modNamespace} Limited Imp Adventure Log");
		logEntryLimit = categoryBase.CreateEntry(
			"Log Entry Limit",
			6,
			description: "Specifies how many months of recent logs to retain.\n"
				+ "Set to 0 or a negative number to disable log limit.\n"
				+ "Unit: months."
		);
		// keepSkillLogTime = categoryBase.CreateEntry(
		// 	"Keep Skill Logs",
		// 	-1,
		// 	description: "Specified how long to keep skill-specific logs.\n"
		// 		+ "-1 = Always retain, 0 = Disable, any positive number = months to retain.\n"
		// 		+ "This setting applies regardless of the 'Log Entry Limit' value."
		// );

		_isInit = true;
	}
}

public class ModMain
{
	internal static readonly string modNamespace = typeof(ModMain).Namespace!;

	Il2CppSystem.Action<ETypeData> callIntoWorld;
	Il2CppSystem.Action<ETypeData> callOpenUIStart;

	public void Init()
	{
		Config.Init();

		callIntoWorld = (Il2CppSystem.Action<ETypeData>)OnIntoWorld;
		callOpenUIStart = (Il2CppSystem.Action<ETypeData>)OnOpenUIStart;
		g.events.On(EGameType.IntoWorld, callIntoWorld);
		g.events.On(EGameType.OpenUIStart, callOpenUIStart);
	}

	public void Destroy()
	{
		g.events.Off(EGameType.IntoWorld, callIntoWorld);
		g.events.Off(EGameType.OpenUIStart, callOpenUIStart);
	}

	void OnIntoWorld(ETypeData _)
	{
		CleanImpAdventureLog();
	}

	void OnOpenUIStart(ETypeData e)
	{
		var edata = e.Cast<OpenUIStart>();
		if (edata.uiType.uiName == UIType.PotmonWorkReturn.uiName)
		{
			CleanImpAdventureLog();
		}
	}

	// static bool IsSkillLog(PotMonOutLog log)
	// {
	// 	return log.potmonSoleID != null && log.studySkill != 0;
	// }

	static void CleanImpAdventureLog()
	{
		if (Config.logEntryLimit.Value <= 0)
			return;

		var logs = g.world.devilDemonMgr.potMonMgr.potMonOutLogs; // asscending sorted by .time

		HashSet<int> keepMonths = [];
		var removeIndexEnd = -1;
		for (int i = logs.Count - 1; i >= 0; i--)
		{
			var log = logs[i];
			if (keepMonths.Count >= Config.logEntryLimit.Value)
			{
				if (!keepMonths.Contains(log.time))
				{
					removeIndexEnd = i;
					break;
				}
			}
			else
			{
				keepMonths.Add(log.time);
			}
		}
		logs.RemoveRange(0, removeIndexEnd + 1);
	}


	internal static void Log(object s)
	{
		MelonLogger.Msg($"{modNamespace}: {s}");
	}
}
