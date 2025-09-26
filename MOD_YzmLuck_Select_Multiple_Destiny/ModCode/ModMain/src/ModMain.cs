using System;
using System.Collections.Generic;
using System.Reflection;
using EGameTypeData;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_YzmLuck;

public class ModMain
{
    private TimerCoroutine corUpdate;

    private static HarmonyLib.Harmony harmony;

    private TestMod testMod;

    private Il2CppSystem.Action<ETypeData> openUIEndCall;

    private Il2CppSystem.Action<ETypeData> intoWorldCall;

    public void Init()
    {
        if (harmony != null)
        {
            harmony.UnpatchSelf();
            harmony = null;
        }
        if (harmony == null)
        {
            harmony = new HarmonyLib.Harmony("MOD_YzmLuck");
        }
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        corUpdate = g.timer.Frame((System.Action)OnUpdate, 1, loop: true);
        ClassInjector.RegisterTypeInIl2Cpp<UILuckFilter>();
        ClassInjector.RegisterTypeInIl2Cpp<UIEffectDisplay>();
        ClassInjector.RegisterTypeInIl2Cpp<UIBornLuckList>();
        ClassInjector.RegisterTypeInIl2Cpp<UIFateFeatureChange>();
        openUIEndCall = (System.Action<ETypeData>)OnOpenUIEnd;
        g.events.On(EGameType.OpenUIEnd, openUIEndCall, -1);
        intoWorldCall = (System.Action<ETypeData>)OnIntoWorld;
        g.events.On(EGameType.IntoWorld, intoWorldCall, -1);
        if (TestMod.IsTest)
        {
            testMod = new TestMod();
            testMod.OnModInit();
        }
    }

    public void Destroy()
    {
        g.timer.Stop(corUpdate);
        g.events.Off(EGameType.OpenUIEnd, openUIEndCall);
        g.events.Off(EGameType.IntoWorld, intoWorldCall);
        if (testMod != null)
        {
            testMod.OnModDestroy();
        }
    }

    private void OnUpdate()
    {
    }

    private void OnOpenUIEnd(ETypeData e)
    {
        OpenUIEnd openUIEnd = e.Cast<OpenUIEnd>();
        if (openUIEnd == null)
        {
            return;
        }
        string uiName = openUIEnd.uiType.uiName;
        try
        {
            if (uiName == UIType.CreatePlayer.uiName)
            {
                OnOpenUICreatePlayer();
            }
            else if (uiName == UIType.PlayerInfo.uiName)
            {
                UIPlayerInfo_TianMingEffect();
            }
        }
        catch (System.Exception ex)
        {
            Tool.Error(ex);
        }
    }

    private void OnIntoWorld(ETypeData e)
    {
        try
        {
            SpecialLuckCreate();
        }
        catch (System.Exception ex)
        {
            Tool.Error(ex);
        }
    }

    private void SpecialLuckCreate()
    {
        try
        {
            if (UILuckFilter.SpecialLuck == null || UILuckFilter.SpecialLuck.Count == 0)
            {
                return;
            }
            System.Collections.Generic.List<DataUnit.LuckData> list = new System.Collections.Generic.List<DataUnit.LuckData>(g.world.playerUnit.data.unitData.propertyData.bornLuck);
            foreach (int featureID in UILuckFilter.SpecialLuck)
            {
                if (list.Find((DataUnit.LuckData v) => v != null && v.id == featureID) == null && g.conf.roleCreateFeature.GetItem(featureID) != null)
                {
                    UnitActionLuckAdd unitActionLuckAdd = new UnitActionLuckAdd(featureID);
                    unitActionLuckAdd.Init(g.world.playerUnit);
                    unitActionLuckAdd.Create();
                    list.Add(unitActionLuckAdd.luckData);
                    Il2CppSystem.Collections.Generic.List<DataUnit.LuckData> addLuck = g.world.playerUnit.data.unitData.propertyData.addLuck;
                    if (addLuck.Contains(unitActionLuckAdd.luckData))
                    {
                        addLuck.Remove(unitActionLuckAdd.luckData);
                    }
                }
            }
            g.world.playerUnit.data.unitData.propertyData.bornLuck = list.ToArray();
            UILuckFilter.SpecialLuck = null;
        }
        catch (System.Exception ex)
        {
            Tool.Error(ex);
        }
    }

