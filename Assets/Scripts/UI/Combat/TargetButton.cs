using TMPro;
using UnityEngine;

public class TargetButton : MonoBehaviour
{
    BattleUnit assignedTarget = null;

    public void SetupTargetButton(BattleUnit unitToSet)
    {
        assignedTarget = unitToSet;
        GetComponentInChildren<TextMeshProUGUI>().text = assignedTarget.GetBattleUnitInfo().GetUnitName();
        GetComponent<BattleUnitIndicatorTrigger>().SetupTrigger(assignedTarget);
    }

    public BattleUnit GetTarget()
    {
        return assignedTarget;
    }
}
