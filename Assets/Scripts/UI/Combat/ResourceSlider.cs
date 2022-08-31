using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// The instance of a slider used to display the values of a character's resources.
    /// </summary>
    public class ResourceSlider : MonoBehaviour
    {
        Slider slider = null;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public void UpdateSliderValue(float _value)
        {
            //slider.value = _value;
        }
    }
}
