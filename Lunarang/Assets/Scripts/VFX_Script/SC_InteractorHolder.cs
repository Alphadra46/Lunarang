using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SC_InteractorHolder : MonoBehaviour
{
    private Interactor[] interactors;
    private Vector4[] positions = new Vector4[100];
    private float[] radiuses = new float[100];
    
    [Range(0, 1)]
    public float shapeCutoff;
    [Range(0, 1)]
    public float shapeSmoothness = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        FindInteractors();
    }

    private void OnEnable()
    {
        FindInteractors();
    }

    // Update is called once per frame
    void Update()
    {
        FindInteractors();

        for (int i = 0; i < interactors.Length; i++)
        {
            positions[i] = interactors[i].transform.position;
            radiuses[i] = interactors[i].radius;
        }
        
        Shader.SetGlobalVectorArray("_ShaderInteractorsPositions", positions);
        Shader.SetGlobalFloatArray("_ShaderInteractorsRadiuses", radiuses);
        
        Shader.SetGlobalFloat("_ShapeCutoff", shapeCutoff);
        Shader.SetGlobalFloat("_ShapeSmoothness", shapeSmoothness);
    }

    public void FindInteractors()
    {
        interactors = FindObjectsOfType<Interactor>();
    }

}
