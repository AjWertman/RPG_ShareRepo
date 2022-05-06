using UnityEngine;
using UnityEngine.EventSystems;

public class BattleUnitIndicatorTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    BattleUIManager uiManager = null;
    BattleUnit thisUnit = null;

    bool isAlreadyActive = false;

    public void SetupTrigger(BattleUnit unit)
    {
        thisUnit = unit;
        uiManager = FindObjectOfType<BattleUIManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiManager.ActivateUnitResourcesUI(thisUnit);

        if (thisUnit.IsIndicatorActive())
        {
            isAlreadyActive = true;
            return;
        }
        thisUnit.ActivateUnitIndicatorUI(true);       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiManager.DeactivateUnitResourcesUI();

        if (isAlreadyActive)
        {
            isAlreadyActive = false;
            return;
        }
        thisUnit.ActivateUnitIndicatorUI(false);    
    }
}
