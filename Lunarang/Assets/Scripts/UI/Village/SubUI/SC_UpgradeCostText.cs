using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_UpgradeCostText : MonoBehaviour
{
    
    public Image iconImage;
    public TextMeshProUGUI nameCostTMP;

    public void Init(SC_Resource resource, int amount)
    {

        iconImage.sprite = resource.sprite;
        
        var color = SC_GameManager.instance.playerResourceInventory.CheckHasRessource(resource, amount)
            ? "<color=white>"
            : "<color=#FF4C4C>";
        
        nameCostTMP.text = color + resource.name + "  -  " + "x" + amount + "</color>\n"; 

    }
    
}
