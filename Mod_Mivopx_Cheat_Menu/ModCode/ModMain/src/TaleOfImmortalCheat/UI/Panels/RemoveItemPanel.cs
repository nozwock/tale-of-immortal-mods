using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.WebPages;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;
using TaleOfImmortalCheat.Localization;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets;

namespace TaleOfImmortalCheat.UI.Panels;

public class RemoveItemPanel : Panel
{
    class InventoryItem
    {
        public int Id;
        public int Quantity;          // actual count in inventory when added to list
        public int SelectedQuantity;  // how much player wants to remove
    }

    public const string PanelName = "RemoveItem";

    private System.Collections.Generic.Dictionary<int, ButtonRef> removeItemButtons = [];

    private GameObject searchGroup;

    private GameObject searchScrollView;

    private System.Collections.Generic.Dictionary<int, GameObject> removeItemGroups = [];

    private System.Collections.Generic.Dictionary<int, InventoryItem> removeItem = [];

    private GameObject itemsScrollView;

    private static bool _isCHActive;

    private InputFieldRef searchInput;

    private ButtonRef showAllButton;

    private Toggle toggleShowChineseName;

    private Text toggleShowChineseNameText;

    private ButtonRef removeItemButton;

    private ButtonRef clearButton;

    private Text titleBarText;

    public static bool IsCHActive
    {
        get
        {
            return _isCHActive;
        }
        set
        {
            _isCHActive = value;
        }
    }

    public RemoveItemPanel()
        : base(isStartedVisible: false)
    {
        UIRefreshManager.OnLanguageChanged += UpdateUITexts;
    }

    ~RemoveItemPanel()
    {
        UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
    }

    internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
    {
        Debug.Log("Creating Remove Items panel");
        GameObject contentHolder;
        GameObject gameObject = UIFactory.CreatePanel("RemoveItem", uiRoot, out contentHolder);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, false, true, true);
        GameObject gameObject2 = UIHelper.CreateTitleBar(contentHolder, delegate
        {
            base.IsVisible = false;
        }, GetTitleBarText());
        titleBarText = gameObject2.GetComponentInChildren<Text>();
        RectTransform component = gameObject.GetComponent<RectTransform>();
        component.anchorMin = new Vector2(0.3f, 0.3f);
        component.anchorMax = new Vector2(0.8f, 0.8f);
        component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 825f);
        component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 800f);
        GameObject parent = UIFactory.CreateHorizontalGroup(contentHolder, "RemoveItem-h-group", forceExpandWidth: true, forceExpandHeight: true, childControlWidth: true, childControlHeight: true);
        UIFactory.CreatePanel("RemoveItem-search-panel", parent, out var contentHolder2);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder2, true, false, true, true);
        searchInput = UIFactory.CreateInputField(contentHolder2, "RemoveItem-search", LocalizationHelper.T("panel_searchitem_input_placeholder"));
        searchInput.Component.onEndEdit.AddListener(OnSearch);
        AutoSliderScrollbar autoScrollbar;
        GameObject gameObject3 = UIFactory.CreateScrollView(contentHolder2, "RemoveItem-search-panel-scroll-view", out searchScrollView, out autoScrollbar);
        int? minWidth = 400;
        int? preferredWidth = 400;
        int? flexibleWidth = 0;
        UIFactory.SetLayoutElement(gameObject3, minWidth, null, flexibleWidth, null, preferredWidth);
        showAllButton = CreateLocalizedButton(contentHolder2, "RemoveItem-show-all", "panel_searchitem_button_show_all", delegate
        {
            OnSearch("Show All");
        });
        UIFactory.SetLayoutElement(showAllButton.Component.gameObject, null, 35, null, 0);
        UIFactory.CreateToggle(contentHolder, LocalizationHelper.T("panel_searchitem_toggle_chinese_name"), out toggleShowChineseName, out toggleShowChineseNameText);
        toggleShowChineseName.isOn = false;
        toggleShowChineseNameText.text = LocalizationHelper.T("panel_searchitem_toggle_chinese_name");
        Action<bool> action = delegate (bool v_ShowChineseName)
        {
            IsCHActive = v_ShowChineseName;
            if (v_ShowChineseName)
            {
                ModMain.LogTip(LocalizationHelper.T("panel_searchitem_status_chinese_name_enabled"));
            }
            else
            {
                ModMain.LogTip(LocalizationHelper.T("panel_searchitem_status_chinese_name_disabled"));
            }
        };
        toggleShowChineseName.onValueChanged.AddListener(action);
        GameObject contentHolder3;
        GameObject parent2 = UIFactory.CreatePanel("RemoveItem-rewards-panel", parent, out contentHolder3);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder3, true, false, true, true);
        UIFactory.CreateScrollView(parent2, "RemoveItem-rewards-panel-items", out itemsScrollView, out var autoScrollbar2);
        GameObject gameObject4 = itemsScrollView;
        int? minWidth2 = 400;
        flexibleWidth = 400;
        preferredWidth = 0;
        UIFactory.SetLayoutElement(gameObject4, minWidth2, null, preferredWidth, null, flexibleWidth);
        autoScrollbar2.Enabled = true;
        UIFactory.CreatePanel("RemoveItem-rewards-panel-reward-button-panel", contentHolder3, out var contentHolder4);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder4, true, false, true, true);
        removeItemButton = CreateLocalizedButton(contentHolder4, "RemoveItem-remove-panel-remove-button-panel-button", "panel_removeitem_button_remove", delegate
        {
            foreach (var invItem in removeItem.Values)
            {
                // Remove item
                Game.WorldManager.Value.playerUnit.data.CostPropItem(invItem.Id, invItem.SelectedQuantity);

                // Refresh UI
                invItem.Quantity -= invItem.SelectedQuantity;
                if (invItem.Quantity <= 0)
                {
                    // no more left - remove from UI
                    RemoveItemQuantityGroup(invItem.Id);
                }
                else
                {
                    // also clamp SelectedQuantity if needed
                    if (invItem.SelectedQuantity > invItem.Quantity)
                        invItem.SelectedQuantity = invItem.Quantity;

                    if (removeItemButtons.TryGetValue(invItem.Id, out var button))
                    {
                        button.ButtonText.text = button.ButtonText.text.Substring(0, button.ButtonText.text.LastIndexOf("x")) + $"x{invItem.Quantity}";
                    }

                    // update label with new Quantity
                    if (removeItemGroups.TryGetValue(invItem.Id, out var groupObj))
                    {
                        var labelText = groupObj.transform.Find($"item-quantity-group-{invItem.Id}-label")?.GetComponent<Text>();
                        if (labelText != null)
                            labelText.text = labelText.text.Substring(0, labelText.text.LastIndexOf("x")) + $"x{invItem.Quantity}";

                        // Update the quantity input field
                        var inputField = groupObj.transform.Find($"item-quantity-group-{invItem.Id}-quantity")?.GetComponent<InputField>();
                        if (inputField != null)
                            inputField.text = invItem.SelectedQuantity.ToString();
                    }
                }
            }

            ModMain.LogTip("Selected items removed.");
        }, Color.gray);
        UIFactory.SetLayoutElement(removeItemButton.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 35);
        RuntimeHelper.SetColorBlock(removeItemButton.Component, new Color(0.1f, 0.3f, 0.1f), new Color(0.2f, 0.5f, 0.2f), new Color(0.1f, 0.2f, 0.1f), new Color(0.2f, 0.2f, 0.2f));
        clearButton = CreateLocalizedButton(contentHolder4, "RemoveItem-rewards-panel-reward-button-panel-clearbutton", "panel_searchitem_button_clear", delegate
        {
            foreach (int item in new System.Collections.Generic.List<int>(removeItemGroups.Keys))
            {
                RemoveItemQuantityGroup(item);
            }
            if (searchGroup != null)
            {
                UnityEngine.Object.Destroy(searchGroup);
                searchGroup = UIFactory.CreateVerticalGroup(searchScrollView, "RemoveItem-search-group", forceWidth: true, forceHeight: false, childControlWidth: true, childControlHeight: false, 1);
            }
            removeItemButtons.Clear();
            ModMain.Log("Cleared all rewards and reset search.");
        }, Color.gray);
        UIFactory.SetLayoutElement(clearButton.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 35);
        RuntimeHelper.SetColorBlock(clearButton.Component, new Color(1f, 44f / 85f, 0.14901961f), new Color(1f, 0.6313726f, 0.34901962f), new Color(1f, 38f / 51f, 28f / 51f), Color.gray);
        return (panelRoot: gameObject, draggableArea: gameObject2);
    }

    private ButtonRef CreateLocalizedButton(GameObject parent, string name, string textKey, Action onClick, Color? color = null)
    {
        ButtonRef buttonRef = UIFactory.CreateButton(parent, name, LocalizationHelper.T(textKey), color);
        buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, onClick);
        return buttonRef;
    }

    private string GetTitleBarText()
    {
        string text = LocalizationHelper.T("common_cheatpanel");
        return "<b>" + text + "</b> - " + LocalizationHelper.T("panel_removeitem_title");
    }

    private void UpdateUITexts()
    {
        if (titleBarText != null)
        {
            titleBarText.text = GetTitleBarText();
        }
        if (searchInput != null)
        {
            searchInput.PlaceholderText.text = LocalizationHelper.T("panel_searchitem_input_placeholder");
        }
        if (showAllButton != null)
        {
            showAllButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_searchitem_button_show_all");
        }
        if (toggleShowChineseNameText != null)
        {
            toggleShowChineseNameText.text = LocalizationHelper.T("panel_searchitem_toggle_chinese_name");
        }
        if (removeItemButton != null)
        {
            removeItemButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_removeitem_button_remove");
        }
        if (clearButton != null)
        {
            clearButton.Component.GetComponentInChildren<Text>().text = LocalizationHelper.T("panel_searchitem_button_clear");
        }
    }

    private void OnSearch(string value)
    {
        try
        {
            removeItemButtons.Clear();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            if (searchGroup != null)
            {
                UnityEngine.Object.Destroy(searchGroup);
            }

            searchGroup = UIFactory.CreateVerticalGroup(searchScrollView, "RemoveItem-search-group", forceWidth: true, forceHeight: false, childControlWidth: true, childControlHeight: false, 1);

            var propData = Game.WorldManager.Value.playerUnit.data.unitData.propData;
            if (propData == null)
            {
                ModMain.LogTip("Inventory data null", "WARNING");
                return;
            }

            var equippedIds = Game.WorldManager.Value.playerUnit.data.unitData.GetAllEquipPropsSoleID();
            foreach (var propsData in propData.allProps)
            {
                if (propsData == null) continue;

                // Don't allow removal of equipped items and some others
                if (equippedIds.Contains(propsData.soleID) ||
                    propsData.propsID == 10001 ||
                    propsData.propsID == 10011 ||
                    propsData.propsID == 10041)
                {
                    continue;
                }

                if (!Game.ConfMgr.localText.allText.ContainsKey(propsData.propsItem.name))
                    continue;

                var confLocalTextItem = Game.ConfMgr.localText.allText[propsData.propsItem.name];
                if (string.IsNullOrEmpty(confLocalTextItem.en))
                    confLocalTextItem.en = confLocalTextItem.ch;

                bool matches = value.Equals("Show All", StringComparison.OrdinalIgnoreCase)
                    || Regex.IsMatch(confLocalTextItem.en, value, RegexOptions.IgnoreCase)
                    || Regex.IsMatch(confLocalTextItem.ch, value, RegexOptions.IgnoreCase);

                if (matches)
                {
                    string label = IsCHActive
                        ? $"{confLocalTextItem.en} ({confLocalTextItem.ch}) x{propsData.propsCount}"
                        : $"{confLocalTextItem.en} x{propsData.propsCount}";

                    CreateItemButton(propsData.propsItem.name, propsData.propsID, label, propsData.propsCount);
                }
            }
        }
        catch (Exception ex)
        {
            ModMain.LogWarning("Search Error: " + ex.Message + ":" + ex.StackTrace);
        }
        ModMain.Log("Searched complete.");
    }

    private void CreateItemButton(string itemName, int itemId, string name, int quantity)
    {
        ButtonRef btn = UIFactory.CreateButton(searchGroup, "RemoveItem-button-" + itemName, name);
        UIFactory.SetLayoutElement(btn.Component.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 35, flexibleWidth: 999);
        removeItemButtons[itemId] = btn;
        if (removeItem.ContainsKey(itemId))
        {
            RuntimeHelper.SetColorBlock(btn.Component, Color.Lerp(Color.gray, Color.green, 0.3f));
        }
        else
        {
            RuntimeHelper.SetColorBlock(btn.Component, Color.gray);
        }
        btn.OnClick = delegate
        {
            if (removeItem.ContainsKey(itemId))
            {
                RuntimeHelper.SetColorBlock(btn.Component, Color.gray);
                RemoveItemQuantityGroup(itemId);
            }
            else
            {
                RuntimeHelper.SetColorBlock(btn.Component, Color.Lerp(Color.gray, Color.green, 0.3f));
                var item = new InventoryItem
                {
                    Id = itemId,
                    SelectedQuantity = quantity,
                    Quantity = quantity
                };
                CreateItemQuantityGroup(item, name);
            }
        };
    }

    private void CreateItemQuantityGroup(InventoryItem invItem, string label)
    {
        removeItem.Add(invItem.Id, invItem);

        GameObject gameObject = UIFactory.CreateHorizontalGroup(itemsScrollView, $"item-quantity-group-{invItem.Id}", forceExpandWidth: true, forceExpandHeight: true, childControlWidth: true, childControlHeight: true, 0, new Vector4(3f, 3f, 15f, 3f));
        int? minWidth = 25;
        int? minHeight = 30;
        int? flexibleHeight = 0;
        UIFactory.SetLayoutElement(gameObject, minWidth, minHeight, null, flexibleHeight);

        ButtonRef buttonRef = UIFactory.CreateButton(gameObject, "HideButton", "X");
        buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, (Action)delegate
        {
            RemoveItemQuantityGroup(invItem.Id);
        });

        GameObject gameObject2 = buttonRef.Component.gameObject;
        int? minWidth2 = 25;
        flexibleHeight = 0;
        int? preferredWidth = 25;
        UIFactory.SetLayoutElement(gameObject2, minWidth2, null, flexibleHeight, null, preferredWidth);
        RuntimeHelper.SetColorBlock(buttonRef.Component, new Color(1f, 0.2f, 0.2f), new Color(1f, 0.6f, 0.6f), new Color(0.3f, 0.1f, 0.1f));

        Text buttonText = buttonRef.ButtonText;
        buttonText.color = Color.white;
        buttonText.resizeTextForBestFit = true;
        buttonText.resizeTextMinSize = 14;
        buttonText.resizeTextMaxSize = 14;

        GameObject gameObject3 = UIFactory.CreateLabel(gameObject, $"item-quantity-group-{invItem.Id}-label", label, TextAnchor.MiddleLeft, default(Color), supportRichText: false, 15).gameObject;
        preferredWidth = 5000;
        UIFactory.SetLayoutElement(gameObject3, null, null, preferredWidth);

        InputFieldRef quantityInput = UIFactory.CreateInputField(gameObject, $"item-quantity-group-{invItem.Id}-quantity", LocalizationHelper.T("panel_searchitem_input_quantity_placeholder"));
        GameObject gameObject4 = quantityInput.Component.gameObject;
        int? minWidth3 = 100;
        preferredWidth = 0;
        flexibleHeight = 100;
        UIFactory.SetLayoutElement(gameObject4, minWidth3, null, preferredWidth, null, flexibleHeight);

        quantityInput.Text = invItem.Quantity.ToString();

        quantityInput.Component.onEndEdit.AddListener(delegate (string value)
        {
            if (int.TryParse(value, out var result))
            {
                // clamp to [1, maxQuantity]
                if (result < 1) result = 1;
                if (result > invItem.Quantity) result = invItem.Quantity;

                invItem.SelectedQuantity = result; // Update
                quantityInput.Text = result.ToString();
            }
            else
            {
                quantityInput.Text = invItem.SelectedQuantity.ToString();
            }
        });
        removeItemGroups.Add(invItem.Id, gameObject);
    }

    private void RemoveItemQuantityGroup(int itemId)
    {
        // UnityEngine.Object.Destroy(removeItemGroups[itemId]);
        removeItem.Remove(itemId);
        UnityEngine.Object.Destroy(removeItemGroups[itemId]);
        removeItemGroups.Remove(itemId);
        if (removeItemButtons.TryGetValue(itemId, out var button))
        {
            UnityEngine.Object.Destroy(button.GameObject);
            removeItemButtons.Remove(itemId);
        }
    }
}
