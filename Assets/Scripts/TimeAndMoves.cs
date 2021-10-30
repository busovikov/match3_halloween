using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeAndMoves : MonoBehaviour
{
    private Text stringValue;
    private int value = 0;
    private float accumulator = 0;
    private bool running = false;

    public Text label;
    public GameObject bonus;

    // Start is called before the first frame update
    void Awake()
    {
        stringValue = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running && value <= 0)
        {
            running = false;
            LevelLoader.EndLevel();
        }
    }

    public void Add(int val)
    {
        value += val;
        stringValue.text = value.ToString();
        var newbonus = Instantiate(bonus, bonus.transform.parent);
        newbonus.GetComponent <Text>().text = "+" + val.ToString();
        newbonus.GetComponent<Animator>().SetTrigger("Bonus");
        Destroy(newbonus, 1.1f);
    }

    public void Sub(int val)
    {
        value -= val;
        stringValue.text = value.ToString();
    }

    public void StartAsMoves(int moves)
    {
        value = moves;
        label.text = "Moves";
        stringValue.text = value.ToString();
        running = true;
    }

    public void StartAsSeconds(int seconds)
    {
        value = seconds;
        label.text = "Time";
        stringValue.text = value.ToString();
        running = true;
        StartCoroutine(StartCountDown());
    }

    private IEnumerator StartCountDown()
    {
        while (value > 0)
        {
            accumulator += Time.deltaTime;
            if (accumulator >= 1)
            {
                accumulator -= 1;
                value--;
                stringValue.text = value.ToString();
            }

            yield return null;
        }
    }
}
