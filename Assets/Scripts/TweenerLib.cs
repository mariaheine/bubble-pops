using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TweenerLib : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] float shakeTime = 0.3f;
    [SerializeField] float strength = 0.5f;
    [SerializeField] int vibrato = 10;
    [SerializeField] float randomness = 90;
    [SerializeField] bool fadeOut = false;

    public void ShakeCamera()
    {
        Camera.main.DOShakePosition(shakeTime, strength, vibrato, randomness, fadeOut);
    }

    [Header("Movement")]
    public Ease movementEasing;

    public Tween MoveTransform(Transform t, Vector3 endValue, float duration, Ease easing = Ease.Unset)
    {
        easing = easing != Ease.Unset ? easing : movementEasing;
        return t.DOMove(endValue, duration).SetEase(easing);
    }

    [Header("Path")]
    public PathType pathType;
    public PathMode pathMode;
    public Ease pathEasing;
    public float pathSpeed;

    public Tween PathTransform(Transform t, Vector3[] path, float distance, Ease easing = Ease.Unset)
    {
        easing = easing != Ease.Unset ? easing : pathEasing;
        float duration = distance / pathSpeed;
        return t.DOPath(path, duration, pathType, pathMode).SetEase(easing);
    }
}
