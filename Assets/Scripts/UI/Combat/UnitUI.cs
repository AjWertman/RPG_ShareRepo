using RPGProject.Combat;
using RPGProject.GameResources;
using System.Collections;
using UnityEngine;

namespace RPGProject.UI
{
    public class UnitUI : MonoBehaviour
    {
        [SerializeField] ResourceSlider healthSlider = null;
        [SerializeField] ResourceSlider manaSlider = null;

        [SerializeField] UnitIndicatorUI unitIndicator = null;
        [SerializeField] UIHealthChange uiHealthChange = null;

        Health health = null;
        Mana mana = null;
        Fighter fighter = null;

        public void InitializeUnitUI()
        {
            fighter = GetComponent<Fighter>();

            health = GetComponent<Health>();
            mana = GetComponent<Mana>();

            health.onHealthChange += UpdateHealthUI;
            mana.onManaChange += UpdateManaUI;
        }

        public void SetupUnitUI()
        {
            healthSlider.UpdateSliderValue(GetHealthPercentage());
            manaSlider.UpdateSliderValue(GetManaPercentage());
        }

        public void ActivateUnitIndicator(bool _shouldActivate)
        {
            if (_shouldActivate) unitIndicator.ActivateIndicator(fighter.GetUnitInfo().IsPlayer());
            else unitIndicator.DeactivateIndicator();
        }

        private void UpdateHealthUI(bool _isCritical, float _changeAmount)
        {
            healthSlider.UpdateSliderValue(GetHealthPercentage());

            StartCoroutine(UIHealthChange(_isCritical, _changeAmount));
        }

        public IEnumerator UIHealthChange(bool _isCritical, float _changeAmount)
        {
            uiHealthChange.ActivateHealthChange(_isCritical, _changeAmount);

            yield return new WaitForSeconds(1f);

            uiHealthChange.DeactivateHealthChange();
        }

        private void UpdateManaUI()
        {
            manaSlider.UpdateSliderValue(GetManaPercentage());
        }

        public void ActivateResourceSliders(bool _shouldActivate)
        {
            healthSlider.gameObject.SetActive(_shouldActivate);
            manaSlider.gameObject.SetActive(_shouldActivate);
        }

        public UnitIndicatorUI GetUnitIndicator()
        {
            return unitIndicator;
        }

        public UIHealthChange GetUIHealthChange()
        {
            return uiHealthChange;
        }

        public float GetHealthPercentage()
        {
            return health.GetHealthPercentage();
        }
        
        public float GetManaPercentage()
        {
            return mana.GetManaPercentage();
        }
    }
}
