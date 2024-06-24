using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class SC_TutoRoomManager : MonoBehaviour
{
    public bool isSpecialRoom;
    [Header("Door possible position")]
    [TabGroup("Settings", "Global Settings")] public SC_Door doorNorth;
    [TabGroup("Settings", "Global Settings")] public SC_Door doorSouth;
    [TabGroup("Settings", "Global Settings")] public SC_Door doorWest;
    [TabGroup("Settings", "Global Settings")] public SC_Door doorEast;
    
    [Header("Other room parameters")] 
    [SerializeField, TabGroup("Settings", "Global Settings")] private Interactor roomInteractor;
    [SerializeField, TabGroup("Settings", "Global Settings")] private Collider spawnArea;
    [SerializeField, TabGroup("Settings", "Global Settings")] private GameObject enterRoomTipsUIPrefab;
    [SerializeField, TabGroup("Settings", "Global Settings")] private GameObject resourceTipsUIPrefab;
    [SerializeField, TabGroup("Settings", "Global Settings")] private GameObject fountainTipsUIPrefab;
    [SerializeField, TabGroup("Settings", "Global Settings")] private GameObject skillChest;
    [SerializeField, TabGroup("Settings", "Global Settings")] private GameObject fountain;
    [SerializeField, TabGroup("Settings", "Global Settings")] private SC_Resource ressourceToForceAdd;
    [SerializeField, TabGroup("Settings", "Global Settings")] private int amountOfRessource;
    [SerializeField, TabGroup("Settings", "Global Settings")] private SC_Resource essenceFragment;

    private GameObject enterRoomTipsUI;
    private GameObject resourceTipsUI;
    private GameObject fountainTipsUI;

    [HideInInspector] public int totalEnemies;
    private bool isInit = false;
    
    private VisualEffect roomClearVFX;
    private List<SC_Door> activeDoors = new List<SC_Door>();
    [ShowInInspector] private List<GameObject> enemiesInRoom = new List<GameObject>();
    [SerializeField] private List<GameObject> enemiesToSpawn = new List<GameObject>();
    [ShowInInspector] public bool isClear=false;
    private SC_ResourcesInventory resourcesInventory;

    private void Awake()
    {
        resourcesInventory = Resources.Load<SC_ResourcesInventory>("ResourceInventory");
    }

    void OnEnable()
    {
        if (isClear)
        {
            roomClearVFX = SC_Pooling.instance.GetItemFromPool("VFX", "VFX_Purification_02").GetComponent<VisualEffect>();
            roomClearVFX.transform.position = transform.position;
            float size = 22;
            roomClearVFX.SetVector2("Dimensions",new Vector2(size,size));
            roomClearVFX.gameObject.SetActive(true);
            roomClearVFX.Play();
        }
        
        if (isInit)
            return;

        isInit = true;
        
        doorNorth.Initialize(this);
        doorSouth.Initialize(this);
        doorWest.Initialize(this);
        doorEast.Initialize(this);
        
        doorNorth.DisableDoor();
        doorSouth.DisableDoor();
        doorWest.DisableDoor();
        doorEast.DisableDoor();
        
        if (isSpecialRoom)
        {
            //doorNorth.animator.SetBool("isOpen", true);
            doorNorth.EnableDoor();
        }
    }

    private void OnDisable()
    {
        if (isClear && roomClearVFX!=null)
        {
            roomClearVFX.Stop();
            roomClearVFX.gameObject.SetActive(false);
            SC_Pooling.instance.ReturnItemToPool("VFX",roomClearVFX.gameObject);
            roomClearVFX = null;
        }
    }

    public void LockDoors()
    {
        //SC_GameManager.clearRoom += ClearRoom;
        
        if (activeDoors.Count==0)
        {
            if (doorNorth.doorToConnect!=null)
            {
                activeDoors.Add(doorNorth);
            }
            if (doorSouth.doorToConnect!=null)
            {
                activeDoors.Add(doorSouth);
            }
            if (doorWest.doorToConnect!=null)
            {
                activeDoors.Add(doorWest);
            }
            if (doorEast.doorToConnect!=null)
            {
                activeDoors.Add(doorEast);
            }
        }
        
        foreach (var door in activeDoors)
        {
            door.doorCollider.isTrigger = false;
            door.animator.SetBool("isOpen", false);
        }

        if (enterRoomTipsUIPrefab == null)
            return;

        StartCoroutine(DisplayTipsUI(1f));

    }
    
    public void UnlockDoors()
    {
        //SC_GameManager.clearRoom -= ClearRoom;
        SC_FeedbackRoomStatusUI.roomCleared?.Invoke();
        
        foreach (var door in activeDoors)
        {
            if (door.doorCollider != null)
            {
                door.doorCollider.isTrigger = true;
                door.animator.SetBool("isOpen", true);
            }
        }
    }

    public IEnumerator DisplayTipsUI(float duration)
    {
        yield return new WaitForSecondsRealtime(0.75f);
        SC_GameManager.instance.SetPause();
        SC_PlayerController.instance.FreezeDash(true);
        SC_PlayerController.instance.FreezeMovement(true);
        enterRoomTipsUI = Instantiate(enterRoomTipsUIPrefab);
        yield return new WaitForSecondsRealtime(duration);
        SC_InputManager.instance.submit.started += HideTipsUI;
    }

    public IEnumerator DisplayResourceUI(float duration)
    {
        yield return new WaitForSecondsRealtime(0.75f);
        SC_GameManager.instance.SetPause();
        SC_PlayerController.instance.FreezeDash(true);
        SC_PlayerController.instance.FreezeMovement(true);
        resourceTipsUI = Instantiate(resourceTipsUIPrefab);
        yield return new WaitForSecondsRealtime(duration);
        SC_InputManager.instance.submit.started += HideResourceUI;
    }

    public IEnumerator DisplayFountainUI(float duration)
    {
        yield return new WaitForSecondsRealtime(0.75f);
        SC_GameManager.instance.SetPause();
        SC_PlayerController.instance.FreezeDash(true);
        SC_PlayerController.instance.FreezeMovement(true);
        fountainTipsUI = Instantiate(fountainTipsUIPrefab);
        yield return new WaitForSecondsRealtime(duration);
        SC_InputManager.instance.submit.started += HideFountainUI;
    }
    
    public void HideFountainUI(InputAction.CallbackContext context)
    {
        SC_InputManager.instance.submit.started -= HideFountainUI;
        SC_PlayerController.instance.FreezeDash(false);
        SC_PlayerController.instance.FreezeMovement(false);
        Destroy(fountainTipsUI);
        SC_GameManager.instance.SetPause();
    }
    
    public void HideResourceUI(InputAction.CallbackContext context)
    {
        SC_InputManager.instance.submit.started -= HideResourceUI;
        SC_PlayerController.instance.FreezeDash(false);
        SC_PlayerController.instance.FreezeMovement(false);
        Destroy(resourceTipsUI);
        SC_GameManager.instance.SetPause();
    }
    
    public void HideTipsUI(InputAction.CallbackContext context)
    {
        SC_InputManager.instance.submit.started -= HideTipsUI;
        SC_PlayerController.instance.FreezeDash(false);
        SC_PlayerController.instance.FreezeMovement(false);
        Destroy(enterRoomTipsUI);
        SC_GameManager.instance.SetPause();
    }
    
    private IEnumerator PurifyRoom(float duration, Interactor interactor)
    {
        float timer = duration;
        float radiusSize =  24;
        while (timer>0)
        {
            interactor.radius = Mathf.Lerp(0, radiusSize, 1 - (timer / duration));
            timer -= Time.deltaTime;
            yield return null;
        }
        interactor.radius = radiusSize;
    }
    
    private IEnumerator EndClearVFX(float duration, VisualEffect vfx)
    {
        yield return new WaitForSeconds(duration);
        SC_Pooling.instance.ReturnItemToPool("VFX",vfx.gameObject);
        vfx.gameObject.SetActive(false);
    }

    public void DecreaseEnemyCount(SC_AIStats enemy)
    {
        if (!enemiesInRoom.Contains(enemy.gameObject))
            return;

        totalEnemies--;
        enemiesInRoom.Remove(enemy.gameObject);

        if (totalEnemies>0)
            return;
        
        enemiesInRoom.Clear();
        isClear = true;
        SC_AIStats.onDeath -= DecreaseEnemyCount;
        UnlockDoors();
        
        var clearRoomVFX = SC_Pooling.instance.GetItemFromPool("VFX", "VFX_Purification_01").GetComponent<VisualEffect>();
        clearRoomVFX.gameObject.SetActive(true);
        clearRoomVFX.transform.position = transform.position;
        clearRoomVFX.Play();
        roomClearVFX = SC_Pooling.instance.GetItemFromPool("VFX", "VFX_Purification_02").GetComponent<VisualEffect>();
        roomClearVFX.transform.position = transform.position;
        roomClearVFX.gameObject.SetActive(true);
        float size = 22;
        roomClearVFX.SetVector2("Dimensions",new Vector2(size,size));
        roomClearVFX.Play();
        StartCoroutine(EndClearVFX(clearRoomVFX.GetFloat("Duration"), clearRoomVFX));
        StartCoroutine(PurifyRoom(2, roomInteractor));

        if (ressourceToForceAdd != null)
        {
            resourcesInventory.AddResource(ressourceToForceAdd,amountOfRessource);
            if (essenceFragment!=null)
                resourcesInventory.AddResource(essenceFragment,1);
            else
                StartCoroutine(DisplayResourceUI(1.25f));
        }

        if (fountain != null)
        {
            fountain.GetComponent<SC_InteractableBase>().isInteractable = true;
            StartCoroutine(DisplayFountainUI(1f));
        }
        
        if (skillChest==null)
            return;
        
        RevealChest(skillChest);
    }

    private void RevealChest(GameObject chest)
    {
        if (!chest.activeSelf)
            return;
        
        var chestInteractor = chest.GetComponentInChildren<Interactor>();
        var chestRewardInteractor = chest.GetComponent<SC_InteractableBase>();
        var ps = chest.GetComponentsInChildren<ParticleSystem>();
        chestRewardInteractor.isInteractable = true;
        foreach (var particle in ps)
        {
            particle.Play();
        }
        StartCoroutine(LerpSwap(chestInteractor,3));
    }
    
    IEnumerator LerpSwap(Interactor interactor, float duration)
    {
        float timer = duration;

        while (timer>0)
        {
            timer -= Time.deltaTime;
            interactor.radius = Mathf.Lerp(0, 3, 1 - (timer / duration));
            yield return null;
        }
    }
    
    public void SpawnEnemies()
    {
        totalEnemies = enemiesToSpawn.Count;

        foreach (var enemy in enemiesToSpawn)
        {
            var e = SC_Pooling.instance.GetItemFromPool("Ennemis", enemy.name);
            enemiesInRoom.Add(e);

            Bounds spawnBounds = new Bounds();
            spawnBounds = spawnArea.bounds;
            
            e.transform.position = new Vector3(spawnBounds.center.x + Random.Range(-spawnBounds.extents.x,spawnBounds.extents.x), enemy.transform.position.y, spawnBounds.center.z + Random.Range(-spawnBounds.extents.z, spawnBounds.extents.z));
            e.SetActive(true);
            e.GetComponent<AI_StateMachine>().TransitionToState(AI_StateMachine.EnemyState.Idle);
            e.GetComponent<AI_StateMachine>().CanAttack(true);
        }
    }
}
