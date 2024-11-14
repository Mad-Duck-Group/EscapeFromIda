using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Searching
{

    public class OOPDemonWall : Identity
    {
        public int Damage;
        public bool IsIceWall;

        private void Start()
        {
            IsIceWall = Random.Range(0, 100) < 50;
            if (IsIceWall)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
        public override void Hit()
        {
            if (IsIceWall)
            {
                OOPMapGenerator.Instance.Player.TakeDamage(Damage, IsIceWall, playAnimation: true);
            }
            else
            {
                OOPMapGenerator.Instance.Player.TakeDamage(Damage, playAnimation: true);
            }
            OOPMapGenerator.Instance.MapData[positionX, positionY] = 0;
            Destroy(gameObject);
        }
    }
}