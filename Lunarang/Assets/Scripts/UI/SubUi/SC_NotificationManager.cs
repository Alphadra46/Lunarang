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
    
    public static Action<GameObject> removeNotification;

    [ShowInInspector] private Dictionary<SC_Ressource,GameObject> ressourceNotificationList = new Dictionary<SC_Ressource,GameObject>();
    [ShowInInspector] private List<GameObject> notificationList = new List<GameObject>();

    #endregion

    private void OnEnable()
    {
        addNotification += AddNotification;
        addRessourceNotification += AddRessourceNotification;
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
        
        var notification = Instantiate(notificationPrefab, notificationTransform);
        if(!notification.TryGetComponent(out SC_NotificationUI notificationUI)) return;

        var finalPrompt = $"{ressource.name} x{amount}";
        
        notificationUI.Init(ressource.sprite, finalPrompt);
        
        ressourceNotificationList.Add(ressource, notification);

    }

    private void UpdateRessourceNotification(SC_Ressource ressource, int amount)
    {
    
        var notification = ressourceNotificationList[ressource];
        if(!notification.TryGetComponent(out SC_NotificationUI notificationUI)) return;
            
        var finalPrompt = $"{ressource.name} x{amount}";
        
        notificationUI.UpdateNotification(finalPrompt);

    }

    private void AddNotification(Sprite sprite, string prompt)
    {
       
        var notification = Instantiate(notificationPrefab, notificationTransform);
        if(!notification.TryGetComponent(out SC_NotificationUI notificationUI)) return;
        
        notificationUI.Init(sprite, prompt);
        
    }

    public void RemoveNotification(GameObject go)
    {

        if (ressourceNotificationList.ContainsValue(go))
        {
            var notif = ressourceNotificationList.FirstOrDefault(x => x.Value == go).Key;
            ressourceNotificationList.Remove(notif);
            Destroy(go);
        }
        else if(CheckNotificationExist(go))
        {
            notificationList.Remove(go);
            Destroy(go);
        }
        
    }
}
