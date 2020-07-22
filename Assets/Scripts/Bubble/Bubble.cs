using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Bubble : MonoBehaviour
{
    [SerializeField] GameObject activeModel;
    [SerializeField] GameObject inactiveModel;
    [SerializeField] GameObject bubbleCanvas;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] BubbleCollider bubbleCollider;
    [SerializeField] List<ColourPalette> colourPalettes;

    public int id;
    public int value;
    public bool isActive;

    public bool IsBubbleActive => isActive;
    public int Value => value;

    void Awake()
    {
        ActivateBubble(false);
        RandomizeBubbleValue();
        // bubbleCollider.BubblePresed += OnBubblePressed;
    }

    public void ActivateBubble(bool activate = true)
    {
        isActive = activate;
        if (activate) UpdateBubbleDisplay();
        activeModel.SetActive(activate);
        // inactiveModel.SetActive(!activate); //todo reenable?
        bubbleCanvas.SetActive(activate);
    }

    public void UpdateBubbleValue(int value)
    {
        this.value = value;
        UpdateBubbleDisplay();
    }

    public Vector3 GetBubblePosition()
    {
        return transform.position;
    }

    public void ResetBubbleScale()
    {
        transform.localScale = Vector3.one;
    }

    public void RandomizeBubbleValue(int? cheatValue = null)
    {
        float rank = (float)UnityEngine.Random.Range(1, 10);

        if (cheatValue != null) 
        {
            value = cheatValue.Value;
        }
        else 
        {
            value = Mathf.FloorToInt(Mathf.Pow(2f, rank));
        }
        
        UpdateBubbleDisplay();
    }

    void UpdateBubbleDisplay()
    {
        valueText.text = value.ToString();
        activeModel.GetComponent<MeshRenderer>().material.color = colourPalettes.Find(_ => _.value == value).color;
    }

    void PrintMeshBounds()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;
        Debug.Log(bounds.size);
    }

    [System.Serializable]
    public class ColourPalette
    {
        public int value;
        public Color color;
    }
}
