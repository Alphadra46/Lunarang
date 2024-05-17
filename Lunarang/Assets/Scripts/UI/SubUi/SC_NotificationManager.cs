using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SC_NotificationManager : MonoBehaviour
{
    #region Variables

    public GameObject notificationPrefab;
    public Transform notificationTransform;

    public static Action<SC_Ressource, int> addRessourceNotification;
    public static Action<Sprite, string> addNotification;
    
    public static Action<SC_NotificationUI> removeNotification;

    [ShowInInspector] private Dictionary<SC_Ressource,GameObject> ressourceNotificationList = new Dictionary<SC_Ressource,GameObject>();
    [ShowInInspector] private List<GameObject> notificationList = new List<GameObject>();

    #endregion

    private void OnEnable()
    {
        addNotification += AddNotification;
        addRessourceNotification += AddRessourceNotification;
        removeNotification += RemoveNotification;
    }

    public bool CheckRessourceNotificationExist(SC_Ressource ressource)
    {

        return ressourceNotificationList.ContainsKey(ressource);

    }
    
    public bool CheckNotificationExist(GameObject notification)
    {

        return notificationList.Contains(notification);

    }

    private void AddRessourceNotification(SC_Ressource ressource, int amount)
    {

        if (CheckRessourceNotificationExist(ressource))
        {

            UpdateRessourceNotification(ressource, amount);
            return;
            
        }
        
        if(this == null) return;
        
        var notification = Instantiate(notificationPrefab, transform);
        if(!notification.TryGetComponent(out SC_NotificationUI notificationUI)) return;

        var finalPrompt = $"{ressource.name} x{amount}";
        
        notificationUI.Init(ressource, amount);
        
        ressourceNotificationList.Add(ressource, notification);

    }

    private void UpdateRessourceNotification(SC_Ressource ressource, int newAmount)
    {
    
        var notification = ressourceNotificationList[ressource];
        if(notification == null) return;
        
        if(!notification.TryGetComponent(out SC_NotificationUI notificationUI)) return;
        
        notificationUI.UpdateNotification(ressource, newAmount);

    }

    private void AddNotification(Sprite sprite, string prompt)
    {
        if(this == null) return;
        
        var notification = Instantiate(notificationPrefab, transform);
        if(!notification.TryGetComponent(out SC_NotificationUI notificationUI)) return;
        
        notificationUI.Init(sprite, prompt);
        
    }

    public void RemoveNotification(SC_NotificationUI notifUI)
    {

        if (ressourceNotificationList.ContainsValue(notifUI.gameObject))
        {
            var notif = ressourceNotificationList.FirstOrDefault(x => x.Value == notifUI.gameObject).Key;
            ressourceNotificationList.Remove(notif);
            notifUI.Destroy();
        }
        else if(CheckNotificationExist(notifUI.gameObject))
        {
            notificationList.Remove(notifUI.gameObject);
            notifUI.Destroy();
        }
        
    }
}
