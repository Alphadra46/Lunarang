using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_RoomManager : MonoBehaviour
{
    [Header("Door possible position")]
    public Transform doorNorth;
    public Transform doorSouth;
    public Transform doorWest;
    public Transform doorEast;

    private void Awake()
    {
        doorNorth.gameObject.SetActive(false);
        doorSouth.gameObject.SetActive(false);
        doorWest.gameObject.SetActive(false);
        doorEast.gameObject.SetActive(false);
    }
}
