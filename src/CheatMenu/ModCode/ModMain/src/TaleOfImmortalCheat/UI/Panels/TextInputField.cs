using System;
using UniverseLib.UI.Models;

namespace TaleOfImmortalCheat.UI.Panels;

public class TextInputField : Field
{
	public int TabIndex { get; set; }

	public InputFieldRef InputField { get; }

	public Func<AttributesState, string> Accessor { get; }

	public Action<AttributesState, string> SaveAccessor { get; }

	public TextInputField(InputFieldRef inputField, Func<AttributesState, string> accessor, Action<AttributesState, string> save, int tabIndex)
	{
		InputField = inputField;
		Accessor = accessor;
		SaveAccessor = save;
		TabIndex = tabIndex;
	}

	public void Reset(AttributesState state)
	{
		InputField.Text = Accessor(state);
	}

	public void Save(AttributesState state, int? tabIndex)
	{
		if (!tabIndex.HasValue || tabIndex == TabIndex)
		{
			SaveAccessor(state, InputField.Text);
		}
	}
}
