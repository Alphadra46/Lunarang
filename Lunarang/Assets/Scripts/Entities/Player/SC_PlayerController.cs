using System;
using System.Collections;
using System.Numerics;
using Sirenix.OdinInspector;
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

    public static SC_PlayerController instance;
    
    private CharacterController _characterController;
    public Animator _animator;

    #region Movements

    [TabGroup("Tabs","Movement")]
    [Tooltip("Current read value from Movement input action")] public Vector2 currentMovementInput;
    
    [TabGroup("Tabs","Movement")]
    [PropertySpace(SpaceAfter = 5)]
    [Tooltip("Current movement value")] public Vector3 currentMovement;
    
    [TabGroup("Tabs","Movement")]
    [Tooltip("Is pressing movement input ?")] public bool isMovementInputPressed;
    
    [TabGroup("Tabs","Movement")]
    public bool canMove = true;

    #endregion

    #region Dash

    [TabGroup("Tabs","Dash")]
    [PropertySpace(SpaceAfter = 5)]
    [Tooltip("Rotation speed of the player.")] public float rotationFactorPerFrame = 1f;
    
    [TabGroup("Tabs","Dash")]
    public bool isDashing;
    
    [TabGroup("Tabs","Dash")]
    [PropertySpace(SpaceAfter = 5)]
    public bool canDash = true;

    [TabGroup("Tabs","Dash")]
    [Tooltip("How long the dash will stay active"), SerializeField] private float dashTime = 0.25f;
    
    [TabGroup("Tabs","Dash")]
    [PropertySpace(SpaceAfter = 5)]
    [Tooltip("The speed of the Dash"), SerializeField] private float dashSpeed = 20f;

    #endregion

    #region Gravity

    [TabGroup("Tabs","Gravity")]
    [Tooltip("Current gravity who impact of the player"), SerializeField] private float gravity = -9.81f;
    
    [TabGroup("Tabs","Gravity")]
    [Tooltip("Current gravity multiplier who impact of the player"), SerializeField] private float gravityMultiplier = 3f;

    #endregion

    public bool isAttacking;
    
    #endregion

    #region Init

    /// <summary>
    /// Init the instance.
    /// Get the character controller
    /// Get the animator
    /// </summary>
    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
        
        _characterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Assign functions to an input
    /// </summary>
    private void Start()
    {
        InitControllerInputs();
    }
    
    
    #endregion
    
    #region Functions

    public void InitControllerInputs()
    {
        //SC_InputManager.instance.move.started += OnMove;
        //SC_InputManager.instance.move.performed += OnMove;
        //SC_InputManager.instance.move.canceled += OnMove;

        SC_InputManager.instance.dash.started += Dash;

        SC_InputManager.instance.inventory.started += _ => SC_GameManager.instance.OpenInventory();
    }

    public void RemoveControllerInputs()
    {
        SC_InputManager.instance.move.started -= OnMove;
        SC_InputManager.instance.move.performed -= OnMove;
        SC_InputManager.instance.move.canceled -= OnMove;

        SC_InputManager.instance.dash.started -= Dash;
    }
    
    /// <summary>
    /// Launch the dash coroutine and change the bool value "isDashing"
    /// </summary>
    private void Dash(InputAction.CallbackContext context)
    {
        if (SC_GameManager.instance.isPause) return;
        
        if(!canDash)
            return;
        if (isDashing)
            return;

        
        if(_animator != null)
            _animator.SetBool("isDashing", true);
        
        //canDash = false;
        isDashing = true;
        
        if (isAttacking)
        {
            SC_ComboController.instance.CancelAttack();
            print("Cancel");
        }
        
        if(this != null)
            StartCoroutine(DashCoroutine());
        
    }

    /// <summary>
    /// Move the player at a high speed during a certain duration.
    /// Reset the bool "isDashing"
    /// </summary>
    IEnumerator DashCoroutine()
    {
        float startTime = Time.time;

        while (Time.time < startTime+dashTime)
        {
            _characterController.Move(transform.forward * (dashSpeed * Time.deltaTime));
            yield return null;
        }
        
        
        _animator.SetBool("isDashing", false);
    }
    
    /// <summary>
    /// Convert a Vector 3 to a Isometric Vector 3
    /// </summary>
    /// <param name="vector">Vector to convert</param>
    /// <returns>Vector converted for Isometric</returns>
    public static Vector3 IsoVectorConvert(Vector3 vector)
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
        var positionToLookAt = new Vector3(currentMovement.x, 0, currentMovement.z);
        var currentRotation = transform.rotation;
        
        var rotation = Quaternion.Euler(0, 45f, 0);
        var isoMatrix = Matrix4x4.Rotate(rotation);
        var result = isoMatrix.MultiplyPoint3x4(positionToLookAt);
        

        if (!isMovementInputPressed) return;
        
        
        var targetRotation = Quaternion.LookRotation(result);

        // print(transform.forward);
        
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
        
    }
    
    /// <summary>
    /// Apply gravity to the controller
    /// </summary>
    private void Gravity()
    {
        float velocity;
        
        if (_characterController.isGrounded)
        {
            velocity = -1f;
        }
        else
        {
            velocity = gravity * gravityMultiplier * Time.deltaTime;
        }
        currentMovement.y = velocity;
    }
    
    /// <summary>
    /// Rotate and Move the player
    /// </summary>
    private void Update()
    {
        if(SC_GameManager.instance.isPause) return;
        
        Move();
        isMovementInputPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0; // Set a boolean to check if the player is pressing the input

        _animator.SetBool("isMoving", isMovementInputPressed);
        _animator.SetBool("canMove", canMove);
        _animator.SetBool("canDash", canDash);

        if (!canMove) return;
            
        Gravity();

        
        Rotate(); // Rotate the player
        
        if(isMovementInputPressed && !isAttacking)
            _characterController.Move((IsoVectorConvert(currentMovement) * SC_PlayerStats.instance.currentStats.currentSpeed) * Time.deltaTime); // Move the player
    }

    public void FreezeMovement(bool value)
    {

        canMove = !value;
        currentMovementInput = value ? Vector2.zero : SC_InputManager.instance.move.ReadValue<Vector2>();
        currentMovement = value ? Vector3.zero : new Vector3(currentMovementInput.x, 0, currentMovementInput.y);

        if (value) return;
        
        // print(currentMovementInput);

    }

    public void FreezeDash(bool value)
    {
        canDash = !value;
    }

    public void TakeKnockback()
    {
        // TODO
        print("KB");
    }

    public void Teleport(Vector3 loc)
    {
        canMove = false;
        currentMovementInput = Vector2.zero;
        currentMovement = Vector3.zero;
        
        transform.position = loc;
        canMove = true;
    }

    #endregion

    #region Events

    /// <summary>
    /// Read the input and set the value in a vector
    /// </summary>
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if(!canMove) return;
        
        isAttacking = false;
        
        currentMovementInput = ctx.ReadValue<Vector2>(); // Read the input value
        currentMovement.x = currentMovementInput.x; // Set the current movement vector x with the input value
        currentMovement.z = currentMovementInput.y; // Set the current movement vector y with the input value

        if (SC_ComboController.instance.canAttack) return;
        
        SC_ComboController.instance.CanPerformCombo();

    }

    public void Move()
    {
        if (!canMove)
            return;

        isAttacking = false;
        
        currentMovementInput = SC_InputManager.instance.move.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        
        if (SC_ComboController.instance.canAttack) return;
        
        SC_ComboController.instance.CanPerformCombo();
    }
    
    #endregion
    
}
