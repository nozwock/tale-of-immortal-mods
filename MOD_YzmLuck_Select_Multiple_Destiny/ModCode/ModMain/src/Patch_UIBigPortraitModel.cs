using System;
using HarmonyLib;

namespace MOD_YzmLuck;

[HarmonyPatch(typeof(UIBigPortraitModel))]
internal class Patch_UIBigPortraitModel
{
    [HarmonyPostfix]
    [HarmonyPatch("InitData", new Type[] { typeof(WorldUnitBase) })]
    private static void Postfix(WorldUnitBase unit)
    {
        try
        {
            if (unit != null && unit.data?.unitData?.unitID == g.world.playerUnit.data.unitData.unitID)
            {
                UIEffectDisplay.UpdateFateEffectState();
            }
        }
        catch (Exception ex)
        {
            Tool.Error(ex);
        }
    }
}
