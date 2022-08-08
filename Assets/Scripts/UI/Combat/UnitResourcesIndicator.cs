using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RPGProject.GameResources;
using RPGProject.Combat;

namespace RPGProject.UI
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
                UnitInfo unitInfo = _fighter.unitInfo;
                SetBackgroundColor(unitInfo.isPlayer);

                nameText.text = unitInfo.unitName;
                faceImage.sprite = _fighter.characterMesh.faceImage;

                Health unitHealth = _fighter.GetHealthComponent();

                healthText.text = ("Health: " + unitHealth.healthPoints.ToString()+ "/" + unitHealth.maxHealthPoints.ToString());
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