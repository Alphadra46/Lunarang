using UnityEngine;
using UnityEngine.InputSystem;

public class SC_InputManager : MonoBehaviour
{
    public static SC_InputManager instance;

    private InputMap playerInputActions;

    [HideInInspector] public string lastDeviceUsed;
        
    [Header("General")]
    [HideInInspector] public InputAction weaponA;
    [HideInInspector] public InputAction weaponB;
    [HideInInspector] public InputAction weaponC;
    [HideInInspector] public InputAction move;
    [HideInInspector] public InputAction dash;
    [HideInInspector] public InputAction interaction;
    [HideInInspector] public InputAction pause;
    [HideInInspector] public InputAction inventory;
    
    [Header("UI")]
    [HideInInspector] public InputAction navigate;
    [HideInInspector] public InputAction submit;
    [HideInInspector] public InputAction cancel;
    [HideInInspector] public InputAction point;
    [HideInInspector] public InputAction click;
    [HideInInspector] public InputAction scrollWheel;
    [HideInInspector] public InputAction middleClick;
    [HideInInspector] public InputAction rightClick;
    [HideInInspector] public InputAction trackedDevicePosition;
    [HideInInspector] public InputAction trackedDeviceOrientation;

    [Header("Debug")]
    [HideInInspector] public InputAction console;


    private void Awake()
    {
        if(instance != null) Destroy(this.gameObject);
        instance = this;
        playerInputActions = new InputMap();
        
        InitUIInputs();
        EnableUIInputs();

        InitGeneralInputs();
        EnableGeneralInputs();
        
        InitDebugInputs();
        EnableDebugInputs();

        AttachToDeviceDetection();
        
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Init all fo the Inputs int the UI map from the custom PlayerInputActions
    /// </summary>
    private void InitUIInputs()
    {
        navigate = playerInputActions.UI.Navigate;
        submit = playerInputActions.UI.Submit;
        cancel = playerInputActions.UI.Cancel;
        point = playerInputActions.UI.Point;
        click = playerInputActions.UI.Click;
        scrollWheel = playerInputActions.UI.ScrollWheel;
        middleClick = playerInputActions.UI.MiddleClick;
        rightClick = playerInputActions.UI.RightClick;
        trackedDevicePosition = playerInputActions.UI.TrackedDevicePosition;
        trackedDeviceOrientation = playerInputActions.UI.TrackedDevicePosition;
    }
    
    /// <summary>
    /// Used to enable all of the Inputs in the UI map from the custom PlayerInputActions
    /// </summary>
    public void EnableUIInputs()
    {
        navigate.Enable();
        submit.Enable();
        cancel.Enable();
        point.Enable();
        click.Enable();
        scrollWheel.Enable();
        middleClick.Enable();
        rightClick.Enable();
        trackedDevicePosition.Enable();
        trackedDeviceOrientation.Enable();
    }
    
    /// <summary>
    /// Used to disable all of the Inputs in the UI map from the custom PlayerInputActions
    /// </summary>
    public void DisableUIInputs()
    {
        navigate.Disable();
        submit.Disable();
        cancel.Disable();
        point.Disable();
        click.Disable();
        scrollWheel.Disable();
        middleClick.Disable();
        rightClick.Disable();
        trackedDevicePosition.Disable();
        trackedDeviceOrientation.Disable();
    }


    /// <summary>
    /// Init all fo the Inputs int the General map from the custom PlayerInputActions
    /// </summary>
    private void InitGeneralInputs()
    {
        weaponA = playerInputActions.General.WeaponA;
        weaponB = playerInputActions.General.WeaponB;
        weaponC = playerInputActions.General.WeaponC;

        move = playerInputActions.General.Move;
        dash = playerInputActions.General.Dash;

        interaction = playerInputActions.General.Interaction;

        pause = playerInputActions.General.Pause;
        inventory = playerInputActions.General.Inventory;
    }

    /// <summary>
    /// Used to enable all of the Inputs in the General map from the custom PlayerInputActions
    /// </summary>
    public void EnableGeneralInputs()
    {
        weaponA.Enable();
        weaponB.Enable();
        weaponC.Enable();

        move.Enable();
        dash.Enable();
        
        interaction.Enable();
        
        pause.Enable();
        inventory.Enable();
    }

    /// <summary>
    /// Used to disable all of the Inputs in the General map from the custom PlayerInputActions
    /// </summary>
    public void DisableGeneralInputs()
    {
        weaponA.Disable();
        weaponB.Disable();
        weaponC.Disable();

        move.Disable();
        dash.Disable();
        
        interaction.Disable();
        
        pause.Disable();
        inventory.Disable();
    }
    
    /// <summary>
    /// Init all fo the Inputs int the Debug map from the custom PlayerInputActions
    /// </summary>
    private void InitDebugInputs()
    {
        console = playerInputActions.Debug.Console;
    }
    
    /// <summary>
    /// Used to enable all of the Inputs in the Debug map from the custom PlayerInputActions
    /// </summary>
    public void EnableDebugInputs()
    {
        console.Enable();
    }

    /// <summary>
    /// Used to disable all of the Inputs in the Debug map from the custom PlayerInputActions
    /// </summary>
    public void DisableDebugInputs()
    {
        console.Disable();
    }

    /// <summary>
    /// Attach input to the DetectDevice function
    /// </summary>
    private void AttachToDeviceDetection()
    {
        weaponA.started += DetectDevice;
        weaponB.started += DetectDevice;
        weaponC.started += DetectDevice;

        move.started += DetectDevice;
        dash.started += DetectDevice;

        interaction.started += DetectDevice;

        pause.started += DetectDevice;
        inventory.started += DetectDevice;
    }
    
    /// <summary>
    /// Detect which device is the input used from
    /// </summary>
    private void DetectDevice(InputAction.CallbackContext context)
    {
        // Debug.Log(context.action.activeControl.device.name);
        // print(context.action.name);

        lastDeviceUsed = context.action.activeControl.device.name;
    }
}
