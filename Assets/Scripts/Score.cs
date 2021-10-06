using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public float duration = 1f;
    private float score = 0;
    private Text element;

    void Awake()
    {
        element = GetComponent<Text>();
    }

    public void AddScore(int val)
    {
        StartCoroutine(Adding(val));
    }

    // Update is called once per frame
    IEnumerator Adding(int val)
    {
        //var buffer = 0;
        float remaining = val;
        while (remaining > 0)
        {
            float part = Mathf.Lerp(0, val, Time.deltaTime / duration );
            Debug.Log("Part: " + part);
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
            
            element.text = score.ToString("0");
            yield return null;
        }
    }
}
