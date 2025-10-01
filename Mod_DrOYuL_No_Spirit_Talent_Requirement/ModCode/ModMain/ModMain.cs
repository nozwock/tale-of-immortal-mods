using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MelonLoader;
using UnhollowerBaseLib;

namespace MOD_DrOYuL;

public class ModMain
{
	internal static readonly string modNamespace = typeof(ModMain).Namespace!;

	Il2CppSystem.Action<ETypeData> callIntoWorld;

	// static readonly Regex DewIdPat = new(@"^5311\d201$", RegexOptions.Compiled);

	public void Init()
	{
		callIntoWorld = (Il2CppSystem.Action<ETypeData>)OnIntoWorld;
		g.events.On(EGameType.IntoWorld, callIntoWorld);
	}

	public void Destroy()
	{
		g.events.Off(EGameType.IntoWorld, callIntoWorld);
	}

	void OnIntoWorld(ETypeData e)
	{
		// var dews = new Dictionary<int, int>();
		// foreach (var conf in g.conf.artifactSpriteTalent._allConfList)
		// {
		// 	if (
		// 		!dews.ContainsKey(conf.spriteID)
		// 		&& conf.activeCost.Length >= 1
		// 		&& conf.activeCost[0].Length >= 2
		// 		&& DewIdPat.IsMatch(conf.activeCost[0][0].ToString())
		// 	)
		// 	{
		// 		dews[conf.spriteID] = conf.activeCost[0][0];
		// 	}
		// }

		Dictionary<(int, int), DataUnit.ArtifactSpriteData.Talent> talents = [];
		foreach (var sprite in g.world.playerUnit.data.unitData.artifactSpriteData.sprites)
		{
			foreach (var talent in sprite.talents)
			{
				talents[(sprite.spriteID, talent.number)] = talent;
			}
		}

		foreach (var conf in g.conf.artifactSpriteTalent._allConfList)
		{
			if (conf.unlock2Type != 0 && conf.unlock2Type != 17 /* No Cost */)
			{
				conf.unlock2Type = 17;
				conf.unlock2Value = conf.axisX.ToString();
				conf.unlock2Count = 1;
				conf.unlock3Type = 0;
				conf.unlock3Value = "0";
				conf.unlock3Count = 0;
				conf.unlockDesc = "0";
				conf.unlockDesc = "spriteTalent_unlockDesc100522"; // No Cost

				if (talents.TryGetValue((conf.spriteID, conf.number), out var talent)) // Artifact Spirit may not be unlocked yet
				{
					// Necessary, otherwise the discrepancy will result in a "No Cost" popup by game on trying to unlock
					talent.unlock2Count = 1;
					talent.unlock3Count = 0;
				}

				// var outer = new Il2CppReferenceArray<Il2CppStructArray<int>>(1);
				// var inner = new Il2CppStructArray<int>(2);
				// inner[0] = dews[conf.spriteID];
				// inner[1] = conf.addSoul;
				// outer[0] = inner;
				// conf.activeCost = outer;
			}
		}
	}

	internal static void Log(object s)
	{
		MelonLogger.Msg($"{modNamespace}: {s}");
	}
}
