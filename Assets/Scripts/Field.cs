using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class Field : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    public int width;
    [SerializeField]
    public int height;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    public GameObject[] TilePool;
    

    private new BoxCollider2D collider;
    private Vector2 firstPosition = Vector2.left;

    private GameObject[,] tiles;
    private HashSet<Vector2> toBeDestroyed;
    private HashSet<Vector2> toCheckForMatch;
    private bool processing = false;
    private bool actionAllowed = true;

    // Start is called before the first frame update
    void Awake()
    {
        toBeDestroyed = new HashSet<Vector2>();
        toCheckForMatch = new HashSet<Vector2>();

        collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(width, height);
        collider.offset = collider.size / 2 - Vector2.one / 2;
        Camera.main.transform.position += (Vector3)collider.offset;
        Camera.main.orthographicSize = Math.Max(width, (float)height / 2) + 1;
        tiles = new GameObject[width, height];
        
        List<int> types = Enumerable.Range(0, TilePool.Length).Select((index) => index).ToList();
        for (int i = 0; i < width; i++)
            for(int j = 0; j < height; j++)
            {
                var position = new Vector3(i, j, 0) + transform.position;
                GameObject tile = Instantiate(prefab, position, Quaternion.identity, transform );
                tile.name = i.ToString() + "," + j.ToString();
                tiles[i, j] = tile;
                CreateContent(i, j, types);
            }
    }

    private void CreateContent(int x, int y, List<int> types)
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

        var i = 0;
        try
        {
            i = UnityEngine.Random.Range(0, allowedTypes.Count);
            var index = allowedTypes[i];
            Tile.CreateContent(GetTile(position), TilePool[index], index);
        }
        catch
        {
            Debug.Log("Index failed: " + i.ToString());
            Tile.CreateContent(GetTile(position), TilePool[i], i);
        }
        
    }

    private void SetPosition(Vector2 position)
    {
        Vector2 offsetPosition = GetArrayPosition(position - (Vector2)collider.transform.position);
        if (!processing && actionAllowed && firstPosition != offsetPosition)
        {

            if (firstPosition != Vector2.left && IsNeighbours(firstPosition, offsetPosition))
            {
                Debug.Log("Second" + offsetPosition.x.ToString() + " " + offsetPosition.y.ToString());
                actionAllowed = false;
                CastSwapOn(firstPosition, offsetPosition);
                firstPosition = Vector2.left;
            }
            else
            {
                Debug.Log("First" + offsetPosition.x.ToString() + " " + offsetPosition.y.ToString());
                firstPosition = offsetPosition;
            }
        }
    }

    private bool IsNeighbours(Vector2 firstPosition, Vector2 secondPosition)
    {
        var xOffset = Math.Abs(firstPosition.x - secondPosition.x);
        var yOffset = Math.Abs(firstPosition.y - secondPosition.y);

        return xOffset == 1 && yOffset == 0 || xOffset == 0 && yOffset == 1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        actionAllowed = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void CastSwapOn(Vector2 first, Vector2 second)
    {
        if (!IsValid(first) || !IsValid(second))
            return;
        var one = GetTile(first);
        var two = GetTile(second);

        one.ExchangeWith(two, ()=> {
            Debug.Log("exchanged");

            var direction = first - second;

            CheckMatch(direction, first);
            CheckMatch(-direction, second);

            StartCoroutine(Processing());
        });
    }

    private IEnumerator Processing()
    {
        processing = true;

        while (toBeDestroyed.Count > 0)
        {
            foreach (var item in toBeDestroyed)
            {
                GetTile(item).DestroyContent();
            }

            toBeDestroyed.Clear();

            List<Coroutine> dropAll = new List<Coroutine>();
            for (int x = 0; x < width; x++)
            {
                 dropAll.Add(StartCoroutine(DropAndReplaceWithNew(x, toCheckForMatch)));
            }

            foreach (Coroutine dropTask in dropAll)
            {
                yield return dropTask;
            }

            foreach (var dropPosition in toCheckForMatch)
            {
                CheckMatch(Vector2.down, dropPosition);
            }
        }

        if (!PossibleMatchExists())
        {
            yield return Reshufle();
        }

        processing = false;
    }

    private IEnumerator Reshufle()
    {
        int processing = 0;
        var until = new WaitUntil(() => processing == 0);
        List<int> axis_y = Enumerable.Range(0, height).Select((index) => index).ToList();
        List<List<int>> remainingPositions = new List<List<int>>();

        for (int i = 0; i < width; i++)
        {
            remainingPositions.Add(new List<int>(axis_y));
        }

        int swap_x = 0;
        int swap_y = 0;
        bool swap = false;
        while (remainingPositions.Count > 0)
        {
            var new_x = UnityEngine.Random.Range(0, remainingPositions.Count);
            var new_y = UnityEngine.Random.Range(0, remainingPositions[new_x].Count);
            remainingPositions[new_x].RemoveAt(new_y);
            if (remainingPositions[new_x].Count == 0)
            {
                remainingPositions.RemoveAt(new_x);
            }

            if (swap)
            {
                processing++;
                GetTile(swap_x, swap_y).ExchangeWith(GetTile(new_x, new_y), ()=> { processing--; });
            }
            else 
            {
                swap_x = new_x;
                swap_y = new_y;
            }

            swap = !swap;
        }

        yield return until;
    }

    private IEnumerator DropAndReplaceWithNew(int x, HashSet<Vector2> toCheck)
    {
        List<Coroutine> animations = new List<Coroutine>();
        int destroyedCount = 0;

        for (int y = 0; y < height; y++)
        {
            Tile tile = GetTile(x, y);
            if (tile.content == null)
            {
                destroyedCount++;
            }
            else if (destroyedCount > 0)
            {
                var dropPosition = new Vector2(x, y - destroyedCount);
                animations.Add(GetTile(x, y).DropTo(GetTile(dropPosition)));
                toCheck.Add(dropPosition);
                yield return null;
            }
        }

        for (int i = 0; i < destroyedCount; i++)
        {
            var index = UnityEngine.Random.Range(0, TilePool.Length);
            var dropPosition = new Vector2(x, height - destroyedCount + i);
            animations.Add(Tile.CreateContent(
                GetTile(dropPosition),
                TilePool[index], 
                index, 
                new Vector2(0, destroyedCount)));
            toCheck.Add(dropPosition);
            yield return null;
        }

        // Wait for all animations are done
        foreach (Coroutine animation in animations)
        {
            yield return animation;
        }

        yield return new WaitForSeconds(.2f); 
    }

    private bool PossibleMatchExists()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var position = new Vector2(x, y);
                var next = new Vector2(x, y + 1);
                if (GetType(position) == GetType(next) && (CheckPotentialMatch(position, Vector2.down) || CheckPotentialMatch(next, Vector2.up)))
                {
                    return true;
                }
                next = new Vector2(x + 1, y);
                if (GetType(position) == GetType(next) && (CheckPotentialMatch(position, Vector2.left) || CheckPotentialMatch(next, Vector2.right)))
                {
                    return true;
                }
            }
        return false;
    }

    private bool IsValid(Vector2 arrayPosition)
    {
        return arrayPosition.y >= 0 && arrayPosition.y < height && arrayPosition.x >= 0 && arrayPosition.x < width;
    }

    Tile GetTile(Vector2 position)
    {
        return GetTile((int)position.x, (int)position.y);
    }
    Tile GetTile(int x, int y)
    {
        return tiles[x, y].GetComponent<Tile>();
    }
    private int GetType(Vector2 arrayPosition)
    {
        if (!IsValid(arrayPosition))
            return -1;
        return tiles[(int)arrayPosition.x, (int)arrayPosition.y].GetComponent<Tile>().tileType;
    }

    private Vector2 GetArrayPosition(Vector2 position)
    {
        return new Vector2((float)Math.Round(position.x),(float)Math.Round(position.y));
    }

    private bool CheckPotentialMatch(Vector2 direction, Vector2 position)
    {
        var type = GetType(position);

        Vector2 right = new Vector2(direction.y, direction.x);
        Vector2 left = -right;

        return GetType(position + direction) == type ||
            GetType(position + left) == type ||
            GetType(position + right) == type;
    }
    private bool CheckMatch(Vector2 direction, Vector2 position)
    {
        var type = GetType(position);
        if (type == -1)
        {
            return false;
        }

        Vector2 right = new Vector2(direction.y,direction.x);
        Vector2 left = -right;

        var matchByDirection = new HashSet<Vector2>();
        var matchPerpendicular = new HashSet<Vector2>();
        var i = position + direction;
        while (type == GetType(i))
        {
            matchByDirection.Add(i);
            i += direction;
        }
        i = position + right;
        while (type == GetType(i))
        {
            matchPerpendicular.Add(i);
            i += right;
        }
        i = position + left;
        while (type == GetType(i))
        {
            matchPerpendicular.Add(i);
            i += left;
        }

        bool found = false;

        if (matchByDirection.Count >= 2)
        {
            toBeDestroyed.UnionWith(matchByDirection);
            found = true;
        }

        if (matchPerpendicular.Count >= 2)
        {
            toBeDestroyed.UnionWith(matchPerpendicular);
            found = true;
        }

        if (found)
        {
            toBeDestroyed.Add(position);
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
