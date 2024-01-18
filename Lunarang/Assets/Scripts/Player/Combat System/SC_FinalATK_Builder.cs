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

    public int hits;
    public int projectilesNumbers;

    public Collider[] ennemiesInAoE;
    public Collider[] ennemiesHitByProjectile;
    public LayerMask layerAttackable;

    public GameObject ExampleMH;
    public GameObject ExampleP;
    public GameObject ExampleAoE;
    
    public void GetInfosFromLastAttacks(List<SC_Weapon> weapons)
    {
        
        comboWeapons = weapons;
        
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

        hits = weapons[^1].hits;
        projectilesNumbers = weapons[^1].projectilesNumbers;

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
                    hits += (currentLevel * currentStrength);
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
        var pos = new Vector3(transform.position.x, 0.4f, transform.position.z);
        var parametersWithoutLastString = String.Join("", paramatersWithoutLast);

        switch (parametersWithoutLastString)
        {
            case "MM":

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
    
    
    
}
