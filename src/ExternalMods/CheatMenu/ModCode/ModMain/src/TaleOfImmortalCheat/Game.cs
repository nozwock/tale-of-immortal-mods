using System;
using System.Collections.Generic;
using MOD_Mivopx;

namespace TaleOfImmortalCheat;

public static class Game
{
	public static ConfMgr ConfMgr;

	public static DynamicInstance<DataWorld> DataWorld = new DynamicInstance<DataWorld>();

	public static DynamicInstance<WorldMgr> WorldManager = new DynamicInstance<WorldMgr>();

	public static DynamicInstance<WorldUnitBase> NpcUnit = new DynamicInstance<WorldUnitBase>();

	public static UIPlayerInfo UIPlayerInfo;

	public static RewardFactory RewardFactory;

	public static readonly List<string> UpGradeNames = new List<string> { "role_grade_name2", "role_grade_name3", "role_grade_name4", "role_grade_name5", "role_grade_name6", "role_grade_name7", "role_grade_name8", "role_grade_name9", "role_grade_name10" };

	public static bool HasChanged()
	{
		if (!WorldManager.HasChanged)
		{
			return NpcUnit.HasChanged;
		}
		return true;
	}

	public static void UpdatePlayerStatsUI()
	{
		try
		{
			if (UIPlayerInfo == null)
			{
				ModMain.LogWarning("UIPlayerInfo is null, cannot update player stats UI");
				return;
			}
			UIPlayerInfo.UpdateTypeUI(1);
			UIPlayerInfo.uiProperty?.UpdateUI();
			UIPlayerInfo.uiPropertyCommon?.UpdateUI();
		}
		catch (Exception ex)
		{
			ModMain.LogError("Failed to update player UI: " + ex.Message);
		}
	}

	public static void Destroy()
	{
		try
		{
			DataWorld = new DynamicInstance<DataWorld>();
			WorldManager = new DynamicInstance<WorldMgr>();
			NpcUnit = new DynamicInstance<WorldUnitBase>();
			UIPlayerInfo = null;
			RewardFactory = null;
			ConfMgr = null;
		}
		catch (Exception ex)
		{
			ModMain.LogError("Error during Game.Destroy(): " + ex.Message);
		}
	}
}
