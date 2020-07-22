using System;
using UnityEngine;

public class BubbleCollider : MonoBehaviour
{
    public Action BubblePresed;

    void OnMouseDown()
    {
        BubblePresed?.Invoke();
    }
}
