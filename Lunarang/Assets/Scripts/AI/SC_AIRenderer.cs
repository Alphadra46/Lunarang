using System;
using System.Collections;
using System.Collections.Generic;
using Enum;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
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


    private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    
    #endregion

    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent(out _animator)) return;
    }


    #region Debug

    /// <summary>
    /// Render the amount of damage taken by the Entity.
    /// </summary>
    /// <param name="damageTaken">Amount of damage taken.</param>
    /// <param name="isCrit"></param>
    public void DebugDamage(float damageTaken, bool isCrit)
    {
        if(DamageUIArea.transform.childCount > 0) Destroy(DamageUIArea.transform.GetChild(0).gameObject);
        
        var text = Instantiate(TextDamageUI, DamageUIArea.transform);
        
        if(text.TryGetComponent(out RectTransform rect)) rect.anchoredPosition =
            new Vector3(Random.Range(-40f, 40f), Random.Range(-35f, 35f));

        if (!text.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) return;
        
        textMeshProUGUI.text = damageTaken + (isCrit ? "!!" : "");
        textMeshProUGUI.color = isCrit ?new Color32(235, 56, 56, 255) : new Color32(232, 232, 232, 255);
    }

    public void DebugDamage(float damageTaken, bool isCrit, Enum_Debuff dotType)
    {
        if(DamageUIArea.transform.childCount > 0) Destroy(DamageUIArea.transform.GetChild(0).gameObject);
        
        var text = Instantiate(TextDamageUI, DamageUIArea.transform);
        
        if(text.TryGetComponent(out RectTransform rect)) rect.anchoredPosition =
            new Vector3(Random.Range(-40f, 40f), Random.Range(-35f, 35f));

        if (!text.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) return;
        
        textMeshProUGUI.text = damageTaken + (isCrit ? "!!" : "");
        
        switch (dotType)
        {
            case Enum_Debuff.Poison:
                textMeshProUGUI.color = isCrit ?new Color32(25, 108, 49, 255) : new Color32(50, 168, 82, 255);
                break;
            case Enum_Debuff.Bleed:
                break;
            case Enum_Debuff.Burn:
                break;
            case Enum_Debuff.Freeze:
                break;
            case Enum_Debuff.Slowdown:
                break;
            default:
                textMeshProUGUI.color = isCrit ?new Color32(235, 56, 56, 255) : new Color32(232, 232, 232, 255);
                break;
        }
        
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
        debugUIWeaknesses.text = "-";

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

    private void Update()
    {
        if(_agent == null) return;
        if(_animator == null) return;
        
        _animator.SetBool("canMove", true);
        _animator.SetBool("isMoving", _agent.velocity.magnitude > 1 ? true : false);

    }

    #endregion
    
}
