using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public enum PlayerMoveType { None, Attack, AbilitySelect, ItemSelect, Escape }

    public class PlayerMoveSelect : MonoBehaviour
    {
        [SerializeField] Button attackButton = null;
        [SerializeField] Button abilitySelectButton = null;
        [SerializeField] Button itemSelectButton = null;
        [SerializeField] Button escapeButton = null;

        public event Action<PlayerMoveType> onPlayerMoveSelect;

        public void InitalizePlayerMoveSelectMenu()
        {
            attackButton.onClick.AddListener(Attack);
            abilitySelectButton.onClick.AddListener(AbilitySelect);
            itemSelectButton.onClick.AddListener(ItemSelect);
            escapeButton.onClick.AddListener(Escape);
        }

        private void Attack()
        {
            onPlayerMoveSelect(PlayerMoveType.Attack);
        }

        private void AbilitySelect()
        {
            onPlayerMoveSelect(PlayerMoveType.AbilitySelect);
        }

        private void ItemSelect()
        {
            onPlayerMoveSelect(PlayerMoveType.ItemSelect);
        }

        private void Escape()
        {
            onPlayerMoveSelect(PlayerMoveType.Escape);
        }
    }
}