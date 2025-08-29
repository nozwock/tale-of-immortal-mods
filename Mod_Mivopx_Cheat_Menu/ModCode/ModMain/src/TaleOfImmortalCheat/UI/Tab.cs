using UnityEngine;
using UnityEngine.UI;

namespace TaleOfImmortalCheat.UI;

internal class Tab
{
	public string Name { get; }

	public ColorBlock? ButtonColorBlock { get; }

	public Color? PanelColor { get; }

	public Tab(string name)
	{
		Name = name;
	}

	public Tab(string name, ColorBlock buttonColor, Color panelColor)
	{
		Name = name;
		ButtonColorBlock = buttonColor;
		PanelColor = panelColor;
	}
}
