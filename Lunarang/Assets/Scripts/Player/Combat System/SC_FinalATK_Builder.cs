using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SC_FinalATK_Builder : MonoBehaviour
{

    public List<SC_Weapon> comboWeapons = new List<SC_Weapon>();
    
    public WeaponType type;
    [ShowInInspector] public Dictionary<ParameterType, int> parameters = new Dictionary<ParameterType, int>();
    [ShowInInspector] public Dictionary<ParameterType, int> typesStrengths = new Dictionary<ParameterType, int>();
    public ImpactPoint impactPoint;
    // public Transform
    [Range(0, 360)] public float areaRadius;
    public float areaSize;
    public float moveValue;
    public float atkSpeed;

    public int hits;
    public int projectilesNumbers;

    public Collider[] ennemiesInAoE;
    public LayerMask layerAttackable;

    public GameObject ExampleMH;
    public GameObject ExampleP;
    public GameObject ExampleAoE;
    
    public void GetInfosFromLastAttacks(List<SC_Weapon> weapons)
    {
        
        comboWeapons = weapons;
        
        foreach (var w in weapons)
        {
            var index = weapons.IndexOf(w);
            print(index);
            
            if (parameters.ContainsKey(w.parameter))
            {
                parameters[w.parameter] += 1;
                // typesStrengths[w.parameter] += w.TypeStrength;
            }else{
                parameters.Add(w.parameter, 1);
                typesStrengths.Add(w.parameter, w.TypeStrength);
            }
        }

        impactPoint = weapons[^1].impactPoint;
        areaRadius = weapons[^1].areaRadius;
        areaSize = weapons[^1].areaSize;

        moveValue = weapons[^1].MovesValues[^1];
        atkSpeed = weapons[^1].atkSpeed;

        hits = weapons[^1].hits;
        projectilesNumbers = weapons[^1].projectilesNumbers;
        
        Combine();
    }

    private void Combine()
    {
        var tempParameters = parameters.ToDictionary(
            entry => entry.Key, entry => entry.Value);

        var lastWeapon = comboWeapons[^1];
        var mainEffect = lastWeapon.parameter;
        var mainEffectLevel = tempParameters[lastWeapon.parameter];
        var mainEffectStrength = typesStrengths[lastWeapon.parameter];

        switch (mainEffect)
        {
            
            case ParameterType.MultiHit:
                hits += (mainEffectLevel * mainEffectStrength);
                break;
            case ParameterType.AreaOfEffect:
                areaSize += (mainEffectLevel * mainEffectStrength);
                break;
            case ParameterType.Projectile:
                projectilesNumbers += (mainEffectLevel * mainEffectStrength);
                break;
            
        }

        tempParameters.Remove(mainEffect);

        foreach (var (key, currentLevel) in tempParameters)
        {

            var currentStrength = typesStrengths[key];
            print(key + " : " + currentLevel + " / " + currentStrength);
            
            switch (key)
            {
            
                case ParameterType.MultiHit:
                    hits += (currentLevel * currentStrength);
                    break;
                case ParameterType.AreaOfEffect:
                    areaSize += (currentLevel * currentStrength);
                    break;
                case ParameterType.Projectile:
                    projectilesNumbers += (currentLevel * currentStrength);
                    print("Added Projectiles");
                    break;
            }

        }
        
        InstantiateCubes();

    }

    private void InstantiateCubes()
    {
        var pos = new Vector3(transform.position.x, 0.4f, transform.position.z);
        ParameterType? previousEffect = null;
        
        foreach (var effect in parameters)
        {

            print(effect.Key);
            switch (effect.Key)
            {
                case ParameterType.MultiHit:
                    switch (previousEffect)
                    {
                        case ParameterType.MultiHit:
                            break;
                        case ParameterType.AreaOfEffect:
                            impactPoint = ImpactPoint.Hit;

                            foreach (var e in ennemiesInAoE)
                            {
                                var mhSettings = Instantiate(ExampleMH);
                        
                                mhSettings.transform.position = e.transform.position;
                            }
                            
                            break;
                        case ParameterType.Projectile:
                            break;
                        default:
                            for (var h = 0; h < hits; h++)
                            {
                                var mhSettings = Instantiate(ExampleMH);
                        
                                mhSettings.transform.position = pos + (transform.forward * 2);
                            }
                            break;
                    }
                    break;
                case ParameterType.AreaOfEffect:
                    var aoeSettings = Instantiate(ExampleAoE);
                    aoeSettings.transform.localScale *= areaSize;
                    aoeSettings.transform.position = pos + (transform.forward * 2);

                    ennemiesInAoE = Physics.OverlapSphere((pos + (transform.forward * 2)), areaSize/2, layerAttackable);
                    
                    break;
                case ParameterType.Projectile:
                    for (var p = 0; p < projectilesNumbers; p++)
                    {
                        var projectile = Instantiate(ExampleP);
                        var angle = Mathf.PI * (p+1) / (projectilesNumbers+1);
                        print(angle);
                
                        var x = Mathf.Sin(angle) * 2;
                        var z = Mathf.Cos(angle) * 2;
                        var posProj = new Vector3(x, 0.4f, z);
            
                        var centerDirection = Quaternion.LookRotation(-transform.right, transform.up);
            
                        posProj = centerDirection * posProj;
            
                        projectile.transform.position = transform.position + posProj;
            
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            previousEffect = effect.Key;
        }
    }

    public void Reset()
    {
        parameters.Clear();
        typesStrengths.Clear();
    }
    
}
