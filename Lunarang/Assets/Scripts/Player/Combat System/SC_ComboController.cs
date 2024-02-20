using System;
using System.Collections.Generic;
using System.Linq;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class SC_ComboController : MonoBehaviour
{
    
    #region Variables

    public static SC_ComboController instance;
    
    [Title("Settings")]

    #region Combos

    [TabGroup("Settings", "Combo")]
    [SerializeField] private int comboMaxLength = 3;
    
    [TabGroup("Settings", "Combo")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
    public int comboCounter = 0;

    #endregion


    #region Weapons
    [PropertySpace(SpaceAfter = 5)]
    [TabGroup("Settings", "Weapon")]
    public List<Transform> weaponSockets = new List<Transform>();
    
    [TabGroup("Settings", "Weapon"), ShowInInspector, ReadOnly]
    public readonly Dictionary<string ,GameObject> equippedWeaponsGO = new Dictionary<string, GameObject>();
    
    [TabGroup("Settings", "Weapon")]
    public List<SC_Weapon> equippedWeapons = new List<SC_Weapon>();
    [PropertySpace(SpaceAfter = 5)]
    
    [TabGroup("Settings", "Weapon"), ShowInInspector, ReadOnly]
    public SC_Weapon currentWeapon;
    
    #endregion


    #region Types & Parameters

    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 5), ReadOnly]
    public WeaponType currentType;
    
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<WeaponType> currentComboWeaponTypes = new List<WeaponType>();
    
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<WeaponType> lastComboWeaponTypes = new List<WeaponType>();
    
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<ParameterType> currentComboParameters;
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<ParameterType> lastComboParameters;
    
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<SC_Weapon> currentComboWeapons;
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<SC_Weapon> lastComboWeapons;

    #endregion

    #region Events

    public static Action<int, int> ComboUpdated;

    [TabGroup("Settings", "Events")]
    public SO_Event onLastAttack;

    #endregion
    
    #region Input Buffering

    [TabGroup("Settings", "Combo")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
    // public SC_Weapon inputBufferedWeapon;
    
    [TabGroup("Settings", "Combo")]
    [SerializeField]public bool canPerformCombo = true;
    // private bool isInputBufferingOn = false;

    #endregion

    public Collider[] currentEnemiesHitted;
    
    public Animator _animator;
    private SC_PlayerController _controller;
    private SC_PlayerStats _stats;
    private SC_FinalATK_Builder _finalBuilder;
    [SerializeField] private List<VisualEffect> vfxParameterList = new List<VisualEffect>();

    public bool canAttack = true;
    
    #endregion

    #region Init

    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
        
        if(!TryGetComponent(out _controller)) return;
        if(!TryGetComponent(out _finalBuilder)) return;
        if(!TryGetComponent(out _stats)) return;
    }

    private void Start()
    {
        AttachInputToAttack();
        if (equippedWeapons.Count == 3)
        {
            AttachWeaponsToSocket();
        }
        
    }
    
    /// <summary>
    /// Attach inputs to functions
    /// </summary>
    public void AttachInputToAttack()
    {
        SC_InputManager.instance.weaponA.performed += _ => Attack(equippedWeapons[0]);
        SC_InputManager.instance.weaponB.performed += _ => Attack(equippedWeapons[1]);
        SC_InputManager.instance.weaponC.performed += _ => Attack(equippedWeapons[2]);
    }
    
    public void DettachInputToAttack()
    {
        SC_InputManager.instance.weaponA.performed -= _ => Attack(equippedWeapons[0]);
        SC_InputManager.instance.weaponB.performed -= _ => Attack(equippedWeapons[1]);
        SC_InputManager.instance.weaponC.performed -= _ => Attack(equippedWeapons[2]);
    }
    
    #endregion

    #region Functions
    
    /// <summary>
    /// Perform an attack and stack it in a combo counter.
    /// Update the animator and play the animation.
    /// Stock an input if already performing an attack.
    /// </summary>
    /// <param name="usedWeapon">Weapon used in this attack</param>
    private void Attack(SC_Weapon usedWeapon)
    {
        
        if(SC_GameManager.instance.isPause || !canAttack) return;


        if (!canPerformCombo) return;
        
        canAttack = false;
            
        lastComboWeapons = currentComboWeapons.ToArray().ToList();
        lastComboParameters = currentComboParameters.ToArray().ToList();
        lastComboWeaponTypes = currentComboWeaponTypes.ToArray().ToList();
        
        IncrementCombo(usedWeapon);
        UpdateAnimator();
            
        _controller.FreezeMovement(true);
        
    }

    /// <summary>
    /// Resend all values to the animator.
    /// </summary>
    private void UpdateAnimator()
    {
        
        _animator.SetInteger("Combo", comboCounter);
        if (currentWeapon != null)
            _animator.SetTrigger(currentWeapon.id);
        
        if (currentComboParameters.Count > 0)
        {
            _animator.SetInteger("Parameter_1", (int) currentComboParameters[0]);
        }
        else if (currentComboParameters.Count > 1)
        {
            _animator.SetInteger("Parameter_1", (int) currentComboParameters[0]);
            _animator.SetInteger("Parameter_2", (int) currentComboParameters[1]);
        }
        
    }

    private void AttachWeaponsToSocket()
    {
        for (var i = 0; i < equippedWeapons.Count; i++)
        {
            var weapon = equippedWeapons[i];
            var go = Instantiate(weapon.weaponPrefab, weaponSockets[i]);
            
            equippedWeaponsGO.Add(weapon.id,go);
        }
    }

    public void CreateHitBox(SO_HitBox hb)
    {
        // print(hb.name);
        var hits = hb.type switch
        {
            HitBoxType.Box => Physics.OverlapBox((transform.GetChild(1).position), hb.halfExtents,
                GetCurrentForwardVector(hb.orientation), hb.layer),
            HitBoxType.Sphere => Physics.OverlapSphere(hb.pos, hb.radiusSphere, hb.layer),
            HitBoxType.Capsule => Physics.OverlapCapsule(hb.point0, hb.point1, hb.radiusCapsule, hb.layer),
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var entity in hits)
        {
            
            var isCritical = Random.Range(0, 100) < _stats.critRate ? true : false;
            
            var currentMV = (currentWeapon.MovesValues[comboCounter-1]/100);
            
            var rawDamage = MathF.Round(currentMV * _stats.currentATK, MidpointRounding.AwayFromZero);
            var effDamage = rawDamage * (1 + (_stats.damageBonus/100));
            var effCrit = effDamage * (1 + (_stats.critDMG/100));
            
            entity.GetComponent<IDamageable>().TakeDamage(isCritical ? effCrit : effDamage, currentWeapon.type, isCritical);
            
            if(Random.Range(1, 100) < _stats.poisonHitRate)
            {
                entity.GetComponent<SC_DebuffsBuffsComponent>().ApplyDebuff(Enum_Debuff.Poison, GetComponent<SC_DebuffsBuffsComponent>());
            }
            
        }

        currentEnemiesHitted = hits;

    }

    /// <summary>
    /// Create a fully customizable projectile.
    /// </summary>
    /// <param name="projectilePrefab">Prefab of the projectile that we create</param>
    /// <param name="number">How many projectile</param>
    /// <param name="areaSize">Size of the projectile</param>
    /// <param name="hitNumber">How many hits he made.</param>
    /// <param name="moveValue">% of the attack</param>
    /// <param name="speed">Speed of the projectile</param>
    /// <param name="distanceMax">Maximum distance that can be covered before self-destruction</param>
    /// <param name="direction">Direction of the projectile</param>
    /// <param name="isAoE"></param>
    public void CreateProjectile(GameObject projectilePrefab, int number, float areaSize, int hitNumber, float moveValue, float speed, float distanceMax, Vector3 direction, bool isAoE = false)
    {

        for (var i = 0; i < number; i++)
        {
            var p = Instantiate(projectilePrefab).GetComponent<SC_Projectile>();
            var angle = Mathf.PI * (i+1) / (number+1);
            // print(angle);
                
            var x = Mathf.Sin(angle) * 2;
            var z = Mathf.Cos(angle) * 2;
            var pos = new Vector3(x, 0, z);
            
            var centerDirection = Quaternion.LookRotation(-transform.GetChild(1).right, transform.GetChild(1).up);

            pos = centerDirection * pos;
            
            
            p.transform.position = transform.position + new Vector3(pos.x, transform.localScale.y, pos.z);
            
            p.hitNumber = hitNumber;
            
            p.areaSize = areaSize;
            p.isAoE = isAoE;
            
            var isCritical = Random.Range(0, 100) < _stats.critRate ? true : false;
            
            var rawDamage = MathF.Round((moveValue/100) * _stats.currentATK, MidpointRounding.AwayFromZero);
            var effDamage = rawDamage * (1 + (_stats.damageBonus/100));
            var effCrit = effDamage * (1 + (_stats.critDMG/100));
            
            p.damage = isCritical ? effCrit : effDamage;
            p.isCrit = isCritical;
            p.weaponType = currentWeapon.type;
            
            p.sender = gameObject;
            
            p.speed = speed;
            p.distanceMax = distanceMax;
            p.direction = pos;

        }
        
    }
    
    #region Combo Part
    
    /// <summary>
    /// Set combo to performable.
    /// </summary>
    public void CanPerformCombo()
    {
        canAttack = true;
        canPerformCombo = true;

        // if (inputBufferedWeapon == null) return;
        //
        // Attack(inputBufferedWeapon);
        // inputBufferedWeapon = null;

    }
    
    /// <summary>
    /// Set combo to not performable.
    /// </summary>
    public void CantPerformCombo()
    {
        canPerformCombo = false;
    }

    public void CancelAttack()
    {
        var lastCounter = comboCounter-1;
        
        comboCounter = lastCounter;
        currentWeapon = null;
        currentType = WeaponType.Null;
        
        currentComboWeapons = lastComboWeapons.ToArray().ToList();
        currentComboParameters = lastComboParameters.ToArray().ToList();
        currentComboWeaponTypes = lastComboWeaponTypes.ToArray().ToList();
        
        lastComboWeapons.Clear();
        lastComboParameters.Clear();
        lastComboWeaponTypes.Clear();
        
        CanPerformCombo();
    }

    /// <summary>
    /// Check if the current combo reach its max length.
    /// Else increment combo, switch the weapon type to current type and add this to a list.
    /// Add parameters of the current combo to a list.
    /// </summary>
    /// <param name="newWeapon">New weapon to add to the current combo list</param>
    private void IncrementCombo(SC_Weapon newWeapon)
    {
        
        // Reset Combo after reach its max length.
        if (comboCounter+1 > comboMaxLength)
        {
            ResetCombo();
            
            //Reset save of last Incrementation of the combo
            lastComboWeapons.Clear();
            lastComboParameters.Clear();
            lastComboWeaponTypes.Clear();
            _finalBuilder.Reset();
        }
        
        // Increment combo, switch the weapon type to current type and add this to a list.
        comboCounter++;
        currentWeapon = newWeapon;
        currentType = newWeapon.type;
        
        currentComboWeaponTypes.Add(currentWeapon.type);
        currentComboParameters.Add(currentWeapon.parameter);
        currentComboWeapons.Add(currentWeapon);
        
        //Only spawn 1 VFX depending on the parameter of the hit
        if (comboCounter<=2)
        {
            switch (newWeapon.parameter)
            {
                case ParameterType.MultiHit:
                    vfxParameterList[0].Play();
                    break;
                case ParameterType.AreaOfEffect:
                    vfxParameterList[1].Play();
                    break;
                case ParameterType.Projectile:
                    vfxParameterList[2].Play();
                    break;
                default:
                    break;
            }
        }
        else //Spawn VFX depending on the current combo parameters for the finisher
        {
            foreach (var comboParameter in currentComboParameters)
            {
                switch (comboParameter)
                {
                    case ParameterType.MultiHit:
                        vfxParameterList[0].Play();
                        break;
                    case ParameterType.AreaOfEffect:
                        vfxParameterList[1].Play();
                        break;
                    case ParameterType.Projectile:
                        vfxParameterList[2].Play();
                        break;
                    default:
                        break;
                }
            }

        }
        
        if(comboCounter == comboMaxLength) onLastAttack.RaiseEvent();
        
        // Debug Side
        print("Combo : " + comboCounter + " / Type : " + currentWeapon.type);
        ComboUpdated?.Invoke(comboCounter == comboMaxLength ? 0 : comboCounter, comboMaxLength);
        
    }

    /// <summary>
    /// Reset the current combo and its parameters.
    /// </summary>
    public void ResetCombo()
    {
        comboCounter = 0;
        currentWeapon = null;
        currentType = WeaponType.Null;
        currentComboWeaponTypes.Clear();
        currentComboParameters.Clear();
        currentComboWeapons.Clear();
        UpdateAnimator();
    }

    #endregion

    #region Input Buffering
    
    /// <summary>
    /// Activate the possibility to do stock an input.
    /// </summary>
    public void ActivateInputBuffering()
    {
        // isInputBufferingOn = true;
        print("Buffering On");
    }
    
    /// <summary>
    /// Deactivate the possibility to do stock an input.
    /// </summary>
    public void DeactivateInputBuffering()
    {
        // isInputBufferingOn = false;
        print("Buffering Off");
    }
    
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position,transform.GetChild(1).forward);
    }
    
    Quaternion GetCurrentForwardVector(Quaternion orientation)

    {

        Vector3 forward = transform.forward;

        forward.y = 0;

        forward.Normalize();

        Quaternion rotation = Quaternion.LookRotation(forward);

        return rotation;

    }

    #endregion
    
}


