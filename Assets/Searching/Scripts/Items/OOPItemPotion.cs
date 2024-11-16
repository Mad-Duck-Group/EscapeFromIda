using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Searching
{

    public class OOPItemPotion : Item
    {
        public int healPoint = 10;
        public bool isBonus;
        public float bonusMultiplier = 2;
        
        // USE HERE, I SAD
        public float bonusRate = 0.2f;
        public int guaranteedDropCount;
        private static int _dropCount;
        // END HERE, BAKA!!!!

        private void Start()
        {
            RandomDrop();
            if (isBonus)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }

        // DO IT HERE, I SAD
        private void RandomDrop()
        {
           
        }
        // END HERE, BAKA!!!!
        
        public override void OnHit()
        {
            if (isBonus)
            {
                OOPMapGenerator.Instance.Player.Heal((int)(healPoint * bonusMultiplier));
                Debug.Log("You got " + Name + " Bonus : " + healPoint * bonusMultiplier);
            }
            else
            {
                OOPMapGenerator.Instance.Player.Heal(healPoint);
                Debug.Log("You got " + Name + " : " + healPoint);
            }
            OOPMapGenerator.Instance.MapData[positionX, positionY] = BlockTypes.Empty;
            Destroy(gameObject);
        }
    }
}