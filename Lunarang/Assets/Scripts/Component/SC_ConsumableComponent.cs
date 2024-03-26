using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_ConsumableComponent : MonoBehaviour
{

    public int selectedConsumableIndex;

    public void Start()
    {
        SC_InputManager.instance.consumable_switch.started += SwitchSelectedConsumable;
        SC_InputManager.instance.consumable_use.started += ConsumeConsumable;
    }

    private void SwitchSelectedConsumable(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<float>();

        if (selectedConsumableIndex + value > SC_GameManager.instance.playerConsumablesInventory.consumablesInventory.Count-1 ||
            selectedConsumableIndex + value < 0) return;
        
        selectedConsumableIndex += (int) value;
        print(SC_GameManager.instance.playerConsumablesInventory.consumablesInventory[selectedConsumableIndex]);

    }

    private void ConsumeConsumable(InputAction.CallbackContext ctx)
    {

        var consumable =
            SC_GameManager.instance.playerConsumablesInventory.consumablesInventory[selectedConsumableIndex];

        consumable.numberOfUses--;

        if(consumable.durationType == DurationType.timeInSeconds)
        {
            foreach (var statModification in consumable.dishesEffects)
            {
                statModification.timer = consumable.duration;
                var buffStatTemp = SC_PlayerStats.instance.debuffsBuffsComponent.BuffStatTemp(statModification);
                StartCoroutine(buffStatTemp);
            }
        }
        else
        {
            
            
            
        }
        
        
        if (consumable.numberOfUses <= 0)
        {
            
            SC_GameManager.instance.playerConsumablesInventory.RemoveConsumable(consumable);
            
        }

    }
    
}
