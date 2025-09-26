using System;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_YzmLuck;

internal class UIEffectDisplay : UIBase
{
    internal static readonly string dataGroup = "MOD_YzmLuck";

    private Toggle item1;

    private Toggle item2;

    private Toggle item3;

    public UIEffectDisplay(IntPtr ptr)
        : base(ptr)
    {
    }

    public void Start()
    {
        ItemState();
    }

    public void Awake()
    {
        base.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("tkFhkr_uieffectdisplay_4"), new Vector3(0f, -1f));
        item1 = base.transform.Find("Item1").GetComponent<Toggle>();
        item1.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uieffectdisplay_1");
        item2 = base.transform.Find("Item2").GetComponent<Toggle>();
        item2.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uieffectdisplay_2");
        item3 = base.transform.Find("Item3").GetComponent<Toggle>();
        item3.transform.Find("Label").GetComponent<Text>().text = GameTool.LS("tkFhkr_uieffectdisplay_3");
        item1.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            OnValueChange("ui_yinyangyan", isOn);
        });
        item2.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            OnValueChange("Jueseqiyun", isOn);
        });
        item3.onValueChanged.AddListener((Action<bool>)delegate (bool isOn)
        {
            OnValueChange("shihunjian_jianzhen", isOn);
        });
        static void OnValueChange(string effectName, bool display)
        {
            int value = (display ? 1 : 0);
            g.data.obj.SetString(dataGroup, effectName, value);
            DisplayEffect(effectName);
        }
    }

    internal static void DelayHandler()
    {
        g.timer.Time((Action)delegate
        {
            try
            {
                DisplayEffect("ui_yinyangyan");
                DisplayEffect("Jueseqiyun");
                DisplayEffect("shihunjian_jianzhen");
            }
            catch (Exception ex)
            {
                Tool.Error(ex);
            }
        }, 0.5f);
    }

    private static void DisplayEffect(string effectName)
    {
        if (!g.data.obj.ContainsKey(dataGroup, effectName))
        {
            return;
        }
        bool display = true;
        if (g.data.obj.GetInt(dataGroup, effectName) == 0)
        {
            display = false;
        }
        UIPlayerInfo uI = g.ui.GetUI<UIPlayerInfo>(UIType.PlayerInfo);
        if (uI != null)
        {
            if (uI.uiProperty.rimgPlayer.gameObject.activeInHierarchy)
            {
                Handler(uI.uiProperty.rimgPlayer.transform);
            }
            if (uI.uiSkill.rimgPlayer.gameObject.activeInHierarchy)
            {
                Handler(uI.uiSkill.rimgPlayer.transform);
            }
            if (uI.uiArt.rimgPlayer.gameObject.activeInHierarchy)
            {
                Handler(uI.uiArt.rimgPlayer.transform);
            }
        }
        UIBigPortraitModel uI2 = g.ui.GetUI<UIBigPortraitModel>(UIType.BigPortraitModel);
        if (uI2 != null)
        {
            Handler(uI2.rimgUnit.transform);
        }
        void Handler(Transform rimgPlayer)
        {
            Transform transform = rimgPlayer?.Find(effectName);
            if (transform != null)
            {
                transform.gameObject.SetActive(display);
            }
        }
    }

    private void ItemState()
    {
        UIPlayerInfo uI = g.ui.GetUI<UIPlayerInfo>(UIType.PlayerInfo);
        if (uI?.uiProperty != null)
        {
            Transform obj = uI.uiProperty.rimgPlayer.transform;
            if (obj.Find("ui_yinyangyan") != null && g.data.obj.ContainsKey(dataGroup, "ui_yinyangyan"))
            {
                item1.isOn = g.data.obj.GetInt(dataGroup, "ui_yinyangyan") == 1;
            }
            if (obj.Find("Jueseqiyun") != null && g.data.obj.ContainsKey(dataGroup, "Jueseqiyun"))
            {
                item2.isOn = g.data.obj.GetInt(dataGroup, "Jueseqiyun") == 1;
            }
            if (obj.Find("shihunjian_jianzhen") != null && g.data.obj.ContainsKey(dataGroup, "shihunjian_jianzhen"))
            {
                item3.isOn = g.data.obj.GetInt(dataGroup, "shihunjian_jianzhen") == 1;
            }
        }
    }
}
