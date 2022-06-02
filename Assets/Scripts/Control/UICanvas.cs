using RPGProject.Dialogue;
using RPGProject.UI;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class UICanvas : MonoBehaviour
    {
        PlayerMenuHandler playerMenu = null;
        CheckpointMenuHandler checkpointMenu = null;
        ActivateUIPrompt activateUIPrompt = null;
        DialogueUI dialogueUI = null;

        //[SerializeField] GameObject tutorial = null;

        private void Awake()
        {
            playerMenu = GetComponentInChildren<PlayerMenuHandler>();
            checkpointMenu = GetComponentInChildren<CheckpointMenuHandler>();
            activateUIPrompt = GetComponentInChildren<ActivateUIPrompt>();
            dialogueUI = GetComponentInChildren<DialogueUI>();
        }

        private void Start()
        {
            DeactivateAllUI();
        }

        public void ActivatePlayerMenu()
        {
            DeactivateAllUI();
            playerMenu.ActivateCoreMainMenu(true);
        }

        public void ActivateCheckpointMenu(Checkpoint _checkpoint)
        {
            DeactivateAllUI();
            checkpointMenu.ActivateCheckpointMenu(_checkpoint);
        }

        public void ActivateActivateUIPrompt(string _whatToActivate)
        {
            activateUIPrompt.ActivateActivateUI(_whatToActivate);
        }

        public void DeactivatePlayerMenu()
        {
            playerMenu.DeactivateAllMenus();
        }

        public void DeactivateCheckpointMenu()
        {
            checkpointMenu.DeactivateAllMenus();
        }

        public void DeactivateActivateUIPrompt()
        {
            activateUIPrompt.DeactivateActivateUI();
        }

        public void DeactivateAllUI()
        {
            foreach (GameObject gameObject in GetAllUIGameObjects())
            {
                gameObject.SetActive(false);
            }
        }

        public bool IsAnyMenuActive()
        {
            foreach(GameObject menu in GetAllUIGameObjects())
            {
                if (menu == activateUIPrompt.GetActivationPromptObject()) continue;
                if (menu.activeSelf) return true;
            }

            return false;
        }

        public bool IsAnyPlayerMenuActive()
        {
            foreach (GameObject menu in playerMenu.GetAllMenus())
            {
                if (menu.activeSelf) return true;
            }

            return false;
        }

        private IEnumerable<GameObject> GetAllUIGameObjects()
        {
            foreach(GameObject menu in playerMenu.GetAllMenus())
            {
                yield return menu;
            }
            yield return checkpointMenu.gameObject;
            yield return activateUIPrompt.GetActivationPromptObject();
            yield return dialogueUI.gameObject;
        }
    }
}
