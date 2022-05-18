using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpellPage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI emblemText = null;
    [SerializeField] Image emblemImage = null;
    [SerializeField] Image bookImage = null;
    [SerializeField] TextMeshProUGUI affilitionText = null;

    [SerializeField] GameObject spellsButtonPrefab = null;
    [SerializeField] Transform spellsContainer = null;

    [SerializeField] TextMeshProUGUI spellDescription = null;

    public void SetupSpellPage(Unit character)
    {
        //affilitionText.text = character.GetAffiliation();

        PopulateSpellList(character.GetSpells());
        DeactivateSpellTooltip();
    }

    public void PopulateSpellList(Ability[] spells)
    {
        ClearSpellList();

        foreach(Ability spell in spells)
        {
            GameObject spellInstance = Instantiate(spellsButtonPrefab, spellsContainer);
            SpellUIButton spellUIButton = spellInstance.GetComponent<SpellUIButton>();
            spellUIButton.SetupSpellButton(spell);
            spellUIButton.onMouseEnter += ActivateSpellTooltip;
            spellUIButton.onMouseExit += DeactivateSpellTooltip;
        }
    }

    private void ClearSpellList()
    {
        foreach(RectTransform spell in spellsContainer)
        {
            Destroy(spell.gameObject);
        }
    }

    private void ActivateSpellTooltip(Ability ability)
    {
        spellDescription.text = ability.spellDescription;
    }

    private void DeactivateSpellTooltip()
    {
        spellDescription.text = "*No Spell Selected*";
    }
}
