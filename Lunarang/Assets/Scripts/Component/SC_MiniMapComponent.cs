using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MinimapMode
{
    
    Room,
    Player
    
}
public class SC_MiniMapComponent : MonoBehaviour
{

    public static Action<GameObject> changeRoomTransform;
    
    public Transform playerTransform;
    public Transform roomTransform;

    public MinimapMode currentMode = MinimapMode.Room;

    private void OnEnable()
    {
        changeRoomTransform += ChangeRoomTransform;
        
        if(SC_InputManager.instance == null) return;
        
        SC_InputManager.instance.minimap_mode.started += ChangeMode;
    }

    private void OnDisable()
    {
        changeRoomTransform -= ChangeRoomTransform;
        SC_InputManager.instance.minimap_mode.started -= ChangeMode;
    }

    private void Update()
    {
        transform.position = currentMode switch
        {
            MinimapMode.Player => new Vector3(playerTransform.position.x, transform.position.y,
                playerTransform.position.z),
            _ => transform.position
        };
    }

    public void ChangeMode(InputAction.CallbackContext context)
    {
        
        switch (currentMode)
        {
            case MinimapMode.Room:
                currentMode = MinimapMode.Player;
                break;
            
            case MinimapMode.Player:
                currentMode = MinimapMode.Room;
                UpdatePostionRoomMode();
                break;
        }
        
        
    }

    public void ChangeRoomTransform(GameObject newRoom)
    {

        roomTransform = newRoom.transform;
        
        if(currentMode == MinimapMode.Room)
            UpdatePostionRoomMode();

    }

    public void UpdatePostionRoomMode()
    {
        
        transform.position = new Vector3(roomTransform.position.x, transform.position.y,
            roomTransform.position.z);
        
    }
    
}
