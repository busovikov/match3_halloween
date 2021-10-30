using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
public class Field : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public GameObject selection;

    private new BoxCollider2D collider;
    private static Vector2 invalidPosition = Vector2.left;
    private Vector2 firstPosition = invalidPosition;
    private bool processing = false;
    private bool actionAllowed = true;
    private bool reshuffleRequest = false;
    private bool rowDestoy = false;

    private TileMap tileMap;
    private Match match;
    private Score score;
    private TimeAndMoves timeAndMoves;
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        tileMap = GetComponent<TileMap>();
        match = new Match(tileMap);

        score = FindObjectOfType<Score>();
        timeAndMoves = FindObjectOfType<TimeAndMoves>();

        collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(tileMap.width, tileMap.height);
        collider.offset = collider.size / 2 - Vector2.one / 2;
        //Camera.main.transform.position += (Vector3)collider.offset;
        //Camera.main.orthographicSize = Math.Max(tileMap.width, (float)tileMap.height / 2) + 1;
    }

    private void Start()
    {
        if (LevelLoader.Instance.mode == LevelLoader.GameMode.Moves)
        {
            timeAndMoves.StartAsMoves(LevelLoader.Instance.levelMoves);
        }
        else
        {
            timeAndMoves.StartAsSeconds(LevelLoader.Instance.levelTime);
        }
        StartCoroutine(ProcessingOnStart());
    }

    public void ActivateBooster(Boosters.BoosterType booster)
    {
        Debug.Log("Booster activated " + booster.ToString());
        if (booster == Boosters.BoosterType.Mix)
        {
            reshuffleRequest = true;
            if (!processing)
            {
                StartCoroutine(Processing());
            }
        }
        else if (booster == Boosters.BoosterType.Row)
        {
            rowDestoy = true;
        }
        else if (booster == Boosters.BoosterType.Plus)
        {

        }
    }

    private void Swap(Vector2 first, Vector2 second)
    {
        if (!tileMap.IsValid(first) || !tileMap.IsValid(second))
            return;
        var one = tileMap.GetTile(first);
        var two = tileMap.GetTile(second);

        one.ExchangeWith(two, () => 
        {
            if (match.IsAny())
            {
                StartCoroutine(Processing());
                timeAndMoves.Sub(1);
            }
            else
                one.ExchangeWith(two, null);
        });
    }
    private void SetPosition(Vector2 position)
    {
        Vector2 offsetPosition = ToField(position - (Vector2)collider.transform.position);
        if (!processing && actionAllowed && firstPosition != offsetPosition)
        {
            if (rowDestoy)
            {
                actionAllowed = false;
                HideSelection();
                DestroyRow(offsetPosition);
                rowDestoy = false;
            }
            else if (firstPosition != invalidPosition && IsNeighbours(firstPosition, offsetPosition))
            {
                actionAllowed = false;
                HideSelection();
                Swap(firstPosition, offsetPosition);
                firstPosition = invalidPosition;
            }
            else
            {
                firstPosition = offsetPosition;
                ShowSelection(firstPosition);
            }
        }
    }

    private void DestroyRow(Vector2 position)
    {
        if (!tileMap.IsValid(position))
            return;

        match.SetRowForDestruction((int)position.y);
        StartCoroutine(Processing());
    }

    private Vector2 ToField(Vector2 position)
    {
        return new Vector2((float)Math.Round(position.x), (float)Math.Round(position.y));
    }
    private bool IsNeighbours(Vector2 firstPosition, Vector2 secondPosition)
    {
        var xOffset = Math.Abs(firstPosition.x - secondPosition.x);
        var yOffset = Math.Abs(firstPosition.y - secondPosition.y);

        return xOffset == 1 && yOffset == 0 || xOffset == 0 && yOffset == 1;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        SetPosition(eventData.pointerCurrentRaycast.worldPosition);
        //SetPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        actionAllowed = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        SetPosition(eventData.pointerCurrentRaycast.worldPosition);
        //SetPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    private IEnumerator ProcessingOnStart()
    {
        processing = true;
        yield return new WaitForSeconds(0.5f);
        while (match.IsAny() || match.SwapsAvailable() == false)
        {
            match.ExecuteAndClear(null);
            yield return Reshuffle();
        }
        processing = false;
    }
    private IEnumerator Processing()
    {
        processing = true;

        int comboCount = 0;
        while (match.IsAny() || match.SwapsAvailable() == false || reshuffleRequest)
        {
            comboCount++;
            bool destroyed = match.ExecuteAndClear((item) =>
            {
                tileMap.SpawnDead(item.tileType, item.transform);
                item.DestroyContent();
                score.AddScore(1);
            });

            if (destroyed)
            {
                soundManager.PlayPop();
            }

            yield return new WaitForSeconds(0.3f);

            List<Coroutine> dropAll = new List<Coroutine>();
            for (int x = 0; x < tileMap.width; x++)
            {
                 dropAll.Add(StartCoroutine(DropAndReplaceWithNew(x)));
            }

            foreach (Coroutine dropTask in dropAll)
            {
                yield return dropTask;
            }

            while (match.IsAny() == false && match.SwapsAvailable() == false || reshuffleRequest )
            {
                reshuffleRequest = false;
                yield return Reshuffle();
            }
        }

        score.AddCombo(--comboCount);
        processing = false;
    }
    private IEnumerator Reshuffle()
    {
        int processing = 0;
        var until = new WaitUntil(() => processing == 0);
        List<int> axis_y = Enumerable.Range(0, tileMap.height).Select((index) => index).ToList();
        List<List<int>> remainingPositions = new List<List<int>>();

        for (int i = 0; i < tileMap.width; i++)
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
                tileMap.GetTile(swap_x, swap_y).ExchangeWith(tileMap.GetTile(new_x, new_y), ()=> { processing--; });
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
    private IEnumerator DropAndReplaceWithNew(int x)
    {
        List<Coroutine> animations = new List<Coroutine>();
        int destroyedCount = 0;

        for (int y = 0; y < tileMap.height; y++)
        {
            Tile tile = tileMap.GetTile(x, y);
            if (tile.content == null)
            {
                destroyedCount++;
            }
            else if (destroyedCount > 0)
            {
                Tile bottom = tileMap.GetTile(x, y - destroyedCount);
                animations.Add(tile.DropTo(bottom));
                yield return null;
            }
        }

        for (int i = 0; i < destroyedCount; i++)
        {
            var position = new Vector2(x, tileMap.height - destroyedCount + i);
            var offset = new Vector2(0, destroyedCount);
            bool dropped = true;
            animations.Add(tileMap.Create(position, offset, dropped));
            yield return null;
        }

        // Wait for all animations are done
        foreach (Coroutine animation in animations)
        {
            yield return animation;
        }

        yield return new WaitForSeconds(.2f); 
    }

    public void ShowSelection(Vector2 position)
    {
        selection.SetActive(true);
        selection.transform.position = position;
    }

    public void HideSelection()
    {
        selection.SetActive(false);
    }
}