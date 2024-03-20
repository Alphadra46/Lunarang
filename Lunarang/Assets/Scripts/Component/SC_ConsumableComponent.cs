using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_ConsumableComponent : MonoBehaviour
{

    public int selectedConsumableIndex;

    public void Start()
    {
        SC_InputManager.instance.consumable_switch.started += SwitchSelectedConsumable;
    }

    private void SwitchSelectedConsumable(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        if (selectedConsumableIndex + 1 >
            SC_GameManager.instance.playercConsumablesInventory.consumablesInventory.Count &&
            selectedConsumableIndex - 1 < 0) return;
        
        selectedConsumableIndex += (int) value;
        print(selectedConsumableIndex);

    }
    
}
