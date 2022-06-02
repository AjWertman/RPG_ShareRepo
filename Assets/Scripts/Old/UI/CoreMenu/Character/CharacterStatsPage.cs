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
        [SerializeField] Image characterImage = null;
        [SerializeField] StatPageUI[] statPageUIs = null;

        public void SetupStatPageUI(PlayableCharacter _character, int _level, Stats _stats, BattleUnitResources _battleUnitResources)
        {
            nameText.text = _character.GetPlayerKey().ToString();
            characterImage.sprite = _character.GetFullBodyImage();
            levelText.text = ("Lv: " + _level.ToString());

            foreach (StatPageUI statPageUI in statPageUIs)
            {
                if (statPageUI.GetStatType() == StatType.Stamina)
                {
                    UpdateCharacterPageHealthUI(_battleUnitResources);

                }
                else if (statPageUI.GetStatType() == StatType.Spirit)
                {
                   UpdateCharacterPageManaUI(_battleUnitResources);
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

        public void UpdateCharacterPageHealthUI(BattleUnitResources _battleUnitResources)
        {
            StatPageUI statPageUI = GetHealthUI();

            BattleUnitResources battleUnitResources = _battleUnitResources;

            float health = battleUnitResources.GetHealthPoints();
            float maxHealth = battleUnitResources.GetMaxHealthPoints();

            float sliderAmount = health / maxHealth;
            statPageUI.GetSlider().value = sliderAmount;
            statPageUI.GetAmountText().text = health.ToString() + "/" + maxHealth.ToString();
        }

        public void UpdateCharacterPageManaUI(BattleUnitResources _battleUnitResources)
        {
            StatPageUI statPageUI = GetManaUI();

            BattleUnitResources battleUnitResources = _battleUnitResources;

            float mana = battleUnitResources.GetManaPoints();
            float maxMana = battleUnitResources.GetMaxManaPoints();

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
