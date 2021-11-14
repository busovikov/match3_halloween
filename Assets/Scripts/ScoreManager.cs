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

        Config.LoadInt(ScoreBestString, out best, best);
        Config.LoadInt(ScoreTotalString, out total, total);
    }

    public void SubTotalScore(int val)
    {
        if (val > 0 && total >= val)
        {
            total -= val;
            Config.SaveInt(ScoreTotalString, total);
        }
    }

    public void AddScore(int val)
    {
        if (val > 0 )
        {
            current += val;
            scoreUI.Set(current);
        }
    }

    public void SetTotalScore()
    {
        if (best < current)
        {
            best = current;
            Config.SaveInt(ScoreBestString, total);
        }
        total += current;
        Config.SaveInt(ScoreTotalString, total);
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
