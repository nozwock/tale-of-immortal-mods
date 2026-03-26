using EGameTypeData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MOD_4ZvKd3
{
    public class ModMain
    {
        const string textWorshipCountName = "textWorshipCount";

        Il2CppSystem.Action<ETypeData> callOpenUIEnd;
        Il2CppSystem.Action<ETypeData> callCloseUIStart;

        public void Init()
        {
            callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
            callCloseUIStart = (Il2CppSystem.Action<ETypeData>)OnCloseUIStart;
            g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
            g.events.On(EGameType.CloseUIStart, callCloseUIStart);
        }

        public void Destroy()
        {
            g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);
            g.events.Off(EGameType.CloseUIStart, callCloseUIStart);
        }

        private void OnOpenUIEnd(ETypeData e)
        {
            if (e.Cast<OpenUIEnd>().uiType.uiName != UIType.ImmortalAncestralHall.uiName)
            {
                return;
            }

            var ui = g.ui.GetUI<UIImmortalAncestralHall>(UIType.ImmortalAncestralHall);
            if (ui == null)
            {
                return;
            }

            // MapBuild10008
            //     MapBuild10008.Data data
            //     MapBuild10008Data build10008Data
            //         ConfWorldBuilding10008Item building10008Item
            //         MapBuild10008 build10008

            CreateWorshipCountText(ui);

            var buildData = ui.build10008Data.build10008.data;
            var dataItem = ui.build10008Data.building10008Item;

            var originalCb = ui.btnChallenge.onClick;
            ui.btnChallenge.onClick = new Button.ButtonClickedEvent();
            ui.btnChallenge.onClick.AddListener((UnityAction)delegate
            {
                var isBlessingReceived = buildData.giveValue == 0 && buildData.lastGiveMonth != 0;
                if (buildData.giveValue < dataItem.consecrateAmount && !isBlessingReceived)
                {
                    buildData.lastGiveMonth = 0; // Allow worship
                }

                originalCb?.Invoke();
            });
        }

        void OnCloseUIStart(ETypeData e)
        {
            var edata = e.Cast<CloseUIStart>();
            if (edata.uiType.uiName == UIType.PropSelect.uiName)
            {
                var ui = g.ui.GetUI<UIImmortalAncestralHall>(UIType.ImmortalAncestralHall);
                if (ui == null)
                    return;

                var buildData = ui.build10008Data.build10008.data;
                var dataItem = ui.build10008Data.building10008Item;

                var root = ui.btnChallenge.gameObject.transform.parent;
                var txt = root.Find(textWorshipCountName)?.GetComponent<Text>();
                if (txt != null)
                {
                    txt.text = $"{buildData.giveValue}/{dataItem.consecrateAmount}";
                }
            }
        }

        static Text CreateWorshipCountText(UIImmortalAncestralHall ui)
        {
            if (!ui.btnChallenge.IsActive())
                return null;

            var root = ui.btnChallenge.gameObject.transform.parent;

            var go = root.Find(textWorshipCountName)?.gameObject;
            if (go == null)
            {
                var srcText = ui.btnChallenge.GetComponentInChildren<Text>(true);
                go = GameObject.Instantiate(srcText.gameObject, root);
                go.name = textWorshipCountName;
            }

            var buildData = ui.build10008Data.build10008.data;
            var dataItem = ui.build10008Data.building10008Item;

            var txt = go.GetComponent<Text>();
            txt.text = $"{buildData.giveValue}/{dataItem.consecrateAmount}";

            var newRect = go.GetComponent<RectTransform>();
            var btnRect = ui.btnChallenge.GetComponent<RectTransform>();

            newRect.anchorMin = btnRect.anchorMin;
            newRect.anchorMax = btnRect.anchorMax;
            newRect.pivot = btnRect.pivot;
            newRect.sizeDelta = btnRect.sizeDelta;

            newRect.anchoredPosition = btnRect.anchoredPosition + new Vector2(0, -(btnRect.rect.height + 10f));

            return txt;
        }
    }
}
