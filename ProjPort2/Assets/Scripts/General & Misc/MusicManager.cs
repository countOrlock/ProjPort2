using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    AudioSource track01, track02;
    [SerializeField] AudioClip defaultTrack;
    [SerializeField] float fadeTime;
    [SerializeField] AudioSource track01AudioSource;
    [SerializeField] AudioSource track02AudioSource;

    public bool track1Playing;

    float track01CurrentVolume;
    float track02CurrentVolume;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        track01 = track01AudioSource;
        track02 = track02AudioSource;

        track1Playing = true;

        ReturnToDefaultTrack();
    }

    public void SwapTrack(AudioClip newTrack)
    {
        StopAllCoroutines();

        StartCoroutine(FadeTrack(newTrack));

        track1Playing = !track1Playing;
    }

    public void ReturnToDefaultTrack()
    {
        SwapTrack(defaultTrack);
    }

    IEnumerator FadeTrack(AudioClip newTrack)
    {
        float timeElapsed = 0;
        float track01StartingVolume = track01.volume;
        float track02StartingVolume = track02.volume;


        if (track1Playing)
        {
            track02.clip = newTrack;
            track02.Play();

            while(timeElapsed < fadeTime)
            {
                track02.volume = Mathf.Lerp(0, 1, timeElapsed / fadeTime);
                track01.volume = Mathf.Lerp(track01StartingVolume, 0, timeElapsed / fadeTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            track01.Stop();
        }
        else
        {
            track01.clip = newTrack;
            track01.Play();

            while (timeElapsed < fadeTime)
            {
                track01.volume = Mathf.Lerp(0, 1, timeElapsed / fadeTime);
                track02.volume = Mathf.Lerp(track02StartingVolume, 0, timeElapsed / fadeTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            track02.Stop();
        }
    }
}
