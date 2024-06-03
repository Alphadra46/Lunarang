using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_FeedbackRoomStatusUI : MonoBehaviour
{

    #region Variables

    public TextMeshProUGUI tmp;
    public Animator animator;

    #endregion

    public static Action roomCleared;
    public static Action<int> roomNewWave;
    
    public static Action resurected;

    private void Awake()
    {
        roomCleared += RoomCleared;
        roomNewWave += RoomNewWave;
        resurected += Resurected;
    }

    private void Resurected()
    {
        
        if(animator == null) return;
        animator.SetTrigger("UpdateText");
        
        tmp.text = $"- L'<color=lightblue> Essence <color=white>vous a réssucité. -";
        
    }

    private void RoomNewWave(int actualWave)
    {
        if(animator == null) return;
        animator.SetTrigger("UpdateText");
        
        tmp.text = $"- Nouvelle Vague -";
    }

    private void RoomCleared()
    {
        if(animator == null) return;
        animator.SetTrigger("UpdateText");
        
        tmp.text = "- Salle Purifiée -";
    }
    
}
