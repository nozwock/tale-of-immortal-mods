using System;
using MOD_Mivopx;
using UnityEngine.UI;
using UniverseLib.UI.Models;

namespace TaleOfImmortalCheat.UI.Panels;

public class DynDropdownField : Field
{
    public int TabIndex { get; set; }
    public Dropdown Dropdown { get; }
    public Func<AttributesState, DynInt> Accessor { get; }
    private int MinId { get; }
    private bool IsClamped { get; }
    private bool IsUsingBaseValue { get; }

    public DynDropdownField(Dropdown dropdown, Func<AttributesState, DynInt> accessor, bool isUsingBaseValue, int tabIndex, int minId, bool isClamped = true)
    {
        Dropdown = dropdown;
        Accessor = accessor;
        TabIndex = tabIndex;
        MinId = minId;
        IsClamped = isClamped;
        IsUsingBaseValue = isUsingBaseValue;
    }

    public void Reset(AttributesState state)
    {
        var dynInt = Accessor(state);
        int id = IsUsingBaseValue ? dynInt.baseValue : dynInt.Value();
        int index = id - MinId;

        if (index >= 0 && index < Dropdown.options.Count)
        {
            Dropdown.value = index;
        }
        else
        {
            Dropdown.value = 0; // fallback
        }
    }

    public void Save(AttributesState state, int? tabIndex)
    {
        if (tabIndex.HasValue && tabIndex != TabIndex)
        {
            return;
        }

        DynInt dynInt = Accessor(state);
        dynInt.baseValue = MinId + Dropdown.value;

        if (IsClamped)
        {
            dynInt.UpdateClampValue();
        }
    }
}

