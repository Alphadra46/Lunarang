using Sirenix.OdinInspector;
using UnityEngine;

public enum HitBoxType
{
    
    Box,
    Sphere,
    Capsule
    
}

[CreateAssetMenu(menuName = "SO/Weapon/Hitbox Template")]
public class SO_HitBox : ScriptableObject
{

    public HitBoxType type;
    
    #region Box

    [ShowIf("type", HitBoxType.Box)]
    public Vector3 center;
    [ShowIf("type", HitBoxType.Box)]
    public Vector3 halfExtents;
    [ShowIf("type", HitBoxType.Box)]
    public Quaternion orientation = new Quaternion(0, 0, 0, 0);

    #endregion

    #region Sphere

    [ShowIf("type", HitBoxType.Sphere)]
    public Vector3 pos;
    [ShowIf("type", HitBoxType.Sphere)]
    public float radiusSphere;

    #endregion
    
    #region Capsule

    [ShowIf("type", HitBoxType.Capsule)]
    public Vector3 point0;
    [ShowIf("type", HitBoxType.Capsule)]
    public Vector3 point1;
    [ShowIf("type", HitBoxType.Capsule)]
    public float radiusCapsule;

    #endregion
    
    public LayerMask layer;
    
    

}