using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Searching
{

    public class OOPEnemy : Character
    {
        public void Start()
        {
            GetRemainEnergy();
        }

        public override void Hit()
        {
            OOPMapGenerator.Instance.Player.Attack(this);
            Attack( OOPMapGenerator.Instance.Player);
        }

        public void Attack(OOPPlayer _player)
        {
            _player.TakeDamage(AttackPoint, playAnimation: true);
        }

        protected override void CheckDead()
        {
            base.CheckDead();
            if (energy <= 0)
            {
                OOPMapGenerator.Instance.Enemies[positionX, positionY] = null;
                OOPMapGenerator.Instance.EntityData[positionX, positionY] = BlockTypes.Empty;
            }
        }

        public void RandomMove()
        {
            if (isDead) return;
            int toX = positionX;
            int toY = positionY;
            int random = Random.Range(0, 4);
            switch (random)
            {
                case 0:
                    // up
                    toY += 1;
                    break;
                case 1:
                    // down 
                    toY -= 1;
                    break;
                case 2:
                    // left
                    toX -= 1;
                    break;
                case 3:
                    // right
                    toX += 1;
                    break;
            }
            spriteRenderer.flipX = toX >= positionX;
            if (!IsValid(toX, toY) || HasPlacement(toX, toY) || IsEnemy(toX, toY)) return;
            OOPMapGenerator.Instance.EntityData[positionX, positionY] = BlockTypes.Empty;
            OOPMapGenerator.Instance.Enemies[positionX, positionY] = null;
            if (IsPlayer(toX, toY)) Hit();
            MoveTween(new Vector2(toX - positionX, toY - positionY));
            OOPMapGenerator.Instance.EntityData[positionX, positionY] = BlockTypes.Enemy;
            OOPMapGenerator.Instance.Enemies[positionX, positionY] = this;
        }
    }
}