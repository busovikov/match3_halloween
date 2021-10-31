using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeAndMoves : MonoBehaviour
{
    private Text stringValue;
    private float accumulator = 0;

    [HideInInspector]
    public int value = 0;
    [HideInInspector]
    public bool running = false;

    public Text label;
    public GameObject bonus;

    // Start is called before the first frame update
    void Awake()
    {
        stringValue = GetComponent<Text>();
    }

     public bool Check()
    {
        if (running && value > 0)
        {
            accumulator += Time.deltaTime;
            if (accumulator >= 1)
            {
                accumulator -= 1;
                Sub(1);
            }
        }
        return value > 0;
    }

    public void Add(int val)
    {
        value += val;
        stringValue.text = value.ToString();
        var newbonus = Instantiate(bonus, bonus.transform.parent);
        newbonus.GetComponent <Text>().text = label.text + " +" + val.ToString();
        newbonus.GetComponent<Animator>().SetTrigger("Bonus");
        Destroy(newbonus, 1.1f);
    }

    public void Sub(int val)
    {
        value -= val;
        stringValue.text = value.ToString();
        if (running && value <= 0)
        {
            running = false;
        }
    }

    public void StartAsMoves(int moves)
    {
        value = moves;
        label.text = "Moves";
        stringValue.text = value.ToString();
    }

    public void StartAsSeconds(int seconds)
    {
        value = seconds;
        label.text = "Time";
        stringValue.text = value.ToString();
        running = true;
    }

    
}
