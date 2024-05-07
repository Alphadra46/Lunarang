using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class SC_AIAnimatorLinker : MonoBehaviour
{

    [ReadOnly] public SC_AIStats stats;
    [ReadOnly] public SC_AIRenderer renderer;

    private void Awake()
    {
        if (!transform.parent.TryGetComponent(out stats)) return;
        if (!transform.parent.TryGetComponent(out renderer)) return;
    }

    public void CreateHitbox(SO_HitBox hb)
    {
        
        stats.CreateHitBox(hb);
        
    }

    public void CreateProjectile(GameObject projectile = null)
    {
        stats.CreateProjectile(projectile);
    }

    public void Death()
    {
        
        stats.Death();
        
    }
    
    public void PlayVFX(GameObject vfxGO)
    {
        
        if(vfxGO.TryGetComponent(out VisualEffect vfx))
            vfx.Play();
        
    }

    public void HideStatsUI()
    {
        
        renderer.hideStatsUI?.Invoke();
        
    }
    
}
