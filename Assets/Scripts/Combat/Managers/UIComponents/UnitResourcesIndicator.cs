using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RPGProject.GameResources;

namespace RPGProject.Combat
{
    public class UnitResourcesIndicator : MonoBehaviour
    {
        [SerializeField] Image background = null;

        [SerializeField] TextMeshProUGUI nameText = null;
        [SerializeField] Image faceImage = null;
        [SerializeField] TextMeshProUGUI healthText = null;
        [SerializeField] TextMeshProUGUI manaText = null;

        public void SetupResourceIndicator(Fighter _fighter)
        {
            if (_fighter != null)
            {
                BattleUnitInfo battleUnitInfo = _fighter.GetUnitInfo();
                SetBackgroundColor(battleUnitInfo.IsPlayer());

                nameText.text = battleUnitInfo.GetUnitName();
                faceImage.sprite = _fighter.GetCharacterMesh().GetFaceImage();

                Health unitHealth = _fighter.GetHealth();
                Mana unitMana = _fighter.GetMana();
                healthText.text = ("Health: " + unitHealth.GetHealthPoints() + "/" + unitHealth.GetMaxHealthPoints());
                manaText.text = ("Mana: " + unitMana.GetManaPoints() + "/" + unitMana.GetMaxManaPoints());
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
}