using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Audio
{
    public string audioName;

    [HideInInspector]
    public AudioSource audioSource;
    public AudioClip audioClip;
    public bool isLooped;
    [Space]
    [Range(0, 1)]
    public float audioVolume;
    [Range(0, 3)]
    public float audioPitch;
}


public class AudioManager : MonoBehaviour
{
    public List<Audio> audioList;
    public static AudioManager audioManager;

    private void Awake()
    {
        if(audioManager)
        {
            Destroy(gameObject);
        }
        audioManager = this;
        DontDestroyOnLoad(gameObject);

        foreach (Audio audio in audioList)
        {
            audio.audioSource = gameObject.AddComponent<AudioSource>();
            audio.audioSource.clip = audio.audioClip;
            audio.audioSource.volume = audio.audioVolume;
            audio.audioSource.pitch = audio.audioPitch;
            audio.audioSource.loop = audio.isLooped;
        }
    }

    private void Start()
    {
        PlayAudio("Test");
    }

    public Audio PlayAudio(string audioName)
    {
        Audio audio = audioList.Find(currentAudio => currentAudio.audioName == audioName);
        audio.audioSource.Play();
        return audio;
    }

    public void StopAudio(string audioName)
    {
        audioList.Find(currentAudio => currentAudio.audioName == audioName).audioSource.Stop();
    }
}
