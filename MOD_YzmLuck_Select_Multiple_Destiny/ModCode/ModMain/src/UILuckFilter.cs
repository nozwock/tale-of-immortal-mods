using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_YzmLuck;

internal class UILuckFilter : UIBase
{
    private static GameObject goRoot;

    private int[] specialLuck = new int[5] { 9300201, 9300202, 9300203, 9300204, 9300205 };

    private Toggle itemTianming;

    private Toggle itemModLuck;

    private Toggle lv_item1;

    private Toggle lv_item2;

    private Toggle lv_item3;

    private Toggle lv_item4;

    private Toggle lv_item5;

    private Toggle lv_item6;

    private Toggle sp_Item1;

    private Toggle sp_Item2;

    private Toggle sp_Item3;

    private Toggle sp_Item4;

    private Toggle sp_Item5;

    private Button btnAllLuck;

    internal static bool IsActive
    {
        get
        {
            if (goRoot == null)
            {
                return false;
            }
            return goRoot.activeSelf;
        }
    }

    internal static Dictionary<int, bool> LuckFilter { get; set; }

    internal static List<int> SpecialLuck { get; set; }

    internal static bool TianMing { get; private set; }

    internal static bool ModLuck { get; private set; }

    internal static bool AllLevel { get; private set; }

    public UILuckFilter(IntPtr ptr)
        : base(ptr)
    {
    }

    public void OnDestroy()
    {
        TianMing = false;
        ModLuck = false;
        LuckFilter = null;
        goRoot = null;
    }

    public void Update()
    {
        UpdateSpecialLuck();
    }

