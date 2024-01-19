using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_FinalATK_Builder : MonoBehaviour
{

    public SC_ComboController _comboController;
    public SC_PlayerStats _stats;
    
    public WeaponType type;
    [ShowInInspector] public Dictionary<string, int> parametersLevel = new Dictionary<string, int>();
    
    public string paramatersString = "";
    
    private List<string> paramatersWithoutLast;
        
    private string lastParameter;
    
    [ShowInInspector] public Dictionary<string, int> typesStrengths = new Dictionary<string, int>();
    public ImpactPoint impactPoint;
    // public Transform
    [Range(0, 360)] public float areaRadius;
    public float areaSize;
    public float moveValue;
    public float atkSpeed;

    public int additionnalHits;
    
    public int projectilesNumbers;
    public float projectilesSpeed;

    public Collider[] ennemiesInAoE;
    public Collider[] ennemiesHitByProjectile;
    
    public LayerMask layerAttackable;

    public GameObject ExampleMH;
    public GameObject ExampleP;
    public GameObject ExampleAoE;

    private void Awake()
    {
        if(!TryGetComponent(out _stats)) return;
        if(!TryGetComponent(out _comboController)) return;
    }

    public void GetInfosFromLastAttacks(List<SC_Weapon> weapons, SC_ComboController newComboController)
    {

        _comboController = newComboController;
        
        foreach (var w in weapons)
        {
            var currentParameter = w.parameter switch
            {
                ParameterType.MultiHit => "M",
                ParameterType.AreaOfEffect => "A",
                ParameterType.Projectile => "P",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (paramatersString.Contains(currentParameter))
            {
                parametersLevel[currentParameter] += 1;
            }
            else{
                parametersLevel.Add(currentParameter, 1);
                typesStrengths.Add(currentParameter, w.TypeStrength);
            }

            paramatersString += currentParameter + ";";
            
        }

        impactPoint = weapons[^1].impactPoint;
        areaRadius = weapons[^1].areaRadius;
        areaSize = weapons[^1].areaSize;

        moveValue = weapons[^1].MovesValues[^1];
        atkSpeed = weapons[^1].atkSpeed;

        additionnalHits = weapons[^1].hits;
        projectilesNumbers = weapons[^1].projectilesNumbers;
        projectilesSpeed = weapons[^1].projectileSpeed;

        // Set parameters
        paramatersWithoutLast = paramatersString.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();

        lastParameter = paramatersWithoutLast[^1];
        paramatersWithoutLast.Remove(paramatersWithoutLast[^1]);
        
        Combine();
    }

    private void Combine()
    {

        foreach (var parameter in parametersLevel)
        {

            var currentStrength = typesStrengths[parameter.Key];
            var currentLevel = parametersLevel[parameter.Key];
            
            switch (parameter.Key)
            {
            
                case "M":
                    additionnalHits += (currentLevel * currentStrength);
                    break;
                case "A":
                    areaSize += (currentLevel * currentStrength);
                    break;
                case "P":
                    projectilesNumbers += (currentLevel * currentStrength);
                    print("Added Projectiles");
                    break;
            }

        }
        
        InstantiateCubes();

    }

    private void InstantiateCubes()
    {
        var pos = Vector3.zero;
        var currentWeaponGO = _comboController.equippedWeaponsGO[_comboController.currentWeapon.id];
        var weaponImpactPoint = currentWeaponGO.transform.Find("ImpactPoint");
        
        var isCritical = Random.Range(0, 100) < _stats.critRate ? true : false;
        var currentMV = (_comboController.currentWeapon.MovesValues[_comboController.comboCounter-1]/100);
            
        var rawDamage = MathF.Round(currentMV * _stats.currentATK, MidpointRounding.AwayFromZero);
        var effDamage = rawDamage * (1 + (_stats.damageBonus/100));
        var effCrit = effDamage * (1 + (_stats.critDMG/100));
        
        switch (impactPoint)
        {
            case ImpactPoint.Player:
                pos = new Vector3(transform.position.x, 0.4f, transform.position.z) + (transform.forward * 2);
                break;
            case ImpactPoint.Weapon:
                pos = weaponImpactPoint.position;
                break;
            case ImpactPoint.Hit:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        var parametersWithoutLastString = string.Join("", paramatersWithoutLast);

        switch (parametersWithoutLastString)
        {
            case "MM":

                switch (lastParameter)
                {
                    case "M":

                        foreach (var e in _comboController.currentEnemiesHitted)
                        {
                            
                            for (var i = 0; i < additionnalHits; i++)
                            {
                                var mhSettings = Instantiate(ExampleMH);

                                mhSettings.transform.localScale *= areaSize;
                                mhSettings.transform.position = new Vector3(e.transform.position.x, e.transform.localScale.y, e.transform.position.z);
                            }
                            
                        }
                        
                        break;
                    case "A":
                        
                        var aoeSettings = Instantiate(ExampleAoE);
                        aoeSettings.transform.localScale *= areaSize;
                        aoeSettings.transform.position = pos + (transform.forward);

                        ennemiesInAoE = Physics.OverlapSphere((pos + (transform.forward)), areaSize/2, layerAttackable); //TODO : Replace Pos by Weapon Hit Pos

                        foreach (var e in ennemiesInAoE)
                        {
                            if (!e.TryGetComponent(out IDamageable damageable)) continue;

                            damageable.TakeDamage(isCritical ? effCrit : effDamage, _comboController.currentWeapon.type, isCritical);
                            
                            for (var i = 0; i < additionnalHits; i++)
                            {
                                var mhSettings = Instantiate(ExampleMH);
                                mhSettings.transform.position = e.transform.position;
                                damageable.TakeDamage(isCritical ? effCrit : effDamage, _comboController.currentWeapon.type, isCritical);
                            }

                        }
                        
                        break;
                    
                    case "P":
                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab, 
                            projectilesNumbers,
                            areaSize,
                            additionnalHits+1, 
                            10f, 
                            projectilesSpeed, 
                            0f, 
                            transform.GetChild(1).forward);
                        
                        break;
                }
                
                break;
            
            case "MA":
                
                switch (lastParameter)
                {
                    case "M":
                        
                        break;
                    case "A":
                        
                        break;
                    
                    case "P":
                        
                        break;
                }
                
                break;
            
            case "MP":
                
                switch (lastParameter)
                {
                    case "M":
                        
                        break;
                    case "A":
                        
                        break;
                    
                    case "P":
                        
                        break;
                }
                
                break;
            
            
            case "AA":
                
                switch (lastParameter)
                {
                    case "M":
                        
                        break;
                    case "A":
                        
                        break;
                    
                    case "P":
                        
                        break;
                }
                
                break;
            
            case "AM":
                
                switch (lastParameter)
                {
                    case "M":
                        
                        break;
                    case "A":
                        
                        break;
                    
                    case "P":
                        
                        break;
                }
                
                break;
            
            case "AP":
                
                switch (lastParameter)
                {
                    case "M":
                        
                        break;
                    case "A":
                        
                        break;
                    
                    case "P":
                        
                        break;
                }
                
                break;
            
            
            case "PP":
                
                switch (lastParameter)
                {
                    case "M":
                        
                        break;
                    case "A":
                        
                        break;
                    
                    case "P":
                        
                        break;
                }
                
                break;
            
            case "PA":
                
                switch (lastParameter)
                {
                    case "M":
                        
                        break;
                    case "A":
                        
                        break;
                    
                    case "P":
                        
                        break;
                }
                
                break;
            
            case "PM":
                
                switch (lastParameter)
                {
                    case "M":
                        
                        break;
                    case "A":
                        
                        break;
                    
                    case "P":
                        
                        break;
                }
                
                break;
        }
        
    }

    public void Reset()
    {
        parametersLevel.Clear();
        typesStrengths.Clear();
        paramatersWithoutLast.Clear();
        paramatersString = "";
        lastParameter = "";
    }

    private void OnDrawGizmos()
    {
    }
}
