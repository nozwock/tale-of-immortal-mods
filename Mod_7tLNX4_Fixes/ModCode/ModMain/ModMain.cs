using MelonLoader;
using EGameTypeData;
using UnityEngine.UI;

namespace MOD_7tLNX4
{
    public class ModMain
    {
        internal static readonly string modNamespace = typeof(ModMain).Namespace!;

        public void Init()
        {
            g.events.On(EGameType.OpenUIEnd, (System.Action<ETypeData>)OnOpenUIEnd);
        }

        public void Destroy()
        {
            g.events.Off(EGameType.OpenUIEnd, (System.Action<ETypeData>)OnOpenUIEnd);
        }

        private void FixSoulReaverEngDescription(UIBase ui)
        {
            // For some reason devs added the last description string here by mistake.
            // Could just build up the string ourselves but I think this is fine.
            var src = ui.transform.Find(
                "Root/G:srDesc/Viewport/G:goContentDesc/G:goDesc1/G:goDescMask/G:textDesc1_En")?.GetComponent<Text>();

            var dst = ui.transform.Find(
                "Root/G:srDesc/Viewport/G:goContentDesc/G:textDesc_En")?.GetComponent<Text>();

            if (src == null || dst == null)
                return;

            if (!dst.text.Contains(src.text))
            {
                dst.text += "\n" + src.text;
            }
        }

        private void OnOpenUIEnd(ETypeData e)
        {
            var edata = e.Cast<OpenUIEnd>();
            if (edata.uiType.uiName == UIType.PropInfoSoulDevourSword.uiName)
            {
                FixSoulReaverEngDescription(edata.ui);
            }
        }

        private static void Log(object s)
        {
            MelonLogger.Msg($"{modNamespace}: {s}");
        }
    }
}
