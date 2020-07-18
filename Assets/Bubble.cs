using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] GameObject activeModel;
    [SerializeField] GameObject inactiveModel;

    public bool IsBubbleActive => activeModel.activeSelf;

    void Awake()
    {
        ActivateBubble(false);
    }

    public void ActivateBubble(bool activate = true)
    {
        activeModel.SetActive(activate);
        inactiveModel.SetActive(!activate);
    }

    void PrintMeshBounds()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;
        Debug.Log(bounds.size);
    }
}
