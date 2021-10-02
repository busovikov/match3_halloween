using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [SerializeField]
    public int width;
    [SerializeField]
    public int height;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    public GameObject[] TilePool;

    private GameObject[,] tiles;

    void Awake()
    {
        tiles = new GameObject[width, height];

        List<int> types = Enumerable.Range(0, TilePool.Length).Select((index) => index).ToList();
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                var position = new Vector3(i, j, 0) + transform.position;
                GameObject tile = GameObject.Instantiate(prefab, position, Quaternion.identity, transform);
                tile.name = i.ToString() + "," + j.ToString();
                tiles[i, j] = tile;
                InitContent(i, j, types);
            }
    }

    public Coroutine Create(Vector2 position, Vector2 offset)
    {
        var index = UnityEngine.Random.Range(0, TilePool.Length);
        return GetTile(position).CreateContent(TilePool[index], index, offset);
    }
    private void InitContent(int x, int y, List<int> types)
    {
        Vector2 position = new Vector2(x, y);
        List<int> allowedTypes = new List<int>(types);
        var back = GetType(new Vector2(x - 1, y));
        if (back == GetType(new Vector2(x - 2, y)))
        {
            allowedTypes.Remove(back);
        }
        var down = GetType(new Vector2(x, y - 1));
        if (down == GetType(new Vector2(x, y - 2)))
        {
            allowedTypes.Remove(down);
        }

        var index = allowedTypes[UnityEngine.Random.Range(0, allowedTypes.Count)];
        GetTile(position).CreateContent(TilePool[index], index);
    }

    public Tile GetTile(Vector2 position)
    {
        return GetTile((int)position.x, (int)position.y);
    }
    public Tile GetTile(int x, int y)
    {
        return tiles[x, y].GetComponent<Tile>();
    }
    public int GetType(Vector2 arrayPosition)
    {
        return GetType((int)arrayPosition.x, (int)arrayPosition.y);
    }

    public int GetType(int x, int y)
    {
        if (!IsValid(x, y))
            return -1;
        return tiles[x, y].GetComponent<Tile>().tileType;
    }
    public bool IsValid(Vector2 arrayPosition)
    {
        return IsValid((int)arrayPosition.x, (int)arrayPosition.y);
    }

    public bool IsValid(int x, int y)
    {
        return y >= 0 && y < height && x >= 0 && x < width;
    }
}
