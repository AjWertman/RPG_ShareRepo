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

    public void SetupResourceIndicator(BattleUnit _battleUnit)
    {
        if(_battleUnit != null)
        {
            BattleUnitInfo battleUnitInfo = _battleUnit.GetBattleUnitInfo();
            SetBackgroundColor(battleUnitInfo.IsPlayer());

            nameText.text = battleUnitInfo.GetUnitName();
            faceImage.sprite = battleUnitInfo.GetFaceImage();
            healthText.text = ("Health: " + _battleUnit.GetUnitHealth() + "/" + _battleUnit.GetUnitMaxHealth());
            manaText.text = ("Mana: " + _battleUnit.GetUnitMana() + "/" + _battleUnit.GetUnitMaxMana());
        }
        else
        {
            ResetResourcesIndicator();
        }
    }

    public void ResetResourcesIndicator()
    {
        background.color = Color.white;

        nameText.text = "";
        faceImage.sprite = null;
        healthText.text = ("Health: 999/999");
        manaText.text = ("Mana: 999/999");
    }

    private void SetBackgroundColor(bool _isPlayer)
    {
        if (_isPlayer)
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
