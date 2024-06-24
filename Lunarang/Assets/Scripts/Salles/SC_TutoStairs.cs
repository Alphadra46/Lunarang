using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_TutoStairs : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        SC_UIManager.instance.CreateLoadingScreen(SceneManager.GetActiveScene().buildIndex+1);
        Resources.Load<SO_SkillInventory>("SkillInventory").ClearInventory();
    }
}
