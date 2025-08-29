using MOD_Mivopx;
using UnityEngine;
using UnityEngine.UI;

namespace TaleOfImmortalCheat.UI.Panels;

public abstract class Panel
{
	private static Material transparentMaterialMain = new Material(Shader.Find("UI/Default"));

	protected int HeightCompensation;

	private PanelDragger PanelDragger { get; set; }

	public GameObject PanelRoot { get; private set; }

	public bool IsVisible
	{
		get
		{
			if (!(PanelRoot == null))
			{
				return PanelRoot.active;
			}
			return false;
		}
		set
		{
			if (!(PanelRoot == null))
			{
				PanelDragger?.Reset();
				PanelRoot.SetActive(value);
			}
		}
	}

	public bool IsStartedVisisble { get; }

	public Panel(bool isStartedVisible)
	{
		IsStartedVisisble = isStartedVisible;
	}

	public virtual bool Update(bool allowDrag)
	{
		if (PanelRoot == null || PanelDragger == null)
		{
			return false;
		}
		if (allowDrag && IsVisible)
		{
			return PanelDragger.Update();
		}
		return false;
	}

	private void CreateUI()
	{
		if (PanelRoot != null)
		{
			return;
		}
		var (gameObject, gameObject2) = CreateUI(UIManager.UIRoot);
		if (gameObject2 != null)
		{
			RectTransform panel = gameObject.GetComponent<RectTransform>();
			PanelDragger = new PanelDragger(gameObject2.GetComponent<RectTransform>(), in panel, HeightCompensation);
		}
		Image[] array = gameObject.GetComponentsInChildren<Image>(includeInactive: true);
		if (transparentMaterialMain == null)
		{
			transparentMaterialMain = new Material(Shader.Find("UI/Default"));
		}
		if (array != null && transparentMaterialMain != null)
		{
			Image[] array2 = array;
			foreach (Image image in array2)
			{
				if (image != null && !image.GetComponent<Button>() && !image.GetComponent<Toggle>() && !image.GetComponent<Scrollbar>() && image.gameObject.name != "Checkmark" && !image.gameObject.name.Contains("Tooltip"))
				{
					image.material = transparentMaterialMain;
					image.color = new Color(0f, 0f, 0f, 0.3f);
				}
			}
		}
		else
		{
			ModMain.LogWarning("Failed to apply transparency to children at Panel.");
		}
		PanelRoot = gameObject;
		IsVisible = IsStartedVisisble;
	}

	public void Create()
	{
		CreateUI();
	}

	internal abstract (GameObject panelRoot, GameObject draggableArea) CreateUI(GameObject uiRoot);

	public virtual void OnGameWorldUpdate()
	{
	}

	public virtual void OnConfUpdate()
	{
	}
}
