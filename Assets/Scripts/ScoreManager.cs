using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    static readonly string ScoreTotalString = "Score.Total";
    static readonly string ScoreBestString = "Score.Best";
    public GameObject comboBonus;
    public Text scoreUI;

    [HideInInspector]
    public int current = 0;
    [HideInInspector]
    public int best = 0;
    [HideInInspector]
    public int total = 0;

    void Awake()
    {
        if (PlayerPrefs.HasKey(ScoreTotalString))
        {
            best = PlayerPrefs.GetInt(ScoreBestString);
            total = PlayerPrefs.GetInt(ScoreTotalString);
        }
    }

    public void SubTotalScore(int val)
    {
        if (val > 0 && total >= val)
        {
            total -= val;
            PlayerPrefs.SetInt(ScoreTotalString, total);
        }
    }

    public void AddScore(int val)
    {
        if (val > 0 )
        {
            current += val;
            scoreUI.GetComponent<ScoreUI>().Set(current);
        }
    }

    public void SetTotalScore()
    {
        if (best < current)
        {
            best = current;
            PlayerPrefs.SetInt(ScoreBestString, total);
        }
        total += current;
        PlayerPrefs.SetInt(ScoreTotalString, total);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    internal void AddCombo(int count)
    {
        if (count > 0)
        {
            var newbonus = Instantiate(comboBonus, comboBonus.transform.parent);
            newbonus.GetComponent<Text>().text = "Combo! +" + (++count).ToString();
            newbonus.GetComponent<Animator>().SetTrigger("Bonus");
            Destroy(newbonus, 1.1f);
        }
    }
}
