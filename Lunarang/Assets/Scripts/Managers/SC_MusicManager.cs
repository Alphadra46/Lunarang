using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SC_MusicManager : SerializedMonoBehaviour
{

    public static SC_MusicManager instance;
    
    public Dictionary<string, AudioClip> musics;

    public AudioMixerGroup audioMixerGroup;
    
    private AudioSource source;
    

    private void Awake()
    {
        
        if(instance != null) Destroy(this);
        instance = this;
        
        if(!TryGetComponent(out source)) return;

        source.outputAudioMixerGroup = audioMixerGroup;
        
    }

    private void Start()
    {
        var music = SC_GameManager.instance.state switch 
        {
            GameState.Menu => "MU_MainMenu",
            GameState.FTUE => "MU_Temple",
            GameState.LOBBY => "MU_Village",
            GameState.TRAINING => "MU_Temple",
            GameState.RUN => "MU_Temple",
            _ => throw new ArgumentOutOfRangeException()
        };
        
        PlayMusic(music);
    }

    public void PlayMusic(string musicName)
    {

        source.clip = musics[musicName];
        source.Play();

    }
    
    
}
