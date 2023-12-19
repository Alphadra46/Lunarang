using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine;

public class TEMP : MonoBehaviour
{
    public int hpTEST;
    public string nameTEST;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
    }

    public async void Save()
    {
        var testData = new Dictionary<string, object>
        {
            {"hp",hpTEST},
            {"name",nameTEST}
        };

        await CloudSaveService.Instance.Data.ForceSaveAsync(testData);
        Debug.Log("Data saved");
    }

    public async void Load()
    {
        var testData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{"hp","name"});
        testData.TryGetValue("hp", out var value);
        hpTEST = int.Parse(value);
        testData.TryGetValue("name", out value);
        nameTEST = value;
    }
}
