using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

internal static class TownStoresPatchStatic
{
	static TownStoresPatchStatic()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void UpdateUITexts()
	{
	}
}
