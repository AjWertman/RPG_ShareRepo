using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    CanvasGroup canvasGroup = null;

    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public IEnumerator FadeOut(Color fadeColor, float time)
    {
        while(canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float time)
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
    }
}
