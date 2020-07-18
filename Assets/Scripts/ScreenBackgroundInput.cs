using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenBackgroundInput : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    float halfScreenWidth;

    public Action<float> HorizontalPressLocationChanged;

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
        // Vector2 localCursor;
        // if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
        //     return;

        // Debug.Log("LocalCursor:" + localCursor);

        // float clickPosition = localCursor.x.Remap(-halfScreenWidth, halfScreenWidth, -1f, 1f);
        // Debug.Log(clickPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float pos = GetPressHorizontalPosition(eventData);
        // Debug.Log(pos);
        HorizontalPressLocationChanged?.Invoke(pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log();
    }
}
