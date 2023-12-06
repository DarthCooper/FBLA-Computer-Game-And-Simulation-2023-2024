using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static UnityEditor.PlayerSettings;

public class Firewall : NetworkBehaviour
{
    bool canInteract;

    public GameTiles tiles;

    bool disabled;

    public Tilemap tilemap;

    public SpriteShapeController[] shapes;

    public bool removingTiles = false;

    public TileBase tile;

    bool useSplines = false;

    public void OnInteract()
    {
        Transform closest = GetClosest();
        if(canInteract && closest.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            ComputerPuzzle.Instance.EnableComputer(this);
            canInteract = false;
        }
    }

    public Transform GetClosest()
    {
        float maxDistance = 10000;
        Transform Target = null;
        foreach (var target in GameObject.FindGameObjectsWithTag("Player"))
        {
            float Distance = Vector2.Distance(transform.position, target.transform.position);
            if (Distance < maxDistance)
            {
                maxDistance = Distance;
                Target = target.transform;
            }
        }
        return Target;
    }

    float progress = 0;
    private void FixedUpdate()
    {
        if(removingTiles)
        {
            if(useSplines)
            {
                foreach (var shape in shapes)
                {
                    Transform MoveTransform = shape.transform.Find("MoveTransform");
                    progress += 0.01f;
                    MoveTransform.localPosition = GetPoint(shape.spline, progress);
                    Vector3Int worldPos = new Vector3Int(Mathf.FloorToInt(MoveTransform.position.x), Mathf.FloorToInt(MoveTransform.position.y), 0);
                    if (tiles.tiles.TryGetValue(worldPos, out WorldTile _tile))
                    {
                        _tile.TilemapMember.SetTile(_tile.LocalPlace, null);
                    }
                    if (progress >= 1)
                    {
                        removingTiles = false;
                    }
                }

            }else
            {
                Vector2 screenPosition = Mouse.current.position.ReadValue();
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                var worldPoint = new Vector3Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), 0);
                if (tiles.tiles.TryGetValue(worldPoint, out WorldTile _tile))
                {
                    _tile.TilemapMember.SetColor(_tile.LocalPlace, Color.red);
                    if (Input.GetMouseButton(0))
                    {
                        _tile.TilemapMember.SetTile(_tile.LocalPlace, null);
                    }
                }
            }
        }
    }

    public void OnEndInteract()
    {
        canInteract = true;
    }

    public void DisableFirewall()
    {
        CmdDisableFirewall();
    }

    [Command(requiresAuthority = false)]
    public void CmdDisableFirewall()
    {
        if(isServer)
        {
            ServerDisableFirewall();
        }
    }

    [Server]
    public void ServerDisableFirewall()
    {
        RpcDisableFirewall();
    }

    [ClientRpc]
    public void RpcDisableFirewall()
    {
        if(!disabled)
        {
            DisableTiles();
        }
    }

    public void DisableTiles()
    {
        if(!tilemap) { Debug.LogError("Script needs tilemap to disable tiles"); return; }
        foreach(var shape in shapes)
        {
            var length = shape.spline.GetPointCount();
            for (int i = 0; i < length; i++)
            {
                removingTiles = true;
                
            }
        }
    }

    public static Vector2 GetPoint(Spline spline, float progress)
    {
        var length = spline.GetPointCount();
        var i = Mathf.Clamp(Mathf.CeilToInt((length - 1) * progress), 0, length - 1);

        var t = progress * (length - 1) % 1f;
        if (i == length - 1 && progress >= 1f)
            t = 1;

        var prevIndex = Mathf.Max(i - 1, 0);

        var _p0 = new Vector2(spline.GetPosition(prevIndex).x, spline.GetPosition(prevIndex).y);
        var _p1 = new Vector2(spline.GetPosition(i).x, spline.GetPosition(i).y);
        var _rt = _p0 + new Vector2(spline.GetRightTangent(prevIndex).x, spline.GetRightTangent(prevIndex).y);
        var _lt = _p1 + new Vector2(spline.GetLeftTangent(i).x, spline.GetLeftTangent(i).y);

        return BezierUtility.BezierPoint(
           new Vector2(_rt.x, _rt.y),
           new Vector2(_p0.x, _p0.y),
           new Vector2(_p1.x, _p1.y),
           new Vector2(_lt.x, _lt.y),
           t
        );
    }
}
