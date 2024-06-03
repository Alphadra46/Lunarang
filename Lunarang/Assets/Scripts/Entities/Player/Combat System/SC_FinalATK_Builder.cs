using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.VFX;

public class SC_FinalATK_Builder : MonoBehaviour
{

    #region Variables

    [SerializeField] private VisualEffect hammerFinalVFX;
    public SC_ComboController _comboController;
    public SC_PlayerStats _stats;
    
    public WeaponType type;
    [ShowInInspector] public Dictionary<string, int> parametersLevel = new Dictionary<string, int>();
    
    public string paramatersString = "";
    
    private List<string> paramatersWithoutLast = new List<string>();
        
    private string lastParameter;
    
    [ShowInInspector] public Dictionary<string, float> typesStrengths = new Dictionary<string, float>();
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

    #endregion

    private void Awake()
    {
        if(!TryGetComponent(out _stats)) return;
        if(!TryGetComponent(out _comboController)) return;
    }

    /// <summary>
    /// Get all informations from last attacks.
    /// Create a String from the types parameters.
    /// </summary>
    /// <param name="weapons">All weapons used in the last attacks.</param>
    /// <param name="newComboController">Get the player combo controller.</param>
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

        moveValue = ((weapons[^1].baseMovesValues[^1] + (weapons[^1].levelUpStatsRate * weapons[^1].currentLevel-1)) / 100);
        atkSpeed = weapons[^1].atkSpeed;

        additionnalHits = weapons[^1].hits;
        projectilesNumbers = weapons[^1].projectilesNumbers;

        // Set parameters
        paramatersWithoutLast = paramatersString.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();

        lastParameter = paramatersWithoutLast[^1];
        paramatersWithoutLast.Remove(paramatersWithoutLast[^1]);
        
