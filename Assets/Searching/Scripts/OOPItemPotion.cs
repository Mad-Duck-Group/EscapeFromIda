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

        private void Start()
        {
            isBonus = Random.Range(0, 100) < 20;
            if (isBonus)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
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