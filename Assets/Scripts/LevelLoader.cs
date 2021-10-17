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
    public static LevelLoader Instance;

    void Awake()
    {
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

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartGameWithMoves()
    {
        mode = GameMode.Moves;
        StartCoroutine(MakeTransactionToQuickGame());
    }

    public void StartGameWithTime()
    {
        mode = GameMode.Time;
        StartCoroutine(MakeTransactionToQuickGame());
    }

    public void RepeatLevel()
    {
        StartCoroutine(MakeTransactionToQuickGame());
    }

    public void GoToMainMenu()
    {
        StartCoroutine(MakeTransactionToMenu());
    }

    public void EndLevel()
    {
        endLevelPopup.SetActive(true);
        endLevelPopup.GetComponent<Animator>().SetTrigger("LevelEnd");
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
