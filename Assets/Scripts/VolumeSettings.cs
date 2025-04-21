using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider narratorSlider;

    void Start()
    {
        // Set ranges BEFORE assigning values
        SetSliderRange(masterSlider);
        SetSliderRange(musicSlider);
        SetSliderRange(sfxSlider);
        SetSliderRange(narratorSlider);

        // Set values BEFORE adding listeners to prevent triggering SetVolume immediately
        float value;

        if (audioMixer.GetFloat("MasterVolume", out value))
            masterSlider.value = DBToSliderValue(value);
        if (audioMixer.GetFloat("MusicVolume", out value))
            musicSlider.value = DBToSliderValue(value);
        if (audioMixer.GetFloat("SFXVolume", out value))
            sfxSlider.value = DBToSliderValue(value);
        if (audioMixer.GetFloat("NarratorVolume", out value))
            narratorSlider.value = DBToSliderValue(value);

        // Only now add listeners
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        narratorSlider.onValueChanged.AddListener(SetNarratorVolume);
    }





    private void SetSliderRange(Slider slider)
    {
        slider.minValue = 0.0001f; // Prevent -infinity dB
        slider.maxValue = 1f;
        if (slider.value == 0f) slider.value = 1f; // default to full volume
    }

    private void SetVolume(string parameterName, float value)
    {
        float dB;

        if (value <= 0.0001f)
            dB = -80f; // mute
        else
            dB = Mathf.Log10(value) * 20f; // standard dB curve

        audioMixer.SetFloat(parameterName, dB);
    }



    public void SetMasterVolume(float value) => SetVolume("MasterVolume", value);
    public void SetMusicVolume(float value) => SetVolume("MusicVolume", value);
    public void SetSFXVolume(float value) => SetVolume("SFXVolume", value);
    public void SetNarratorVolume(float value) => SetVolume("NarratorVolume", value);

    float DBToSliderValue(float db)
    {
        return Mathf.Pow(10f, db / 20f); // This matches the log10*20 calculation
    }



}
