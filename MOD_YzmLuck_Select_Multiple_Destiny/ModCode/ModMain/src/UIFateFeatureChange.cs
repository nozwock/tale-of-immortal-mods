using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_YzmLuck;

internal class UIFateFeatureChange : UIBase
{
    private UIFateFeatureGradeInfo _uiInstance;

    private System.Collections.Generic.List<int> allLeftItems;

    private int newFateFeatureID;

    private (int grade, int featureID) delFateFeature;

    private GameObject AS_Item;

    private Transform leftList;

    private Transform oldFatePoint;

    private Transform newFatePoint;

    private Button btnTips;

    private Button btnComplete;

    private Button btnClose;

    private InputField inputSearch;

    public UIFateFeatureChange(IntPtr ptr)
        : base(ptr)
    {
    }

    internal void Init(int grade, UIFateFeatureGradeInfo ui)
    {
        FindUI();
        UIEvent();
        base.gameObject.AddComponent<UIFastClose>();
        if (g.data.globle.gameSetting.languageType == LanguageType.English)
        {
            leftList.GetComponent<GridLayoutGroup>().spacing += new Vector2(0f, 20f);
        }
        InitData(grade);
        _uiInstance = ui;
    }

    private void FindUI()
    {
        Transform obj = base.transform.Find("Mask");
        Material material = new Material(Shader.Find("Custom/BackBlur"));
        obj.GetComponent<Image>().material = material;
        leftList = base.transform.Find("Root/Left/ScrollView/Viewport/Content");
        AS_Item = base.transform.Find("AS/Item").gameObject;
        btnClose = base.transform.Find("Root/btnClose").GetComponent<Button>();
        btnComplete = base.transform.Find("Root/Left/btnComplete").GetComponent<Button>();
        btnComplete.transform.Find("Text").GetComponent<Text>().text = GameTool.LS("tkFhkr_uifatefeaturechange_1");
        inputSearch = base.transform.Find("Root/Left/inputSearch").GetComponent<InputField>();
        inputSearch.placeholder.GetComponent<Text>().text = GameTool.LS("tkFhkr_uifatefeaturechange_2");
        oldFatePoint = base.transform.Find("Root/Right/oldFatePoint");
        newFatePoint = base.transform.Find("Root/Right/newFatePoint");
        base.transform.Find("Root/Right/title").GetComponent<Text>().text = GameTool.LS("tkFhkr_uifatefeaturechange_3");
        base.transform.Find("Root/Right/modName").GetComponent<Text>().text = GameTool.LS("tkFhkr_uifatefeaturechange_8");
        btnTips = base.transform.Find("Root/Right/btnTips").GetComponent<Button>();
        btnTips.transform.Find("Text").GetComponent<Text>().text = GameTool.LS("tkFhkr_uifatefeaturechange_4");
    }

    private void UIEvent()
    {
        btnClose.onClick.AddListener((Action)delegate
        {
            g.ui.CloseUI(new UIType.UITypeBase(base.gameObject.name, UILayer.UI));
        });
        inputSearch.onEndEdit.AddListener((Action<string>)FeatureSearch);
        btnComplete.onClick.AddListener((Action)ResetFate);
        btnTips.onClick.AddListener((Action)delegate
        {
            string info = GameTool.LS("tkFhkr_uifatefeaturechange_7");
            g.ui.OpenUI<UITextInfo>(UIType.TextInfo).InitData(GameTool.LS("common_tishi"), info);
        });
    }

