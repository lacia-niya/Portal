using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// 音量管理脚本
/// </summary>
public class VolumeManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider MasterVolumeSlider;
    public Slider BGMVolumeSlider;
    public Slider SFXVolumeSlider;

    private void Start()
    {
        audioMixer.GetFloat("MasterVolume", out var currentVolume);
        MasterVolumeSlider.value = currentVolume;
        MasterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

        audioMixer.GetFloat("BGMVolume", out var currentBGMVolume);
        BGMVolumeSlider.value = currentBGMVolume;
        BGMVolumeSlider.onValueChanged.AddListener(SetBGMVolume);

        audioMixer.GetFloat("SFXVolume", out var currentSFXVolume);
        SFXVolumeSlider.value = currentSFXVolume;
        SFXVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        AudioManager.PlayAudio(AudioName.SLIDER);
        audioMixer?.SetFloat("MasterVolume", volume);
    }

    public void SetBGMVolume(float volume)
    {
        AudioManager.PlayAudio(AudioName.SLIDER);
        audioMixer?.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.PlayAudio(AudioName.SLIDER);
        audioMixer?.SetFloat("SFXVolume", volume);
    }
}
