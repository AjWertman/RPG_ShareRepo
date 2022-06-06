using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class ResourceSlider : MonoBehaviour
    {
        Slider slider = null;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public void UpdateSliderValue(float _value)
        {
            slider.value = _value;
        }
    }
}
