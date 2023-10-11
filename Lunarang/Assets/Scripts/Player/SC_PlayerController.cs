using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
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
    [Tooltip("Rotation speed of the player.")] public float rotationFactorPerFrame = 1f;
    #endregion


    #region Init

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        
        // Calculate the effective speed
        speedEffective = speedBase * speedMultiplier;
        
        // Assign the moving function to an input
        SC_InputManager.instance.move.started += OnMove;
        SC_InputManager.instance.move.performed += OnMove;
        SC_InputManager.instance.move.canceled += OnMove;
        
    }
    
    /// <summary>
    /// Read the input and set the value in a vector
    /// </summary>
    /// <param name="ctx"></param>
    private void OnMove(InputAction.CallbackContext ctx)
    {
        
        currentMovementInput = ctx.ReadValue<Vector2>(); // Read the input value
        currentMovement.x = currentMovementInput.x; // Set the current movement vector x with the input value
        currentMovement.z = currentMovementInput.y; // Set the current movement vector y with the input value
        isMovementInputPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0; // Set a boolean to check if the player is pressing the input
        
    }
    
    /// <summary>
    /// Convert a Vector 3 to a Isometric Vector 3
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    private static Vector3 IsoVectorConvert(Vector3 vector)
    {
        var rotation = Quaternion.Euler(0, 45f, 0); // Initialize the value to add to the base Vector 3
        var isoMatrix = Matrix4x4.Rotate(rotation); // Convert the previous value to isometric rotation
        var result = isoMatrix.MultiplyPoint3x4(vector); // Add it to the base vector
        return result;
    }
    
    /// <summary>
    /// Rotate the player to the facing direction
    /// </summary>
    private void Rotate()
    {
        
        var positionToLookAt = currentMovement;
        var currentRotation = transform.rotation;
        
        var rotation = Quaternion.Euler(0, 45f, 0);
        var isoMatrix = Matrix4x4.Rotate(rotation);
        var result = isoMatrix.MultiplyPoint3x4(positionToLookAt);

        if (!isMovementInputPressed) return;
        
        
        var targetRotation = Quaternion.LookRotation(result);

        print(transform.forward);
        
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
        
    }
    
    /// <summary>
    /// Rotate and Move the player
    /// </summary>
    private void Update()
    {
        Rotate(); // Rotate the player
        _characterController.Move((IsoVectorConvert(currentMovement) * speedEffective) * Time.deltaTime); // Move the player
    }

    #endregion
    
}
