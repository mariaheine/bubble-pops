using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenBackgroundInput : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    float halfScreenWidth;

    public Action<float> HorizontalPressLocationChanged;
    public Action<float> HorizontalDragStarted;
    public Action<float> HorizontalDragEnded;

    void Start()
    {
        Debug.Log("Screen width:" + Screen.width);
        halfScreenWidth = Screen.width / 2f;
    }

    float GetPressHorizontalPosition(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return 0f;

        return localCursor.x.Remap(-halfScreenWidth, halfScreenWidth, -1f, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        float pos = GetPressHorizontalPosition(eventData);
        HorizontalDragStarted?.Invoke(pos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float pos = GetPressHorizontalPosition(eventData);
        HorizontalPressLocationChanged?.Invoke(pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float pos = GetPressHorizontalPosition(eventData);
        HorizontalDragEnded?.Invoke(pos);
    }
}
