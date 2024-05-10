using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class SC_SFXPlayerComponent : SerializedMonoBehaviour
{

    #region Variables

    private AudioSource _audioSource;

    public Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    #endregion


    private void Awake()
    {
        if(!TryGetComponent(out _audioSource)) return;

        _audioSource.loop = false;
    }

    public void PlayClip(string clipName)
    {

        _audioSource.clip = clips[clipName];
        _audioSource.Play();

    }

    public void PlayRandomClip(List<string> clipNames)
    {

        _audioSource.clip = clips[clipNames[Random.Range(0, clipNames.Count)]];
        _audioSource.Play();

    }

    public void StopClip()
    {
        
        _audioSource.Stop();
        
    }
    
}
