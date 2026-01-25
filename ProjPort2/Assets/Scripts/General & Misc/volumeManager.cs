using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class volumeManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    private bool disableToggleEvent;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    public const string MIXER_MASTER = "MasterVolume";
    public const string MIXER_MUSIC  = "MusicVolume";
    public const string MIXER_SFX    = "SFXVolume";

    [SerializeField] private Toggle masterToggle;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        masterToggle.onValueChanged.AddListener(SetMasterToggle);
        musicToggle.onValueChanged.AddListener(SetMusicToggle);
        sfxToggle.onValueChanged.AddListener(SetSFXToggle);
    }

    void SetMasterVolume(float value)
    {
        mixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
    }

    void SetMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
    }

    void SetSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
    }

    private void SetMasterToggle(bool toggleSound)
    {
        if (disableToggleEvent) return;

        if (toggleSound)
        {
            masterSlider.value = masterSlider.maxValue;
        }
        else
        {
            masterSlider.value = masterSlider.minValue;
        }
    }
    private void SetMusicToggle(bool toggleSound)
    {
        if (disableToggleEvent) return;

        if (toggleSound)
        {
            musicSlider.value = musicSlider.maxValue;
        }
        else
        {
            musicSlider.value = musicSlider.minValue;
        }
    }
    private void SetSFXToggle(bool toggleSound)
    {
        if (disableToggleEvent) return;

        if (toggleSound)
        {
            sfxSlider.value = sfxSlider.maxValue;
        }
        else
        {
            sfxSlider.value = sfxSlider.minValue;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(MusicManager.MASTER_KEY, masterSlider.value);
        PlayerPrefs.SetFloat(MusicManager.MUSIC_KEY, musicSlider.value);
        PlayerPrefs.SetFloat(MusicManager.SFX_KEY, sfxSlider.value);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat(MusicManager.MASTER_KEY, 1f);
        musicSlider.value  = PlayerPrefs.GetFloat(MusicManager.MUSIC_KEY, 1f);
        sfxSlider.value    = PlayerPrefs.GetFloat(MusicManager.SFX_KEY, 1f);
    }
}
