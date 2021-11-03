using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public enum GameMode
    {
        Time,
        Moves
    }

    private Animator animator;

    public GameObject credits;
    public GameObject menu;
    public GameObject ghost;
    public GameObject endLevelPopup;
    public Text winLabel;
    public Text nextLabel;
    public GameMode mode = GameMode.Time;
    public float trnsactionTime = 1f;
    public int levelMoves = 0;
    public int levelTime = 0;
    public static LevelLoader Instance;
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            animator = GetComponent<Animator>();
        }
        else if (Instance != this)
        {
            // TODO: make endgame manager
            Instance.credits = credits;
            Instance.menu = menu;
            Instance.ghost = ghost;
            Instance.endLevelPopup = endLevelPopup;
            Instance.winLabel = winLabel;
            Instance.nextLabel = nextLabel;
            Instance.soundManager = soundManager;
            Instance.animator.Rebind();
            Instance.animator.ResetTrigger("Out");
            //Instance.animator.SetTrigger("Reset");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public static void Exit()
    {
        Application.Quit();
    }

    public static void ToCredits()
    {
        Instance.menu.SetActive(false);
        Instance.ghost.SetActive(false);
        Instance.credits.SetActive(true);
    }

    public static void BackToMainMenu()
    {
        Instance.menu.SetActive(true);
        Instance.ghost.SetActive(true);
        Instance.credits.SetActive(false);
    }

    public static int LevelBonus(int bonus)
    {
        if (Instance.mode == GameMode.Moves)
        {
            return (int)((bonus - 3)/1.5f);
        }
        return bonus - 3;
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
        Instance.StartCoroutine(Instance.MakeTransactionToQuickGame());
    }

    public static void GoToMainMenu()
    {
        Instance.StartCoroutine(Instance.MakeTransactionToMenu());
    }

    public static void EndLevel(bool win)
    {
        if (win)
        {
            Instance.winLabel.text = "You Win\nNext?";
            Instance.nextLabel.text = "Next";
        }
        else
        {
            Instance.winLabel.text = "Not This\nTime!";
            Instance.nextLabel.text = "Again";
        }

        Instance.endLevelPopup.SetActive(true);
        Instance.endLevelPopup.GetComponent<Animator>().SetTrigger("LevelEnd");
        Instance.soundManager.PlayPopupSound();
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
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
            }
        }
    }
}
