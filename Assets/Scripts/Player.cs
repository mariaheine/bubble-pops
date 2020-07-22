using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject playerBubblePrefab;
    [SerializeField] GameObject aimerBubbleHologram;
    [SerializeField] LineRenderer lineRenderer;
    public ParticleSystem rabbitParticles;

    Bubble bubble;

    public GameObject AimerBubbleHologram => aimerBubbleHologram;
    public Bubble PlayerBubble => bubble;

    void Awake()
    {
        GameObject playerBubbleGO = GameObject.Instantiate(
                    playerBubblePrefab,
                    transform.position,
                    Quaternion.identity,
                    transform); //* switch taht to arena transform

        bubble = playerBubbleGO.GetComponent<Bubble>();

        AimerBubbleHologram.SetActive(false);
        ToggleLineRenderer(false);
    }

    public void CreatePlayerBubble()
    {
        bubble.transform.position = transform.position; // todo change for the secondary shoot bubble
        bubble.RandomizeBubbleValue();
        bubble.ActivateBubble(true);
    }

    public void ToggleLineRenderer(bool toggle)
    {
        lineRenderer.enabled = toggle;
    }

    public void UpdateLineRenderer(Vector3[] worldSpacePath)
    {
        Vector3[] localPositons = new Vector3[worldSpacePath.Length];
        for (int i = 0; i < worldSpacePath.Length; i++)
        {
            localPositons[i] = transform.InverseTransformPoint(worldSpacePath[i]);
        }
        lineRenderer.positionCount = localPositons.Length;
        lineRenderer.SetPositions(localPositons);
    }

    public void ToggleAimerHologram(bool toggle)
    {
        aimerBubbleHologram.SetActive(toggle);
    }

    public void DisablePlayerBubble()
    {
        bubble.ActivateBubble(false);
        ToggleAimerHologram(false);
        ToggleLineRenderer(false);
    }
}
