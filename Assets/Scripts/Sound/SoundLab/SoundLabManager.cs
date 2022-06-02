using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.Sound
{
    public class SoundLabManager : MonoBehaviour
    {
        [SerializeField] GameObject soundButtonPrefab = null;
        [SerializeField] Transform buttonContentTransform = null;

        GameSound[] gameSounds = null;

        Dictionary<SoundLabButton, GameSound> soundDictionary = new Dictionary<SoundLabButton, GameSound>();

        List<SoundLabButton> songButtons = new List<SoundLabButton>();
        List<SoundLabButton> soundEffectButton = new List<SoundLabButton>();

        private void Awake()
        {
            gameSounds = Resources.FindObjectsOfTypeAll<GameSound>();
            PopulateSoundDatabase();
        }

        private void PopulateSoundDatabase()
        {
            foreach (GameSound gameSound in gameSounds)
            {
                bool isSong = (typeof(Song) == gameSound.GetType());

                SoundLabButton soundLabButton = CreateSoundButton(isSong);
                soundDictionary.Add(soundLabButton, gameSound);

                soundLabButton.GetButton().onClick.AddListener(() => ClickSoundButton(soundLabButton));

                if (isSong) songButtons.Add(soundLabButton);
                else soundEffectButton.Add(soundLabButton);
            }
        }

        private void ClickSoundButton(SoundLabButton _soundLabButton)
        {
            GameSound gameSound = soundDictionary[_soundLabButton];
            SelectSound(gameSound);
            _soundLabButton.GetButton().interactable = false;
        }

        private void SelectSound(GameSound gameSound)
        {
            //Populate current sound controls
            //Allow to change all variables
            //Save to scriptable object
            //Test
            //Reset
        }

        private SoundLabButton CreateSoundButton(bool _isSongButton)
        {
            GameObject buttonInstance = Instantiate(soundButtonPrefab, buttonContentTransform);
            SoundLabButton soundLabButton = buttonInstance.GetComponent<SoundLabButton>();
            soundLabButton.InitalizeSoundButton(_isSongButton);
            return soundLabButton;
        }
    }
}