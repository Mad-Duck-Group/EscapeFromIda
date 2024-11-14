using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{

    public class OOPFireStormItem : Item
    {
        public override void OnHit()
        {
            OOPMapGenerator.Instance.Player.inventory.AddItem("FireStorm");
            OOPMapGenerator.Instance.Items[positionX, positionY] = null;
            OOPMapGenerator.Instance.MapData[positionX, positionY] = BlockTypes.Empty;
            Destroy(gameObject);
        }
    }

}