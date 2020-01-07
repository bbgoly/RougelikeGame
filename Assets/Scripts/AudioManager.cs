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
    public List<Audio> audioList;

    private void Awake()
    {
        foreach (Audio audio in audioList)
        {
            audio.audioSource = gameObject.AddComponent<AudioSource>();
            audio.audioSource.clip = audio.audioClip;
            audio.audioSource.volume = audio.audioVolume;
            audio.audioSource.pitch = audio.audioPitch;
            audio.audioSource.loop = audio.isLooped;
        }
    }

    public static Audio PlayAudio(string audioName, float startTime = 0)
    {
        Audio audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().audioList.Find(targetAudio => targetAudio.audioName == audioName);
        audio.audioSource.time = startTime;
        audio.audioSource.Play();
        Debug.Log("playing");
        return audio;
    }

    public static void StopAudio(string audioName)
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().audioList.Find(targetAudio => targetAudio.audioName == audioName).audioSource.Stop();
    }
}

/*
public static AudioManager audioManager;
if(audioManager)
{
    Destroy(gameObject);
}
audioManager = this;
DontDestroyOnLoad(gameObject);
*/