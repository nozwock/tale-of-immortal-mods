using System.Collections.Generic;
using HarmonyLib;

namespace MOD_YzmLuck;

[HarmonyPatch]
class Patch_UpdateFateEffect
{
    static IEnumerable<System.Reflection.MethodBase> TargetMethods()
    {
        var method = nameof(UIPlayerInfoProperty.UpdateFateEffect);
        yield return AccessTools.Method(typeof(UIPlayerInfoProperty), method);
        yield return AccessTools.Method(typeof(UIPlayerInfoSkill), method);
        yield return AccessTools.Method(typeof(UIPlayerInfoArt), method);
    }

    static void Postfix()
    {
        UIEffectDisplay.UpdateFateEffectState();
    }
}