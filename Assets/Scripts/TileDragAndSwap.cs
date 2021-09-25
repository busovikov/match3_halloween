using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDragAndSwap : MonoBehaviour
{
    [SerializeField]
    public float SpeedOfSwap = 0.5f;
    private SpriteRenderer spriteRenderer;
    private Vector3 savedPosition;
    private Vector3 targetPosition;
    private Vector3 mousePosition;
    private Color savedColor;
    private TileDragAndSwap objectForSwap;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        saveState();
    }

    private void OnMouseDown()
    {
        saveState();
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spriteRenderer.color = Color.blue;
    }

    private void OnMouseUp()
    {
        if (transform.position != savedPosition && objectForSwap != null)
        {
            Swap(objectForSwap);
        }
        spriteRenderer.color = savedColor;
    }

    private void saveState()
    {
        savedPosition = transform.position;
        savedColor = spriteRenderer.color;
    }

    private void OnMouseDrag()
    {
        var currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position += (currentMousePosition - mousePosition); 
        mousePosition = currentMousePosition;
    }
    
    public void Swap(TileDragAndSwap obj)
    {
        obj.targetPosition = savedPosition;
        obj.savedPosition = obj.transform.position;
        savedPosition = transform.position;
        targetPosition = obj.transform.position;
        StartCoroutine(SwapAnimation());
        StartCoroutine(obj.SwapAnimation());
    }

    public IEnumerator SwapAnimation()
    {
        float elapsed = 0f;

        while (transform.position != targetPosition)
        {
            elapsed += Time.deltaTime;
            var weight = Math.Min(1, elapsed / SpeedOfSwap);
            transform.position = Vector3.Lerp(savedPosition, targetPosition, weight);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectForSwap = collision.gameObject.GetComponent<TileDragAndSwap>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectForSwap = null;
    }
   
}
