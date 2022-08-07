using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.Progression;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class CharacterStatsPage : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText = null;
        [SerializeField] TextMeshProUGUI levelText = null;
        [SerializeField] StatPageUI[] statPageUIs = null;

        public void SetupStatPageUI(PlayableCharacter _character, int _level, Stats _stats, UnitResources _unitResources)
        {
            nameText.text = _character.GetName();
            levelText.text = ("Lv: " + _level.ToString());

            foreach (StatPageUI statPageUI in statPageUIs)
            {
                if (statPageUI.GetStatType() == StatType.Stamina)
                {
                    UpdateCharacterPageHealthUI(_unitResources);

                }
                else if (statPageUI.GetStatType() == StatType.Spirit)
                {
                   UpdateCharacterPageManaUI(_unitResources);
                }
                else
                {
                    Stats stats = _stats;
                    int statLevel = stats.GetStat(statPageUI.GetStatType());

                    float sliderAmount = statLevel / 100f;

                    statPageUI.GetSlider().value = sliderAmount;
                    statPageUI.GetAmountText().text = statLevel.ToString();
                }
            }
        }

        public void UpdateCharacterPageHealthUI(UnitResources _unitResources)
        {
            StatPageUI statPageUI = GetHealthUI();

            UnitResources unitResources = _unitResources;

            float health = unitResources.healthPoints;
            float maxHealth = unitResources.maxHealthPoints;

            float sliderAmount = health / maxHealth;
            statPageUI.GetSlider().value = sliderAmount;
            statPageUI.GetAmountText().text = health.ToString() + "/" + maxHealth.ToString();
        }

        public void UpdateCharacterPageManaUI(UnitResources _unitResources)
        {
            StatPageUI statPageUI = GetManaUI();

            UnitResources unitResources = _unitResources;

            //float mana = unitResources.GetManaPoints();
            //float maxMana = unitResources.GetMaxManaPoints();
            float mana = 0f;
            float maxMana = 0f;

            float sliderAmount = mana / maxMana;
            statPageUI.GetSlider().value = sliderAmount;
            statPageUI.GetAmountText().text = mana.ToString() + "/" + maxMana.ToString();
        }

        public StatPageUI GetHealthUI()
        {
            foreach (StatPageUI ui in statPageUIs)
            {
                if (ui.GetStatType() == StatType.Stamina)
                {
                    return ui;
                }
            }

            return null;
        }

        public StatPageUI GetManaUI()
        {
            foreach (StatPageUI ui in statPageUIs)
            {
                if (ui.GetStatType() == StatType.Spirit)
                {
                    return ui;
                }
            }

            return null;
        }
    }

    [Serializable]
    public class StatPageUI
    {
        [SerializeField] StatType statType = StatType.Armor;
        [SerializeField] Slider statSlider = null;
        [SerializeField] TextMeshProUGUI amountText = null;

        public StatType GetStatType()
        {
            return statType;
        }

        public Slider GetSlider()
        {
            return statSlider;
        }

        public TextMeshProUGUI GetAmountText()
        {
            return amountText;
        }
    }
}
