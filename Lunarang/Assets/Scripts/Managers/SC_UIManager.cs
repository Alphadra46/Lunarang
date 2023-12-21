using UnityEngine;

public class SC_UIManager : MonoBehaviour
{
    public static SC_UIManager instance;

    [SerializeField] private GameObject InventoryPrefab;

    private GameObject currentInventory;

    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
    }

    public void ShowInventory()
    {
        if (currentInventory == null) currentInventory = Instantiate(InventoryPrefab);
        else Destroy(currentInventory);
    }
}
