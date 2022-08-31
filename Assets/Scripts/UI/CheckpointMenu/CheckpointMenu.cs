using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public enum CheckpointMenuKey { Heal, FastTravel, Save, Quit}

    /// <summary>
    /// The menu used when at a checkpoint. Can be used to heal, open the fast travel menu, or save.
    /// </summary>
    public class CheckpointMenu : MonoBehaviour
    {
        [SerializeField] Button healTeamButton = null;
        [SerializeField] Button fastTravelButton = null;
        [SerializeField] Button saveButton = null;
        [SerializeField] Button exitButton = null;

        public event Action<CheckpointMenuKey> onCheckpointMenuButton;

        public void InitializeCheckpointMenu()
        {
            healTeamButton.onClick.AddListener(() => onCheckpointMenuButton(CheckpointMenuKey.Heal));
            fastTravelButton.onClick.AddListener(() => onCheckpointMenuButton(CheckpointMenuKey.FastTravel));
            saveButton.onClick.AddListener(() => onCheckpointMenuButton(CheckpointMenuKey.Save));
            exitButton.onClick.AddListener(() => onCheckpointMenuButton(CheckpointMenuKey.Quit));
        }
    }
}