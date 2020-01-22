using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Public Properties
    public List<Audio> audioList;
    #endregion

    #region Private Properties
    private static Audio currentRotatingAudio;
    #endregion

    #region Main code
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

    private void Update()
    {
        if (currentRotatingAudio == null || !currentRotatingAudio.audioSource.isPlaying)
        {
            currentRotatingAudio = audioList[Random.Range(0, audioList.Count - 1)];
            currentRotatingAudio.audioSource.Play();
        }
    }

    public static Audio PlayAudio(string audioName, bool loopAudio = false, float startTime = 0)
    {
        Audio audio = FindObjectOfType<AudioManager>().audioList.Find(targetAudio => targetAudio.audioName == audioName);
        audio.audioSource.time = startTime;
        audio.audioSource.loop = loopAudio;
        audio.audioSource.Play();
        return audio;
    }

    public static void StopAudio(string audioName)
    {
        FindObjectOfType<AudioManager>().audioList.Find(targetAudio => targetAudio.audioName == audioName).audioSource.Stop();
    }
    #endregion
}

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

#region Unused code
/*
public static AudioManager audioManager;
if(audioManager)
{
    Destroy(gameObject);
}
audioManager = this;
DontDestroyOnLoad(gameObject);

public void PlayList(List<string> audioNames, params float[] startTimes)
{
    foreach (Audio audio in FindObjectOfType<AudioManager>().audioList)
    {
        if (audioNames.Contains(audio.audioName))
        {
            audio.audioSource.time = startTimes.Length > 0 ? startTimes[audioNames.FindIndex(_audioName => _audioName == audio.audioName)] : 0;
            audio.audioSource.Play();
        }
    }
}
*/
#endregion