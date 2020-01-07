using System.Collections.Generic;
using UnityEngine;

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
    public static List<Audio> audioList;
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

    public static void PlayAudio(string audioName)
    {
        audioList.Find(currentAudio => currentAudio.audioName == audioName).audioSource.Play();
    }

    public static void StopAudio(string audioName)
    {
        audioList.Find(currentAudio => currentAudio.audioName == audioName).audioSource.Stop();
    }
}
