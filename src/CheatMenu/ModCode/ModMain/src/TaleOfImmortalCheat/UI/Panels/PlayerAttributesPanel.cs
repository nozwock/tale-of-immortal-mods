using System;

namespace TaleOfImmortalCheat.UI.Panels;

public class PlayerAttributesPanel : AttributesPanel
{
	internal override AttributesState State => new AttributesState
	{
		Unit = Game.WorldManager.Value?.playerUnit,
		GradeLog = Game.DataWorld.Value?.data.playerLog.upGrade
	};

	internal override Action OnUpdate => delegate
	{
		Game.UpdatePlayerStatsUI();
	};

	public PlayerAttributesPanel()
		: base("Player", isUsingBaseValue: true)
	{
	}
}
