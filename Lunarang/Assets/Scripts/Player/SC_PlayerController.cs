using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SC_PlayerController : MonoBehaviour
{
    #region Variables

    private CharacterController _characterController;
    private Animator _animator;
    
    [Header("Debug Info")] 
    [Tooltip("Current read value from Movement input action")] public Vector2 currentMovementInput;
    [Tooltip("Current movement value")] public Vector3 currentMovement;
    [Tooltip("Is pressing movement input ?")] public bool isMovementInputPressed;
    
    [Header("Movement Settings")]
    [Tooltip("Current speed base value of the player.")] public float speedBase = 2;
    [Tooltip("Current speed multiplier of the player.")] public float speedMultiplier = 1;
    [Tooltip("Current real speed of the player.")] private float speedEffective;
    [Tooltip("Rotation speed of the player.")] public float turnSpeed = 360;
    #endregion


    #region Init

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        speedEffective = speedBase * speedMultiplier;

        SC_InputManager.instance.move.started += OnMove;
        SC_InputManager.instance.move.performed += OnMove;
        SC_InputManager.instance.move.canceled += OnMove;
        
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementInputPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        
    }

    private static Vector3 IsoVectorConvert(Vector3 vector)
    {
        var rotation = Quaternion.Euler(0, 45f, 0);
        var isoMatrix = Matrix4x4.Rotate(rotation);
        var result = isoMatrix.MultiplyPoint3x4(vector);
        return result;
    }

    private void Update()
    {
        _characterController.Move((IsoVectorConvert(currentMovement) * speedEffective) * Time.deltaTime);
    }

    #endregion
    
}
