using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndLevel : MonoBehaviour
{

    public GameObject scoreObject;
    private Score score;

    public Boosters boostersObject;
    private Boosters boosters;

    private Text scoreLabel;
    private Text bestScoreLabel;
    private Text bestCombo;
    
    private Text boostersLabel;

    private void Awake()
    {
        score = scoreObject.GetComponent<Score>();
        boosters = boostersObject.GetComponent<Boosters>();

        scoreLabel = transform.Find("Score").GetComponent<Text>();
        bestScoreLabel = transform.Find("Best Score").GetComponent<Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
