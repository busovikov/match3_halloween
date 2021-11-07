using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public float countDuration = 0.5f;
    private Text value;

    private void Awake()
    {
        value = GetComponent<Text>();
    }
    public void Set(int newvalue)
    {
        StopAllCoroutines();
        StartCoroutine(Count(int.Parse(value.text), newvalue)); 
    }

    IEnumerator Count(int from, int to)
    {
        float elapsed = 0f;


        while (elapsed < countDuration)
        {
            float val = Mathf.Lerp(from, to, elapsed / countDuration);
            value.text = ((int)val).ToString();
            elapsed += Time.deltaTime;
            yield return null;
        }

        value.text = to.ToString();
    }
}
