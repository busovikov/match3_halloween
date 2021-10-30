using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public float duration = 1f;
    private int score = 0;
    private int comboCount = 0;
    
    private int bestScore = 0;
    private int totalScore = 0;
    private int bestCombo = 0;
    private int bestMatch = 0;

    float remaining = 0;
    float elapsed = 0;

    public Text comboBonus;
    public Text[] elements;
    public Text bestScoreLabel;
    public Text maxMatchLabel;
    public Text bestComboLabel;
    public Text totalScoreLabel;

    void Awake()
    {
        if (PlayerPrefs.HasKey("bestScore"))
        {
            bestScore = PlayerPrefs.GetInt("bestScore");
            totalScore = PlayerPrefs.GetInt("totalScore");
            //bestCombo = PlayerPrefs.GetInt("bestCombo");
            //bestMatch = PlayerPrefs.GetInt("bestMatch");
        }
    }

    public void AddScore(int val)
    {
        score += val;
        foreach (var element in elements)
        {
            element.text = score.ToString("0");
        }
    }

    public void SetTotalScore()
    {
        if (bestScore < score)
        {
            bestScore = score;
            PlayerPrefs.SetInt("bestScore", totalScore);
        }
        totalScore += score;
        PlayerPrefs.SetInt("totalScore", totalScore);

        bestScoreLabel.text = bestScore.ToString();
        totalScoreLabel.text = totalScore.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void AddDestroyed(int val)
    {
        if (val > bestMatch)
        {
            bestMatch = val;
            maxMatchLabel.text = comboCount.ToString();
        }
    }
    internal void AddCombo(int count)
    {
        if (count > 1)
        {
            comboCount++;
            if (count > bestCombo)
            {
                bestCombo = count;
                bestComboLabel.text = bestCombo.ToString();
            }

            var newbonus = Instantiate(comboBonus, comboBonus.transform.parent);
            newbonus.GetComponent<Text>().text = "Combo x " + count.ToString();
            newbonus.GetComponent<Animator>().SetTrigger("Bonus");
            Destroy(newbonus, 1.1f);
        }
    }
}
