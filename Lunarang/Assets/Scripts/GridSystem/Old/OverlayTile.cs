using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OverlayTile : MonoBehaviour
{

    public int G;
    public int H;
    
    public int F
    {
        get { return G + H; }
    }

    public bool isBlocked;
    
    public OverlayTile previous;
    public Vector3Int gridLocation;
    public Vector2Int grid2DLocation
    {
        get { return new Vector2Int(gridLocation.x,gridLocation.y); }
    }

    public List<Sprite> arrowSpriteList;

    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }
    
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        SetArrowSprite(ArrowTranslator.ArrowDirection.None);
    }

    public void SetArrowSprite(ArrowTranslator.ArrowDirection d)
    {
        var arrow = GetComponentsInChildren<SpriteRenderer>()[1];
        if (d==ArrowTranslator.ArrowDirection.None)
        {
            arrow.color = new Color(1, 1, 1, 0);
        }
        else
        {
            arrow.color = new Color(1, 1, 1, 1);
            arrow.sprite = arrowSpriteList[(int)d];
            arrow.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        }
    }
    
}
