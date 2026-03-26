using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_YzmLuck;

[HarmonyPatch(typeof(UICreatePlayerProperty))]
internal class Patch_UICreatePlayerProperty
{
    internal static Dictionary<int, bool> lockLucks = new Dictionary<int, bool>();

    [HarmonyPrefix]
    [HarmonyPatch("RandomProperty")]
    private static bool Prefix_RandomProperty(UICreatePlayerProperty __instance)
    {
        if (UILuckFilter.IsActive)
        {
            __instance.lockLuckID = 0;
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch("RandomProperty")]
    private static void Postfix_RandomProperty(UICreatePlayerProperty __instance)
    {
        if (UILuckFilter.IsActive)
        {
            g.timer.Time((Action)delegate
            {
                LuckItemHandler(__instance);
            }, 1f);
        }
    }

    internal static void LuckItemHandler(UICreatePlayerProperty instance)
    {
        Transform gotfBornLuck = instance.goBornLuck.transform;
        for (int i = 0; i < gotfBornLuck.childCount && i < 9; i++)
        {
            Toggle toggle = gotfBornLuck.GetChild(i)?.GetComponent<Toggle>();
            if (toggle == null)
            {
                continue;
            }
            int.TryParse(toggle.gameObject.name, out var luckID);
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
            {
                if (isOn && !instance.lastClickBorn.Contains(toggle))
                {
                    instance.lastClickBorn.Add(toggle);
                }
                else if (!isOn && instance.lastClickBorn.Contains(toggle))
                {
                    instance.lastClickBorn.Remove(toggle);
                }
                if (lockLucks.ContainsKey(luckID))
                {
                    lockLucks[luckID] = isOn;
                }
                UpdataLock();
                instance.UpdatePlayerBornLuckData();
                g.timer.Frame((Action)delegate
                {
                    instance.UpdatePropertyUI(isAnim: true, isResetAnim: false);
                }, 3);
            });
            LockClickCall(toggle);
            if (lockLucks.ContainsKey(luckID))
            {
                toggle.isOn = lockLucks[luckID];
            }
        }
        UpdataLock();
        instance.UpdatePlayerBornLuckData();
        g.timer.Frame((Action)delegate
        {
            instance.UpdatePropertyUI(isAnim: true, isResetAnim: false);
        }, 3);
        void LockClickCall(Toggle toggle2)
        {
            Button button = toggle2?.transform.Find("Lock")?.GetComponent<Button>();
            Button button2 = toggle2?.transform.Find("Unlock")?.GetComponent<Button>();
            if (!int.TryParse(toggle2.gameObject.name, out var luckID2))
            {
                Tool.Error("气运ID转换出错！itemName：" + toggle2.gameObject.name);
            }
            if (!(button == null) && !(button2 == null))
            {
                button.onClick.RemoveAllListeners();
                button2.onClick.RemoveAllListeners();
                button.onClick.AddListener((Action)delegate
                {
                    if (lockLucks.ContainsKey(luckID2))
                    {
                        lockLucks.Remove(luckID2);
                    }
                    UpdataLock();
                });
                button2.onClick.AddListener((Action)delegate
                {
                    if (lockLucks.Count >= 9)
                    {
                        UITipItem.AddTip(GameTool.LS("tkFhkr_patchuicreateplayerProperty"), 3f);
                    }
                    else
                    {
                        if (!lockLucks.ContainsKey(luckID2) && luckID2 != 0)
                        {
                            lockLucks[luckID2] = toggle2.isOn;
                        }
                        UpdataLock();
                    }
                });
            }
        }
        void UpdataLock()
        {
            for (int j = 0; j < gotfBornLuck.childCount && j < 9; j++)
            {
                Toggle component = gotfBornLuck.GetChild(j).GetComponent<Toggle>();
                GameObject gameObject = component?.transform.Find("Lock").gameObject;
                GameObject gameObject2 = component?.transform.Find("Unlock").gameObject;
                int.TryParse(component.gameObject.name, out var result);
                if (!(gameObject == null) && !(gameObject2 == null) && result != 0)
                {
                    if (lockLucks.ContainsKey(result))
                    {
                        gameObject.SetActive(value: true);
                        gameObject2.SetActive(value: false);
                    }
                    else if (!lockLucks.ContainsKey(result) && component.isOn)
                    {
                        gameObject.SetActive(value: false);
                        gameObject2.SetActive(value: true);
                    }
                    else
                    {
                        gameObject.SetActive(value: false);
                        gameObject2.SetActive(value: false);
                    }
                }
            }
        }
    }
}
