using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class volumeManager : MonoBehaviour
{
    [SerializeField] string volumeParameter = "MasterVolume";
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] float multiplier = 20f;
    [SerializeField] private Toggle toggle;

    private bool disableToggleEvent;

    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener(HandleSliderValueChanged);
        toggle.onValueChanged.AddListener(HandleToggleValueChanged);
    }

    private void HandleToggleValueChanged(bool toggleSound)
    {
        if (disableToggleEvent) return;

        if (toggleSound)
        {
            volumeSlider.value = volumeSlider.maxValue;
        }
        else
        {
            volumeSlider.value = volumeSlider.minValue;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, volumeSlider.value);
    }

    private void HandleSliderValueChanged(float value)
    {
        mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);

        disableToggleEvent = true;
        toggle.isOn = volumeSlider.value > volumeSlider.minValue;
        disableToggleEvent = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumeParameter, volumeSlider.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
