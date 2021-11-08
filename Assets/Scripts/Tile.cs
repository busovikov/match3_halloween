using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour
{
    [SerializeField]
    public GameObject container;
    public GameObject content;
    public int tileType;
    public bool invalid = false;

    private Queue<string> lastaction;

    public void AddAction(string action)
    {
        lastaction.Enqueue(action);
        if (lastaction.Count > 20)
            lastaction.Dequeue();
    }

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        lastaction = new Queue<string>(10);
}

    private void Update()
    {
        if (invalid && content != null)
            content.GetComponent<SpriteRenderer>().color = Color.red;
        else if (content != null)
            content.GetComponent<SpriteRenderer>().color = Color.white;
    }
    public void ExchangeWith(Tile other, Action onExchanged)
    {
        if (content == null || other.content == null)
            return;

        other.invalid = true;
        invalid = true;

        AddAction("ExchangeWith Invalid = true");
        var tmp = other.content;
        other.content = this.content;
        content = tmp;

        content.transform.SetParent(container.transform);
        other.content.transform.SetParent(other.container.transform);

        var type = other.tileType;
        other.tileType = tileType;
        tileType = type;

        StartCoroutine(SyncContent(other, onExchanged));
    }
    public IEnumerator SyncContent(Tile other, Action onExchanged)
    {
        AddAction("SyncContent");
        Coroutine first =  StartCoroutine(SwapAnimation());
        Coroutine second = StartCoroutine(other.SwapAnimation());
        
        yield return first;
        yield return second;

        if (onExchanged != null)
        {
            onExchanged();
        }

        invalid = false;
        other.invalid = false;
        AddAction("SwapAnimation Invalid = false");
    }
    public bool IsSet()
    {
        return content != null && container.transform.position == content.transform.position;
    }
    public IEnumerator SwapAnimation(bool dropped = false)
    {
        float elapsed = 0f;
        var InitialOffset = content.transform.position;
        float path = (InitialOffset - container.transform.position).magnitude / 10;
        
        do
        {
            yield return null;
            elapsed += Time.deltaTime;
            var weight = Math.Min(1, elapsed / 0.2f);
            try
            {
                content.transform.position = Vector3.Lerp(InitialOffset, container.transform.position, weight);
            }
            catch
            {
                Debug.Log("Catch " + name);
            }
        } while (!IsSet());

        if (dropped)
        {
            yield return StartCoroutine(WaitDroppedAnimation());
            invalid = false;
        }
        
    }
    public void DestroyContent()
    {
        AddAction("DestroyContent");
        StopCoroutine(SwapAnimation());
        tileType = 0;
        Destroy(content);
        content = null;
    }

    public IEnumerator WaitDroppedAnimation()
    {
        animator.SetTrigger("Dropped");
        yield return new WaitForSeconds(1f/5);
        //yield return null;
    }
    public Coroutine DropTo(Tile tileToDrop)
    {
        if (content != null)
        {
            StopCoroutine(SwapAnimation());
            tileToDrop.StopCoroutine(SwapAnimation());
            tileToDrop.invalid = true;
            tileToDrop.content = content;
            tileToDrop.tileType = tileType;
            tileToDrop.content.transform.SetParent(tileToDrop.container.transform);

            content = null;
            tileType = 0;
            AddAction("DropTo");
            return StartCoroutine(tileToDrop.SwapAnimation(true));
        }
        return null;
    }
    public Coroutine CreateContent( GameObject prefab, int index, bool dropped, Vector3 offset = default)
    {
        content = Instantiate(prefab, container.transform.position + offset, Quaternion.identity, container.transform);
        tileType = index;
        
        if (dropped)
        {
            invalid = true;
            AddAction("CreateContent");
            return StartCoroutine(SwapAnimation(dropped));
        }
        return null;
    }


}
