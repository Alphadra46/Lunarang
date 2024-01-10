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
    [Range(0, 360)] public float areaRadius;
    public float areaSize;
    public float moveValue;
    public float atkSpeed;

    public int hits = 1;
    public int projectilesNumbers = 1;
    
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
                typesStrengths[w.parameter] += w.TypeStrength;
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
        
        Combine();
    }

    public void Combine()
    {
        var lastWeapon = comboWeapons[^1];
        var mainEffect = lastWeapon.parameter;
        var mainEffectLevel = parameters[lastWeapon.parameter];
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

        foreach (var (key, currentLevel) in parameters)
        {
            if(key == mainEffect) return;
            
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

    }

    public void Reset()
    {
        parameters.Clear();
        typesStrengths.Clear();

        hits = 0;
        projectilesNumbers = 0;
        areaSize = 0;
    }
    
}
