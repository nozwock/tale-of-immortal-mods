using System;
using System.Collections.Generic;
using EGameTypeData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MOD_2UTwDC;

public class ModMain
{
    Il2CppSystem.Action<ETypeData> callOpenUIEnd;

    public void Init()
    {
        callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
        g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
    }

    public void Destroy()
    {
        g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);
    }

    void OnOpenUIEnd(ETypeData e)
    {
        var edata = e.Cast<OpenUIEnd>();
        if (edata.uiType.uiName == UIType.MinMap.uiName)
        {
            var ui = edata.ui.GetComponent<UIMinMap>();
            var root = ui.opGroupUnitRoot.transform;

            var btnSortName = "btnSort";

            var btnDel = root.Find("Language/G:btnDel").GetComponent<Button>();
            if (btnDel == null)
            {
                Log("Couldn't add Sort button due to btnDel being not found");
                return;
            }
            if (!btnDel.IsActive())
                btnDel = root.Find("Language/G:btnDel_En").GetComponent<Button>();

            var goBtnRoot = btnDel.transform.parent.gameObject;

            if (goBtnRoot.transform.Find(btnSortName) != null)
                return;

            var layout = goBtnRoot.GetComponent<HorizontalLayoutGroup>() ?? goBtnRoot.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            var fitter = goBtnRoot.GetComponent<ContentSizeFitter>() ?? goBtnRoot.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var btnSort = GameObject.Instantiate(btnDel, goBtnRoot.transform);
            btnSort.gameObject.name = btnSortName;
            btnSort.transform.SetSiblingIndex(0);

            var txt = btnSort.GetComponentInChildren<Text>();
            if (txt != null)
                txt.text = LS("btn_sort");

            btnSort.onClick.AddListener((UnityAction)delegate
            {
                var playerUnit = g.world.playerUnit;
                var playerUnitRel = g.world.playerUnit.data.unitData.relationData;
                var markUnitsRoot = root.Find("G:srRoot/G:goUnitRoot");
                var markUnitIds = g.data.dataWorld.data.marksUnitID;
                Dictionary<string, (Transform, DataUnit.RelationData)> unitData = [];
                for (int i = 0; i < markUnitsRoot.childCount; i++)
                {
                    var tUnit = markUnitsRoot.GetChild(i);
                    var view = markUnitsRoot.GetChild(i).GetComponent<UIListCacheItemView>();
                    var unit = view.itemData.Cast<WorldUnitBase>();
                    var relationData = unit.data.unitData.relationData;
                    unitData[unit.data.unitData.unitID] = (tUnit, relationData);
                }

                markUnitIds.Sort((Func<string, string, int>)((a, b) =>
                {
                    var (tA, aRel) = unitData[a];
                    var (tB, bRel) = unitData[b];

                    var cmp = bRel.intimToPlayerUnit.CompareTo(aRel.intimToPlayerUnit);
                    if (cmp != 0) return cmp;
                    cmp = playerUnitRel.GetIntim(bRel.unit).CompareTo(playerUnitRel.GetIntim(aRel.unit));
                    if (cmp != 0) return cmp;

                    return (bRel.GetRelation(playerUnit) != UnitRelationType.None)
                        .CompareTo(aRel.GetRelation(playerUnit) != UnitRelationType.None);
                }));

                g.ui.CloseUI(ui);
                g.ui.OpenUI(UIType.MinMap);

                // for (int i = 0; i < markUnitIds.Count; i++)
                // {
                //     unitData[markUnitIds[i]].Item1.SetSiblingIndex(i);
                // }

                UITipItem.AddTip(LS("tip_sorted"), 2f);
            });
        }
    }
}
