using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_AIRenderer : MonoBehaviour
{

    #region Variables
    
    [BoxGroup("Damage Area")]
    public GameObject TextDamageUI;
    [BoxGroup("Damage Area")]
    public GameObject DamageUIArea;
    
    #region Debug

    [Space(10)]
    [BoxGroup("Debug Settings")] 
    public TextMeshProUGUI debugUIHP;
    [BoxGroup("Debug Settings")]
    public TextMeshProUGUI debugUIWeaknesses;
    
    #endregion
    
    
    #endregion


    #region Debug

    /// <summary>
    /// Render the amount of damage taken by the Entity.
    /// </summary>
    /// <param name="damageTaken">Amount of damage taken.</param>
    public void DebugDamage(float damageTaken)
    {
        if(DamageUIArea.transform.childCount > 0) Destroy(DamageUIArea.transform.GetChild(0).gameObject);
        
        var text = Instantiate(TextDamageUI, DamageUIArea.transform);
        
        if(text.TryGetComponent(out RectTransform rect)) rect.anchoredPosition =
            new Vector3(Random.Range(-0.55f, 0.55f), Random.Range(-0.55f, 0.55f));
        
        if(text.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) textMeshProUGUI.text = damageTaken.ToString();
    }

    #endregion

    #region Functions

    /// <summary>
    /// Update HP UI to the current HP Values.
    /// </summary>
    /// <param name="currentHP">Amount of current Health Point.</param>
    /// <param name="maxHP">Maximal amount of Health Point.</param>
    public void UpdateHealthBar(float currentHP, float maxHP)
    {
        debugUIHP.text = currentHP + "/" + maxHP;
    }
    
    /// <summary>
    /// Update Weakness UI to the current list of Weaknesses.
    /// </summary>
    /// <param name="currentWeakness">List of current Weaknesses</param>
    public void UpdateWeaknessBar(List<WeaponType> currentWeakness)
    {
        debugUIWeaknesses.text = "";

        for (var i = 0; i < currentWeakness.Count; i++)
        {
            if(i != currentWeakness.Count - 1)
            {
                debugUIWeaknesses.text += currentWeakness[i].ToString()[6..] + " | ";
            }
            else
            {
                debugUIWeaknesses.text += currentWeakness[i].ToString()[6..];
            }
        }    
    }
    
    #endregion
    
}
