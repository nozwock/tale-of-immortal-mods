using System;
using EGameTypeData;
using MelonLoader;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MOD_0VsKyr;

struct Config
{
    static bool _init = false;
    internal static MelonPreferences_Category categoryBase;
    internal static MelonPreferences_Entry<bool> isEnable;
    internal static MelonPreferences_Entry<bool> isSpendTime;

    static internal void Init()
    {
        if (_init)
            return;

        categoryBase = MelonPreferences.CreateCategory($"{ModMain.modNamespace.Replace("MOD_", "")}-Skip Manual Minigame");
        isEnable = categoryBase.CreateEntry("Enable Mod", true);
        isSpendTime = categoryBase.CreateEntry("Spend Time On Learning", true);

        _init = true;
    }
}

public class ModMain
{
    internal static readonly string modNamespace = typeof(ModMain).Namespace!;

    Il2CppSystem.Action<ETypeData> callOpenUIEnd;

    public void Init()
    {
        Config.Init();

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
        if (edata.uiType.uiName == UIType.MartialPropInfo.uiName)
        {
            if (!Config.isEnable.Value)
                return;

            var ui = edata.ui.GetComponent<UIMartialPropInfo>();

            var btnGray = ui.btnStudy.GetComponent<UIButtonGray>();
            if (btnGray != null && btnGray.group.alpha == btnGray.grayAlpha)
            {
                btnGray.ActiveGray(true); // Why does the game even calls it with isClick true??
            }

            // There's a OnStudyClick() too
            ui.btnStudy.onClick = new Button.ButtonClickedEvent();
            ui.btnStudy.onClick.AddListener((UnityAction)delegate
            {
                LearnSkill(ui.martialData, Config.isSpendTime.Value);
                var uiSuccess = g.ui.OpenUI<UIMartialLearnSuccess>(UIType.MartialLearnSuccess);
                uiSuccess.InitData(
                    String.Format(
                        GameTool.LS("wuxue_xuexitishi2").Split('\n')[0],
                        // This is colored name, not sure how to apply grade based color to text by ourselves
                        (ui.textName.IsActive() ? ui.textName : ui.textName_En).GetComponent<Text>().text
                    )
                );
                uiSuccess.btnOK.onClick.AddListener((UnityAction)delegate
                {
                    g.ui.CloseUI(ui);
                });
            });
        }
    }

    static void LearnSkill(DataProps.MartialData martialData, bool spendTime = true)
    {
        // Learned from
        // DramaFunction
        //     LearnSkillFF() // Used in UISpecialNpcVillage
        //         <LearnSkillFF>b__1()

        // FIXME: costMood isn't handled
        var unit = g.world.playerUnit;
        var study = new UnitActionPropMartialStudy(martialData.data);
        // study.checkProp = false;
        study.Init(unit);
        // Setting these afterwards since Init performs some operations on them
        if (!spendTime)
            study.day = 0;
        // Can use isOneStudy|isStudyComplete instead of setting exp too
        study.exp = int.MaxValue;
        unit.CreateAction(study);
    }

    internal static void Log(object s)
    {
        MelonLogger.Msg($"{modNamespace}: {s}");
    }
}
