using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_Lobby : MonoBehaviour
{
    public void TrainingRoom()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }

    public void Temple()
    {
        SceneManager.LoadSceneAsync(3, LoadSceneMode.Single);
    }
}
