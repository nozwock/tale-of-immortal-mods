using System;
using MOD_Mivopx;
using UniverseLib.UI.Models;

namespace TaleOfImmortalCheat.UI.Panels;

public class DynInputField : Field
{
	public int TabIndex { get; set; }

	private bool IsUsingBaseValue { get; }

	private bool IsClamped { get; }

	public InputFieldRef InputField { get; }

	public Func<AttributesState, DynInt> Accessor { get; }

	public DynInputField(InputFieldRef inputField, Func<AttributesState, DynInt> accessor, bool isUsingBaseValue, int tabIndex, bool isClamped = true)
	{
		IsUsingBaseValue = isUsingBaseValue;
		InputField = inputField;
		Accessor = accessor;
		IsClamped = isClamped;
		TabIndex = tabIndex;
	}

	public void Reset(AttributesState state)
	{
		if (IsUsingBaseValue)
		{
			InputField.Text = Accessor(state).baseValue.ToString();
		}
		else
		{
			InputField.Text = Accessor(state).Value().ToString();
		}
	}

	public void Save(AttributesState state, int? tabIndex)
	{
		if (tabIndex.HasValue && tabIndex != TabIndex)
		{
			return;
		}
		if (int.TryParse(InputField.Text, out var result))
		{
			DynInt dynInt = Accessor(state);
			dynInt.baseValue = result;
			if (IsClamped)
			{
				dynInt.UpdateClampValue();
			}
		}
		else
		{
			ModMain.LogWarning("Could not parse value for " + InputField.Component.name);
		}
	}
}
