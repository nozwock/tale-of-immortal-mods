using System;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace MOD_YzmLuck;

[HarmonyPatch(typeof(UIPlayerInfoProperty))]
internal class Patch_UIPlayerInfoProperty
{
    [HarmonyPostfix]
    [HarmonyPatch("UpdateUI")]
    private static void Postfix(UIPlayerInfoProperty __instance)
    {
        try
        {
            g.timer.Frame((Action)delegate
            {
                CreateBornList(__instance);
            }, 3);
        }
        catch (Exception ex)
        {
            Tool.Error(ex);
        }
    }

    private static void CreateBornList(UIPlayerInfoProperty _instance)
    {
        if (_instance == null)
        {
            return;
        }
        Transform parent = _instance.goBornLuckRoot.transform.parent;
        string text = "YzmLuck_ScrollView";
        GameObject gameObject = parent.Find(text)?.gameObject;
        Il2CppReferenceArray<DataUnit.LuckData> bornLuck = g.world.playerUnit.data.unitData.propertyData.bornLuck;
        int num = 8;
        if (g.data.globle.gameSetting.languageType == LanguageType.English)
        {
            num = 5;
        }
        if (gameObject == null && bornLuck != null && bornLuck.Count >= num)
        {
            gameObject = UnityEngine.Object.Instantiate(g.res.Load<GameObject>("UI/YzmLuck/ScrollView"), parent);
            gameObject.name = text;
            gameObject.SetActive(value: true);
            gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(32f, -168f);
            Transform parent2 = gameObject.transform.Find("Viewport/Content");
            GameObject gameObject2 = g.res.Load<GameObject>("UI/Item/BornLuckItem");
            foreach (DataUnit.LuckData item in bornLuck)
            {
                if (item != null)
                {
                    GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2, parent2);
                    gameObject3.name = gameObject2.name;
                    UIBornLuckItem component = gameObject3.GetComponent<UIBornLuckItem>();
                    component.Init();
                    component.InitData(item);
                    component.UpdateFateBtn();
                }
            }
        }
        if (gameObject != null)
        {
            _instance.goBornLuckRoot.SetActive(value: false);
            _instance.goBornLuckRoot_En.SetActive(value: false);
        }
    }
}
