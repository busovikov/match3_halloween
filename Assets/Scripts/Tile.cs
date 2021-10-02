using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour
{
    public GameObject content;
    public int tileType;
    public void ExchangeWith(Tile other, Action onExchanged)
    {
        if (content == null || other.content == null)
            return;

        var tmp = other.content;
        other.content = this.content;
        content = tmp;

        content.transform.SetParent(transform);
        other.content.transform.SetParent(other.transform);

        var type = other.tileType;
        other.tileType = tileType;
        tileType = type;

        StartCoroutine(SyncContent(other, onExchanged));
    }
    public IEnumerator SyncContent(Tile other, Action onExchanged)
    {
        var until = new WaitUntil(() => IsSet() && other.IsSet());
        StartCoroutine(SwapAnimation());
        StartCoroutine(other.SwapAnimation());
        yield return until;
        if (onExchanged != null)
        {
            onExchanged();
        }
    }
    public bool IsSet()
    {
        return transform.position == content.transform.position;
    }
    public IEnumerator SwapAnimation()
    {
        float elapsed = 0f;
        var InitialOffset = content.transform.position;
        float path = (InitialOffset - transform.position).magnitude / 10;
        do
        {
            yield return null;
            elapsed += Time.deltaTime;
            var weight = Math.Min(1, elapsed / 0.2f);
            content.transform.position = Vector3.Lerp(InitialOffset, transform.position, weight);
        } while (!IsSet());
    }
    public void DestroyContent()
    {
        StopCoroutine(SwapAnimation());
        tileType = 0;
        Destroy(content);
        content = null;
    }
    public Coroutine DropTo(Tile tileToDrop)
    {
        if (content != null)
        {
            tileToDrop.content = content;
            tileToDrop.tileType = tileType;
            tileToDrop.content.transform.SetParent(tileToDrop.transform);

            content = null;
            tileType = 0;
            return StartCoroutine(tileToDrop.SwapAnimation());
        }
        return null;
    }
    public Coroutine CreateContent( GameObject prefab, int index, Vector3 offset = default)
    {
        content = Instantiate(prefab, transform.position + offset, Quaternion.identity, transform);
        tileType = index;
        return StartCoroutine(SwapAnimation());
    }
}
