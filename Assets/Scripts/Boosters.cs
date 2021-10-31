using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class Boosters : MonoBehaviour
{
    public Field listener;
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
        public BoosterType type;
        public int amount;
        public float cooldown = 10;

        public delegate void ActivateBoosterCallback(BoosterType booster);

        private float timer = 0;
        private ActivateBoosterCallback activate;

        public void Fill(Transform child, ActivateBoosterCallback cb)
        {
            btn = child.GetComponent<Button>();
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
            }
        }

        private void Dec()
        {
            if (amount > 0)
            {
                amount--;
            }
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

        
    }

    public Booster[] boosters;

    private void Awake()
    {
        int size = Mathf.Max(transform.childCount, boosters.Length);
        for (int i = 0; i < size; i++)
        {
            boosters[i].Fill(transform.GetChild(i), listener.ActivateBooster);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
