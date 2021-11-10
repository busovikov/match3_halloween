using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [HideInInspector]
    public static ObjectPool Instance;

    private static readonly int amount = 10;
    private List<DeadObject>[] DeadPool;
    private List<GameObject>[] AlivePool;

    private int[] deadIndex;
    private int[] aliveIndex;

    public struct DeadObject
    {
        public GameObject obj;
        public Rigidbody2D body;
        public Animator anim;
    }

    public  GameObject[] deadTilePrefabs;
    public  GameObject[] tilePrefabs;
    void Awake()
    {
        Instance = FindObjectOfType<ObjectPool>();
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DeadPool = new List<DeadObject>[deadTilePrefabs.Length];
        AlivePool = new List<GameObject>[tilePrefabs.Length];
        deadIndex = new int[deadTilePrefabs.Length];
        aliveIndex = new int[tilePrefabs.Length];

        for (int i = 0; i < DeadPool.Length; i++)
        {
            DeadPool[i] = new List<DeadObject>(amount);
            for (int j = 0; j < amount; j++)
            {
                DeadObject dead;
                dead.obj = Instantiate(deadTilePrefabs[i]);
                dead.body = dead.obj.GetComponent<Rigidbody2D>();
                dead.anim = dead.obj.GetComponent<Animator>();
                dead.obj.SetActive(false);
                DeadPool[i].Add(dead);
            }
        }
        for (int i = 0; i < AlivePool.Length; i++)
        {
            AlivePool[i] = new List<GameObject>(amount);
            for (int j = 0; j < AlivePool.Length; j++)
            {
                GameObject o = Instantiate(tilePrefabs[i]);
                o.SetActive(false);
                AlivePool[i].Add(o);
            }
        }
    }

    public DeadObject GetDead(int type)
    {
        if (type < 0 && type > deadTilePrefabs.Length)
            return default(DeadObject);

        for (int i = 0; i < amount; i++)
        {
            int index = (amount + deadIndex[type]++) % amount;
            if (DeadPool[type][index].obj.activeInHierarchy == false)
            {
                return DeadPool[type][index];
            }
        }

        return default(DeadObject);
    }

    public GameObject GetAlive(int type)
    {
        if (type < 0 && type > tilePrefabs.Length)
            return null;

        for (int i = 0; i < amount; i++)
        {
            int index = (amount + aliveIndex[type]++) % amount;
            if (AlivePool[type][index].activeInHierarchy == false)
            {
                return AlivePool[type][index];
            }
        }

        return null;
    }
}