    private void UIPlayerInfo_TianMingEffect()
    {
        UIPlayerInfo uI = g.ui.GetUI<UIPlayerInfo>(UIType.PlayerInfo);
        if (uI == null)
        {
            return;
        }
        UIEffectDisplay.DelayHandler();
        string text = "YzmLuck_EffectDisplay";
        Transform parent = uI.uiProperty.btnBigPortrait.transform.parent;
        GameObject goEffectDisplay = parent.Find(text)?.gameObject;
        if (goEffectDisplay == null)
        {
            GameObject original = g.res.Load<GameObject>("UI/YzmLuck/EffectDisplay");
            goEffectDisplay = UnityEngine.Object.Instantiate(original, parent);
            goEffectDisplay.name = text;
            goEffectDisplay.GetComponent<RectTransform>().anchoredPosition += new Vector2(-80f, 230f);
            goEffectDisplay.AddComponent<UIEffectDisplay>();
            goEffectDisplay.SetActive(value: false);
            uI.uiProperty.btnBigPortrait.onClick.AddListener((System.Action)delegate
            {
                goEffectDisplay?.SetActive(!goEffectDisplay.active);
            });
        }
        uI.tglSkill.onValueChanged.AddListener((System.Action<bool>)delegate (bool isOn)
        {
            if (isOn)
            {
                UIEffectDisplay.DelayHandler();
            }
        });
        uI.tglArt.onValueChanged.AddListener((System.Action<bool>)delegate (bool isOn)
        {
            if (isOn)
            {
                UIEffectDisplay.DelayHandler();
            }
        });
        uI.tglPrpperty.onValueChanged.AddListener((System.Action<bool>)delegate (bool isOn)
        {
            if (isOn)
            {
                UIEffectDisplay.DelayHandler();
            }
        });
    }

    private void OnOpenUICreatePlayer()
    {
        UICreatePlayer uICreatePlayer = g.ui.GetUI<UICreatePlayer>(UIType.CreatePlayer);
        GameObject goLuckConfig;
        string panelName;
        string goLuckFilterName;
        if (uICreatePlayer?.uiProperty != null)
        {
            goLuckConfig = null;
            panelName = "YzmLuck_LuckFilter";
            if (uICreatePlayer.uiProperty.btnPropertyRandom.transform.parent.Find(panelName) == null)
            {
                CreatePanel();
            }
            goLuckFilterName = "YzmLuck_BtnLuckFilter";
            if (uICreatePlayer.uiProperty.btnPropertyRandom.transform.parent.Find(goLuckFilterName) == null)
            {
                CreateBtn();
            }
        }
        void CreateBtn()
        {
            GameObject gameObject = uICreatePlayer.uiProperty.btnPropertyRandom.gameObject;
            GameObject goBtnLuckFilter = UnityEngine.Object.Instantiate(gameObject, gameObject.transform.parent);
            goBtnLuckFilter.name = goLuckFilterName;
            goBtnLuckFilter.SetActive(value: true);
            goBtnLuckFilter.GetComponent<RectTransform>().anchoredPosition += new Vector2(100f, 0f);
            goBtnLuckFilter.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("tkFhkr_modmain_1"));
            goBtnLuckFilter.transform.GetChild(1).gameObject.SetActive(value: false);
            GameObject gameObject2 = UnityEngine.Object.Instantiate(goBtnLuckFilter.transform.GetChild(0).gameObject, goBtnLuckFilter.transform);
            gameObject2.name = "Icon";
            gameObject2.SetActive(value: true);
            gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 40f);
            gameObject2.GetComponent<Image>().sprite = SpriteTool.GetSprite("Common", "tkFhkr_suo");
            Button component = goBtnLuckFilter.GetComponent<Button>();
            component.onClick.RemoveAllListeners();
            component.onClick.AddListener((System.Action)delegate
            {
                if (goLuckConfig != null)
                {
                    bool active = goLuckConfig.active;
                    goLuckConfig.SetActive(!active);
                }
                goBtnLuckFilter.transform.Find("G:goPropertyTip")?.gameObject.SetActive(value: false);
            });
        }
        void CreatePanel()
        {
            GameObject gameObject = g.res.Load<GameObject>("UI/YzmLuck/LuckFilter");
            if (!(gameObject == null))
            {
                goLuckConfig = UnityEngine.Object.Instantiate(gameObject, uICreatePlayer.uiProperty.btnPropertyRandom.transform.parent);
                goLuckConfig.name = panelName;
                goLuckConfig.SetActive(value: false);
                goLuckConfig.GetComponent<RectTransform>().anchoredPosition += new Vector2(400f, 320f);
                goLuckConfig.AddComponent<UILuckFilter>();
            }
        }
    }

    private void VersionTips()
    {
        string key = "v1";
        if (g.data.obj.GetInt(UIEffectDisplay.dataGroup, key) == 0)
        {
            string info = GameTool.LS("tkFhkr_modmain_2");
            g.ui.OpenUI<UITextInfo>(UIType.TextInfo).InitData(GameTool.LS("common_tishi"), info);
            g.data.obj.SetString(UIEffectDisplay.dataGroup, key, 1);
        }
    }
}
