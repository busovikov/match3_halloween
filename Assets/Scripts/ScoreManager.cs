using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private static readonly string ScoreTotalString = "Score.Total";
    private static readonly string ScoreBestString = "Score.Best";
    private static readonly string TriggerName = "Combo";

    //private string[20] bo

    public GameObject comboBonus;
    private Text comboBonusVal;
    private Animator comboBonusAnimator;
    public ScoreUI scoreUI;

    [HideInInspector]
    public int current = 0;
    [HideInInspector]
    public int best = 0;
    [HideInInspector]
    public int total = 0;

    void Awake()
    {
        comboBonusVal = comboBonus.transform.Find("Val").GetComponent<Text>();
        comboBonusAnimator = comboBonus.GetComponent<Animator>();

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
        }
    }

    public void SetTotalScore()
    {
        scoreUI.Set(current);
        if (best < current)
        {
            best = current;
            PlayerPrefs.SetInt(ScoreBestString, total);
        }
        total += current;
        PlayerPrefs.SetInt(ScoreTotalString, total);
    }

    internal void AddCombo(int count)
    {
        if (count > 0)
        {
            comboBonusVal.text = (++count * 10).ToString();
            comboBonusAnimator.SetTrigger(TriggerName);
        }
    }
}
