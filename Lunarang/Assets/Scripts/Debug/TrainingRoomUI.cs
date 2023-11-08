using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoomUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !transform.GetChild(0).gameObject.activeSelf)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            SC_RoomRewards.instance.SimulateReward();
        }
    }

    public void SelectDrop(int index)
    {
        SC_RoomRewards.instance.ChooseReward(index);
        transform.GetChild(0).gameObject.SetActive(false);
    } 
    
    
}
