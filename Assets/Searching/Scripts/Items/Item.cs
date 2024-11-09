using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string Name;
    public int positionX;
    public int positionY;
    public int dropRate; // Use this to implement drop rate for items

    public virtual void OnHit()
    {
        
    }
}
