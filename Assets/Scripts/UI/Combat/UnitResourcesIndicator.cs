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
    [SerializeField] TextMeshProUGUI soulWellText = null;

    public void SetupResourceIndicator(BattleUnit unit)
    {
        SetBackgroundColor(unit.IsPlayer());

        nameText.text = unit.GetName();
        faceImage.sprite = unit.GetFaceImage();
        healthText.text = ("Health: " + unit.GetUnitHealth() + "/" + unit.GetUnitMaxHealth());
        soulWellText.text = ("Soul Well: " + unit.GetUnitSoulWell() + "/" + unit.GetUnitMaxSoulWell());
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
