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
    [HideInInspector] public InputAction skillA;
    [HideInInspector] public InputAction skillB;
    [HideInInspector] public InputAction move;
    [HideInInspector] public InputAction dash;

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


    private void Awake()
    {
        instance = this;
        playerInputActions = new InputMap();
        
        InitUIInputs();
        OnEnableUIInputs();

        InitGeneralInputs();
        OnEnableGeneralInputs();

        AttachToDeviceDetection();
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
    private void OnEnableUIInputs()
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
    public void OnDisableUIInputs()
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
    /// Init all fo the Inputs int the In-Combat map from the custom PlayerInputActions
    /// </summary>
    private void InitGeneralInputs()
    {
        weaponA = playerInputActions.General.WeaponA;
        weaponB = playerInputActions.General.WeaponB;
        weaponC = playerInputActions.General.WeaponC;
        skillA = playerInputActions.General.SkillA;
        skillB = playerInputActions.General.SkillB;
        
        move = playerInputActions.General.Move;
        dash = playerInputActions.General.Dash;
    }

    /// <summary>
    /// Used to enable all of the Inputs in the In-Combat map from the custom PlayerInputActions
    /// </summary>
    private void OnEnableGeneralInputs()
    {
        weaponA.Enable();
        weaponB.Enable();
        weaponC.Enable();
        skillA.Enable();
        skillB.Enable();
        
        move.Enable();
        dash.Enable();
    }

    /// <summary>
    /// Used to disable all of the Inputs in the In-Combat map from the custom PlayerInputActions
    /// </summary>
    public void OnDisableGeneralInputs()
    {
        weaponA.Disable();
        weaponB.Disable();
        weaponC.Disable();
        skillA.Disable();
        skillB.Disable();
        
        move.Disable();
        dash.Disable();
    }

    /// <summary>
    /// Attach input to the DetectDevice function
    /// </summary>
    private void AttachToDeviceDetection()
    {
        weaponA.started += DetectDevice;
        weaponB.started += DetectDevice;
        weaponC.started += DetectDevice;
        skillA.started += DetectDevice;
        skillB.started += DetectDevice;

        move.started += DetectDevice;
        dash.started += DetectDevice;
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
