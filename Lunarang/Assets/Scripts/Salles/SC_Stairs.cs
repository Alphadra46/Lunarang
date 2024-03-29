using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Stairs : MonoBehaviour
{
    public SC_Stairs stairToConnect;
    public Transform stairTPPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        //SC_GameManager.instance.ChangeState(GameState.WIN); //Debug only to test the win feature

        var thisFloorIndex = SC_PathCreator.instance.actualFloor;
        
        if (SC_PathCreator.instance.floorsList[SC_PathCreator.instance.actualFloor].floorEndRoom.GetComponent<SC_RoomManager>().stair == this) //This stair will lead to the floor above
        {
            SC_PathCreator.instance.actualFloor++;
        }
        else if (SC_PathCreator.instance.floorsList[SC_PathCreator.instance.actualFloor].floorStartRoom.GetComponent<SC_RoomManager>().stair == this) //This stair will lead to the floor below
        {
            SC_PathCreator.instance.actualFloor--;
        }
        else
        {
            //TODO - Error
        }
        
        if(stairToConnect != null) {
            SC_PlayerController.instance.transform.position = stairToConnect.stairTPPoint.position;
        }
        else
        {
            SC_GameManager.instance.ChangeState(GameState.WIN);
        }
        
        
        SC_PathCreator.instance.floorsList[SC_PathCreator.instance.actualFloor].DisplayFloor(); //Display the floor in which the player will be transported
        SC_PathCreator.instance.floorsList[thisFloorIndex].HideFloor(); //Hide the current floor
    }
}
