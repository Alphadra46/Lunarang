using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class SC_InputManager : MonoBehaviour
{
    public static SC_InputManager instance;
    public static Action<string> newControllerUsed;
    [HideInInspector] public string lastDeviceUsed;
    private InputMap InputMap;
    [Header("General")]
    [HideInInspector] public InputAction weaponA;
    [HideInInspector] public InputAction weaponB;
    [HideInInspector] public InputAction weaponC;
    [HideInInspector] public InputAction move;
    [HideInInspector] public InputAction dash;
    [HideInInspector] public InputAction pause;
    [HideInInspector] public InputAction inventory;
    [HideInInspector] public InputAction interaction;
    [HideInInspector] public InputAction consumable_Switch;
    [HideInInspector] public InputAction consumable_Use;
    [HideInInspector] public InputAction minimapMode;

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
    [HideInInspector] public InputAction develop;
    [HideInInspector] public InputAction switchToRight;
    [HideInInspector] public InputAction switchToLeft;

    [Header("Debug")]
    [HideInInspector] public InputAction console;

    void Awake()
    {
        if (instance!=null) Destroy(this.gameObject);
        instance = this;
        InputMap = new InputMap();

        InitGeneralInputs();
        EnableGeneralInputs();

        InitUIInputs();
        EnableUIInputs();

        InitDebugInputs();
        EnableDebugInputs();

        AttachToDeviceDetection();
    }

    public void InitGeneralInputs()
    {
        weaponA = InputMap.General.WeaponA;
        weaponB = InputMap.General.WeaponB;
        weaponC = InputMap.General.WeaponC;
        move = InputMap.General.Move;
        dash = InputMap.General.Dash;
        pause = InputMap.General.Pause;
        inventory = InputMap.General.Inventory;
        interaction = InputMap.General.Interaction;
        consumable_Switch = InputMap.General.Consumable_Switch;
        consumable_Use = InputMap.General.Consumable_Use;
        minimapMode = InputMap.General.MinimapMode;
    }

    public void InitUIInputs()
    {
        navigate = InputMap.UI.Navigate;
        submit = InputMap.UI.Submit;
        cancel = InputMap.UI.Cancel;
        point = InputMap.UI.Point;
        click = InputMap.UI.Click;
        scrollWheel = InputMap.UI.ScrollWheel;
        middleClick = InputMap.UI.MiddleClick;
        rightClick = InputMap.UI.RightClick;
        trackedDevicePosition = InputMap.UI.TrackedDevicePosition;
        trackedDeviceOrientation = InputMap.UI.TrackedDeviceOrientation;
        develop = InputMap.UI.Develop;
        switchToRight = InputMap.UI.SwitchToRight;
        switchToLeft = InputMap.UI.SwitchToLeft;
    }

    public void InitDebugInputs()
    {
        console = InputMap.Debug.Console;
    }

    public void EnableGeneralInputs()
    {
        weaponA.Enable();
        weaponB.Enable();
        weaponC.Enable();
        move.Enable();
        dash.Enable();
        pause.Enable();
        inventory.Enable();
        interaction.Enable();
        consumable_Switch.Enable();
        consumable_Use.Enable();
        minimapMode.Enable();
    }
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
        develop.Enable();
        switchToRight.Enable();
        switchToLeft.Enable();
    }
    public void EnableDebugInputs()
    {
        console.Enable();
    }

    public void DisableGeneralInputs()
    {
        weaponA.Disable();
        weaponB.Disable();
        weaponC.Disable();
        move.Disable();
        dash.Disable();
        pause.Disable();
        inventory.Disable();
        interaction.Disable();
        consumable_Switch.Disable();
        consumable_Use.Disable();
        minimapMode.Disable();
    }
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
        develop.Disable();
        switchToRight.Disable();
        switchToLeft.Disable();
    }
    public void DisableDebugInputs()
    {
        console.Disable();
    }

    private void AttachToDeviceDetection()
    {
        weaponA.started += DetectDevice;
        weaponB.started += DetectDevice;
        weaponC.started += DetectDevice;
        move.started += DetectDevice;
        dash.started += DetectDevice;
        pause.started += DetectDevice;
        inventory.started += DetectDevice;
        interaction.started += DetectDevice;
        consumable_Switch.started += DetectDevice;
        consumable_Use.started += DetectDevice;
        minimapMode.started += DetectDevice;
        navigate.started += DetectDevice;
        submit.started += DetectDevice;
        cancel.started += DetectDevice;
        point.started += DetectDevice;
        click.started += DetectDevice;
        scrollWheel.started += DetectDevice;
        middleClick.started += DetectDevice;
        rightClick.started += DetectDevice;
        trackedDevicePosition.started += DetectDevice;
        trackedDeviceOrientation.started += DetectDevice;
        develop.started += DetectDevice;
        switchToRight.started += DetectDevice;
        switchToLeft.started += DetectDevice;
        console.started += DetectDevice;
    }

    private void DetectDevice(InputAction.CallbackContext context)
    {
        lastDeviceUsed = context.action.activeControl.device.name;
        newControllerUsed?.Invoke(lastDeviceUsed);
    }
}
