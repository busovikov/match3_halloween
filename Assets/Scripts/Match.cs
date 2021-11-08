using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match 
{
    private TileMap tiles;
    private HashSet<Vector2> toBeDestroyed;
    private List<Vector2> setX;
    private List<Vector2>[] setY;

    public bool bisy = false;
    public Match(TileMap tilesMap)
    {
        tiles = tilesMap;
        toBeDestroyed = new HashSet<Vector2>();
    }
    public bool ToBeDestroyed()
    {
        return toBeDestroyed.Count > 0;
    }

    public bool SwapsAvailable()
    {
        for (int x = 0; x < tiles.width; x++)
            for (int y = 0; y < tiles.height; y++)
            {
                var position = new Vector2(x, y);
                var next = new Vector2(x, y + 1);
                if (tiles.GetType(position) == tiles.GetType(next) && (ForTwo(position, Vector2.down) || ForTwo(next, Vector2.up)))
                {
                    return true;
                }
                next = new Vector2(x, y + 2);
                if (tiles.GetType(position) == tiles.GetType(next) && ForHole(position, Vector2.up))
                {
                    return true;
                }
                next = new Vector2(x + 1, y);
                if (tiles.GetType(position) == tiles.GetType(next) && (ForTwo(position, Vector2.left) || ForTwo(next, Vector2.right)))
                {
                    return true;
                }
                next = new Vector2(x + 2, y);
                if (tiles.GetType(position) == tiles.GetType(next) && ForHole(position, Vector2.right))
                {
                    return true;
                }
            }
        return false;
    }
    private bool ForTwo(Vector2 position, Vector2 direction)
    {
        var type = tiles.GetType(position);

        var forward = position + direction;
        var right = new Vector2(direction.y, direction.x);
        var left = -right;

        return tiles.GetType(forward + direction) == type ||
            tiles.GetType(forward + left) == type ||
            tiles.GetType(forward + right) == type;
    }

    private bool ForHole(Vector2 position, Vector2 direction)
    {
        var type = tiles.GetType(position);

        var forward = position + direction;
        var right = new Vector2(direction.y, direction.x);
        var left = -right;

        return tiles.GetType(forward + left) == type || tiles.GetType(forward + right) == type;
    }
    private void InitSets()
    {
        setX = new List<Vector2>();
        setX.Capacity = tiles.width;
        setY = new List<Vector2>[tiles.width];
        for (int i = 0; i < tiles.width; i++)
        {
            setY[i] = new List<Vector2>();
            setY[i].Capacity = tiles.height;
        }
    }

    public bool IsAny(out HashSet<Vector2> destructionList)
    {
        bisy = true;
        destructionList = null;
        InitSets();
        for (int y = 0; y < tiles.height; y++)
        {
            for (int x = 0; x < tiles.width; x++)
            {
                StackOn(x, y, setX);
                StackOn(x, y, setY[x]);
                
            }
            Sink(setX);
        }
        for (int x = 0; x < tiles.width; x++)
            Sink(setY[x]);
        bisy = false;
        if (ToBeDestroyed())
        {
            destructionList = new HashSet<Vector2>(toBeDestroyed);
            toBeDestroyed.Clear();
            return true;
        }
        return false;
    }
    private void StackOn(int x, int y, List<Vector2> set)
    {
        if (set.Count > 0 && tiles.GetType(set[0]) != tiles.GetType(x, y))
        {
            Sink(set);
        }
        if (tiles.IsValid(x, y))
        {
            tiles.GetTile(x,y).AddAction("Add VALID");
            set.Add(new Vector2(x, y));
        }
    }

    private void Sink(List<Vector2> set)
    {
        if (set.Count >= 3)
        {
            foreach (Vector2 v in set) { tiles.GetTile(v).invalid = true; tiles.GetTile(v).AddAction("Sink Invalid"); }
            toBeDestroyed.UnionWith(set);
        }
        set.Clear();
    }
}
