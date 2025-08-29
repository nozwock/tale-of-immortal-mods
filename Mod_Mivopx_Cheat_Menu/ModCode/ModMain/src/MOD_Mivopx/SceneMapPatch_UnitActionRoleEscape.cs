using TaleOfImmortalCheat;
using TaleOfImmortalCheat.Localization;

namespace MOD_Mivopx;

internal class SceneMapPatch_UnitActionRoleEscape
{
	public static ConfRoleEscape EscapeBitch = Game.ConfMgr.roleEscape;

	private static bool _isEscapeBitchActive;

	public static bool IsEscapeBitchActive
	{
		get
		{
			return _isEscapeBitchActive;
		}
		set
		{
			_isEscapeBitchActive = value;
			if (value)
			{
				if (EscapeBitch != null)
				{
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("OtherPanel - EscapeBitch - Called");
					ModMain.Log("----- ----- ----- ----- -----");
					ModMain.Log("escapeSucceedParaA: " + EscapeBitch.escapeSucceedParaA);
					EscapeBitch._escapeSucceedParaA.value = 10000;
					ModMain.Log("Modified - escapeSucceedParaA: " + EscapeBitch.escapeSucceedParaA);
					ModMain.Log("escapeSucceedParaB: " + EscapeBitch.escapeSucceedParaB);
					EscapeBitch._escapeSucceedParaB.value = 10000;
					ModMain.Log("Modified - escapeSucceedParaB: " + EscapeBitch.escapeSucceedParaB);
					ModMain.Log("escapeSucceedParaC: " + EscapeBitch.escapeSucceedParaC);
					EscapeBitch._escapeSucceedParaC.value = 10000;
					ModMain.Log("Modified - escapeSucceedParaC: " + EscapeBitch.escapeSucceedParaC);
					ModMain.Log("escapeLostProb: " + EscapeBitch.escapeLostProb);
					EscapeBitch._escapeLostProb.value = 0;
					ModMain.Log("Modified - escapeLostProb: " + EscapeBitch.escapeLostProb);
					ModMain.Log("escapeDistanceMax: " + EscapeBitch.escapeDistanceMax);
					EscapeBitch._escapeDistanceMax.value = 1;
					ModMain.Log("Modified - escapeDistanceMax: " + EscapeBitch.escapeDistanceMax);
					ModMain.Log("escapeDistanceMax9Area: " + EscapeBitch.escapeDistanceMax9Area);
					EscapeBitch._escapeDistanceMax9Area.value = 1;
					ModMain.Log("Modified - escapeDistanceMax9Area: " + EscapeBitch.escapeDistanceMax9Area);
					ModMain.Log("escapeDistanceMin: " + EscapeBitch.escapeDistanceMin);
					EscapeBitch._escapeDistanceMin.value = 1;
					ModMain.Log("Modified - escapeDistanceMin: " + EscapeBitch.escapeDistanceMin);
					ModMain.Log("escapeDistanceMin9Area: " + EscapeBitch.escapeDistanceMin9Area);
					EscapeBitch._escapeDistanceMin9Area.value = 1;
					ModMain.Log("Modified - escapeDistanceMin9Area: " + EscapeBitch.escapeDistanceMin9Area);
					ModMain.Log("escapeLostDistanceMax: " + EscapeBitch.escapeLostDistanceMax);
					EscapeBitch._escapeLostDistanceMax.value = 1;
					ModMain.Log("Modified - escapeLostDistanceMax: " + EscapeBitch.escapeLostDistanceMax);
					ModMain.Log("escapeLostDistanceMin: " + EscapeBitch.escapeLostDistanceMin);
					EscapeBitch._escapeLostDistanceMin.value = 1;
					ModMain.Log("Modified - escapeLostDistanceMin: " + EscapeBitch.escapeLostDistanceMin);
					ModMain.Log("escapeReduceEnergyMax: " + EscapeBitch.escapeReduceEnergyMax);
					EscapeBitch._escapeReduceEnergyMax.value = 0;
					ModMain.Log("Modified - escapeReduceEnergyMax: " + EscapeBitch.escapeReduceEnergyMax);
					ModMain.Log("escapeReduceEnergyMin: " + EscapeBitch.escapeReduceEnergyMin);
					EscapeBitch._escapeReduceEnergyMin.value = 0;
					ModMain.Log("Modified - escapeReduceEnergyMin: " + EscapeBitch.escapeReduceEnergyMin);
					ModMain.Log("escapeReduceMoodMax: " + EscapeBitch.escapeReduceMoodMax);
					EscapeBitch._escapeReduceMoodMax.value = 0;
					ModMain.Log("Modified - escapeReduceMoodMax: " + EscapeBitch.escapeReduceMoodMax);
					ModMain.Log("escapeReduceMoodMin: " + EscapeBitch.escapeReduceMoodMin);
					EscapeBitch._escapeReduceMoodMin.value = 0;
					ModMain.Log("Modified - escapeReduceMoodMin: " + EscapeBitch.escapeReduceMoodMin);
					ModMain.Log("escapeRateMinGrade10: " + EscapeBitch.escapeRateMinGrade10);
					EscapeBitch._escapeRateMinGrade10.value = 0;
					ModMain.Log("Modified - escapeRateMinGrade10: " + EscapeBitch.escapeRateMinGrade10);
				}
				ModMain.LogTip(LocalizationHelper.T("other_msgs_100_escape_enabled"));
			}
			else
			{
				if (EscapeBitch != null)
				{
					ModMain.Log("escapeSucceedParaA: " + EscapeBitch.escapeSucceedParaA);
					EscapeBitch._escapeSucceedParaA.value = 150;
					ModMain.Log("Modified - escapeSucceedParaA: " + EscapeBitch.escapeSucceedParaA);
					ModMain.Log("escapeSucceedParaB: " + EscapeBitch.escapeSucceedParaB);
					EscapeBitch._escapeSucceedParaB.value = 100;
					ModMain.Log("Modified - escapeSucceedParaB: " + EscapeBitch.escapeSucceedParaB);
					ModMain.Log("escapeSucceedParaC: " + EscapeBitch.escapeSucceedParaC);
					EscapeBitch._escapeSucceedParaC.value = 500;
					ModMain.Log("Modified - escapeSucceedParaC: " + EscapeBitch.escapeSucceedParaC);
					ModMain.Log("escapeLostProb: " + EscapeBitch.escapeLostProb);
					EscapeBitch._escapeLostProb.value = 3;
					ModMain.Log("Modified - escapeLostProb: " + EscapeBitch.escapeLostProb);
					ModMain.Log("escapeDistanceMax: " + EscapeBitch.escapeDistanceMax);
					EscapeBitch._escapeDistanceMax.value = 10;
					ModMain.Log("Modified - escapeDistanceMax: " + EscapeBitch.escapeDistanceMax);
					ModMain.Log("escapeDistanceMax9Area: " + EscapeBitch.escapeDistanceMax9Area);
					EscapeBitch._escapeDistanceMax9Area.value = 5;
					ModMain.Log("Modified - escapeDistanceMax9Area: " + EscapeBitch.escapeDistanceMax9Area);
					ModMain.Log("escapeDistanceMin: " + EscapeBitch.escapeDistanceMin);
					EscapeBitch._escapeDistanceMin.value = 5;
					ModMain.Log("Modified - escapeDistanceMin: " + EscapeBitch.escapeDistanceMin);
					ModMain.Log("escapeDistanceMin9Area: " + EscapeBitch.escapeDistanceMin9Area);
					EscapeBitch._escapeDistanceMin9Area.value = 4;
					ModMain.Log("Modified - escapeDistanceMin9Area: " + EscapeBitch.escapeDistanceMin9Area);
					ModMain.Log("escapeLostDistanceMax: " + EscapeBitch.escapeLostDistanceMax);
					EscapeBitch._escapeLostDistanceMax.value = 40;
					ModMain.Log("Modified - escapeLostDistanceMax: " + EscapeBitch.escapeLostDistanceMax);
					ModMain.Log("escapeLostDistanceMin: " + EscapeBitch.escapeLostDistanceMin);
					EscapeBitch._escapeLostDistanceMin.value = 30;
					ModMain.Log("Modified - escapeLostDistanceMin: " + EscapeBitch.escapeLostDistanceMin);
					ModMain.Log("escapeReduceEnergyMax: " + EscapeBitch.escapeReduceEnergyMax);
					EscapeBitch._escapeReduceEnergyMax.value = 30;
					ModMain.Log("Modified - escapeReduceEnergyMax: " + EscapeBitch.escapeReduceEnergyMax);
					ModMain.Log("escapeReduceEnergyMin: " + EscapeBitch.escapeReduceEnergyMin);
					EscapeBitch._escapeReduceEnergyMin.value = 10;
					ModMain.Log("Modified - escapeReduceEnergyMin: " + EscapeBitch.escapeReduceEnergyMin);
					ModMain.Log("escapeReduceMoodMax: " + EscapeBitch.escapeReduceMoodMax);
					EscapeBitch._escapeReduceMoodMax.value = 15;
					ModMain.Log("Modified - escapeReduceMoodMax: " + EscapeBitch.escapeReduceMoodMax);
					ModMain.Log("escapeReduceMoodMin: " + EscapeBitch.escapeReduceMoodMin);
					EscapeBitch._escapeReduceMoodMin.value = 5;
					ModMain.Log("Modified - escapeReduceMoodMin: " + EscapeBitch.escapeReduceMoodMin);
					ModMain.Log("escapeRateMinGrade10: " + EscapeBitch.escapeRateMinGrade10);
					EscapeBitch._escapeRateMinGrade10.value = 75;
					ModMain.Log("Modified - escapeRateMinGrade10: " + EscapeBitch.escapeRateMinGrade10);
				}
				ModMain.LogTip(LocalizationHelper.T("other_msgs_100_escape_disabled"));
				ModMain.LogTip(LocalizationHelper.T("other_msgs_restored_original_values"), null, 3f);
			}
		}
	}
}
