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

    private void Awake()
    {
        roomCleared += RoomCleared;
        roomNewWave += RoomNewWave;
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
        
        tmp.text = "- Salle Purifié -";
    }
    
}
