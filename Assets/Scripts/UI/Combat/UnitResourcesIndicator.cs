using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitResourcesIndicator : MonoBehaviour
{
    [SerializeField] Image background = null;

    [SerializeField] TextMeshProUGUI nameText = null;
    [SerializeField] Image faceImage = null;
    [SerializeField] TextMeshProUGUI healthText = null;
    [SerializeField] TextMeshProUGUI manaText = null;

    public void SetupResourceIndicator(BattleUnit _unit)
    {
        BattleUnitInfo battleUnitInfo = _unit.GetBattleUnitInfo();
        SetBackgroundColor(battleUnitInfo.IsPlayer());

        nameText.text = battleUnitInfo.GetUnitName();
        faceImage.sprite = _unit.GetFaceImage();
        healthText.text = ("Health: " + _unit.GetUnitHealth() + "/" + _unit.GetUnitMaxHealth());
        manaText.text = ("Mana: " + _unit.GetUnitMana() + "/" + _unit.GetUnitMaxMana());
    }

    private void SetBackgroundColor(bool isPlayer)
    {
        if (isPlayer)
        {
            Color playerColor = new Color(0, 0, 1, .35f);
            background.color = playerColor;
        }
        else
        {
            Color enemyColor = new Color(1, 0, 0, .35f);
            background.color = enemyColor;
        }
    }
}
