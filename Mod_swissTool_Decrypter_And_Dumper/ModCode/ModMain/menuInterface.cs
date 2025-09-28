using EGameTypeData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MOD_swissTool
{
    public class menuInterface
    {
        public menuInterface(UILogin ui)
        {
            if (ui != null)
            {
                InitUI(ui);
            }
        }

        private void InitUI(UILogin ui)
        {
            // Create decryption button
            mkBtn(ui, "btnDecrypt", "Decrypt mod", new Vector2(-320f, -400f), (Action)delegate {
                CryptFile.Decrypt();
            });
            // Create encryption button
            mkBtn(ui, "btnEncrypt", "Encrypt mod", new Vector2(-320f, -455f), (Action)delegate {
                CryptFile.Encrypt2();
            });
            // Create data-dump button
            mkBtn(ui, "btnDumpData", "Dump Json-data", new Vector2(-320f, -510f), (Action)delegate {
                Console.WriteLine("Start dumping");
                DumpFile.dumpData(dlc: false); // Init
                Console.WriteLine("Finished dumping");
            });
            // Create data-dump button
            mkBtn(ui, "btnDumpDataDLC", "Dump DLC-data", new Vector2(-320f, -565f), (Action)delegate {
                Console.WriteLine("Start dumping");
                DumpFile.dumpData(dlc: true); // Init
                Console.WriteLine("Finished dumping");
            });
            // Create decrypt save data button
            mkBtn(ui, "btnDecryptSav", "Decrypt Save", new Vector2(-320f, -620f), (Action)delegate {
                CryptFile.DecryptSav();
            });
        }

        private void mkBtn(UILogin ui, string objName, string objText, Vector2 objPos, UnityEngine.Events.UnityAction objDeleg)
        {
            if (GameObject.Find(objName) == null)
            {
                GameObject DecryptBtn = UnityEngine.Object.Instantiate(ui.transform.Find("Root/btnGroup/G:btnPaperChange_En").gameObject);
                DecryptBtn.GetComponentInChildren<Text>().text = objText;
                DecryptBtn.transform.SetParent(ui.transform.Find("Root"), worldPositionStays: false);
                DecryptBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
                DecryptBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                DecryptBtn.GetComponent<RectTransform>().anchoredPosition = objPos;
                DecryptBtn.name = objName;
                Button DecryptBtnComp = DecryptBtn.GetComponent<Button>();
                DecryptBtnComp.onClick.RemoveAllListeners();
                DecryptBtnComp.onClick.AddListener(objDeleg);
            }
        }
    }
}
