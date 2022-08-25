using RPGProject.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class zzz_TargetSelect : MonoBehaviour
    {
        [SerializeField] GameObject targetButtonPrefab = null;
        [SerializeField] RectTransform contentRectTransform = null;

        [SerializeField] Button playerGroupButton = null;
        [SerializeField] Button enemyGroupButton = null;
        [SerializeField] Button backButton = null;

        List<Fighter> playerTargets = new List<Fighter>();
        List<Fighter> enemyTargets = new List<Fighter>();

        Dictionary<zzz_TargetButton, bool> targetButtons = new Dictionary<zzz_TargetButton, bool>();

        public event Action<Fighter> onTargetSelect;
        public event Action<Fighter> onTargetHighlight;
        public event Action onTargetUnhighlight;

        public void InitalizeTargetSelectMenu()
        {
            CreateTargetButtonsPool();

            enemyGroupButton.onClick.AddListener(() => OnGroupButtonSelect(false));
            playerGroupButton.onClick.AddListener(() => OnGroupButtonSelect(true));

            backButton.onClick.AddListener(OnBackButton);

            DeactivateGroupButtons();
        }

        private void CreateTargetButtonsPool()
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject targetButtonInstance = Instantiate(targetButtonPrefab, contentRectTransform);
                zzz_TargetButton targetButton = targetButtonInstance.GetComponent<zzz_TargetButton>();
                targetButton.InitalizeTargetButton();

                targetButton.onSelect += OnTargetSelect;

                targetButton.onPointerEnter += OnTargetHighlight;
                targetButton.onPointerExit += OnTargetUnhighlight;

                targetButtons.Add(targetButton, false);
                targetButton.gameObject.SetActive(false);
            }
        }

        private void OnTargetSelect(Fighter _target)
        {
            ResetTargetButtons();
            onTargetSelect(_target);
        }

        private void OnTargetHighlight(Fighter _target)
        {
            onTargetHighlight(_target);
        }

        private void OnTargetUnhighlight()
        {
            onTargetUnhighlight();
        }

        public void SetupTargetSelectMenu(TargetingType _targetingType)
        {

            SetupGroupButtons(_targetingType);

            bool isPlayerTarget = (_targetingType == TargetingType.PlayersOnly);
            PopulateTargetButtons(isPlayerTarget);
        }

        public void PopulateTargetButtons(bool _isPlayer)
        {
            ResetTargetButtons();

            List<Fighter> targets = GetTargets(_isPlayer);

            foreach (Fighter target in targets)
            {
                zzz_TargetButton targetButton = GetAvailableTargetButton();
                targetButton.SetupTargetButton(target);
                targetButtons[targetButton] = true;
                targetButton.gameObject.SetActive(true);
            }
        }

        public void SetupGroupButtons(TargetingType _targetingType)
        {
            DeactivateGroupButtons();
            switch (_targetingType)
            {
                case TargetingType.PlayersOnly:

                    playerGroupButton.gameObject.SetActive(true);
                    playerGroupButton.interactable = false;

                    enemyGroupButton.gameObject.SetActive(false);
                    break;

                case TargetingType.EnemiesOnly:

                    enemyGroupButton.gameObject.SetActive(true);
                    enemyGroupButton.interactable = false;

                    playerGroupButton.gameObject.SetActive(false);
                    break;

                case TargetingType.Everyone:

                    //have a bool to decide whether spell is friendly or not and start with that button.
                    //IE:Player has a holy spell that would be used to heal living beings, however, it does extra damage to undead
                    enemyGroupButton.gameObject.SetActive(true);
                    enemyGroupButton.interactable = false;

                    playerGroupButton.gameObject.SetActive(true);
                    playerGroupButton.interactable = true;
                    break;
            }
        }

        private void OnGroupButtonSelect(bool _isPlayer)
        {
            if (_isPlayer)
            {
                playerGroupButton.interactable = false;
                enemyGroupButton.interactable = true;
                PopulateTargetButtons(true);
            }
            else
            {
                enemyGroupButton.interactable = false;
                playerGroupButton.interactable = true;
                PopulateTargetButtons(false);
            }
        }

        public void UpdateUnitLists(List<Fighter> _playerTargets, List<Fighter> _enemyTargets)
        {
            playerTargets = _playerTargets;

            enemyTargets = _enemyTargets;
        }

        public void ResetTargetSelectMenu()
        {
            //previousPageKey = BattleUIMenuKey.None;
            playerTargets.Clear();
            enemyTargets.Clear();

            ResetTargetButtons();
        }

        public void ResetTargetButtons()
        {
            List<zzz_TargetButton> activeTargetButtons = GetActiveTargetButtons();
            if (activeTargetButtons.Count <= 0) return;

            List<zzz_TargetButton> dictionaryUpdateList = new List<zzz_TargetButton>();

            foreach (zzz_TargetButton targetButton in activeTargetButtons)
            {
                targetButton.ResetTargetButton();
                dictionaryUpdateList.Add(targetButton);
                targetButton.gameObject.SetActive(false);
            }

            foreach (zzz_TargetButton targetButton in dictionaryUpdateList)
            {
                targetButtons[targetButton] = false;
            }
        }

        private void DeactivateGroupButtons()
        {
            playerGroupButton.interactable = false;
            enemyGroupButton.interactable = false;
            playerGroupButton.gameObject.SetActive(false);
            enemyGroupButton.gameObject.SetActive(false);
        }

        public void OnBackButton()
        {
            //onBackButton(previousPageKey);
        }

        public List<Fighter> GetTargets(bool _isPlayer)
        {
            List<Fighter> targets = new List<Fighter>();

            foreach (Fighter target in GetTeamList(_isPlayer))
            {
                if (!target.GetHealth().isDead)
                {
                    targets.Add(target);
                }
            }

            return targets;
        }

        public List<Fighter> GetTeamList(bool _isPlayer)
        {
            List<Fighter> teamList = new List<Fighter>();

            if (_isPlayer)
            {
                teamList = playerTargets;
            }
            else
            {
                teamList = enemyTargets;
            }

            return teamList;
        }

        private zzz_TargetButton GetAvailableTargetButton()
        {
            zzz_TargetButton availableTargetButton = null;

            foreach (zzz_TargetButton targetButton in targetButtons.Keys)
            {

                if (targetButtons[targetButton] == false)
                {
                    availableTargetButton = targetButton;
                    break;
                }
            }

            return availableTargetButton;
        }

        private List<zzz_TargetButton> GetActiveTargetButtons()
        {
            List<zzz_TargetButton> activeTargetButtons = new List<zzz_TargetButton>();

            foreach (zzz_TargetButton targetButton in targetButtons.Keys)
            {
                if (targetButtons[targetButton])
                {
                    activeTargetButtons.Add(targetButton);
                }
            }

            return activeTargetButtons;
        }
    }
}
