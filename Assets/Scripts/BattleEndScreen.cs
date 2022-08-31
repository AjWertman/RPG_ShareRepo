using RPGProject.Core;
using System.Collections;
using TMPro;
using UnityEngine;

public class BattleEndScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText = null;

    CanvasGroup canvasGroup = null;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public IEnumerator EndDemo(bool? _won)
    {
        if (_won == true)
        {
            resultText.text = "Win!!!";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "Lose";
            resultText.color = Color.red;
        }

        yield return FadeOut(Color.black, .5f);
        yield return new WaitForSeconds(3f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public IEnumerator FadeOut(Color _fadeColor, float _time)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / _time;
            yield return null;
        }
    }
}
