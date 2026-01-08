using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController I;
    public Image fadeImage;

    void Awake()
    {
        I = this;
    }

    public IEnumerator FadeIn(float duration = 1f)
    {
        Color c = fadeImage.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(1, 0, t / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0;
        fadeImage.color = c;
    }

    public IEnumerator FadeOut(float duration = 1f)
    {
        Color c = fadeImage.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(0, 1, t / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1;
        fadeImage.color = c;
    }
}
