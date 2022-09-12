using RPGProject.Combat;
using RPGProject.GameResources;
using System.Collections;
using UnityEngine;

namespace RPGProject.UI
{
    /// <summary>
    /// Component that handles all of the UI attached to a unit.
    /// </summary>
    public class UnitUI : MonoBehaviour
    {
        [SerializeField] UnitIndicatorUI unitIndicator = null;
        [SerializeField] UIHealthChange uiHealthChange = null;

        Health health = null;
        Fighter fighter = null;
        Energy energy = null;

        public void InitializeUnitUI()
        {
            fighter = GetComponent<Fighter>();
            fighter.onHighlight += ActivateUnitIndicator;

            health = fighter.GetHealth();
            health.onHealthChange += UpdateHealthUI;

            energy = fighter.GetEnergy();
            //energy.onEnergyChange += UpdateEnergyUI;
        }

        public void ActivateUnitIndicator(bool _shouldActivate)
        {
            if (_shouldActivate) unitIndicator.ActivateIndicator(fighter.unitInfo.isPlayer);
            else unitIndicator.DeactivateIndicator();
        }

        private void UpdateHealthUI(bool _isCritical, float _changeAmount)
        {
            //healthSlider.UpdateSliderValue(health.healthPercentage);

            StartCoroutine(UIHealthChange(_isCritical, _changeAmount));
        }

        public IEnumerator UIHealthChange(bool _isCritical, float _changeAmount)
        {
            uiHealthChange.ActivateHealthChange(_isCritical, _changeAmount);

            yield return new WaitForSeconds(1f);

            uiHealthChange.DeactivateHealthChange();
        }

        public UnitIndicatorUI GetUnitIndicator()
        {
            return unitIndicator;
        }

        public UIHealthChange GetUIHealthChange()
        {
            return uiHealthChange;
        }
    }
}
