using EGameTypeData;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MOD_4ZvKd3
{
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
    }
}
