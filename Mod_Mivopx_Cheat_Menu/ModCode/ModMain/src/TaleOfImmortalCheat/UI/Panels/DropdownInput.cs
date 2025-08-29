using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using MOD_Mivopx;
using UnityEngine.UI;

namespace TaleOfImmortalCheat.UI.Panels;

public class DropdownInput<T> : Field
{
	private Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> currentOptionsData;

	private System.Collections.Generic.List<T> currentOptions;

	public int TabIndex { get; set; }

	public Dropdown Dropdown { get; }

	public Func<AttributesState, int?> OptionsAccessor { get; }

	public Action<AttributesState, T> SaveAccessor { get; }

	public bool HasOptions
	{
		get
		{
			if (currentOptions != null && currentOptions.Count > 0 && Dropdown.options.Count > 0)
			{
				return Dropdown.interactable;
			}
			return false;
		}
	}

	public T Value => currentOptions[Dropdown.value];

	public DropdownInput(Dropdown dropdown, Func<AttributesState, int?> options, Action<AttributesState, T> save, Il2CppSystem.Collections.Generic.List<Dropdown.OptionData> currentOptionsData, System.Collections.Generic.List<T> currentOptions, int tabIndex)
	{
		this.currentOptionsData = currentOptionsData;
		this.currentOptions = currentOptions;
		Dropdown = dropdown;
		OptionsAccessor = options;
		SaveAccessor = save;
		TabIndex = tabIndex;
	}

	public void Reset(AttributesState state)
	{
		int? num = OptionsAccessor(state);
		if (!num.HasValue)
		{
			Dropdown.ClearOptions();
			Dropdown.interactable = false;
			return;
		}
		if (Dropdown.options.Count == 0)
		{
			Dropdown.AddOptions(currentOptionsData);
		}
		Dropdown.value = num.Value;
		Dropdown.interactable = true;
	}

	public void Save(AttributesState state, int? tabIndex)
	{
		if ((tabIndex.HasValue && tabIndex != TabIndex) || !HasOptions)
		{
			return;
		}
		try
		{
			if (Dropdown.value < currentOptions.Count)
			{
				SaveAccessor(state, currentOptions[Dropdown.value]);
			}
		}
		catch (Exception ex)
		{
			ModMain.LogError("Failed to save dropdown " + ex.Message + ":" + ex.StackTrace);
		}
	}
}
