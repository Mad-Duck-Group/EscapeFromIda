using System.Collections;
using System.Collections.Generic;
using Searching;
using UnityEngine;

public class OOPItemKey : Item
{
    public string key;

    public override void OnHit()
    {
        OOPMapGenerator.Instance.Player.inventory.AddItem(key);
        OOPMapGenerator.Instance.MapData[positionX, positionY] = BlockTypes.Empty;
        Destroy(gameObject);
    }
}
