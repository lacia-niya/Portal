using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音频管理用数据结构
/// </summary>
[Serializable]
public class Sound
{
    public AudioClip audioClip;
    public AudioMixerGroup outputAudioMixerGroup;
    [Range(0, 1)]
    public float volume = 1f;
    public bool playOnAwake = false;
    public bool loop = false;
}
