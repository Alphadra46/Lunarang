using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GameOver : MonoBehaviour
{
    public void Retry()
    {
        Resources.Load<SO_SkillInventory>("SkillInventory").ResetSkills();
        SC_UIManager.instance.CreateLoadingScreen(3);
        SC_GameManager.instance.ChangeState(GameState.RUN);
    }

    public void ReturnToLobby()
    {
        Resources.Load<SO_SkillInventory>("SkillInventory").ResetSkills();
        SC_UIManager.instance.CreateLoadingScreen(1);
        SC_GameManager.instance.ChangeState(GameState.LOBBY);
    }

    public void Quit()
    {
        SC_GameManager.instance.QuitGame();
    }
}
