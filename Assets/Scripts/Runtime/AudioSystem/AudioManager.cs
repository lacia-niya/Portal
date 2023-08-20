using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局音频管理器
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameEntry.Instance.gameManager.GetComponentInChildren<AudioManager>();
            }

            return instance;
        }
    }

    public List<Sound> sounds;

    private Dictionary<string, AudioSource> nameToAudioSource = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        foreach(var sound in sounds)
        {
            GameObject obj = new GameObject(sound.audioClip.name);
            obj.transform.SetParent(transform);

            AudioSource audioSource = obj.AddComponent<AudioSource>();
            audioSource.clip = sound.audioClip;
            audioSource.outputAudioMixerGroup = sound.outputAudioMixerGroup;
            audioSource.volume = sound.volume;
            audioSource.playOnAwake = sound.playOnAwake;
            audioSource.loop = sound.loop;

            if (sound.playOnAwake)
            {
                audioSource.Play();
            }

            nameToAudioSource.Add(sound.audioClip.name, audioSource);
        }
    }

    public static void PlayAudio(string audioClipName, bool wait = false)
    {
        if (Instance.nameToAudioSource.TryGetValue(audioClipName, out AudioSource audioSource))
        {
            if (!(audioSource.isPlaying && wait))
            {
                audioSource.Play();
            }
        }
        else
        {
            Debug.LogWarning($"音频{audioClipName}不存在");
        }
    }

    public static void StopAudio(string audioClipName)
    {
        if (Instance.nameToAudioSource.TryGetValue(audioClipName, out AudioSource audioSource))
        {
            audioSource.Stop();
        }
        else
        {
            Debug.LogWarning($"音频{audioClipName}不存在");
        }
    }
}
