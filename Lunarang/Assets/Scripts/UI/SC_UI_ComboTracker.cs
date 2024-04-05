using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_ComboTracker : MonoBehaviour
{
    public Sprite projectile, multiHit, aoe;
    [SerializeField] private GameObject slotPrefab;
    private List<Image> comboImagesSlot = new List<Image>();

    // Start is called before the first frame update
    void Awake()
    {
        SC_ComboController.ComboUpdated += ComboUpdate;
    }

    private void OnDisable()
    {
        SC_ComboController.ComboUpdated -= ComboUpdate;
    }

    private void ComboUpdate(int comboCounter, int comboMaxLength, ParameterType attackParameter)
    {
        comboImagesSlot.Add(Instantiate(slotPrefab, transform).GetComponent<Image>());
        
        Sprite image = projectile;
        
        switch (attackParameter)
        {
            case ParameterType.Projectile:
                image = projectile;
                break;
            case ParameterType.MultiHit:
                image = multiHit;
                break;
            case ParameterType.AreaOfEffect:
                image = aoe;
                break;
            default:
                break;
        }

        comboImagesSlot[comboCounter - 1].sprite = image;

        if (comboCounter >= comboMaxLength)
        {
            StartCoroutine(ResetImages());
        }
    }


    private IEnumerator ResetImages()
    {
        yield return new WaitForSeconds(0.5f);
        
        for (int i = 0; i < comboImagesSlot.Count; i++)
        {
            Destroy(comboImagesSlot[i].gameObject);
        }
        
        comboImagesSlot.Clear();
    }
}