    private void InitData(int grade)
    {
        if (grade < 2 || grade > 10)
        {
            return;
        }
        Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData> upGrade = g.data.world.playerLog.upGrade;
        if (upGrade == null || upGrade.Count == 0)
        {
            return;
        }
        int num = 0;
        if (upGrade.ContainsKey(grade) && upGrade[grade] != null)
        {
            num = upGrade[grade].luck;
        }
        ConfRoleCreateFeatureItem item = g.conf.roleCreateFeature.GetItem(num);
        if (item == null || item.type != 3)
        {
            return;
        }
        delFateFeature = (grade: grade, featureID: num);
        UpdateRight();
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData>.Enumerator enumerator = upGrade.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Il2CppSystem.Collections.Generic.KeyValuePair<int, DataWorld.World.PlayerLogData.GradeData> current = enumerator.Current;
            if (current?.Value != null)
            {
                list.Add(current.Value.luck);
            }
        }
        System.Collections.Generic.List<ConfRoleCreateFeatureItem> list2 = new System.Collections.Generic.List<ConfRoleCreateFeatureItem>();
        Il2CppSystem.Collections.Generic.List<ConfFateFeatureItem>.Enumerator enumerator2 = g.conf.fateFeature._allConfList.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            ConfFateFeatureItem current2 = enumerator2.Current;
            if (current2 != null && !list.Contains(current2.id) && (!string.IsNullOrWhiteSpace(GameTool.LS(current2.picture)) || !string.IsNullOrWhiteSpace(GameTool.LS(current2.desc))))
            {
                ConfRoleCreateFeatureItem item2 = g.conf.roleCreateFeature.GetItem(current2.id);
                if (item2 != null && item2.type == 3 && !string.IsNullOrWhiteSpace(item2.effect) && !(item2.effect == "0") && !string.IsNullOrWhiteSpace(GameTool.LS(item2.name)))
                {
                    list2.Add(item2);
                }
            }
        }
        list2.Sort(delegate (ConfRoleCreateFeatureItem fate1, ConfRoleCreateFeatureItem fate2)
        {
            if (fate1 == null || fate2 == null)
            {
                return 1;
            }
            ConfSchoolFateItem itemByFate = g.conf.schoolFate.GetItemByFate(fate1.id);
            ConfSchoolFateItem itemByFate2 = g.conf.schoolFate.GetItemByFate(fate2.id);
            if (itemByFate != null && itemByFate2 == null)
            {
                return -1;
            }
            if (itemByFate == null && itemByFate2 != null)
            {
                return 1;
            }
            return (fate1.level <= fate2.level) ? 1 : (-1);
        });
        allLeftItems = new System.Collections.Generic.List<int>();
        foreach (ConfRoleCreateFeatureItem item3 in list2)
        {
            allLeftItems.Add(item3.id);
            AddLeftItem(item3.id);
        }
    }

    private void UpdateLeft()
    {
        for (int i = 0; i < leftList.childCount; i++)
        {
            Toggle toggle = leftList.GetChild(i)?.GetComponent<Toggle>();
            if (!(toggle == null))
            {
                int.TryParse(toggle.gameObject.name, out var result);
                if (result != 0 && newFateFeatureID == result)
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

    private void UpdateRight()
    {
        if (delFateFeature.featureID != 0 && oldFatePoint.Find("AddLuckItem") == null)
        {
            CreateFateItem(delFateFeature.featureID, oldFatePoint, onClickDel: false);
        }
        for (int num = newFatePoint.childCount - 1; num >= 0; num--)
        {
            UnityEngine.Object.Destroy(newFatePoint.GetChild(num)?.gameObject);
        }
        ConfRoleCreateFeatureItem item = g.conf.roleCreateFeature.GetItem(newFateFeatureID);
        if (item != null)
        {
            CreateFateItem(item.id, newFatePoint, onClickDel: true);
        }
        void CreateFateItem(int featureID, Transform parent, bool onClickDel)
        {
            if (g.conf.roleCreateFeature.GetItem(featureID) != null && !(parent == null))
            {
                GameObject original = g.res.Load<GameObject>("UI/Item/AddLuckItem");
                GameObject goBornLuckItem = UnityEngine.Object.Instantiate(original, parent);
                goBornLuckItem.name = "AddLuckItem";
                UIAddLuckItem component = goBornLuckItem.GetComponent<UIAddLuckItem>();
                component.Init();
                component.InitData(featureID);
                if (onClickDel)
                {
                    component.imgBG.transform.GetComponent<UIEventListener>().onClick.AddListener((Action)delegate
                    {
                        newFateFeatureID = 0;
                        UnityEngine.Object.Destroy(goBornLuckItem);
                        UpdateLeft();
                    });
                }
            }
        }
    }

    private void AddLeftItem(int fateFeatureID)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate(AS_Item, leftList);
        gameObject.name = fateFeatureID.ToString();
        GameObject gameObject2 = g.res.Load<GameObject>("UI/Item/AddLuckItem");
        GameObject obj = UnityEngine.Object.Instantiate(gameObject2, gameObject.transform);
        obj.name = gameObject2.name;
        UIAddLuckItem component = obj.GetComponent<UIAddLuckItem>();
        component.Init();
        component.InitData(fateFeatureID);
        obj.transform.SetAsFirstSibling();
        Toggle toggle = gameObject.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener((Action<bool>)OnValueChanged);
        component.imgBG.transform.GetComponent<UIEventListener>().onClick.AddListener((Action)delegate
        {
            toggle.isOn = !toggle.isOn;
        });
        component.gaimingkuang.transform.Find("root")?.gameObject.SetActive(value: false);
        void OnValueChanged(bool isOn)
        {
            if (isOn)
            {
                newFateFeatureID = fateFeatureID;
            }
            else
            {
                newFateFeatureID = 0;
            }
            UpdateRight();
        }
    }

    private void FeatureSearch(string value)
    {
        if (allLeftItems == null)
        {
            return;
        }
        for (int num = leftList.childCount - 1; num >= 0; num--)
        {
            UnityEngine.Object.Destroy(leftList.GetChild(num).gameObject);
        }
        if (string.IsNullOrEmpty(value))
        {
            foreach (int allLeftItem in allLeftItems)
            {
                AddLeftItem(allLeftItem);
            }
            UpdateLeft();
            return;
        }
        foreach (int allLeftItem2 in allLeftItems)
        {
            string text = GameTool.LS(g.conf.roleCreateFeature.GetItem(allLeftItem2).name);
            if (text != null && text.Contains(value))
            {
                AddLeftItem(allLeftItem2);
            }
        }
        UpdateLeft();
    }

    private void ResetFate()
    {
        int item = delFateFeature.grade;
        int item2 = delFateFeature.featureID;
        if (item < 2 || item > 10 || delFateFeature.featureID == newFateFeatureID)
        {
            return;
        }
        ConfFateFeatureItem item3 = g.conf.fateFeature.GetItem(newFateFeatureID);
        ConfFateFeatureItem item4 = g.conf.fateFeature.GetItem(item2);
        if (item3 == null || item4 == null)
        {
            if (item3 == null)
            {
                UITipItem.AddTip(GameTool.LS("tkFhkr_uifatefeaturechange_5"), 3f);
            }
            return;
        }
        Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData> playerUpGrade = g.data.world.playerLog.upGrade;
        Il2CppSystem.Collections.Generic.List<ConfFateFeatureItem> groupFeatureItems = g.conf.fateFeature.GetGroupFeatureItems(item3.group);
        int i;
        for (i = 2; i < item; i++)
        {
            if (playerUpGrade.ContainsKey(i) && playerUpGrade[i] != null && groupFeatureItems.Find((Func<ConfFateFeatureItem, bool>)((ConfFateFeatureItem v) => v.id == playerUpGrade[i].luck)) != null && g.conf.fateFeature.GetItemIndex(playerUpGrade[i].luck) >= g.conf.fateFeature.GetItemIndex(item3.id))
            {
                UITipItem.AddTip(string.Format(GameTool.LS("tkFhkr_uifatefeaturechange_9"), GameTool.LS($"role_grade_name{i}")), 3f);
                return;
            }
        }
        playerUpGrade[item].luck = item3.id;
        WorldUnitLuckBase luck = g.world.playerUnit.GetLuck(item4.id);
        if (luck != null)
        {
            g.world.playerUnit.CreateAction(new UnitActionLuckDel(luck));
        }
        bool flag = true;
        Il2CppSystem.Collections.Generic.Dictionary<int, DataWorld.World.PlayerLogData.GradeData>.Enumerator enumerator = playerUpGrade.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Il2CppSystem.Collections.Generic.KeyValuePair<int, DataWorld.World.PlayerLogData.GradeData> item5 = enumerator.Current;
            if (item5 != null && item5.Value != null && groupFeatureItems.Find((Func<ConfFateFeatureItem, bool>)((ConfFateFeatureItem v) => v.id == item5.Value.luck)) != null && g.conf.fateFeature.GetItemIndex(item5.Value.luck) > g.conf.fateFeature.GetItemIndex(item3.id))
            {
                flag = false;
            }
        }
        if (flag)
        {
            UnitActionLuckAdd unitActionLuckAdd = new UnitActionLuckAdd(item3.id);
            unitActionLuckAdd.fateFeatureGrade = item;
            unitActionLuckAdd.Init(g.world.playerUnit);
            unitActionLuckAdd.Create();
        }
        Tuple<int, int> tuple = null;
        Il2CppSystem.Collections.Generic.List<ConfFateFeatureItem> groupFeatureItems2 = g.conf.fateFeature.GetGroupFeatureItems(item4.group);
        enumerator = playerUpGrade.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Il2CppSystem.Collections.Generic.KeyValuePair<int, DataWorld.World.PlayerLogData.GradeData> current = enumerator.Current;
            int luck2 = current.Value.luck;
            Il2CppSystem.Collections.Generic.List<ConfFateFeatureItem>.Enumerator enumerator2 = groupFeatureItems2.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                ConfFateFeatureItem current2 = enumerator2.Current;
                if (luck2 == current2.id && luck2 != item3.id && g.conf.fateFeature.GetItemIndex(luck2) < g.conf.fateFeature.GetItemIndex(item4.id))
                {
                    if (tuple == null)
                    {
                        tuple = new Tuple<int, int>(luck2, current.key);
                    }
                    else if (g.conf.fateFeature.GetItemIndex(luck2) > g.conf.fateFeature.GetItemIndex(tuple.Item1))
                    {
                        tuple = new Tuple<int, int>(luck2, current.key);
                    }
                }
            }
        }
        if (tuple != null)
        {
            UnitActionLuckAdd unitActionLuckAdd2 = new UnitActionLuckAdd(tuple.Item1);
            unitActionLuckAdd2.fateFeatureGrade = tuple.Item2;
            unitActionLuckAdd2.Init(g.world.playerUnit);
            unitActionLuckAdd2.Create();
        }
        btnClose.onClick.Invoke();
        if (_uiInstance != null)
        {
            _uiInstance.UpdateGradeInfo(item);
        }
        UITipItem.AddTip(GameTool.LS("tkFhkr_uifatefeaturechange_6"), 3f);
    }
}
