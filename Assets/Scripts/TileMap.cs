using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject goal;
    public GameObject prefab;
    public GameObject[] TilePool;
    public GameObject[] DeadTilePool;

    private Tile[,] tiles;
    private int goalType;

    //private GameObject[,] DeadPool;
    void Awake()
    {
        tiles = new Tile[width, height];

        //DeadPool = new GameObject[TilePool.Length, (int)(width * height / 2)];

        List<int> types = Enumerable.Range(0, TilePool.Length).Select((index) => index).ToList();
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                var position = new Vector3(i, j, 0) + transform.position;
                Tile tile = GameObject.Instantiate(prefab, position, Quaternion.identity, transform).GetComponent<Tile>();
                tile.name = i.ToString() + "," + j.ToString();
                tiles[i, j] = tile;
                InitContent(i, j, types);
            }
    }

    private void Start()
    {
        goalType = goal.GetComponent<Goals>().type;
    }

    public Coroutine Create(Vector2 position, Vector2 offset, bool dropped = false)
    {
        var index = UnityEngine.Random.Range(0, TilePool.Length);
        return GetTile(position).CreateContent(TilePool[index], index, dropped, offset);
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
        const bool dropped = false;
        GetTile(position).CreateContent(TilePool[index], index, dropped);
    }

    public Tile GetTile(Vector2 position)
    {
        return GetTile((int)position.x, (int)position.y);
    }
    public Tile GetTile(int x, int y)
    {
        return tiles[x, y];
    }
    public int GetType(Vector2 arrayPosition)
    {
        return GetType((int)arrayPosition.x, (int)arrayPosition.y);
    }

    public int GetType(int x, int y)
    {
        if (!IsValid(x, y))
            return -1;
        return tiles[x, y].tileType;
    }
    public bool IsValid(Vector2 arrayPosition)
    {
        return IsValid((int)arrayPosition.x, (int)arrayPosition.y);
    }

    public bool IsValid(int x, int y)
    {
        bool withInBoundaries = y >= 0 && y < height && x >= 0 && x < width;
        return withInBoundaries && !tiles[x, y].invalid && tiles[x, y].IsSet();
    }

    internal void SpawnDead(int tileType, Transform transform)
    {
        ObjectPool.DeadObject dead = ObjectPool.Instance.GetDead(tileType);
        if (dead.obj == null)
        {
            return;//Instantiate(DeadTilePool[tileType], transform.position, Quaternion.identity, transform);
        }

        dead.obj.transform.position = transform.position;
        dead.obj.SetActive(true);
        if (tileType != goalType)
        {
            var x = UnityEngine.Random.Range(-1f, 1f);
            var y = UnityEngine.Random.Range(0.1f, 1f);
            dead.body.AddForce(new Vector2(x, y) * 5, ForceMode2D.Impulse);
            //dead.anim.SetTrigger("Dead");
        }
        else
        {
            var toGoal =  goal.transform.position - transform.position;
            dead.anim.SetTrigger("Dead");
            dead.body.gravityScale = 0;
            dead.body.AddForce (toGoal * 2.5f, ForceMode2D.Impulse );
        }
        
    }
}
