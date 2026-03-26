using System;
using Il2CppSystem.Collections.Generic;

namespace TaleOfImmortalCheat.UI.Panels;

public class AttributesState
{
	public Action<string, int, int> ChangeLuck;

	public WorldUnitBase Unit { get; internal set; }

	public Dictionary<int, DataWorld.World.PlayerLogData.GradeData> GradeLog { get; internal set; }
}
