using System;
using System.Collections;
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
    public int comboMaxLength = 3;
    
    [TabGroup("Settings", "Combo")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
    public int comboCounter = 0;

    public VisualEffect comboCounterVFX;

    [HideInInspector] public StateMachineBehaviour currentState;
    
    #endregion


    #region Weapons
    [PropertySpace(SpaceAfter = 5)]
    [TabGroup("Settings", "Weapon")]
    public List<Transform> weaponSockets = new List<Transform>();
    
    // [TabGroup("Settings", "Weapon"), ShowInInspector, ReadOnly]
    // public readonly Dictionary<string ,GameObject> equippedWeaponsGO = new Dictionary<string, GameObject>();
    
    [PropertySpace(SpaceAfter = 5)]
    [TabGroup("Settings", "Weapon"), ShowInInspector, ReadOnly]
    public SC_Weapon currentWeapon;
    
    #endregion


    #region Types & Parameters
    
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<SC_Weapon> currentComboWeapons;
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<SC_Weapon> lastComboWeapons;

    #endregion

    #region Events

    public static Action<int, int, ParameterType> ComboUpdated; //UI

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
    
    [PropertySpace(SpaceBefore = 15f)]
    public LayerMask layerAttackable;
    [PropertySpace(SpaceBefore = 15f)]
    public Collider[] currentEnemiesHitted;
    
    [PropertySpace(SpaceBefore = 15f)]
    [HideInInspector] public Animator _animator;
    
    private SC_PlayerController _controller;
    private SC_PlayerStats _stats;
    private SC_FinalATK_Builder _finalBuilder;
    private SC_DebuffsBuffsComponent _debuffsBuffsComponent;

    public Dictionary<string, GameObject> equippedWeaponsGO = new Dictionary<string, GameObject>();

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
        if(!TryGetComponent(out _debuffsBuffsComponent)) return;
    }

    private void Start()
    {
        //AttachInputToAttack();
        if (SC_GameManager.instance.weaponInventory.weaponsEquipped.Count == 3)
        {
            AttachWeaponsToSocket();
        }
        
    }
    
    
    
    #endregion

    #region Functions

    /// <summary>
    /// Resend all values to the animator.
    /// </summary>
    private void UpdateAnimator()
    {
        if(_animator == null) return;

        _animator.SetInteger("Combo", comboCounter);
    }

    private void AttachWeaponsToSocket()
    {
        if(equippedWeaponsGO.Count > 0) DettachWeaponsToSocket();
        
        for (var i = 0; i < SC_GameManager.instance.weaponInventory.weaponsEquipped.Count; i++)
        {
            var weapon = SC_GameManager.instance.weaponInventory.weaponsEquipped[i];
            var go = Instantiate(weapon.weaponPrefab, weaponSockets[i]);
            
            equippedWeaponsGO.Add(weapon.id, go);
        }
    }

    private void DettachWeaponsToSocket()
    {

        foreach (var go in equippedWeaponsGO.Values)
        {
            Destroy(go);
        }
        
        equippedWeaponsGO.Clear();
    }

    #region Hurtboxes

    public void CreateHitBox(SO_HitBox hb)
    {
        var hbTransform = transform.GetChild(1);
        transform.GetChild(1).localPosition = hb.center;
        
        // print(hb.name);
        var hits = hb.type switch
        {
            HitBoxType.Box => Physics.OverlapBox(hbTransform.position, hb.halfExtents,
                GetCurrentForwardVector(hb.orientation), hb.layer),
            HitBoxType.Sphere => Physics.OverlapSphere(hb.pos, hb.radiusSphere, hb.layer),
            HitBoxType.Capsule => Physics.OverlapCapsule(hb.point0, hb.point1, hb.radiusCapsule, hb.layer),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        currentEnemiesHitted = hits;

        foreach (var e in hits)
        {
            
            var isCritical = Random.Range(0, 100) < _stats.currentStats.critRate ? true : false;
            
            var currentMV = ((currentWeapon.baseMovesValues[comboCounter - 1] + (currentWeapon.levelUpStatsRate * currentWeapon.currentLevel-1)) / 100);
            
            var rawDamage = MathF.Round(currentMV * _stats.currentStats.currentATK, MidpointRounding.AwayFromZero);
            var effDamage = rawDamage * (1 + (_stats.currentStats.damageBonus/100));
            var effCrit = effDamage * (1 + (_stats.currentStats.critDMG/100));
            
            e.GetComponent<IDamageable>().TakeDamage(isCritical ? effCrit : effDamage, isCritical, gameObject);
            
            if(e.GetComponent<SC_AIStats>().isDead) continue;
            
            CheckAllDebuffApplication(e);
        }

        if (currentEnemiesHitted.Length > 0)
            StartCoroutine(SC_CameraShake.instance.ShakeCamera(currentWeapon.parameter==ParameterType.AreaOfEffect?2f:1f, 1f, 0.2f));

    }

   /// <summary>
   /// Create a fully customizable projectile.
   /// </summary>
   /// <param name="projectilePrefab">Prefab of the projectile that we create</param>
   /// <param name="number">How many projectile</param>
   /// <param name="areaSize">Size of the projectile</param>
   /// <param name="additionalHits">How many hits he made.</param>
   /// <param name="moveValue">% of the attack</param>
   /// <param name="distanceMax">Maximum distance that can be covered before self-destruction</param>
   /// <param name="direction">Direction of the projectile</param>
   /// <param name="isAoE"></param>
   public void CreateProjectile(GameObject projectilePrefab, int number, float areaSize, int additionalHits, float moveValue, float distanceMax, Vector3 direction, bool isAoE = false)
   {
       var scProjectile = projectilePrefab.GetComponent<SC_Projectile>();

       var formation = scProjectile.formations[number];
       var spawnPoint = scProjectile.spawnPoint;

       var spawnPos = spawnPoint switch
       {
           ProjectileSpawnPoint.PlayerCenterPoint => transform.position,
           ProjectileSpawnPoint.PlayerCenterPointFloor => new Vector3(transform.position.x, 0.10f, transform.position.z),
           ProjectileSpawnPoint.PlayerHead => transform.GetChild(2).position,
           ProjectileSpawnPoint.Weapon => equippedWeaponsGO[currentWeapon.id].transform.Find("ImpactPoint").position,
           _ => throw new ArgumentOutOfRangeException()
       };

       switch (formation)
       {
           case ProjectileFormation.Cone:
           {
               for (var i = 0; i < number; i++)
               {
                   var p = Instantiate(projectilePrefab).GetComponent<SC_Projectile>();

                   var angle = Mathf.PI * (i + 1) / (number + 1);

                   var x = Mathf.Sin(angle) * 2;
                   var z = Mathf.Cos(angle) * 2;
                   var pos = new Vector3(x, 0, z);

                   var centerDirection = Quaternion.LookRotation(-transform.GetChild(1).right, transform.GetChild(1).up);

                   pos = centerDirection * pos;

                   p.transform.position = spawnPoint switch
                   {
                       ProjectileSpawnPoint.PlayerCenterPoint => spawnPos + new Vector3(pos.x, transform.localScale.y, pos.z),
                       ProjectileSpawnPoint.PlayerCenterPointFloor => spawnPos + new Vector3(pos.x, 0.1f, pos.z),
                       ProjectileSpawnPoint.PlayerHead => spawnPos + new Vector3(pos.x, transform.localScale.y, pos.z),
                       ProjectileSpawnPoint.Weapon => spawnPos + new Vector3(pos.x, 0.25f, pos.z) * 0.25f,
                       _ => throw new ArgumentOutOfRangeException()
                   };

                   p.additionalHits = additionalHits;

                   p.areaSize = areaSize;
                   p.isAoE = isAoE;

                   var isCritical = Random.Range(0, 100) < _stats.currentStats.critRate ? true : false;

                   var rawDamage = MathF.Round((moveValue / 100) * _stats.currentStats.currentATK, MidpointRounding.AwayFromZero);
                   var effDamage = rawDamage * (1 + (_stats.currentStats.damageBonus / 100) + (_stats.currentStats.projectileDamageBonus / 100) + (isAoE ? _stats.currentStats.projectileDamageBonus : 0 / 100));
                   var effCrit = effDamage * (1 + (_stats.currentStats.critDMG / 100));

                   p.damage = isCritical ? effCrit : effDamage;
                   p.isCrit = isCritical;

                   p.sender = gameObject;
               
                   p.distanceMax = distanceMax;
                   p.direction = pos;

               }

               break;
           }
           case ProjectileFormation.Inline:
               StartCoroutine(ProjectileInline(projectilePrefab, number, areaSize, additionalHits, moveValue, distanceMax, direction, isAoE));
               break;
           default:
               throw new ArgumentOutOfRangeException();
       }
   }

   public IEnumerator ProjectileInline(GameObject projectilePrefab, int numberTotal, float areaSize, int hitNumber, float moveValue, float distanceMax, Vector3 direction, bool isAoE = false)
   {
       var number = 0;
       
       var scProjectile = projectilePrefab.GetComponent<SC_Projectile>();
       
       var spawnPoint = scProjectile.spawnPoint;

       var spawnPos = spawnPoint switch
       {
           ProjectileSpawnPoint.PlayerCenterPoint => transform.position,
           ProjectileSpawnPoint.PlayerCenterPointFloor => new Vector3(transform.position.x, 0.10f, transform.position.z),
           ProjectileSpawnPoint.PlayerHead => transform.GetChild(2).position,
           ProjectileSpawnPoint.Weapon => equippedWeaponsGO[currentWeapon.id].transform.Find("ImpactPoint").position,
           _ => throw new ArgumentOutOfRangeException()
       };

       while (number != numberTotal)
       {
           var p = Instantiate(projectilePrefab).GetComponent<SC_Projectile>();

           p.transform.position = p.transform.position = spawnPoint switch
           {
               ProjectileSpawnPoint.PlayerCenterPoint => new Vector3(spawnPos.x, transform.localScale.y,
                   spawnPos.z),
               ProjectileSpawnPoint.PlayerCenterPointFloor => new Vector3(spawnPos.x, 0.2f,
                   spawnPos.z),
               ProjectileSpawnPoint.PlayerHead => spawnPos,
               ProjectileSpawnPoint.Weapon => spawnPos + new Vector3(0, 0.25f, 0),
           };

           p.additionalHits = hitNumber;

           p.areaSize = areaSize;
           p.isAoE = isAoE;

           var isCritical = Random.Range(0, 100) < _stats.currentStats.critRate ? true : false;

           var rawDamage = MathF.Round((moveValue / 100) * _stats.currentStats.currentATK, MidpointRounding.AwayFromZero);
           var effDamage = rawDamage * (1 + (_stats.currentStats.damageBonus / 100) + (_stats.currentStats.projectileDamageBonus / 100) + (isAoE ? _stats.currentStats.projectileDamageBonus : 0 / 100));
           var effCrit = effDamage * (1 + (_stats.currentStats.critDMG / 100));

           p.damage = isCritical ? effCrit : effDamage;
           p.isCrit = isCritical;

           p.sender = gameObject;
           
           p.distanceMax = distanceMax;
           p.direction = transform.GetChild(1).forward;

           number++;
           yield return new WaitForSeconds(0.25f);
       }

   }
    
    /// <summary>
    /// Create multiples hits.
    /// </summary>
    public void Multihit(int additionnalHits)
    {
        
        var currentMV = ((currentWeapon.baseMovesValues[comboCounter - 1] + (currentWeapon.levelUpStatsRate * currentWeapon.currentLevel-1)) / 100);

        var rawDamage = MathF.Round(currentMV * _stats.currentStats.currentATK, MidpointRounding.AwayFromZero);
        var effDamage = rawDamage * (1 + (_stats.currentStats.damageBonus / 100) + (_stats.currentStats.mhDamageBonus / 100));
        var effCrit = effDamage * (1 + (_stats.currentStats.critDMG / 100));
        
        print(currentEnemiesHitted.Length);
        
        foreach (var e in currentEnemiesHitted)
        {
            if (!e.TryGetComponent(out IDamageable damageable)) continue;
                            
            for (var i = 0; i < additionnalHits; i++)
            {
                                
                var isCritical = Random.Range(0, 100) < _stats.currentStats.critRate ? true : false;
                damageable.TakeDamage(isCritical ? effCrit : effDamage, isCritical, gameObject);

                CheckAllDebuffApplication(e);
                
            }

        }
        
    }

    /// <summary>
    /// Create an area of effect at a certain pos.
    /// </summary>
    /// <param name="pos">Center of the AoE</param>
    /// <param name="areaSize"></param>
    /// <param name="isDoT"></param>
    /// <param name="hasAdditionnalHits">Is the AoE has additionnal hits ?</param>
    /// <param name="additionnalHits"></param>
    /// <param name="currentMV"></param>
    public void CreateAoE(Vector3 pos, float areaSize, float currentMV, bool isDoT = false,bool hasAdditionnalHits = false, int additionnalHits = 0)
    {

        var rawDamage = MathF.Round(currentMV * _stats.currentStats.currentATK, MidpointRounding.AwayFromZero);
        var effDamage = rawDamage * (1 + (_stats.currentStats.damageBonus / 100) + (_stats.currentStats.aoeDamageBonus / 100));
        var effCrit = effDamage * (1 + (_stats.currentStats.critDMG / 100));
        
        var effDoTDamage = Mathf.Round(rawDamage * (1 + (_debuffsBuffsComponent.burnDMGBonus + (_debuffsBuffsComponent._playerStats.currentStats.dotDamageBonus))/100));
        
        var effDoTCrit = effDoTDamage * (1 + (_debuffsBuffsComponent.dotCritDamage/100));

        var ennemiesInAoE =
            Physics.OverlapSphere((pos), areaSize,
                layerAttackable); //TODO : Replace Pos by Weapon Hit Pos

        foreach (var e in ennemiesInAoE)
        {
            
            if (!e.TryGetComponent(out IDamageable damageable)) continue;
            var isCritical = Random.Range(0, 100) < _stats.currentStats.critRate ? true : false;
            var isDoTCritical = Random.Range(0, 100) < _debuffsBuffsComponent.dotCritRate ? true : false;

            if (!isDoT)
            {
                damageable.TakeDamage(isCritical ? effCrit : effDamage, isCritical, gameObject);
            }
            else
            {
                damageable.TakeDoTDamage(isDoTCritical ? effDoTCrit : effDoTDamage, isDoTCritical, Enum_Debuff.Burn);
            }
            
            CheckPoisonHit(e);

            if(!hasAdditionnalHits) continue;
            for (var i = 0; i < additionnalHits; i++)
            {
                                
                isCritical = Random.Range(0, 100) < _stats.currentStats.critRate ? true : false;
                damageable.TakeDamage(isCritical ? effCrit : effDamage, isCritical, gameObject);

            }

        }
    }

    #endregion
    
    
    #region Combo Part


    public void ExitPreviousState(StateMachineBehaviour newState)
    {
        if (currentState == null)
            return;

        var idleState = currentState as SC_Idle;
        if (idleState != null)
            ((SC_Idle)currentState).Exit();

        var attackState = currentState as SC_Attack;
        if (attackState != null)
            ((SC_Attack)currentState).Exit();

        var moveState = currentState as SC_Move;
        if (moveState !=null)
            ((SC_Move)currentState).Exit();

        currentState = newState;
    }
    
    public void CancelAttack()
    {
        var lastCounter = comboCounter-1;
        
        comboCounter = lastCounter;
        currentWeapon = null;
        
        //currentComboWeapons = lastComboWeapons.ToList();
        
        //lastComboWeapons.Clear();
        UpdateAnimator();
        ComboUpdated?.Invoke(comboCounter, comboMaxLength, ParameterType.Projectile);
        //CanPerformCombo();
    }

    public void EnableDash()
    {
        SC_PlayerController.instance.canDash = true;
    }
    
    public void DisableDash()
    {
        SC_PlayerController.instance.canDash = false;
    }
    
    /// <summary>
    /// Check if the current combo reach its max length.
    /// Else increment combo, switch the weapon type to current type and add this to a list.
    /// Add parameters of the current combo to a list.
    /// </summary>
    /// <param name="newWeapon">New weapon to add to the current combo list</param>
    public void IncrementCombo(SC_Weapon newWeapon)
    {
        // Reset Combo after reach its max length.
        if (comboCounter+1 > comboMaxLength)
        {
            ResetCombo();
            
            //Reset save of last Incrementation of the combo
            lastComboWeapons.Clear();
            _finalBuilder.Reset();
        }
        
        // Increment combo, switch the weapon type to current type and add this to a list.
        comboCounter++;
        ManageComboVFX(comboCounterVFX, comboCounter);
        currentWeapon = newWeapon;
        
        UpdateAnimator();
        
        currentComboWeapons.Add(currentWeapon);
            
        if (comboCounter == comboMaxLength)
        {
            onLastAttack.RaiseEvent();
        }
        
        // Debug Side
        print("Combo : " + comboCounter + " / Type : " + currentWeapon.weaponName);
        
        ComboUpdated?.Invoke(comboCounter, comboMaxLength, currentWeapon.parameter);
        
    }

    /// <summary>
    /// Reset the current combo and its parameters.
    /// </summary>
    public void ResetCombo()
    {
        comboCounter = 0;
        currentWeapon = null;
        currentComboWeapons.Clear();
        //UpdateAnimator();
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

    public void CheckAllDebuffApplication(Collider e, bool isTriggeringBurn = true)
    {
        
        // Check Poison
        CheckPoisonHit(e);

        // Check Freeze
        CheckFreezeHit(e, comboCounter == comboMaxLength && SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Châtiment Glacial"));
        
        // Check Burn
        var entityDebuff = e.GetComponent<SC_DebuffsBuffsComponent>();
        
        if(!entityDebuff.currentDebuffs.Contains(Enum_Debuff.Burn)) CheckBurnHit(e);
        
        else if (entityDebuff.currentDebuffs.Contains(Enum_Debuff.Burn) && isTriggeringBurn)
        {
            entityDebuff.doTStates.Burn(_debuffsBuffsComponent, entityDebuff);
        }
        
        // Check Bleed
        CheckBleedHit(e);
        
    }

    public void CheckPoisonHit(Collider entity)
    {
        
        if(Random.Range(1, 100) < _stats.currentStats.poisonHitRate)
        {
            entity.GetComponent<SC_DebuffsBuffsComponent>().ApplyDebuff(Enum_Debuff.Poison, GetComponent<SC_DebuffsBuffsComponent>());
        }
        
    }
    
    public void CheckFreezeHit(Collider entity, bool isLastHit = false)
    {
        
        var freezeHitRateBonus = (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_1_Freeze") 
                                     ? float.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_1_Freeze").buffsParentEffect["freezeHitRate"]) : 0)
                                 + (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_3_Freeze") 
                                     ? float.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_3_Freeze").buffsParentEffect["freezeHitRate"]) : 0);

        var baseFreezeHitRate = currentWeapon.id == "hammer" ? currentWeapon.effectValue : 0f;

        var freezeHitRate = isLastHit ? baseFreezeHitRate + 50f + freezeHitRateBonus : baseFreezeHitRate;
            
        // print(freezeHitRate);
            
        if(Random.Range(1, 100) < freezeHitRate)
        {
            entity.GetComponent<SC_DebuffsBuffsComponent>().ApplyDebuff(Enum_Debuff.Freeze, GetComponent<SC_DebuffsBuffsComponent>());
        }
        
    }
    
    public void CheckBurnHit(Collider entity, bool isLastHit = false)
    {
        
        var burnHitRateBonus = 0f;

        var baseBurnHitRate = currentWeapon.id == "chakram" ? currentWeapon.effectValue : 0f;

        var burnHitRate = isLastHit ? baseBurnHitRate + 50f + burnHitRateBonus : baseBurnHitRate;
        
        // print(burnHitRate);
        
        if(Random.Range(1, 100) < burnHitRate)
        {
            entity.GetComponent<SC_DebuffsBuffsComponent>().ApplyDebuff(Enum_Debuff.Burn, GetComponent<SC_DebuffsBuffsComponent>());
        }
        
    }
    
    public void CheckBleedHit(Collider entity, bool isLastHit = false)
    {
        var bleedHitRateBonus = 0f;

        var baseBleedHitRate = currentWeapon.id == "rapier" ? currentWeapon.effectValue : 0f;

        var bleedHitRate = isLastHit ? baseBleedHitRate + 50f + bleedHitRateBonus : baseBleedHitRate;
        
        // print(bleedHitRate);
        
        if(Random.Range(1, 100) < bleedHitRate)
        {
            entity.GetComponent<SC_DebuffsBuffsComponent>().ApplyDebuff(Enum_Debuff.Bleed, GetComponent<SC_DebuffsBuffsComponent>());
        }
        
    }

    public void ManageComboVFX(VisualEffect vfx, int orbNumber)
    {

        if(vfx == null) return;
        
        vfx.SetInt("Orb Number", orbNumber+1);
        vfx.SetFloat("Rotation Speed", orbNumber+1);
        if (orbNumber != 0)
        {
            vfx.SetFloat("Trail Size", 1f / orbNumber);
        }
        vfx.Reinit();
    }

    #endregion

}


