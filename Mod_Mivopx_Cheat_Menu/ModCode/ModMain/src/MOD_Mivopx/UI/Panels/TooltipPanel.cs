using TaleOfImmortalCheat.Localization;
using TaleOfImmortalCheat.UI;
using TaleOfImmortalCheat.UI.Panels;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace MOD_Mivopx.UI.Panels;

internal class TooltipPanel : Panel
{
	public GameObject Panel;

	public Text Label;

	public string Name { get; }

	public TooltipPanel(string name)
		: base(isStartedVisible: false)
	{
		Name = name;
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	~TooltipPanel()
	{
		UIRefreshManager.OnLanguageChanged -= UpdateUITexts;
	}

	internal override (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot)
	{
		ModMain.Log(string.Format(LocalizationHelper.T("other_msgs_tooltip_creating"), Name));
		GameObject contentHolder;
		GameObject gameObject = UIFactory.CreatePanel(Name + "-Tooltip", uiRoot, out contentHolder);
		UIFactory.SetLayoutGroup<VerticalLayoutGroup>(contentHolder, true, true, true, true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 1f);
		component.anchorMax = new Vector2(0.5f, 1f);
		component.pivot = new Vector2(0.5f, 1f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 120f);
		Text text = UIFactory.CreateLabel(gameObject, Name + "-Tooltip-Label", LocalizationHelper.T("other_ui_tooltip_placeholder"), TextAnchor.MiddleCenter);
		UIFactory.SetLayoutElement(text.gameObject, 90, flexibleWidth: 1000, minHeight: 25, flexibleHeight: 100000);
		Panel = gameObject;
		Label = text;
		ModMain.Log(string.Format(LocalizationHelper.T("other_msgs_tooltip_created"), Name));
		return (panelRoot: gameObject, draggableArea: null);
	}

	internal void TooltipFor(RectTransform targetRect, string label)
	{
		Label.text = label;
		AdjustTooltipPosition(targetRect);
	}

	private void AdjustTooltipPosition(RectTransform targetRect)
	{
		RectTransform component = Panel.GetComponent<RectTransform>();
		float num = 90f;
		Vector3 position = new Vector3(Screen.width / 2, (float)Screen.height - num, 0f);
		Vector3[] array = new Vector3[4];
		component.GetWorldCorners(array);
		float num2 = array[2].y - array[0].y;
		float num3 = array[2].x - array[0].x;
		float num4 = Screen.width;
		float num5 = Screen.height;
		if (position.x + num3 / 2f > num4)
		{
			position.x = num4 - num3 / 2f;
		}
		else if (position.x - num3 / 2f < 0f)
		{
			position.x = num3 / 2f;
		}
		if (position.y + num2 / 2f > num5)
		{
			position.y = num5 - num2 / 2f;
		}
		else if (position.y - num2 / 2f < 0f)
		{
			position.y = num2 / 2f;
		}
		component.position = position;
	}

	private void UpdateUITexts()
	{
	}
}
