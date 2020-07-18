using UnityEngine;

public class GridBubble
{
    Bubble bubble;

    public bool IsActive => bubble.IsBubbleActive;

    public GridBubble()
    {

    }

    public GridBubble(GameObject bubbleGO)
    {
        Bubble slotBubble = bubbleGO.GetComponent<Bubble>();
        this.bubble = slotBubble;
    }

    public void ActivateBubble(Vector3 position)
    {
        bubble.transform.localPosition = position;
        bubble.ActivateBubble();
    }

    internal void OffsetBubble(float v)
    {
        bubble.transform.Translate(0f, 0f, v, Space.Self);
    }
}
