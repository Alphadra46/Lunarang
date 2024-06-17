using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_InventoryStatTemplate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    
    public TextMeshProUGUI statNameTMP;
    public TextMeshProUGUI statValueTMP;

    [PropertySpace(SpaceBefore = 15f)]
    public GameObject tooltipLeft;
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject tooltipRight;
    
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject tooltipTextTemplate;

    public bool isLeft = false;
    
    public void Init(string stat,string statName, string statValue, bool changeSide)
    {

        statNameTMP.text = statName;
        statValueTMP.text = statValue;

        isLeft = changeSide;
        
        InitTooltip(stat);

    }

    private void InitTooltip(string stat)
    {

        var parent = isLeft ? tooltipLeft.transform : tooltipRight.transform;
        
        switch (stat)
        {
            
            case "PV":
        
                var baseHP = Instantiate(tooltipTextTemplate, parent);
                if (baseHP.TryGetComponent(out TextMeshProUGUI baseHP_tmp))
                    baseHP_tmp.text = "PV de Base : " + SC_PlayerStats.instance.currentStats.maxHealth;
                baseHP.SetActive(true);
                
                var modifierHP = Instantiate(tooltipTextTemplate, parent);
                if (modifierHP.TryGetComponent(out TextMeshProUGUI modifierHP_tmp))
                    modifierHP_tmp.text = "Modificateur : " + SC_PlayerStats.instance.currentStats.maxHealthModifier + "%";
                modifierHP.SetActive(true);
                
                
                break;
            
            case "ATK":
        
                var baseATK = Instantiate(tooltipTextTemplate, parent);
                if (baseATK.TryGetComponent(out TextMeshProUGUI baseATK_tmp))
                    baseATK_tmp.text = "ATQ de Base : " + SC_PlayerStats.instance.currentStats.atkBase;
                baseATK.SetActive(true);
                
                var modifierATK = Instantiate(tooltipTextTemplate, parent);
                if (modifierATK.TryGetComponent(out TextMeshProUGUI modifierATK_tmp))
                    modifierATK_tmp.text = "Modificateur : " + SC_PlayerStats.instance.currentStats.atkModifier + "%";
                modifierATK.SetActive(true);
                
                
                break;
            
            case "DEF":
        
                var baseDEF = Instantiate(tooltipTextTemplate, parent);
                if (baseDEF.TryGetComponent(out TextMeshProUGUI baseDEF_tmp))
                    baseDEF_tmp.text = "DEF de Base : " + SC_PlayerStats.instance.currentStats.defBase;
                baseDEF.SetActive(true);
                
                var modifierDEF = Instantiate(tooltipTextTemplate, parent);
                if (modifierDEF.TryGetComponent(out TextMeshProUGUI modifierDEF_tmp))
                    modifierDEF_tmp.text = "Modificateur : " + SC_PlayerStats.instance.currentStats.defModifier + "%";
                modifierDEF.SetActive(true);
                
                
                break;
            
            case "SPD":
        
                var baseSPD = Instantiate(tooltipTextTemplate, parent);
                if (baseSPD.TryGetComponent(out TextMeshProUGUI baseSPD_tmp))
                    baseSPD_tmp.text = "VIT de Base : " + SC_PlayerStats.instance.currentStats.baseSpeed;
                baseSPD.SetActive(true);
                
                var modifierSPD = Instantiate(tooltipTextTemplate, parent);
                if (modifierSPD.TryGetComponent(out TextMeshProUGUI modifierSPD_tmp))
                    modifierSPD_tmp.text = "Modificateur : " + SC_PlayerStats.instance.currentStats.speedModifier + "%";
                modifierSPD.SetActive(true);
                
                
                break;
            
            case "DMG":
                    
                var baseDMG = Instantiate(tooltipTextTemplate, parent);
                if (baseDMG.TryGetComponent(out TextMeshProUGUI baseDMG_tmp))
                    baseDMG_tmp.text = "Bonus de DGT Généraux : " + SC_PlayerStats.instance.currentStats.damageBonus + "%";
                baseDMG.SetActive(true);
                
                var dotDMG = Instantiate(tooltipTextTemplate, parent);
                if (dotDMG.TryGetComponent(out TextMeshProUGUI dotDMG_tmp))
                    dotDMG_tmp.text = "Bonus de DGT sur la durée : " + SC_PlayerStats.instance.currentStats.dotDamageBonus + "%";
                dotDMG.SetActive(true);
                
                var mhDMG = Instantiate(tooltipTextTemplate, parent);
                if (mhDMG.TryGetComponent(out TextMeshProUGUI mhDMG_tmp))
                    mhDMG_tmp.text = "Bonus de DGT des Coups Multiples : " + SC_PlayerStats.instance.currentStats.mhDamageBonus + "%";
                mhDMG.SetActive(true);
                
                var pDMG = Instantiate(tooltipTextTemplate, parent);
                if (pDMG.TryGetComponent(out TextMeshProUGUI pDMG_tmp))
                    pDMG_tmp.text = "Bonus de DGT des Projectiles : " + SC_PlayerStats.instance.currentStats.projectileDamageBonus + "%";
                pDMG.SetActive(true);
                
                var aoeDMG = Instantiate(tooltipTextTemplate, parent);
                if (aoeDMG.TryGetComponent(out TextMeshProUGUI aoeDMG_tmp))
                    aoeDMG_tmp.text = "Bonus de DGT de Zone d'Effet : " + SC_PlayerStats.instance.currentStats.aoeDamageBonus + "%";
                aoeDMG.SetActive(true);
                
                break;
            
            case "TC":
        
                var baseTC = Instantiate(tooltipTextTemplate, parent);
                if (baseTC.TryGetComponent(out TextMeshProUGUI baseTC_tmp))
                    baseTC_tmp.text = "TC de Base : " + SC_PlayerStats.instance.currentStats.baseCritRate;
                baseTC.SetActive(true);
                
                var modifierTC = Instantiate(tooltipTextTemplate, parent);
                if (modifierTC.TryGetComponent(out TextMeshProUGUI modifierTC_tmp))
                    modifierTC_tmp.text = "Modificateur : " + SC_PlayerStats.instance.currentStats.bonusCritRate + "%";
                modifierTC.SetActive(true);
                
                break;
            
            case "DC":
        
                var baseDC = Instantiate(tooltipTextTemplate, parent);
                if (baseDC.TryGetComponent(out TextMeshProUGUI baseDC_tmp))
                    baseDC_tmp.text = "DC de Base : " + SC_PlayerStats.instance.currentStats.baseCritDMG;
                baseDC.SetActive(true);
                
                var modifierDC = Instantiate(tooltipTextTemplate, parent);
                if (modifierDC.TryGetComponent(out TextMeshProUGUI modifierDC_tmp))
                    modifierDC_tmp.text = "Modificateur : " + SC_PlayerStats.instance.currentStats.bonusCritDMG + "%";
                modifierDC.SetActive(true);
                
                break;
            
            case "CA":
        
                var poisonHR = Instantiate(tooltipTextTemplate, parent);
                if (poisonHR.TryGetComponent(out TextMeshProUGUI poisonHR_tmp))
                    poisonHR_tmp.text = "Chance d'appliquer Poison : " + SC_PlayerStats.instance.currentStats.poisonHitRate + "%";
                poisonHR.SetActive(true);
                
                var burnHR = Instantiate(tooltipTextTemplate, parent);
                if (burnHR.TryGetComponent(out TextMeshProUGUI burnHR_tmp))
                    burnHR_tmp.text = "Chance d'appliquer Brûlure : " + SC_PlayerStats.instance.currentStats.burnHitRate + "%";
                burnHR.SetActive(true);
                
                var freezeHR = Instantiate(tooltipTextTemplate, parent);
                if (freezeHR.TryGetComponent(out TextMeshProUGUI freezeHR_tmp))
                    freezeHR_tmp.text = "Chance d'appliquer Gel : " + SC_PlayerStats.instance.currentStats.freezeHitRate + "%";
                freezeHR.SetActive(true);
                
                var bleedHR = Instantiate(tooltipTextTemplate, parent);
                if (bleedHR.TryGetComponent(out TextMeshProUGUI bleedHR_tmp))
                    bleedHR_tmp.text = "Chance d'appliquer Saignement : " + SC_PlayerStats.instance.currentStats.bleedHitRate + "%";
                bleedHR.SetActive(true);
                
                break;
            
        }
        
    }

    private void ShowTooltip(bool value)
    {

        var parent = isLeft ? tooltipLeft : tooltipRight;
        
        parent.SetActive(value);
        
    }

    private void Update()
    {
        var parent = isLeft ? tooltipLeft : tooltipRight;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowTooltip(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ShowTooltip(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ShowTooltip(false);
    }
}
