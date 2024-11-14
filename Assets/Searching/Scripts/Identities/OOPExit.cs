using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Searching
{

    public class OOPExit : Identity
    {
        public string unlockKey;

        public override void Hit()
        {
            if (OOPMapGenerator.Instance.Player.inventory.numberOfItem(unlockKey) > 0)
            {
                Debug.Log("Exit unlocked");
                OOPMapGenerator.Instance.Player.enabled = false;
                GameManager.Instance.Win();
                Debug.Log("You win");
            }
            else
            {
                Debug.Log($"Exit locked, require key: {unlockKey}");
            }
        }
    }
}