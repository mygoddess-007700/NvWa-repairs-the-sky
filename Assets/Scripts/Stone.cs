using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stone : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public int num;
    public Vector2 initPosition;
    public Vector2 dragPosition;
    public bool dragging = false;
    public bool unselectable = false;
    public BoundsCheck boundsCheck;
    public float stoneStayDuration = 1.5f;
    public float returnDuration = 1.5f;
    public bool isInHole0 = false;
    public bool isInHole1 = false;
    
    private float returnTime;
    private float stoneStayTime;

    public void OnDrag(PointerEventData eventData)
    {
        dragging = true;
        dragPosition = ScreenToWorldPoint(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        if (!isInHole0 && !isInHole1)
        {
            transform.position = initPosition;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnEndDrag(eventData);
    }

    void Start()
    {
        initPosition = transform.position;
        boundsCheck = GetComponent<BoundsCheck>();    
    }

    void Update()
    {
        if (dragging && !unselectable)
        {
            transform.position = dragPosition;
        }
    }

    public IEnumerator ReadyReturn()
    {
        stoneStayTime = Time.time + stoneStayDuration;
        Color tColor = GetComponent<SpriteRenderer>().color;
        while (Time.time < stoneStayTime)
        {
            tColor.a = (stoneStayTime - Time.time) / stoneStayDuration;
            GetComponent<SpriteRenderer>().color = tColor;
            yield return null;
        }
        tColor.a = 0;
        GetComponent<SpriteRenderer>().color = tColor;
        StartCoroutine(StoneReturn());
    }

    public IEnumerator StoneReturn()
    {
        Vector2 startPos = transform.position;
        returnTime = Time.time + returnDuration;
        while (Time.time < returnTime)
        {
            transform.position = Vector2.Lerp(startPos, initPosition, 1 - (returnTime - Time.time));
            yield return null;
        }
        stoneStayTime = Time.time + stoneStayDuration;
        Color tColor = GetComponent<SpriteRenderer>().color;
        while (Time.time < stoneStayTime)
        {
            tColor.a = (1 - (stoneStayTime - Time.time)) / stoneStayDuration;
            GetComponent<SpriteRenderer>().color = tColor;
            yield return null;
        }
        tColor.a = 1;
        GetComponent<SpriteRenderer>().color = tColor;

        unselectable = false;
    }

    /// <summary>
    /// 将屏幕坐标转换成世界坐标
    /// </summary>>
    /// <param name = "pos"></param>
    /// <returns></returns>
    public Vector2 ScreenToWorldPoint(Vector3 pos)
    {
        return Camera.main.ScreenToWorldPoint(pos);
    }
}
