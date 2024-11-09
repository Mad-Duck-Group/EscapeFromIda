using System.Collections;
using System.Collections.Generic;
using Searching;
using UnityEngine;

public class BarbWallTile : Tile
{
    [SerializeField] private int damage = 1;
    
    public override bool OnBeforeHit()
    {
        OOPMapGenerator.Instance.Player.TakeDamage(damage, playAnimation: true);
        return canStepOn;
    }
}
