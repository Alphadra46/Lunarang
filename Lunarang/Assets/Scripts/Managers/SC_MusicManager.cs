using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SC_MusicManager : SerializedMonoBehaviour
{

    public Dictionary<string, AudioClip> musics;

    public AudioMixerGroup audioMixerGroup;
    
    private AudioSource source;
    

    private void Awake()
    {
        
        if(!TryGetComponent(out source)) return;

        source.outputAudioMixerGroup = audioMixerGroup;
        
        
    }

    public void PlayMusic(string musicName)
    {

        source.clip = musics[musicName];
        source.Play();

    }
    
    
}
