using UnityEngine;

public static class Extensions
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float SumPathDistance(this Vector3[] path)
    {
        float distance = 0f;
        for (int i = 0; i < path.Length - 1; i++)
        {
            distance += (path[i] - path[i+1]).magnitude;
        }
        return distance;
    }
}
