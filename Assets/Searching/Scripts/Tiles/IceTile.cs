using System.Collections;
using System.Collections.Generic;
using Searching;
using UnityEngine;

public class IceTile : Tile
{
    [SerializeField] private int slippery = 1;
    private Vector2 playerDirection;

    public override bool OnBeforeHit()
    {
        playerDirection = OOPMapGenerator.Instance.Player.LastDirection;
        return canStepOn;
    }
    public override void OnHit()
    {
        var signX = playerDirection.x >= 0 ? 1 : -1;
        var signY = playerDirection.y >= 0 ? 1 : -1;
        var slipDirection = new Vector2(playerDirection.x == 0 ? 0 : playerDirection.x + signX * slippery,
            playerDirection.y == 0 ? 0 : playerDirection.y + signY * slippery);
        var playerPos = new Vector2(OOPMapGenerator.Instance.Player.positionX, OOPMapGenerator.Instance.Player.positionY);
        var finalPos = playerPos + slipDirection;
        var nextPos = playerPos;
        var finalSlipDirection = Vector2.zero;
        Debug.Log("Slippery: " + slipDirection.normalized);
        Debug.Log("Next Pos: " + nextPos);
        // while (!OOPMapGenerator.Instance.Player.IsValid((int)nextPos.x, (int)nextPos.y) ||
        //        (OOPMapGenerator.Instance.GetTile((int)nextPos.x, (int)nextPos.y) && 
        //        !OOPMapGenerator.Instance.GetTile((int)nextPos.x, (int)nextPos.y).CanStepOn))
        // {
        //     slipDirection = new Vector2(slipDirection.x == 0 ? 0 : slipDirection.x - signX,
        //         slipDirection.y == 0 ? 0 : slipDirection.y - signY);
        //     nextPos = playerPos + slipDirection;
        // }
        while (nextPos != finalPos)
        {
            nextPos += slipDirection.normalized;
            if (!OOPMapGenerator.Instance.Player.IsValid((int)nextPos.x, (int)nextPos.y)) break;
            if (OOPMapGenerator.Instance.GetTile((int)nextPos.x, (int)nextPos.y) &&
                !OOPMapGenerator.Instance.GetTile((int)nextPos.x, (int)nextPos.y).CanStepOn)
            {
                Debug.Log("break");
                break;
            }
            finalSlipDirection += slipDirection.normalized;
        }
        Debug.Log("Final Slip Direction: " + finalSlipDirection);
        var beforeLastPos = nextPos - slipDirection.normalized;
        if (OOPMapGenerator.Instance.GetTile((int)beforeLastPos.x, (int)beforeLastPos.y) is IceTile)
        {
            OOPMapGenerator.Instance.Player.MoveTween(finalSlipDirection);
            OOPMapGenerator.Instance.Player.TakeDamage(1);
            return;
        }
        OOPMapGenerator.Instance.Player.Move(finalSlipDirection);
    }
}
