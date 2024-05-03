using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GameOver : MonoBehaviour
{
    public void Retry()
    {
        SC_GameManager.instance.ChangeState(GameState.RUN);
    }

    public void ReturnToLobby()
    {
        SC_GameManager.instance.ChangeState(GameState.LOBBY);
    }

    public void Quit()
    {
        SC_GameManager.instance.QuitGame();
    }
}
