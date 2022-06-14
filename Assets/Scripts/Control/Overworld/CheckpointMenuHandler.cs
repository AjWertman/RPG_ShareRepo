using RPGProject.Core;
using RPGProject.Saving;
using RPGProject.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class CheckpointMenuHandler : MonoBehaviour
    {
        [SerializeField] CheckpointMenu checkpointMenu = null;
        [SerializeField] FastTravelMenu fastTravelMenu = null;

        PlayerController player = null;

        FastTravelPoint[] fastTravelPoints = null;
        Checkpoint currentCheckpoint = null;

        Dictionary<string, FastTravelPoint> fastTravelDictionary = new Dictionary<string, FastTravelPoint>();

        private void Awake()
        {
            player = FindObjectOfType<PlayerController>();

            PopulateFastTravelDictionary();

            checkpointMenu.onCheckpointMenuButton += OnCheckpointMenuButton;
            fastTravelMenu.onFastTravelButtonClick += OnFastTravelSelected;
            fastTravelMenu.onBackButton += ActivateCheckpointMenu;

            checkpointMenu.InitializeCheckpointMenu();
            fastTravelMenu.InitalizeFastTravelMenu();
        }

        private void OnFastTravelSelected(string id)
        {
            FastTravelPoint selectedFastTravelPoint = fastTravelDictionary[id];

            currentCheckpoint.FastTravel(selectedFastTravelPoint);
            DeactivateAllMenus();
        }

        public void ActivateCheckpointMenu(Checkpoint _checkpoint)
        {
            DeactivateAllMenus();
            currentCheckpoint = _checkpoint;
            checkpointMenu.gameObject.SetActive(true);
        }

        public void ActivateCheckpointMenu()
        {
            DeactivateAllMenus();
            checkpointMenu.gameObject.SetActive(true);
        }

        private void ActivateFastTravelMenu()
        {
            DeactivateAllMenus();
            fastTravelMenu.gameObject.SetActive(true);
            fastTravelMenu.SetupFastTravelMenu(GetDictKeys());
        }

        public void DeactivateAllMenus()
        {
            checkpointMenu.gameObject.SetActive(false);
            fastTravelMenu.gameObject.SetActive(false);
        }

        public void ResetCheckpointMenu()
        {
            currentCheckpoint = null;
            DeactivateAllMenus();
            fastTravelMenu.ResetFastTravelMenu();
        }

        public void OnCheckpointMenuButton(CheckpointMenuKey _checkpointMenuKey)
        {
            switch (_checkpointMenuKey)
            {
                case CheckpointMenuKey.FastTravel:

                    ActivateFastTravelMenu();
                    break;

                case CheckpointMenuKey.Heal:

                    FindObjectOfType<PlayerTeamManager>().RestoreAllResources();
                    break;

                case CheckpointMenuKey.Save:

                    //FindObjectOfType<SavingWrapper>().Save
                    break;
                    
                case CheckpointMenuKey.Quit:

                    DeactivateAllMenus();
                    break;
            }
        }

        private void PopulateFastTravelDictionary()
        {
            fastTravelPoints = FindObjectsOfType<FastTravelPoint>();

            foreach (FastTravelPoint fastTravelPoint in fastTravelPoints)
            {
                string pointName = fastTravelPoint.GetName();
                fastTravelDictionary.Add(pointName, fastTravelPoint);
            }
        }

        public IEnumerable<string> GetDictKeys()
        {
            foreach(string id in fastTravelDictionary.Keys)
            {
                yield return id;
            }
        }
    }
}
