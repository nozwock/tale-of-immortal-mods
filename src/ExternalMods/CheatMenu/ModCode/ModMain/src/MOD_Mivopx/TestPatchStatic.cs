using TaleOfImmortalCheat.UI;

namespace MOD_Mivopx;

internal static class TestPatchStatic
{
	static TestPatchStatic()
	{
		UIRefreshManager.OnLanguageChanged += UpdateUITexts;
	}

	private static void UpdateUITexts()
	{
	}
}
