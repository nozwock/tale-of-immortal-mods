using System;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;

namespace TaleOfImmortalCheat.UI.Panels;

public class NPCAttributesPanel : AttributesPanel
{
	internal override AttributesState State => new AttributesState
	{
		Unit = Game.NpcUnit.Value,
		GradeLog = (Game.NpcUnit.Value?.data?.unitData?.npcUpGrade ?? new Dictionary<int, DataWorld.World.PlayerLogData.GradeData>()),
		ChangeLuck = TaleOfImmortalCheat.State.ChangeNpcLuck
	};

	internal override Action OnUpdate => delegate
	{
		UINPCInfoPatchInitData.Update();
	};

	public NPCAttributesPanel()
		: base("NPC", isUsingBaseValue: true)
	{
	}
}
