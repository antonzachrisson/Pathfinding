using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite blockedSprite_;
    [SerializeField] private Sprite unblockedSprite_;

    private SpriteRenderer rend_;
    
    public bool blocked;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public Tile cameFromTile;

    public void Init(bool blocked_, int x_, int y_)
    {
        rend_ = GetComponent<SpriteRenderer>();
        blocked = blocked_;
        x = x_;
        y = y_;
    }

    public void CalcFCost()
    {
        fCost = gCost + hCost;
    }

    private void Update()
    {
        if (blocked)
            rend_.sprite = blockedSprite_;
        else
            rend_.sprite = unblockedSprite_;
    }
}
