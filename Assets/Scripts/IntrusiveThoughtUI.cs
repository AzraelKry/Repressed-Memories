using UnityEngine;
using TMPro;
using System.Collections;

public class IntrusiveThoughtUI : MonoBehaviour
{
    public static IntrusiveThoughtUI I;

    public GameObject canvas;
    public TMP_Text text;

    public AudioSource sfxSource;
    public AudioClip intrusiveSFX;

    private Coroutine fadeRoutine;

    void Awake()
    {
        I = this;
        canvas.SetActive(false);
    }

    public void Show(string line)
    {
        canvas.SetActive(true);
        text.text = line;

        if (PlayerMovement.Instance != null)
            PlayerMovement.Instance.CanMove = false;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        sfxSource.volume = 0.4f;

        if (intrusiveSFX != null && sfxSource != null)
            sfxSource.PlayOneShot(intrusiveSFX);


        StartCoroutine(ShakeLoop());  
    }


    public void Hide()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutSFX(0.2f));


        canvas.SetActive(false);
        text.text = "";

        if (PlayerMovement.Instance != null)
            PlayerMovement.Instance.CanMove = true;
    }

    private IEnumerator ShakeLoop()
    {
        RectTransform rt = text.GetComponent<RectTransform>();
        Vector3 originalPos = rt.localPosition;

        while (canvas.activeSelf)
        {
            float strength = StageManager.Instance != null
                ? StageManager.Instance.currentShakeStrength
                : 0.5f;

            rt.localPosition = originalPos + (Vector3)Random.insideUnitCircle * strength;

            yield return null;
        }

        rt.localPosition = originalPos;
    }

    private IEnumerator FadeOutSFX(float duration = 0.2f)
    {
        if (sfxSource == null) yield break;

        float startVolume = sfxSource.volume;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            sfxSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        sfxSource.Stop();
        sfxSource.volume = startVolume; 
    }
}
