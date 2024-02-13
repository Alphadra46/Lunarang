using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI textLoading;

    public void LoadScene(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        var op = SceneManager.LoadSceneAsync(sceneID);

        textLoading.text = "Chargement";
        
        while (!op.isDone)
        {
            if (textLoading.text == "Chargement...")
            {
                textLoading.text = "Chargement";
            }
            else
            {
                textLoading.text +=  ".";
            }
            yield return null;
        }
        
    }

}
