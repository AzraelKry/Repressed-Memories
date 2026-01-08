using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Tracks")]
    public AudioClip mainMenuMusic;
    public AudioClip room1Music;
    public AudioClip room2Music;
    public AudioClip psychMusic;

    [Header("Settings")]
    public float musicVolume = 0.3f;
    public float fadeTime = 1.5f;

    private AudioSource sourceA;
    private AudioSource sourceB;

    private bool usingA = true;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        sourceA.loop = true;
        sourceB.loop = true;

        sourceA.volume = 0;
        sourceB.volume = 0;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clip = null;

        switch (scene.name)
        {
            case "MainMenu":
                clip = mainMenuMusic;
                break;
            case "Room1":
                clip = room1Music;
                break;
            case "Room2":
                clip = room2Music;
                break;
            case "PsychScene":
                clip = psychMusic;
                break;
            default:
                clip = null;
                break;
        }

        if (clip != null)
            PlayMusic(clip);
        else
            FadeOutAll();
    }

    public void PlayMusic(AudioClip clip)
    {
        AudioSource active = usingA ? sourceA : sourceB;
        AudioSource inactive = usingA ? sourceB : sourceA;

        usingA = !usingA;

        inactive.clip = clip;
        inactive.volume = 0f;
        inactive.Play();

        StartCoroutine(Crossfade(active, inactive, fadeTime));
    }

    public void FadeOutAll()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(sourceA, fadeTime));
        StartCoroutine(FadeOut(sourceB, fadeTime));
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to, float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float k = t / time;

            to.volume = Mathf.Lerp(0, musicVolume, k);
            from.volume = Mathf.Lerp(musicVolume, 0, k);

            yield return null;
        }

        from.Stop();
    }

    private IEnumerator FadeOut(AudioSource src, float time)
    {
        float start = src.volume;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(start, 0, t / time);
            yield return null;
        }

        src.Stop();
    }
}
