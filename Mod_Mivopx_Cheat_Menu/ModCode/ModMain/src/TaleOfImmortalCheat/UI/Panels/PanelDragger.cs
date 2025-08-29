using System;
using UnityEngine;
using UniverseLib.Input;

namespace TaleOfImmortalCheat.UI.Panels;

internal class PanelDragger
{
	private static PanelDragger beingDragged;

	private readonly RectTransform DraggableArea;

	private readonly RectTransform Panel;

	private bool isDragging;

	private int heightCompensation;

	private MouseState CurrentMouseState = MouseState.NotPressed;

	private Vector3 LastDragPosition;

	public PanelDragger(in RectTransform draggableArea, in RectTransform panel, int heightCompensation = 0)
	{
		DraggableArea = draggableArea;
		Panel = panel;
		this.heightCompensation = heightCompensation;
	}

	public bool Update()
	{
		if (beingDragged != null && beingDragged != this)
		{
			return false;
		}
		Vector3 mousePosition = InputManager.MousePosition;
		Vector3 point = DraggableArea.InverseTransformPoint(mousePosition);
		bool num = DraggableArea.rect.Contains(point);
		if (InputManager.GetMouseButtonDown(0))
		{
			CurrentMouseState = MouseState.Down;
		}
		else if (InputManager.GetMouseButton(0))
		{
			CurrentMouseState = MouseState.Held;
		}
		else
		{
			CurrentMouseState = MouseState.NotPressed;
			isDragging = false;
			beingDragged = null;
		}
		if (!num && !isDragging)
		{
			return false;
		}
		switch (CurrentMouseState)
		{
		case MouseState.Down:
			OnBeginDrag();
			return true;
		case MouseState.Held:
			beingDragged = this;
			isDragging = true;
			OnDrag();
			return true;
		default:
			return false;
		}
	}

	public void Reset()
	{
		beingDragged = null;
	}

	private void OnDrag()
	{
		Vector3 mousePosition = InputManager.MousePosition;
		Vector3 vector = mousePosition - LastDragPosition;
		LastDragPosition = mousePosition;
		Panel.localPosition += vector;
		EnsureValidPosition(Panel);
	}

	private void OnBeginDrag()
	{
		LastDragPosition = InputManager.MousePosition;
	}

	private void EnsureValidPosition(RectTransform panel)
	{
		Vector3 localPosition = panel.localPosition;
		float num = (float)Screen.width * 0.5f;
		float num2 = (float)Screen.height * 0.5f;
		localPosition.x = Math.Max(0f - num + panel.rect.width / 2f, Math.Min(localPosition.x, num - panel.rect.width / 2f));
		localPosition.y = Math.Max(0f - num2 + 50f, Math.Min(localPosition.y, num2 - panel.rect.height / 2f + (float)heightCompensation));
		panel.localPosition = localPosition;
	}
}
