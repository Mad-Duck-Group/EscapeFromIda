using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Searching;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected bool canStepOn;
    [SerializeField, EnumFlags] protected TerrainTypes terrainType;
    public bool CanStepOn => canStepOn;
    public TerrainTypes TerrainType => terrainType;
    
    public int PosX { get; set; }
    public int PosY { get; set; }

    public virtual bool OnBeforeHit()
    {
        return canStepOn;
    }

    public virtual void OnHit()
    {
        
    }
}
