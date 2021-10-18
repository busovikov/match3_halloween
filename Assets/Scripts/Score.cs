using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public float duration = .3f;
    private float score = 0;
    private int comboCount = 0;
    private int bestCombo = 0;
    float remaining = 0;
    public Text[] elements;
    public Text comboLabel;
    public Text bestComboLabel;

    void Awake()
    {
    }

    public void AddScore(int val)
    {
        remaining += val;
    }

    // Update is called once per frame
    private void Update()
    {
        if (remaining > 0)
        {
            float part = Time.deltaTime * 5;
            if (remaining <= part)
            {
                score += remaining;
                remaining = 0;
            }
            else
            {
                score += part;
                remaining -= part;
            }

            foreach (var element in elements)
            {
                element.text = score.ToString("0");
            }
        }
    }

    internal void AddCombo(int count)
    {
        if (count > 1)
        {
            comboCount++;
            if (count > bestCombo)
                bestCombo = count;
            comboLabel.text = comboCount.ToString();
            bestComboLabel.text = bestCombo.ToString();
        }
    }
}
