using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public float duration = .3f;
    private float score = 0;
    float remaining = 0;
    public Text[] elements;

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
}
