using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Searching
{
    public class Character : Identity
    {
        [Header("Character")]
        public int energy;
        public int AttackPoint;
        public float moveTweenTime = 0.2f;
        protected bool isDead;

        protected bool isAlive;
        protected bool isFreeze;
        
        protected Tween positionTween;
        protected Tween colorTween;
        protected Color originalColor;
        [SerializeField] protected SpriteRenderer spriteRenderer;

        public Vector2 LastDirection { get; protected set; }

        protected void Awake()
        {
            originalColor = spriteRenderer.color;
        }

        // Start is called before the first frame update
        protected void GetRemainEnergy()
        {
            Debug.Log(Name + " : " + energy);
        }

        public virtual void Move(Vector2 direction)
        {
            if (GameManager.Instance.GameIsOver) return;
            if (isFreeze)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                isFreeze = false;
                return;
            }
            LastDirection = direction;
            int toX = (int)(positionX + direction.x);
            int toY = (int)(positionY + direction.y);
            spriteRenderer.flipX = !(direction.x >= 0);
            bool checkHit = CheckHit(toX, toY);
            if (checkHit)
            {
                MoveTween(direction);
                TakeDamage(1);
            }
            OOPMapGenerator.Instance.MoveEnemies();

        }

        private bool CheckHit(int toX, int toY, bool activateTile = true)
        {
            if (this is not OOPPlayer) return false;
            if (IsEnemy(toX, toY))
            {
                OOPMapGenerator.Instance.Enemies[toX, toY].Hit();
            }
            if (HasPlacement(toX, toY))
            {
                if (IsDemonWalls(toX, toY))
                {
                    OOPMapGenerator.Instance.DemonWalls[toX, toY].Hit();
                    return false;
                }
                if (IsItem(toX, toY))
                {
                    OOPMapGenerator.Instance.Items[toX, toY].OnHit();
                    return true;
                }
                if (IsTile(toX, toY) && activateTile)
                {
                    if (!OOPMapGenerator.Instance.Tiles[toX, toY].OnBeforeHit()) return false;
                    OOPMapGenerator.Instance.Tiles[toX, toY].OnHit();
                    return OOPMapGenerator.Instance.Tiles[toX, toY] is not IceTile;
                }
                if (IsExit(toX, toY))
                {
                    OOPMapGenerator.Instance.Exit.Hit();
                    return true;
                }
            }
            else if (IsValid(toX, toY))
            {
                return true;
            }
            return false;
        }
        public void MoveTween(Vector2 direction)
        {
            int toX = (int)(positionX + direction.x);
            int toY = (int)(positionY + direction.y);
            int fromX = positionX;
            int fromY = positionY;
            if (!IsValid(toX, toY)) return;
            var tile = OOPMapGenerator.Instance.GetTile(toX, toY);
            if (this is OOPPlayer && (tile && !tile.CanStepOn)) return;
            positionX = toX;
            positionY = toY;
            if (positionTween.IsActive()) positionTween.Kill();
            positionTween = transform.DOMove(new Vector3(positionX, positionY, 0), moveTweenTime).OnComplete(() => CheckHit(toX, toY, false));
            if (this is not OOPPlayer) return;
            if (fromX == positionX && fromY == positionY) return;
            OOPMapGenerator.Instance.EntityData[fromX, fromY] = BlockTypes.Empty;
            OOPMapGenerator.Instance.EntityData[toX, toY] = BlockTypes.PlayerBlock;
            if (OOPMapGenerator.Instance.Tiles[toX, toY] is IceTile)
            {
                MoveTween(direction.normalized);
            }
        }
        
        // hasPlacement คืนค่า true ถ้ามีการวางอะไรไว้บน map ที่ตำแหน่ง x,y
        public bool HasPlacement(int x, int y)
        {
            BlockTypes mapData = OOPMapGenerator.Instance.GetMapData(x, y);
            Debug.Log("HasPlacement : " + mapData);
            return (mapData != BlockTypes.Invalid && mapData != BlockTypes.Empty) || IsTile(x, y);
        }

        public bool IsValid(int x, int y)
        {
            return OOPMapGenerator.Instance.GetMapData(x, y) != BlockTypes.Invalid;
        }
        public bool IsDemonWalls(int x, int y)
        {
            BlockTypes mapData = OOPMapGenerator.Instance.GetMapData(x, y);
            return mapData == BlockTypes.DemonWall;
        }

        public bool IsItem(int x, int y)
        {
            return OOPMapGenerator.Instance.Items[x, y];
        }

        public bool IsTile(int x, int y)
        {
            return OOPMapGenerator.Instance.GetTile(x, y);
        }
        public bool IsEnemy(int x, int y)
        {
            BlockTypes entityData = OOPMapGenerator.Instance.GetEntityData(x, y);
            return entityData == BlockTypes.Enemy;
        }
        
        public bool IsPlayer(int x, int y)
        {
            BlockTypes entityData = OOPMapGenerator.Instance.GetEntityData(x, y);
            return entityData == BlockTypes.PlayerBlock;
        }
        
        public bool IsExit(int x, int y)
        {
            BlockTypes mapData = OOPMapGenerator.Instance.GetMapData(x, y);
            return mapData == BlockTypes.Exit;
        }

        public virtual void TakeDamage(int damage, bool freeze = false, bool playAnimation = false)
        {
            energy -= damage;
            Debug.Log(Name + " Current Energy : " + energy);
            if (playAnimation)
            {
                if (colorTween.IsActive()) colorTween.Kill(complete: true);
                switch (isFreeze)
                {
                    case false when freeze:
                        colorTween = spriteRenderer.DOColor(Color.blue, 0.2f);
                        break;
                    case true when !freeze:
                        colorTween = spriteRenderer.DOColor(originalColor, 0.2f);
                        break;
                    default:
                        colorTween = spriteRenderer.DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo);
                        break;
                }
            }
            isFreeze = freeze;
            CheckDead();
        }


        public void Heal(int healPoint)
        {
            energy += healPoint;
            if (colorTween.IsActive()) colorTween.Kill(complete: true);
            colorTween = spriteRenderer.DOColor(Color.green, 0.2f).SetLoops(2, LoopType.Yoyo);
        }

        protected virtual void CheckDead()
        {
            if (energy > 0) return;
            isDead = true;
            if (this is OOPEnemy)
            {
                var sequence = DOTween.Sequence();
                sequence.Append(transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0), 0.2f));
                sequence.Join(spriteRenderer.DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo));
                sequence.OnComplete(() => Destroy(gameObject));
            }
            else
            {
                GameManager.Instance.Lose();
            }
        }
    }
}