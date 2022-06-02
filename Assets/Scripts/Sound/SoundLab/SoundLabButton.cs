using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.Sound
{
    public class SoundLabButton : MonoBehaviour
    {
        Button button = null;

        bool isSongButton = true;

        public void InitalizeSoundButton(bool _isSongButton)
        {
            button.GetComponent<Button>();

            isSongButton = _isSongButton;
        }

        public Button GetButton()
        {
            return button;
        }

        public bool IsSongButton()
        {
            return isSongButton;
        }
    }
}