        Combine();
    }
    
    /// <summary>
    /// Look at each of the previous attack types and modify the final attack parameters according to them.
    /// </summary>
    private void Combine()
    {

        foreach (var parameter in parametersLevel)
        {

            var currentStrength = typesStrengths[parameter.Key];
            var currentLevel = parametersLevel[parameter.Key];
            
            switch (parameter.Key)
            {
            
                case "M":
                    additionnalHits += (int) MathF.Ceiling(currentLevel * currentStrength);
                    break;
                case "A":
                    areaSize += (currentLevel * currentStrength);
                    break;
                case "P":
                    projectilesNumbers *= (int) MathF.Ceiling(currentLevel * currentStrength);
                    break;
            }

        }
        
        Result();

    }

    /// <summary>
    /// Create the Result of the combinaison.
    /// </summary>
    private void Result()
    {
        var pos = Vector3.zero;
        var currentWeaponGO = _comboController.equippedWeaponsGO[_comboController.currentWeapon.id]; //_comboController.equippedWeaponsGO[_comboController.currentWeapon.id];
        var weaponImpactPoint = currentWeaponGO.transform.Find("ImpactPoint");

        var currentMV = ((_comboController.currentWeapon.baseMovesValues[_comboController.comboCounter - 1] + (_comboController.currentWeapon.levelUpStatsRate * _comboController.currentWeapon.currentLevel-1)) / 100);

        var rawDamage = MathF.Round(currentMV * _stats.currentStats.currentATK, MidpointRounding.AwayFromZero);

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

                        _comboController.Multihit(additionnalHits);

                        break;

                    case "A":

                        _comboController.CreateAoE(pos, areaSize, currentMV,false,true, additionnalHits);
                        // vfx marteau play
                        PlayFinalVFX(hammerFinalVFX, weaponImpactPoint);
                       
                        
                        break;

                    case "P":
                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            additionnalHits + 1,
                            50f,
                            0f,
                            transform.GetChild(1).forward);

                        break;
                }

                break;

            case "MA" or "AM":

                switch (lastParameter)
                {
                    case "M":
                        
                        _comboController.Multihit(additionnalHits);
                        foreach (var e in _comboController.currentEnemiesHitted)
                        {
                            _comboController.CreateAoE(new Vector3(e.transform.position.x, e.transform.localScale.y, e.transform.position.z), areaSize, currentMV);
                        }
                        
                        break;
                    case "A":
                        
                        _comboController.CreateAoE(pos, areaSize,currentMV,true,true, additionnalHits);
                        PlayFinalVFX(hammerFinalVFX, weaponImpactPoint);

                        break;

                    case "P":
                        print(projectilesNumbers);
                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            additionnalHits + 1,
                            50f,
                            0f,
                            transform.GetChild(1).forward,
                            true);
                        
                        break;
                }

                break;

            case "MP" or "PM":

                switch (lastParameter)
                {
                    
                    case "M":
                        _comboController.Multihit(additionnalHits);
                        
                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            1,
                            50f,
                            0f,
                            transform.GetChild(1).forward);
                        break;
                    
                    case "A":
                        _comboController.CreateAoE(pos, areaSize, currentMV, false,true, additionnalHits);
                        
                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            1,
                            50f,
                            0f,
                            transform.GetChild(1).forward,
                            false);
                        PlayFinalVFX(hammerFinalVFX, weaponImpactPoint);

                        break;

                    case "P":

                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            additionnalHits + 1,
                            50f,
                            0f,
                            transform.GetChild(1).forward,
                            false);
                        
                        break;
                    
                }

                break;

            case "AA":

                switch (lastParameter)
                {
                    case "M":
                        
                        _comboController.Multihit(additionnalHits);
                        foreach (var e in _comboController.currentEnemiesHitted)
                        {
                            _comboController.CreateAoE(pos, areaSize, currentMV);
                        }

                        break;
                    case "A":
                        
                        _comboController.CreateAoE(pos, areaSize, currentMV);
                        PlayFinalVFX(hammerFinalVFX, weaponImpactPoint);

                        break;

                    case "P":

                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                             1,
                            50f,
                            0f,
                            transform.GetChild(1).forward,
                            true);
                        
                        break;
                }

                break;

            case "AP" or "PA":

                switch (lastParameter)
                {
                    
                    case "M":
                        _comboController.Multihit(additionnalHits);
                        
                        foreach (var e in _comboController.currentEnemiesHitted)
                        {
                            _comboController.CreateAoE(new Vector3(e.transform.position.x, e.transform.localScale.y, e.transform.position.z), areaSize, currentMV);
                            _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                                projectilesNumbers,
                                areaSize,
                                1,
                                50f,
                                0f,
                                transform.GetChild(1).forward,
                                false);
                        }
                        
                        break;
                    
                    case "A":
                        
                        _comboController.CreateAoE(pos, areaSize, currentMV);
                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            1,
                            50f,
                            0f,
                            transform.GetChild(1).forward,
                            false);
                        PlayFinalVFX(hammerFinalVFX, weaponImpactPoint);

                        break;

                    case "P":

                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            1,
                            50f,
                            0f,
                            transform.GetChild(1).forward,
                            true);
                        
                        break;
                    
                }

                break;

            case "PP":

                switch (lastParameter)
                {
                    case "M":

                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            additionnalHits + 1,
                            50f,
                            0f,
                            transform.GetChild(1).forward,
                            false);
                        
                        break;
                    
                    case "A":
                        
                        _comboController.CreateAoE(pos, areaSize, currentMV);
                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            additionnalHits + 1,
                            50f,
                            0f,
                            transform.GetChild(1).forward,
                            false);
                        PlayFinalVFX(hammerFinalVFX, weaponImpactPoint);

                        break;

                    case "P":

                        _comboController.CreateProjectile(_comboController.currentWeapon.projectilePrefab,
                            projectilesNumbers,
                            areaSize,
                            1,
                            50f,
                            0f,
                            transform.GetChild(1).forward,
                            false);
                        
                        break;
                }

                break;

        }
    }

    /// <summary>
    /// Reset all paramaters.
    /// </summary>
    public void Reset()
    {
        parametersLevel.Clear();
        typesStrengths.Clear();
        paramatersWithoutLast.Clear();
        paramatersString = "";
        lastParameter = "";
    }

    public void PlayFinalVFX(VisualEffect finalFX, Transform weaponImpactPoint)
    {
        switch (impactPoint)
        {
            case ImpactPoint.Player:
                finalFX.transform.position = transform.position;
                break;
            case ImpactPoint.Weapon:
                finalFX.transform.position = weaponImpactPoint.position;
                print(weaponImpactPoint);
                break;
        }

        finalFX.SetFloat("Scale", areaSize);
        finalFX.Play();
    }
}
