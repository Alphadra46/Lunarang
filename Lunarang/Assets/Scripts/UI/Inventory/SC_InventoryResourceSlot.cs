using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_InventoryResourceSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler,
    IDeselectHandler
{

    #region Variables

    public SC_Resource resource;

    public Image iconImage;
    public TextMeshProUGUI amountTMP;

    public GameObject tooltip;

    #endregion

    private void Awake()
    {
        if(resource != null)
            Init(resource, SC_GameManager.instance.playerResourceInventory.AmountOf(resource));
        else
        {

            amountTMP.text = SC_GameManager.instance.archivesInventory.archivesOwned.Count.ToString();

            if (tooltip.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI name)) name.text = "Archives";
        }
        
    }

    public void Init(SC_Resource newResource, int amount)
    {

        resource = newResource;

        amountTMP.text = amount.ToString();
        iconImage.sprite = resource.sprite;

        if (tooltip.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI name)) name.text = resource.name;

    }

    public void RefreshAmount(int newAmount)
    {

        amountTMP.text = newAmount.ToString();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        tooltip.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        tooltip.SetActive(false);
    }

}
