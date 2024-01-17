using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_Lobby : MonoBehaviour
{
    public void TrainingRoom()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
        SC_GameManager.instance.ChangeState(GameState.RUN);
    }

    public void Temple()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
        SC_GameManager.instance.ChangeState(GameState.RUN);
    }
}
