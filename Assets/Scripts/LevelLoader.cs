using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public enum GameMode
    {
        Time,
        Moves
    }

    private Animator animator;

    public GameObject endLevelPopup;
    public GameMode mode = GameMode.Time;
    public float trnsactionTime = 1f;
    public int levelMoves = 0;
    public int levelTime = 0;
    public static LevelLoader Instance;

    void Awake()
    {
        Debug.Log("Scene " + endLevelPopup.GetHashCode().ToString());
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            animator = GetComponent<Animator>();
        }
        else if (Instance != this)
        {
            Instance.endLevelPopup = endLevelPopup;
            Instance.animator.Rebind();
            Instance.animator.ResetTrigger("Out");
            //Instance.animator.SetTrigger("Reset");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public static void StartGameWithMoves()
    {
        Instance.mode = GameMode.Moves;
        Instance.StartCoroutine(Instance.MakeTransactionToQuickGame());
    }

    public static void StartGameWithTime()
    {
        Instance.mode = GameMode.Time;
        Instance.StartCoroutine(Instance.MakeTransactionToQuickGame());
    }

    public static void RepeatLevel()
    {
        Debug.Log("Repeat");
        Instance.StartCoroutine(Instance.MakeTransactionToQuickGame());
    }

    public static void GoToMainMenu()
    {
        Instance.StartCoroutine(Instance.MakeTransactionToMenu());
    }

    public static void EndLevel()
    {
         Instance.endLevelPopup.SetActive(true);
         Instance.endLevelPopup.GetComponent<Animator>().SetTrigger("LevelEnd");
    }

    IEnumerator MakeTransactionToQuickGame()
    {
        animator.SetTrigger("Out");

        yield return new WaitForSeconds(trnsactionTime);

        SceneManager.LoadScene("Quick Game");
    }

    IEnumerator MakeTransactionToMenu()
    {
        animator.SetTrigger("Out");

        yield return new WaitForSeconds(trnsactionTime);

        SceneManager.LoadScene("Main Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
