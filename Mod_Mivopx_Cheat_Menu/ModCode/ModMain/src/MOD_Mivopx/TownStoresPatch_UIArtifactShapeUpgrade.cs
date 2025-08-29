using HarmonyLib;

namespace MOD_Mivopx;

[HarmonyPatch(typeof(UIArtifactShapeUpgrade), "Init")]
internal class TownStoresPatch_UIArtifactShapeUpgrade
{
	public static UIArtifactShapeUpgrade _UIArtifactShapeUpgrade;

	private static void Postfix(UIArtifactShapeUpgrade __instance)
	{
		_UIArtifactShapeUpgrade = __instance;
	}
}
