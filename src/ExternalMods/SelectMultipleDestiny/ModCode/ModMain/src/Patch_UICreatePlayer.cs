using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_YzmLuck;

[HarmonyPatch(typeof(UICreatePlayer))]
internal class Patch_UICreatePlayer
{
    [HarmonyPrefix]
    [HarmonyPatch("OnOkClick")]
    private static bool Prefix(UICreatePlayer __instance, out Dictionary<int, bool> __state)
    {
        __state = new Dictionary<int, bool>();
        if (!UILuckFilter.IsActive)
        {
            return true;
        }
        if (!CheckBornCount(__instance, isTips: true))
        {
            return false;
        }
        PassedInspection(__instance, __state);
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch("OnOkClick")]
    private static void Posifix(UICreatePlayer __instance, Dictionary<int, bool> __state)
    {
        if (!UILuckFilter.IsActive || !CheckBornCount(__instance))
        {
            return;
        }
        Transform transform = __instance.uiProperty.goBornLuck.transform;
        if (transform == null)
        {
            return;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Toggle toggle = transform.GetChild(i)?.GetComponent<Toggle>();
            if (!(toggle == null))
            {
                toggle.isOn = __state[i];
            }
        }
        TimerCoroutine timerCoroutine = null;
        timerCoroutine = g.timer.Frame((System.Action)delegate
        {
            UICheckPopup uI = g.ui.GetUI<UICheckPopup>(UIType.CheckPopup);
            if (uI != null)
            {
                g.timer.Stop(timerCoroutine);
                uI.onNoCall = (System.Action)delegate
                {
                    Patch_UICreatePlayerProperty.LuckItemHandler(__instance.uiProperty);
                };
                Il2CppSystem.Action onYesCall = uI.onYesCall;
                uI.onYesCall = (System.Action)delegate
                {
                    UILuckFilter.LuckFilter = null;
                    if (!UILuckFilter.IsActive)
                    {
                        UILuckFilter.SpecialLuck.Clear();
                    }
                    PassedInspection(__instance, __state);
                };
                uI.onYesCall += onYesCall;
            }
        }, 1, loop: true);
    }

    private static void PassedInspection(UICreatePlayer instance, Dictionary<int, bool> state)
    {
        Transform transform = instance.uiProperty.goBornLuck.transform;
        if (transform == null)
        {
            return;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Toggle toggle = transform.GetChild(i)?.GetComponent<Toggle>();
            if (!(toggle == null))
            {
                state[i] = toggle.isOn;
                toggle.onValueChanged.RemoveAllListeners();
                if (i <= 2)
                {
                    toggle.isOn = true;
                }
                else
                {
                    toggle.isOn = false;
                }
            }
        }
    }

    private static bool CheckBornCount(UICreatePlayer instance, bool isTips = false)
    {
        Transform transform = instance.uiProperty.goBornLuck.transform;
        if (transform == null)
        {
            return false;
        }
        int num = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Toggle toggle = transform.GetChild(i)?.GetComponent<Toggle>();
            if (!(toggle == null) && toggle.isOn)
            {
                num++;
            }
        }
        if (num >= 3)
        {
            return true;
        }
        string str = GameTool.LS("tkFhkr_patchuicreateplayer");
        if (isTips)
        {
            UITipItem.AddTip(str, 3f);
        }
        return false;
    }
}
