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
        
        public float bonusRate;
        public int guaranteedDropCount;
        private static int _dropCount = 0;

        private void Start()
        {
            RandomDrop();
            if (isBonus)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
                
            }
        }
        
        private void RandomDrop()
        {
            float bonusPointDrop = Random.Range(0f,1f);
            if (_dropCount >= guaranteedDropCount - 1)
            {
                isBonus = true;
                _dropCount = 0;
                return;
            }
            if (bonusPointDrop <= bonusRate )
            {
                isBonus = true;
                
            }
            else
            {
                _dropCount++;
            }
        }
      
        
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