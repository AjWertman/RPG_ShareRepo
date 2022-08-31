using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RPGProject.GameResources;
using RPGProject.Combat;

namespace RPGProject.UI
{
    /// <summary>
    /// UI to represent the highlighted character in combat, and provide deatailed information
    /// on their status.
    /// </summary>
    public class SelectedUnitIndicator : MonoBehaviour
    {
        [SerializeField] Image background = null;
        [SerializeField] TextMeshProUGUI nameText = null;
        [SerializeField] Image faceImage = null;

        [SerializeField] Slider healthSlider = null;
        [SerializeField] TextMeshProUGUI healthText = null;
        [SerializeField] Slider energySlider = null;
        [SerializeField] TextMeshProUGUI energyText = null;

        [SerializeField] Vector2 defaultPosition = new Vector2();

        TeamMemberIndicator coveredIndicator = null;

        RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        //public void InitalizeUnitIndicator()
        //{
        //    rect = GetComponent<RectTransform>();
        //    DeactivateIndicator();
        //}

        public void SetupResourceIndicator(Fighter _fighter)
        {
            if (_fighter != null)
            {
                UnitInfo unitInfo = _fighter.unitInfo;

                nameText.text = unitInfo.unitName;
                faceImage.sprite = _fighter.characterMesh.faceImage;
                background.color = _fighter.characterMesh.uiColor;

                Health unitHealth = _fighter.GetHealth();
                Energy unitEnergy = _fighter.GetEnergy();

                healthText.text = (unitHealth.healthPoints.ToString() + "/" + unitHealth.maxHealthPoints.ToString());
                healthSlider.value = unitHealth.healthPercentage;

                energyText.text = (unitEnergy.energyPoints.ToString() + "/" + unitEnergy.maxEnergyPoints.ToString());
                energySlider.value = unitEnergy.GetEnergyPercentage();

                if (!_fighter.unitInfo.isPlayer) MoveToPosition(defaultPosition);
            }
        }

        public void OnTeammateSelection(TeamMemberIndicator _teamMemberIndicator)
        {
            if (coveredIndicator != null && coveredIndicator != _teamMemberIndicator)
            {
                coveredIndicator.HideIndicator(false);
                coveredIndicator = null;
            }

            coveredIndicator = _teamMemberIndicator;
            SetupResourceIndicator(coveredIndicator.GetFighter());

            Vector2 indicatorPosition = coveredIndicator.GetRect().anchoredPosition;
            Vector2 offsetPosition = new Vector2(100, Mathf.Round(indicatorPosition.y));

            coveredIndicator.HideIndicator(true);

            MoveToPosition(offsetPosition);          
        }

        public void DeactivateIndicator()
        {
            gameObject.SetActive(false);
            background.color = Color.white;

            nameText.text = "";
            faceImage.sprite = null;
            //healthText.text = ("Health: 999/999");
            //manaText.text = ("Mana: 999/999");
            MoveToPosition(defaultPosition);

            if(coveredIndicator != null)
            {
                coveredIndicator.HideIndicator(false);
                coveredIndicator = null;
            }
        }

        private void MoveToPosition(Vector2 _position)
        {
            if (rect == null) return;
            rect.anchoredPosition = _position;
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