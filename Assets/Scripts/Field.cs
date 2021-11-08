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

    private Goals goals;
    private TileMap tileMap;
    private Match match;
    private UIManager uiManager;
    private ScoreManager score;
    private TimeAndMoves timeAndMoves;
    private SoundManager soundManager;
    private Boosters boosters;
    private int comboCount = -1;
    private int processing = 0;
    private bool dirty = false;
    private bool actionAllowed = true;
    private bool rowDestoy = false;

    private class ToSwap
    {
        public ToSwap(Vector2 f, Vector2 s) 
        {
            first = f;
            second = s;
        }
        public Vector2 first;
        public Vector2 second;
    }

    private ToSwap toSwap;

    // Start is called before the first frame update
    void Awake()
    {
        boosters = FindObjectOfType<Boosters>();
        boosters.InitBoosters(ActivateBooster);
        uiManager = FindObjectOfType<UIManager>();
        soundManager = FindObjectOfType<SoundManager>();
        tileMap = GetComponent<TileMap>();
        goals = tileMap.goal.GetComponent<Goals>();
        match = new Match(tileMap);

        score = FindObjectOfType<ScoreManager>();
        timeAndMoves = FindObjectOfType<TimeAndMoves>();

        collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(tileMap.width, tileMap.height);
        collider.offset = collider.size / 2 - Vector2.one / 2;
    }

    private void Start()
    {
        var movesOrTime = goals.GetGoalForGameMode(LevelLoader.Instance.mode);
        if (LevelLoader.Instance.mode == LevelLoader.GameMode.Moves)
        {
            timeAndMoves.StartAsMoves(LevelLoader.Instance.levelMoves > 0 ? LevelLoader.Instance.levelMoves : movesOrTime);
        }
        else
        {
            timeAndMoves.StartAsSeconds(LevelLoader.Instance.levelTime > 0 ? LevelLoader.Instance.levelTime : movesOrTime);
        }
    }

    private void Update()
    {
        
        if (dirty)
        {
            dirty = false;
            HashSet<Vector2> destroy;
            if (match.IsAny(out destroy))
            {
                StartCoroutine(Processing(destroy));
            }
            else
            {
                if (comboCount >= 0)
                {
                    score.AddCombo(comboCount);
                    score.AddScore(10 * comboCount);
                    comboCount = -1;
                }

                if (match.SwapsAvailable() == false)
                {
                    uiManager.ShowNoMatches();
                    StartCoroutine(Shuffeling());
                }
            }

            if (toSwap != null)
            {
                if (destroy != null && (destroy.Contains(toSwap.first) || destroy.Contains(toSwap.second)))
                {
                    if (LevelLoader.Instance.mode == LevelLoader.GameMode.Moves)
                    {
                        timeAndMoves.Sub(1);
                    }
                }
                else 
                {
                    tileMap.GetTile(toSwap.first).ExchangeWith(tileMap.GetTile(toSwap.second), null);
                }
                toSwap = null;
            }
        }

        if ((goals.reached || !timeAndMoves.Check() ) && processing == 0)
        {
            processing++;
            if (goals.reached)
                score.SetTotalScore();
            else
                score.current = 0;
            LevelLoader.EndLevel(goals.reached);
        }
    }

    private IEnumerator Shuffeling(bool once = false)
    {
        if (once)
        {
            yield return Reshuffle();
            dirty = true;
        }
        else
        {
            HashSet<Vector2> destroy;
            while (match.IsAny(out destroy) || match.SwapsAvailable() == false)
            {
                yield return Reshuffle();
            }
        }
    }
    public void ActivateBooster(Boosters.BoosterType booster)
    {
        if (booster == Boosters.BoosterType.Mix)
        {
            StartCoroutine(Shuffeling(true)); // true - Once
        }
        else if (booster == Boosters.BoosterType.Erase)
        {
            rowDestoy = true;
        }
        else if (booster == Boosters.BoosterType.Add)
        {
            if (LevelLoader.Instance.mode == LevelLoader.GameMode.Moves)
            {
                timeAndMoves.Add(2);
            }
            else
            {
                timeAndMoves.Add(5);
            }
        }
    }

    private void Swap(Vector2 first, Vector2 second)
    {
        if (!tileMap.IsValid(first) || !tileMap.IsValid(second))
            return;
       
        tileMap.GetTile(first).ExchangeWith(tileMap.GetTile(second), () => { toSwap = new ToSwap(first, second); dirty = true; });
    }
    private void SetPosition(Vector2 position)
    {
        Vector2 offsetPosition = ToField(position - (Vector2)collider.transform.position);
        if (actionAllowed && firstPosition != offsetPosition)
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

        HashSet<Vector2> row = new HashSet<Vector2>(); 
        for (int x = 0; x < tileMap.width; x++)
        {
            Vector2 pos = new Vector2(x, position.y);
            tileMap.GetTile(pos).invalid = true;
            row.Add(pos);
        }

        StartCoroutine(Processing(row));
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
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        actionAllowed = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        SetPosition(eventData.pointerCurrentRaycast.worldPosition);
    }
    private IEnumerator Processing(HashSet<Vector2> destroy)
    {
        if (destroy == null || destroy.Count == 0)
            yield break;

        processing++;
        comboCount++;
        foreach (var p in destroy)
        {
            Tile item = tileMap.GetTile(p);
            tileMap.SpawnDead(item.tileType, item.transform);
            item.DestroyContent();
        }
        
        if (destroy.Count > 0)
        {
            int currentScore = Enumerable.Range(0, destroy.Count - 3).Select((index) => index).Sum() + destroy.Count;
            score.AddScore(currentScore);
            soundManager.PlayPop();
        }

        yield return new WaitForSeconds(0.2f);

        List<Coroutine> dropAll = new List<Coroutine>();
        for (int x = 0; x < tileMap.width; x++)
        {
                dropAll.Add(StartCoroutine(DropAndReplaceWithNew(x)));
        }

        foreach (Coroutine dropTask in dropAll)
        {
            yield return dropTask;
        }
        
        processing--;
        dirty = true;
    }

    
    private IEnumerator Reshuffle()
    {
        int shuffeling = 0;
        var until = new WaitUntil(() => shuffeling == 0);
        List<int> axis_y = Enumerable.Range(0, tileMap.height + 1).Select((index) => index - 1).ToList();
        List<List<int>> remainingPositions = new List<List<int>>();

        for (int i = 0; i < tileMap.width; i++)
        {
            axis_y[0] = i;
            remainingPositions.Add(new List<int>(axis_y));
        }

        int swap_x = 0;
        int swap_y = 0;
        bool swap = false;

        for (int i = 0; i <  tileMap.width * tileMap.height; i++)
        {
            int index_x = UnityEngine.Random.Range(0, remainingPositions.Count);
            int index_y = UnityEngine.Random.Range(1, remainingPositions[index_x].Count);
            int new_y = remainingPositions[index_x][index_y];
            int new_x = remainingPositions[index_x][0];
            remainingPositions[index_x].RemoveAt(index_y);
            if (remainingPositions[index_x].Count == 1)
            {
                remainingPositions.RemoveAt(index_x);
            }
            
            if (swap)
            {
                shuffeling++;
                if (!tileMap.GetTile(swap_x, swap_y).ExchangeWith(tileMap.GetTile(new_x, new_y), () => { shuffeling--;  }))
                {
                    continue;
                }
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
            }
        }

        for (int i = 0; i < destroyedCount; i++)
        {
            var position = new Vector2(x, tileMap.height - destroyedCount + i);
            var offset = new Vector2(0, destroyedCount);
            bool dropped = true;
            animations.Add(tileMap.Create(position, offset, dropped));
        }

        // Wait for all animations are done
        foreach (Coroutine animation in animations)
        {
            yield return animation;
        }
    }

    public void ShowSelection(Vector3 position)
    {
        selection.SetActive(true);
        selection.transform.position = position + transform.position;
    }

    public void HideSelection()
    {
        selection.SetActive(false);
    }
}