using UnityEngine;

[ExecuteInEditMode]
public class OrthoCamera : MonoBehaviour
{
    public float horizontalResolution = 1920;

    void OnGUI()
    {
        float currentAspect = (float)Screen.width / (float)Screen.height;
        Camera.main.orthographicSize = horizontalResolution / currentAspect / 200;
    }
}

