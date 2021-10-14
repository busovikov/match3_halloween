using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private Animator animator;
    public float trnsactionTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartQuickGame()
    {
         StartCoroutine(MakeTransactionToQuickGame());
        
    }

    IEnumerator MakeTransactionToQuickGame()
    {
        animator.SetTrigger("Out");

        yield return new WaitForSeconds(trnsactionTime);

        SceneManager.LoadScene("Quick Game");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
