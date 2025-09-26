using System;
using HarmonyLib;

namespace MOD_YzmLuck;

[HarmonyPatch(typeof(UIFateFeatureGradeInfo))]
internal class Patch_UIFateFeatureGradeInfo
{
    [HarmonyPostfix]
    [HarmonyPatch("UpdateGradeInfo")]
    private static void Postfix(UIFateFeatureGradeInfo __instance, int grade)
    {
        try
        {
            if (grade < 2 || grade > 10)
            {
                return;
            }
            UIAddLuckItem uIAddLuckItem = __instance?.goLuckPosi?.transform.Find("AddLuckItem")?.GetComponent<UIAddLuckItem>();
            if (uIAddLuckItem == null)
            {
                return;
            }
            uIAddLuckItem.imgBG.gameObject.GetComponent<UIEventListener>()?.onClick.AddListener((Action)delegate
            {
                UIBase uIBase = g.ui.OpenUI(new UIType.UITypeBase("YzmLuck/FateFeatureChange", UILayer.UI));
                if (!(uIBase == null))
                {
                    uIBase.gameObject.AddComponent<UIFateFeatureChange>().Init(grade, __instance);
                }
            });
        }
        catch (Exception ex)
        {
            Tool.Error(ex);
        }
    }
}
