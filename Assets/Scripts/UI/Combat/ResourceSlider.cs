using UnityEngine;
using UnityEngine.UI;

public class ResourceSlider : MonoBehaviour
{
    Slider slider = null;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void UpdateSliderValue(float value)
    {
        slider.value = value;
    }
}
