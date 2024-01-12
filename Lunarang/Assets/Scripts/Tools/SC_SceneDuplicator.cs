using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<GameObject> allGameObjectsInNewScene = new List<GameObject>();

    /// <summary>
    /// This will load the sceneToCopy asynchronously in additive mode and then copy each different Game Objects to the actual scene
    /// </summary>
    [Button("Copy Scene objects")]
    public void DuplicateScene()
    {
        //Get the reference to the active scene
        currentScene = EditorSceneManager.GetActiveScene();

        var objectsInCurrentScene = currentScene.GetRootGameObjects();
        
        //First we load the scene you want to copy in additive mode
        var s = EditorSceneManager.OpenScene(sceneToCopy,OpenSceneMode.Additive);

        //Then go through every game objects in the scene to copy from and copy them to the actual scene if they are different.
        var gameObjectList = s.GetRootGameObjects(); //Get all root game objects from the scene

        //Get all item in from the scene to copy from
        // foreach (var go in gameObjectList)
        // {
        //     HaveChildren(go);
        // }

        for (var i = 0; i < s.rootCount; i++)
        {
            //TODO - Check if the game object is already in the scene and if it's not different
            if (IsObjectEqual(gameObjectList[i], objectsInCurrentScene[i]))
                continue;

            var go = Instantiate(gameObjectList[i]);
            go.name = go.name.Split('(')[0];
        }
        
        //Unload the scene once everything is finished
        EditorSceneManager.CloseScene(s, true);
    }

    /// <summary>
    /// Recursive function to check if the game object have one or more child and if not it adds itself to the list of game objects to copy
    /// </summary>
    /// <param name="go"></param>
    private void HaveChildren(GameObject go)
    {
        if (go.transform.childCount <= 0)
        {
            allGameObjectsInNewScene.Add(go);
        }
        else
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                HaveChildren(go.transform.GetChild(i).gameObject);
            }
        }
    }


    /// <summary>
    /// Will check if the items are exactly the same by going through every components they have
    /// </summary>
    /// <param name="goToCopy"></param>
    /// <param name="goFromOriginalScene"></param>
    /// <returns></returns>
    private bool IsObjectEqual(GameObject goToCopy, GameObject goFromOriginalScene)
    {
        List<Component> go1Components = new List<Component>();
        List<Component> go2Components = new List<Component>();

        go1Components.AddRange(goToCopy.GetComponents<Component>());
        go1Components.AddRange(goToCopy.GetComponentsInChildren<Component>());
        
        go2Components.AddRange(goFromOriginalScene.GetComponents<Component>());
        go2Components.AddRange(goFromOriginalScene.GetComponentsInChildren<Component>());

        //Dans cette boucle il y a un problème, ma liste d'objet de ma scène actuelle est bien trop grand comparé à celle de la scène à copier, donc quand je fais la comparaison c'est forcément pas bon
        foreach (var c1 in go1Components)
        {
            if (go2Components.Contains(c1))
            {
                foreach (var c2 in go2Components)
                {
                    if (!c1.Equals(c2))
                    {
                        return false;
                    }
                }
            }
        }

        Debug.Log("Object is the same");
        return true;
    }
}
