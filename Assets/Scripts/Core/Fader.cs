using System.Collections;
using UnityEngine;

namespace RPGProject.Core
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup = null;

        private void Awake()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
        }

        public IEnumerator FadeOut(Color _fadeColor, float _time)
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / _time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float _time)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / _time;
                yield return null;
            }
        }
    }
}
