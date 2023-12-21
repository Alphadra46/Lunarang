using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_PlayButton : MonoBehaviour
{
    /// <summary>
    /// Start the game
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //Can be set to 1 if the MainMenu scene is build index 0
    }
}
