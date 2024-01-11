using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Tymski;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class SC_SceneDuplicator : MonoBehaviour
{
    [Tooltip("The scene you want to copy to the loaded scene")]
    public SceneReference sceneToCopy;

    private Scene currentScene;

    /// <summary>
    /// This will load the sceneToCopy asynchronously in additive mode and then copy each different Game Objects to the actual scene
    /// </summary>
    [Button("Copy Scene objects")]
    public void DuplicateScene()
    {
        //Get the reference to the active scene
        currentScene = EditorSceneManager.GetActiveScene();
        
        //First we load the scene you want to copy in additive mode
        var s = EditorSceneManager.OpenScene(sceneToCopy,OpenSceneMode.Additive);

        //Then go through every game objects in the scene to copy from and copy them to the actual scene if they are different.
        var gameObjectList = s.GetRootGameObjects(); //Get all root game objects from the scene
        for (var i = 0; i < s.rootCount; i++)
        {
            //TODO - Check if the game object is already in the scene and if it's not different
            
            
            var go = Instantiate(gameObjectList[i]);
        }
        
        //Unload the scene once everything is finished
        EditorSceneManager.CloseScene(s, true);
    }
}
