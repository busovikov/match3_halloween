using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class Boosters : MonoBehaviour
{
    public enum BoosterType
    {
        Mix,
        Row,
        Plus
    }

    [System.Serializable]
    public class Booster
    {
        public static readonly int max = 99;
        [HideInInspector]
        public Button btn;
        [HideInInspector]
        public Text label;
        [HideInInspector]
        public Animator animation;
        public BoosterType type;
        public int amount;
        public float cooldown = 10;

        public delegate void ActivateBoosterCallback(BoosterType booster);

        private float timer = 0;
        private ActivateBoosterCallback activate;

        public void Fill(Transform child, ActivateBoosterCallback cb)
        {
            btn = child.GetComponent<Button>();
            animation = child.GetComponent<Animator>();
            activate = cb;
            btn.onClick.AddListener(delegate (){
                if (amount > 0)
                {
                    timer = cooldown;
                    Dec();
                    activate(type);
                }
            });
            label = btn.transform.GetChild(0).GetComponent<Text>();
        }

        public void Update()
        {
            if (label != null)
            {
                label.text = amount.ToString();
                if (timer <= 0)
                {
                    btn.interactable = amount > 0;
                }
                else
                {
                    btn.interactable = false;
                    timer -= Time.deltaTime;
                }
            }
        }

        private void Inc()
        {
            if (amount < max)
            {
                amount++;
                animation.SetTrigger("Add");
            }
        }

        private void Dec()
        {
            if (amount > 0)
            {
                amount--;
            }
        }

        private void Mul(int val)
        {
            if (amount > 0)
            {
                amount = amount * val;
            }
            else
            {
                amount = 1;
            }
            animation.SetTrigger("Add");
        }
        public static Booster operator ++(Booster b)
        {
            b.Inc();
            return b;
        }

        public static Booster operator --(Booster b)
        {
            b.Dec();
            return b;
        }

        public static Booster operator *(Booster b, int val)
        {
            b.Mul(val);
            return b;
        }

    }

    public Booster[] boosters;
    Dictionary<BoosterType, int> lookupTable = new Dictionary<BoosterType, int>();

    public void InitBoosters(Booster.ActivateBoosterCallback cb)
    {
        int size = Mathf.Max(transform.childCount, boosters.Length);
        for (int i = 0; i < size; i++)
        {
            boosters[i].Fill(transform.GetChild(i), cb);
            lookupTable[boosters[i].type] = i;
        }
    }

    public void AddBonus(int val)
    {
        if (val < 5)
        {
            return;
        }
        else if (val < 9)
        {
            for (int i = 6; i <= val; i++) // 3 - 8
            {
                boosters[Random.Range(0, boosters.Length)]++;
            }
        }
        else
        {
            for (int i = 0; i < boosters.Length; i++) { _ = boosters[i] * (val - 8); }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < boosters.Length; i++)
        {
            boosters[i].Update();
        }
    }
}
