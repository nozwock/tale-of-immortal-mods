using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_YzmLuck;

internal class UIBornLuckList : UIBase
{
    private static System.Collections.Generic.List<int> tempSelectLuck;

    private static System.Collections.Generic.List<int> selectLuck;

    private System.Collections.Generic.List<ConfRoleCreateFeatureItem> allLeftItems;

    private GameObject AS_Item;

    private Transform leftList;

    private Transform rightList;

    private Button btnClear;

    private Button btnComplete;

    private Button btnClose;

    private InputField inputSearch;

    public UIBornLuckList(IntPtr ptr)
        : base(ptr)
    {
    }

    public void Start()
    {
        base.gameObject.AddComponent<UIFastClose>();
        if (g.data.globle.gameSetting.languageType == LanguageType.English)
        {
            leftList.GetComponent<GridLayoutGroup>().spacing += new Vector2(0f, 20f);
        }
        InitData();
    }

    public void Awake()
    {
        if (selectLuck == null)
        {
            selectLuck = new System.Collections.Generic.List<int>();
        }
        FindUI();
        UIEvent();
    }

    private void FindUI()
    {
        Transform obj = base.transform.Find("Mask");
        Material material = new Material(Shader.Find("Custom/BackBlur"));
        obj.GetComponent<Image>().material = material;
        leftList = base.transform.Find("Root/Left/ScrollView/Viewport/Content");
        rightList = base.transform.Find("Root/Right/ScrollView/Viewport/Content");
        AS_Item = base.transform.Find("AS/Item").gameObject;
        btnClose = base.transform.Find("Root/btnClose").GetComponent<Button>();
        btnClear = base.transform.Find("Root/Right/btnClear").GetComponent<Button>();
        btnClear.transform.Find("Text").GetComponent<Text>().text = GameTool.LS("tkFhkr_uibornlucklist_1");
        btnComplete = base.transform.Find("Root/Left/btnComplete").GetComponent<Button>();
        btnComplete.transform.Find("Text").GetComponent<Text>().text = GameTool.LS("tkFhkr_uibornlucklist_2");
        btnComplete.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("tkFhkr_uibornlucklist_3"));
        inputSearch = base.transform.Find("Root/Left/inputSearch").GetComponent<InputField>();
        inputSearch.placeholder.GetComponent<Text>().text = GameTool.LS("tkFhkr_uibornlucklist_4");
    }

    private void UIEvent()
    {
        btnClose.onClick.AddListener((Action)delegate
        {
            g.ui.CloseUI(new UIType.UITypeBase(base.gameObject.name, UILayer.UI));
        });
        btnClear.onClick.AddListener((Action)delegate
        {
            selectLuck.Clear();
            UpdateRight();
            UpdateLeft();
        });
        btnComplete.onClick.AddListener((Action)BtnComplete);
        inputSearch.onEndEdit.AddListener((Action<string>)FeatureSearch);
    }

    private void InitData()
    {
        allLeftItems = new System.Collections.Generic.List<ConfRoleCreateFeatureItem>();
        Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem> list = g.conf.roleCreateFeature.allLuckList[1];
        int[] array = new int[3] { 2804, 2805, 2806 };
        System.Collections.Generic.List<ConfRoleCreateFeatureItem> list2 = new System.Collections.Generic.List<ConfRoleCreateFeatureItem>();
        Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem>.Enumerator enumerator = list.GetEnumerator();
        while (enumerator.MoveNext())
        {
            ConfRoleCreateFeatureItem current = enumerator.Current;
            if (current != null && current.weight > 0 && !array.Contains(current.id) && !list2.Contains(current))
            {
                list2.Add(current);
            }
        }
        list2.Sort(delegate (ConfRoleCreateFeatureItem luck1, ConfRoleCreateFeatureItem luck2)
        {
            if (luck1 == null || luck2 == null)
            {
                return 1;
            }
            return (luck1.level <= luck2.level) ? 1 : (-1);
        });
        allLeftItems.Add(g.conf.roleCreateFeature.GetItem(array[0]));
        allLeftItems.Add(g.conf.roleCreateFeature.GetItem(array[1]));
        allLeftItems.Add(g.conf.roleCreateFeature.GetItem(array[2]));
        allLeftItems.AddRange(list2);
        foreach (ConfRoleCreateFeatureItem allLeftItem in allLeftItems)
        {
            AddLeftItem(allLeftItem);
        }
        UpdateLeft();
    }

    private void UpdateRight()
    {
        for (int num = rightList.childCount - 1; num >= 0; num--)
        {
            UnityEngine.Object.Destroy(rightList.GetChild(num).gameObject);
        }
        foreach (int item in selectLuck)
        {
            AddRightItem(item);
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
                if (selectLuck.Contains(result))
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

    private void AddLeftItem(ConfRoleCreateFeatureItem featureItem)
    {
        int featureID;
        Toggle toggle;
        if (featureItem != null)
        {
            featureID = featureItem.id;
            GameObject gameObject = UnityEngine.Object.Instantiate(AS_Item, leftList);
            gameObject.name = featureID.ToString();
            GameObject gameObject2 = g.res.Load<GameObject>("UI/Item/BornLuckItem");
            GameObject obj = UnityEngine.Object.Instantiate(gameObject2, gameObject.transform);
            obj.name = gameObject2.name;
            UIBornLuckItem component = obj.GetComponent<UIBornLuckItem>();
            component.Init();
            component.InitData(featureItem);
            obj.transform.SetAsFirstSibling();
            toggle = gameObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((Action<bool>)OnValueChanged);
            component.imgBG.transform.GetComponent<UIEventListener>().onClick.AddListener((Action)delegate
            {
                toggle.isOn = !toggle.isOn;
            });
        }
        void OnValueChanged(bool isOn)
        {
            if (isOn)
            {
                if (selectLuck.Count >= 9 && !selectLuck.Contains(featureID))
                {
                    UITipItem.AddTip(GameTool.LS("tkFhkr_uibornlucklist_5"), 3f);
                    toggle.isOn = false;
                    return;
                }
                if (!selectLuck.Contains(featureID))
                {
                    selectLuck.Add(featureID);
                }
            }
            else if (selectLuck.Contains(featureID))
            {
                selectLuck.Remove(featureID);
            }
            UpdateRight();
        }
    }

    private void AddRightItem(int featureID)
    {
        ConfRoleCreateFeatureItem featureItem = g.conf.roleCreateFeature.GetItem(featureID);
        if (featureItem == null)
        {
            return;
        }
        GameObject gameObject = g.res.Load<GameObject>("UI/Item/BornLuckItem");
        GameObject obj = UnityEngine.Object.Instantiate(gameObject, rightList);
        obj.name = gameObject.name;
        UIBornLuckItem component = obj.GetComponent<UIBornLuckItem>();
        component.Init();
        component.InitData(featureItem);
        component.imgBG.transform.GetComponent<UIEventListener>().onClick.AddListener((Action)delegate
        {
            if (selectLuck.Contains(featureItem.id))
            {
                selectLuck.Remove(featureItem.id);
            }
            UpdateRight();
            UpdateLeft();
        });
    }

    private void FeatureSearch(string value)
    {
        for (int num = leftList.childCount - 1; num >= 0; num--)
        {
            UnityEngine.Object.Destroy(leftList.GetChild(num).gameObject);
        }
        if (string.IsNullOrEmpty(value))
        {
            foreach (ConfRoleCreateFeatureItem allLeftItem in allLeftItems)
            {
                AddLeftItem(allLeftItem);
            }
            UpdateLeft();
            return;
        }
        foreach (ConfRoleCreateFeatureItem allLeftItem2 in allLeftItems)
        {
            string text = GameTool.LS(allLeftItem2.name);
            if (text != null && text.Contains(value))
            {
                AddLeftItem(allLeftItem2);
            }
        }
        UpdateLeft();
    }

    private void BtnComplete()
    {
        tempSelectLuck = new System.Collections.Generic.List<int>(selectLuck);
        if (tempSelectLuck.Count == 0)
        {
            btnClose.onClick.Invoke();
            return;
        }
        UICreatePlayer uI = g.ui.GetUI<UICreatePlayer>(UIType.CreatePlayer);
        if (uI == null)
        {
            tempSelectLuck.Clear();
            btnClose.onClick.Invoke();
            return;
        }
        if (Patch_UICreatePlayerProperty.lockLucks != null)
        {
            System.Collections.Generic.Dictionary<int, bool> dictionary = new System.Collections.Generic.Dictionary<int, bool>();
            foreach (System.Collections.Generic.KeyValuePair<int, bool> lockLuck in Patch_UICreatePlayerProperty.lockLucks)
            {
                if (tempSelectLuck.Contains(lockLuck.Key))
                {
                    dictionary[lockLuck.Key] = lockLuck.Value;
                }
            }
            Patch_UICreatePlayerProperty.lockLucks = new System.Collections.Generic.Dictionary<int, bool>(dictionary);
        }
        btnClose.onClick.Invoke();
        uI.uiProperty.btnPropertyRandom.onClick.Invoke();
    }

    internal static bool Patch_Customize(ref Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem> __result)
    {
        if (!UILuckFilter.IsActive)
        {
            return false;
        }
        if (tempSelectLuck == null || tempSelectLuck.Count == 0)
        {
            return false;
        }
        __result = new Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem>();
        foreach (int item2 in tempSelectLuck)
        {
            ConfRoleCreateFeatureItem item = g.conf.roleCreateFeature.GetItem(item2);
            if (item != null)
            {
                __result.Add(item);
            }
        }
        tempSelectLuck.Clear();
        return true;
    }
}
