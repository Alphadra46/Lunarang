using System;
using System.Collections.Generic;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class SC_ModifierPanel : SerializedMonoBehaviour
{
    
    [FoldoutGroup("Settings")]
    public Sprite downArrowSprite;
    [FoldoutGroup("Settings")]
    public Color32 debuffColor;
    [FoldoutGroup("Settings")]
    public Sprite upArrowSprite;
    [FoldoutGroup("Settings")]
    public Color32 buffColor;

    [FoldoutGroup("Settings")]
    public Dictionary<Enum_Debuff, Sprite> debuffsIcons = new Dictionary<Enum_Debuff, Sprite>();
    [FoldoutGroup("Settings")]
    public Dictionary<Enum_Buff, Sprite> buffsIcons = new Dictionary<Enum_Buff, Sprite>();

    public GameObject widgetPrefab;

    private Dictionary<Enum_Buff, GameObject> currentBuffsWidgets = new Dictionary<Enum_Buff, GameObject>();
    private Dictionary<Enum_Debuff,GameObject> currentDebuffsWidgets = new Dictionary<Enum_Debuff, GameObject>();


    #region Actions

    public Action<Enum_Debuff> debuffAdded;
    public Action<Enum_Buff> buffAdded;
    
    public Action<Enum_Debuff> debuffRemoved;
    public Action<Enum_Buff> buffRemoved;

    #endregion

    private void Awake()
    {
        debuffAdded += DebuffAdded;
        buffAdded += BuffAdded;
        debuffRemoved += DebuffRemoved;
        buffRemoved += BuffRemoved;
    }


    private void BuffAdded(Enum_Buff buff)
    {
        currentBuffsWidgets.Add(buff, CreateWidget(buffsIcons[buff], upArrowSprite, buffColor));
    }
    
    private void BuffRemoved(Enum_Buff buff)
    {
        RemoveWidget(buff);
    }

    private void DebuffAdded(Enum_Debuff debuff)
    {
        currentDebuffsWidgets.Add(debuff, CreateWidget(debuffsIcons[debuff], downArrowSprite, debuffColor));
    }
    
    private void DebuffRemoved(Enum_Debuff debuff)
    {
        RemoveWidget(debuff);
    }

    private GameObject CreateWidget(Sprite icon, Sprite arrow, Color32 color)
    {
        if (!Instantiate(widgetPrefab, transform).TryGetComponent(out SC_ModifierWidget widgetSC)) return null;

        widgetSC.Init(icon, arrow, color);

        return widgetSC.gameObject;
    }
    
    private void RemoveWidget(Enum_Debuff debuff)
    {
        if(currentDebuffsWidgets.Count < 1) return;
        
        Destroy(currentDebuffsWidgets[debuff]);
        currentDebuffsWidgets.Remove(debuff);

    }

    private void RemoveWidget(Enum_Buff buff)
    {
        if(currentBuffsWidgets.Count < 1) return;
        
        Destroy(currentBuffsWidgets[buff]);
        currentBuffsWidgets.Remove(buff);
    }

}
