using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SC_AIRenderer : MonoBehaviour
{
    
    #region Variables
    
    [BoxGroup("Damage Area")]
    public GameObject TextDamageUI;
    [BoxGroup("Damage Area")]
    public GameObject StatsUI;
    [BoxGroup("Damage Area")]
    public GameObject DamageUIArea;

    public Slider HPBar;
    public Slider ShieldBar;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI ShieldText;
    

    [FormerlySerializedAs("_meshRenderer")]
    [BoxGroup("Damage Area")] 
    [SerializeField] private List<SkinnedMeshRenderer> _skinnedMeshRenderer;
    [BoxGroup("Damage Area")] 
    [SerializeField] private List<MeshRenderer> _meshRenderer;
    
    public Animator _animator;
    private NavMeshAgent _agent;

    public Action hideStatsUI;
    public Action showStatsUI;
    
    #endregion

    private void Awake()
    {
        if(!transform.GetChild(0).TryGetComponent(out _animator)) return;
        if(!TryGetComponent(out _agent)) return;

        hideStatsUI += HideStatsUI;
        showStatsUI += ShowStatsUI;
    }

    #region Debug

    /// <summary>
    /// Render the amount of damage taken by the Entity.
    /// </summary>
    /// <param name="damageTaken">Amount of damage taken.</param>
    /// <param name="isCrit"></param>
    public void DebugDamage(float damageTaken, bool isCrit)
    {
        
        var text = Instantiate(TextDamageUI, DamageUIArea.transform);
        
        if(text.TryGetComponent(out RectTransform rect)) rect.anchoredPosition =
            new Vector3(Random.Range(-40f, 40f), Random.Range(-35f, 35f));

        if (!text.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) return;
        
        textMeshProUGUI.text = damageTaken + (isCrit ? "!!" : "");
        textMeshProUGUI.color = isCrit ?new Color32(235, 56, 56, 255) : new Color32(232, 232, 232, 255);
    }

    public void DebugDamage(float damageTaken, bool isCrit, Enum_Debuff dotType)
    {
        
        var text = Instantiate(TextDamageUI, DamageUIArea.transform);
        
        if(text.TryGetComponent(out RectTransform rect)) rect.anchoredPosition =
            new Vector3(Random.Range(-40f, 40f), Random.Range(-35f, 35f));

        if (!text.TryGetComponent(out TextMeshProUGUI textMeshProUGUI)) return;
        
        textMeshProUGUI.text = damageTaken + (isCrit ? "!!" : "");
        
        switch (dotType)
        {
            case Enum_Debuff.Poison:
                textMeshProUGUI.color = isCrit ?new Color32(132, 191, 31, 255) : new Color32(179, 245, 66, 255);
                break;
            case Enum_Debuff.Bleed:
                break;
            case Enum_Debuff.Burn:
                textMeshProUGUI.color = isCrit ?new Color32(173, 38, 14, 255) : new Color32(232, 60, 30, 255);
                break;
            case Enum_Debuff.Freeze:
                textMeshProUGUI.color = isCrit ?new Color32(60, 164, 201, 255) : new Color32(114, 191, 219, 255);
                break;
            case Enum_Debuff.Slowdown:
                break;
            default:
                textMeshProUGUI.color = isCrit ?new Color32(235, 56, 56, 255) : new Color32(232, 232, 232, 255);
                break;
        }
        
    }

    public void RemoveDebugDamageChildren()
    {
        if (DamageUIArea.transform.childCount <= 0) return;
        for (int i = 0; i < DamageUIArea.transform.childCount; i++)
        {
            Destroy(DamageUIArea.transform.GetChild(i).gameObject);
        }

    }

    public void HideStatsUI()
    {
        StatsUI.SetActive(false);
    }
    public void ShowStatsUI()
    {
        StatsUI.SetActive(true);
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
        HPText.text = currentHP + "/" + maxHP;

        HPBar.value = currentHP / maxHP;
    }
    
    /// <summary>
    /// Update Shield UI.
    /// </summary>
    public void UpdateShieldBar(float currentShield, float maxShield)
    {
        
        ShieldText.text = $"{currentShield}/{maxShield}";
        ShieldBar.value = currentShield / maxShield;

    }

    private void Update()
    {
        if(_agent == null) return;
        if(_animator == null) return;
        
        SendBoolToAnimator("canMove", true);
        SendBoolToAnimator("isMoving", _agent.velocity.magnitude > 1 ? true : false);

    }

    public void SendTriggerToAnimator(string triggerName)
    {
        if(_animator == null) return;
        print("Trigger sended : " + triggerName);
        _animator.SetTrigger(triggerName);
        
    }
    
    public void SendBoolToAnimator(string booleanName, bool value)
    {
        if(_animator == null) return;
        _animator.SetBool(booleanName, value);
        
    }
    
    public void SendIntToAnimator(string integerName, int value)
    {
        if(_animator == null) return;
        _animator.SetInteger(integerName, value);
        
    }

    public void PauseAnimator(bool value)
    {

        _animator.speed = value ? 0 : 1;

    }

    public void StopAnimator()
    {
        _animator.enabled = false;
    }
    
    public IEnumerator DamageTaken()
    {
        if(_skinnedMeshRenderer.Count > 0) {
        
            foreach (var meshRenderer in _skinnedMeshRenderer)
            {
                var materials = meshRenderer.materials;
            
                foreach (var material in materials)
                {
                    material.SetFloat("_DamageAmount", 1f);
                }
            }
            yield return new WaitForSeconds(0.3f);
            foreach (var meshRenderer in _skinnedMeshRenderer)
            {
                var materials = meshRenderer.materials;
                foreach (var material in materials)
                {
                    material.SetFloat("_DamageAmount", 0f);
                }
            }
        
        }
        else
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                var materials = meshRenderer.materials;
            
                foreach (var material in materials)
                {
                    material.SetFloat("_DamageAmount", 1f);
                }
            }
            yield return new WaitForSeconds(0.3f);
            foreach (var meshRenderer in _meshRenderer)
            {
                var materials = meshRenderer.materials;
                foreach (var material in materials)
                {
                    material.SetFloat("_DamageAmount", 0f);
                }
            }
        }
    }

    public void EnterFreezeState()
    {
        if(_skinnedMeshRenderer.Count > 0){
        
            foreach (var meshRenderer in _skinnedMeshRenderer)
            {
                meshRenderer.materials[^1].SetFloat("_Opacity_Multiplier", 1f);
            }
            
        }
        else
        {
            
            foreach (var meshRenderer in _meshRenderer)
            {
                meshRenderer.materials[^1].SetFloat("_Opacity_Multiplier", 1f);
            }
            
        }
        
    }
    
    public void ExitFreezeState()
    {
        if(_skinnedMeshRenderer.Count > 0){
        
            foreach (var meshRenderer in _skinnedMeshRenderer)
            {
                meshRenderer.materials[^1].SetFloat("_Opacity_Multiplier", -0.5f);
            }
            
        }
        else
        {
            
            foreach (var meshRenderer in _meshRenderer)
            {
                meshRenderer.materials[^1].SetFloat("_Opacity_Multiplier", -0.5f);
            }
            
        }
    }

    public void ResetColor()
    {
        if(_skinnedMeshRenderer.Count > 0) {
        
            foreach (var meshRenderer in _skinnedMeshRenderer)
            {
                foreach (var material in meshRenderer.materials)
                {
                    material.SetFloat("_DamageAmount",0);
                }
            }
        
        }
        else
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                foreach (var material in meshRenderer.materials)
                {
                    material.SetFloat("_DamageAmount",0);
                }
            }
        }
    }
    
    #endregion
    
}
