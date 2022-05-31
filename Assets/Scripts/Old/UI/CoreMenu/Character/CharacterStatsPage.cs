using RPGProject.Combat;
using RPGProject.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
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

public class CharacterStatsPage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText = null;
    [SerializeField] TextMeshProUGUI levelText = null;
    [SerializeField] Image characterImage = null;
    [SerializeField] StatPageUI[] statPageUIs = null;

    public void SetupStatPageUI(Unit character, TeamInfo teamInfo)
    {
        nameText.text = teamInfo.GetName();
        //characterImage.sprite = character.GetFullBodyImage();
        levelText.text = ("Lv: " + teamInfo.GetLevel().ToString());
        
        foreach(StatPageUI statPageUI in statPageUIs)
        {
            if (statPageUI.GetStatType() == StatType.Stamina)
            {
                UpdateCharacterPageHealthUI(teamInfo);

            }
            else if(statPageUI.GetStatType() == StatType.Spirit)
            {
                UpdateCharacterPageManaUI(teamInfo);
            }
            else
            {
                Stats stats = teamInfo.GetStats();
                Stat newStat = stats.GetStat(statPageUI.GetStatType());

                float sliderAmount = newStat.GetStatLevel() / 100f;

                statPageUI.GetSlider().value = sliderAmount;
                statPageUI.GetAmountText().text = newStat.GetStatLevel().ToString();
            }
        }
    }

    public void UpdateCharacterPageHealthUI(TeamInfo teamInfo)
    {
        StatPageUI statPageUI = GetHealthUI();

        BattleUnitResources battleUnitResources = teamInfo.GetBattleUnitResources();

        float health = battleUnitResources.GetHealthPoints();
        float maxHealth = battleUnitResources.GetMaxHealthPoints();

        float sliderAmount = health / maxHealth;
        statPageUI.GetSlider().value = sliderAmount;
        statPageUI.GetAmountText().text = health.ToString() + "/" + maxHealth.ToString();
    }

    public void UpdateCharacterPageManaUI(TeamInfo teamInfo)
    {
        StatPageUI statPageUI = GetManaUI();

        BattleUnitResources battleUnitResources = teamInfo.GetBattleUnitResources();

        float mana = battleUnitResources.GetManaPoints();
        float maxMana = battleUnitResources.GetMaxManaPoints();

        float sliderAmount = mana / maxMana;
        statPageUI.GetSlider().value = sliderAmount;
        statPageUI.GetAmountText().text = mana.ToString() + "/" + maxMana.ToString();
    }

    public StatPageUI GetHealthUI()
    {
        foreach(StatPageUI ui in statPageUIs)
        {
            if(ui.GetStatType() == StatType.Stamina)
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