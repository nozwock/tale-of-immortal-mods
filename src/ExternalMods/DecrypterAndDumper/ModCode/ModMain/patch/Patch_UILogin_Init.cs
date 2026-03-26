using System;
using HarmonyLib;

namespace MOD_swissTool.patch
{
    [HarmonyPatch(typeof(UILogin), "Init")]
    internal class Patch_UILogin_Init
    {
        [HarmonyPostfix]
        private static void Postfix(UILogin __instance)
        {
            g.timer.Time((Action)delegate { new menuInterface(__instance); }, 0.25f);
        }
    }
}
