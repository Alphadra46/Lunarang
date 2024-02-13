using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_Lobby : MonoBehaviour
{
    public void TrainingRoom()
    {
        SC_UIManager.instance.CreateLoadingScreen(2);
        SC_GameManager.instance.ChangeState(GameState.RUN);
    }

    public void Temple()
    {
        SC_UIManager.instance.CreateLoadingScreen(3);
        SC_GameManager.instance.ChangeState(GameState.RUN);
    }
}
