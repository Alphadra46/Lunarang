using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_AIAnimatorLinker : MonoBehaviour
{

    [ReadOnly] public SC_AIStats stats;

    private void Awake()
    {
        if (!transform.parent.TryGetComponent(out stats)) return;
    }

    public void CreateHitbox(SO_HitBox hb)
    {
        
        stats.CreateHitBox(hb);
        
    }

    public void CreateProjectile(GameObject projectile = null)
    {
        stats.CreateProjectile(projectile);
    }
    
}
