using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeRender : MonoBehaviour
{

    //variables
    public GameObject FxRapiere;
    public GameObject FxHammer;
    public GameObject FxHammerFinal;
    public GameObject FxRapiereFinal;

    private bool HActive = false;
    private bool JActive = false;
    private bool KActive = false;
    private bool LActive = false;

    // Start is called before the first frame update
    void Start()
    {
        FxRapiere.SetActive(false);
        FxHammer.SetActive(false);
        FxHammerFinal.SetActive(false);
        FxRapiereFinal.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            if (HActive == false)
            {
                FxRapiere.SetActive(true);
                HActive = true;
            }
            else 
            {
                FxRapiere.SetActive(false);
                HActive = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (JActive == false)
            {
                FxHammer.SetActive(true);
                JActive = true;
            }
            else
            {
                FxHammer.SetActive(false);
                JActive = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (KActive == false)
            {
                FxHammerFinal.SetActive(true);
                KActive = true;
            }
            else
            {
                FxHammerFinal.SetActive(false);
                KActive = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (LActive == false)
            {
                FxRapiereFinal.SetActive(true);
                LActive = true;
            }
            else
            {
                FxRapiereFinal.SetActive(false);
                LActive = false;
            }
        }

    }

    
}
