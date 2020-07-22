using UnityEngine;

public class GridBubble
{
    Bubble bubble;

    public bool IsActive => bubble.IsBubbleActive;
    public Bubble Bubble => bubble;
    public bool HasBeenPropagatedTo = false;

    public GridBubble(GameObject bubbleGO, int bubbleID)
    {
        Bubble slotBubble = bubbleGO.GetComponent<Bubble>();
        this.bubble = slotBubble;
        slotBubble.id = bubbleID;
    }

    public void ActivateBubble(bool activate)
    {
        bubble.ActivateBubble(activate);
    }

    public void ActivateBubble(Vector3 position)
    {
        bubble.transform.localPosition = position;
        ActivateBubble(true);
    }

    public void OffsetBubble(float v)
    {
        bubble.transform.Translate(0f, 0f, v, Space.Self);
    }
}
