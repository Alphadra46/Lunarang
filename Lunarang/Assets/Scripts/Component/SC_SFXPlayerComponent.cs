using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class SC_SFXPlayerComponent : SerializedMonoBehaviour
{

    #region Variables

    private List<AudioSource> _audioSources = new List<AudioSource>();

    public Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    public AudioMixerGroup audioMixerGroup;

    #endregion
    

    public void CheckIfPlaying()
    {

        var deleteLists = new List<AudioSource>();
        
        foreach (var source in _audioSources.Where(source => !source.isPlaying))
        {
            Destroy(source);
            deleteLists.Add(source);
        }

        foreach (var deleteSource in deleteLists)
        {

            _audioSources.Remove(deleteSource);
            
        }
        
        deleteLists.Clear();
        
    }

    public void PlayClip(string clipName)
    {
        CheckIfPlaying();
        
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        
        audioSource.clip = clips[clipName];
        audioSource.Play();
        
        _audioSources.Add(audioSource);

    }
    
    public void PlayClip(AudioClip clip)
    {
        CheckIfPlaying();
        
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        
        audioSource.clip = clip;
        audioSource.Play();
        
        _audioSources.Add(audioSource);

    }

    public void PlayRandomClip(List<string> clipNames)
    {

        CheckIfPlaying();
        
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        
        audioSource.clip = clips[clipNames[Random.Range(0, clipNames.Count)]];
        audioSource.Play();
        
        _audioSources.Add(audioSource);

    }
    
}