    public void OnEnable()
    {
        UICreatePlayer uI = g.ui.GetUI<UICreatePlayer>(UIType.CreatePlayer);
        if (uI?.uiProperty == null)
        {
            return;
        }
        Transform transform = uI.uiProperty.goBornLuck.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            Toggle component = transform.GetChild(i).GetComponent<Toggle>();
            if (component != null)
            {
                component.isOn = false;
            }
        }
    }

    public void Awake()
    {
        goRoot = base.gameObject;
        AllLevel = true;
        SpecialLuck = new List<int>();
        LuckFilter = new Dictionary<int, bool>
        {
            [1] = false,
            [2] = false,
            [3] = false,
            [4] = false,
            [5] = false,
            [6] = false
        };
        FindUI();
    }

    private void FindUI()
    {
        btnAllLuck = base.transform.Find("btnAllLuck").GetComponent<Button>();
        btnAllLuck.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("tkFhkr_uiluckfilter_19"));
        btnAllLuck.onClick.AddListener((Action)delegate
        {
            g.ui.OpenUI(new UIType.UITypeBase("YzmLuck/BornLuckList", UILayer.UI)).gameObject.AddComponent<UIBornLuckList>();
        });
        string tip = GameTool.LS("tkFhkr_uiluckfilter_14");
        base.transform.Find("Levels")?.gameObject.AddComponent<UISkyTipEffect>().InitData(tip, new Vector3(0f, -2f));
        string tip2 = GameTool.LS("tkFhkr_uiluckfilter_15");
        base.transform.Find("SpecialLuck")?.gameObject.AddComponent<UISkyTipEffect>().InitData(tip2, new Vector3(0f, -2f));
        itemTianming = base.transform.Find("ItemTianMing").GetComponent<Toggle>();
        itemTianming.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_8");
        itemTianming.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("tkFhkr_uiluckfilter_16"), new Vector3(0f, -1.5f));
        itemTianming.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            TianMing = isOn;
        });
        itemModLuck = base.transform.Find("ItemModLuck").GetComponent<Toggle>();
        itemModLuck.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_17");
        itemModLuck.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("tkFhkr_uiluckfilter_18"), new Vector3(0f, -1.5f));
        itemModLuck.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            ModLuck = isOn;
            if (isOn)
            {
                lv_item1.isOn = false;
                lv_item2.isOn = false;
                lv_item3.isOn = false;
                lv_item4.isOn = false;
                lv_item5.isOn = false;
                lv_item6.isOn = false;
            }
        });
        lv_item1 = base.transform.Find("Levels/Item1").GetComponent<Toggle>();
        lv_item1.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_2");
        lv_item2 = base.transform.Find("Levels/Item2").GetComponent<Toggle>();
        lv_item2.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_3");
        lv_item3 = base.transform.Find("Levels/Item3").GetComponent<Toggle>();
        lv_item3.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_4");
        lv_item4 = base.transform.Find("Levels/Item4").GetComponent<Toggle>();
        lv_item4.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_5");
        lv_item5 = base.transform.Find("Levels/Item5").GetComponent<Toggle>();
        lv_item5.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_6");
        lv_item6 = base.transform.Find("Levels/Item6").GetComponent<Toggle>();
        lv_item6.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_7");
        sp_Item1 = base.transform.Find("SpecialLuck/Item1").GetComponent<Toggle>();
        sp_Item1.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_9");
        sp_Item2 = base.transform.Find("SpecialLuck/Item2").GetComponent<Toggle>();
        sp_Item2.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_10");
        sp_Item3 = base.transform.Find("SpecialLuck/Item3").GetComponent<Toggle>();
        sp_Item3.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_11");
        sp_Item4 = base.transform.Find("SpecialLuck/Item4").GetComponent<Toggle>();
        sp_Item4.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_12");
        sp_Item5 = base.transform.Find("SpecialLuck/Item5").GetComponent<Toggle>();
        sp_Item5.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uiluckfilter_13");
        lv_item1.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            LevelItem(isOn, 6);
        });
        lv_item2.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            LevelItem(isOn, 5);
        });
        lv_item3.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            LevelItem(isOn, 4);
        });
        lv_item4.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            LevelItem(isOn, 3);
        });
        lv_item5.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            LevelItem(isOn, 2);
        });
        lv_item6.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            LevelItem(isOn, 1);
        });
        sp_Item1.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            SpecialLuckItem(isOn, 9300201);
        });
        sp_Item2.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            SpecialLuckItem(isOn, 9300202);
        });
        sp_Item3.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            SpecialLuckItem(isOn, 9300203);
        });
        sp_Item4.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            SpecialLuckItem(isOn, 9300204);
        });
        sp_Item5.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            SpecialLuckItem(isOn, 9300205);
        });
        void LevelItem(bool isOn, int level)
        {
            if (isOn)
            {
                itemModLuck.isOn = false;
            }
            LuckFilter[level] = isOn;
            int num = 0;
            int num2 = 0;
            foreach (KeyValuePair<int, bool> item in LuckFilter)
            {
                if (item.Value)
                {
                    num++;
                }
                else
                {
                    num2++;
                }
            }
            if (num == LuckFilter.Count || num2 == LuckFilter.Count)
            {
                AllLevel = true;
            }
            else
            {
                AllLevel = false;
            }
        }
        static void SpecialLuckItem(bool isOn, int featureID)
        {
            if (isOn && !SpecialLuck.Contains(featureID))
            {
                SpecialLuck.Add(featureID);
            }
            else if (!isOn && SpecialLuck.Contains(featureID))
            {
                SpecialLuck.Remove(featureID);
            }
        }
    }

    internal void UpdateSpecialLuck()
    {
        try
        {
            UICreatePlayer uI = g.ui.GetUI<UICreatePlayer>(UIType.CreatePlayer);
            if (uI?.uiProperty == null)
            {
                return;
            }
            Transform transform = uI.uiProperty.goBornLuck.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                UIBornLuckItem uIBornLuckItem = child?.Find("Root/BornLuckItem")?.GetComponent<UIBornLuckItem>();
                int num = ((uIBornLuckItem?.item != null) ? uIBornLuckItem.item.id : 0);
                if (num != 0 && specialLuck.Contains(num))
                {
                    child.gameObject.SetActive(value: false);
                }
            }
        }
        catch
        {
        }
    }
}
