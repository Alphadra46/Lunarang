using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;

public class SC_SaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        InitCloudSave();
    }

    /// <summary>
    /// Connect and authenticate to the Cloud Save service 
    /// </summary>
    private async void InitCloudSave()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
