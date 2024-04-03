using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SC_CameraShake : MonoBehaviour
{
    public static SC_CameraShake instance;

    private CinemachineBasicMultiChannelPerlin cameraNoise;
    private float timer;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraNoise = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }


    public IEnumerator ShakeCamera(float shakeAmplitude, float shakeFrequency, float shakeDuration)
    {
        timer = shakeDuration;

        cameraNoise.m_FrequencyGain = shakeFrequency;
        
        while (timer>=0)
        {
            cameraNoise.m_AmplitudeGain = Mathf.Lerp(shakeAmplitude, 0f, 1-(timer / shakeDuration));
            
            yield return null;
            timer -= Time.deltaTime;
        }

        cameraNoise.m_AmplitudeGain = 0f;
    }
    
}
