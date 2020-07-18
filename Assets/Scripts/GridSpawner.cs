using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSpecifications
{
    public const float DISK_WIDTH = 2f;
    public const float DISK_HEIGHT = 2f;
}

public class GridSpawner : MonoBehaviour
{
    public GameObject DiskPrefab;
    public delegate void OnRowSpawnResult(bool isGameOver);

    const float bubbleDiameter = 0.6f;
    const int gridWidth = 6;
    const int gridHeight = 10;
    const float gridSpacing = 0.02f;
    const int startingRows = 2;

    bool isOddRow;
    float bubbleRadius;
    float horizontalStep;
    float verticalStep;
    Queue<GridBubble> bubbleGrid;

    void Awake()
    {
        bubbleGrid = new Queue<GridBubble>(gridWidth * gridHeight);

        horizontalStep = bubbleDiameter + gridSpacing;
        float d = Mathf.Sin(Mathf.PI / 3) * bubbleDiameter;
        verticalStep = d + gridSpacing;
        bubbleRadius = bubbleDiameter / 2f;

        isOddRow = gridHeight % 2 == 0 ? true : false;
    }

    public void CreateFirstRows(Action onCompleted)
    {
        StartCoroutine(CreateFirstRowsRoutine(onCompleted));
    }

    public void SpawnRow(OnRowSpawnResult onRowSpawn)
    {
        StartCoroutine(SpawnRowRoutine(onRowSpawn));
    }

    void GenerateInativeGrid()
    {
        for (int j = gridHeight - 1; j >= 0; j--)
        {
            for (int i = gridWidth - 1; i >= 0; i--)
            {
                Vector3 offset = new Vector3(horizontalStep * i, 0f, -verticalStep * j);
                offset.x += j % 2 * bubbleRadius;
                GameObject bubbleGO = GameObject.Instantiate(DiskPrefab, transform.position + offset, Quaternion.identity, transform);
                bubbleGrid.Enqueue(new GridBubble(bubbleGO));
            }
        }
    }

    IEnumerator CreateFirstRowsRoutine(Action onCompleted)
    {
        GenerateInativeGrid();

        for (int j = 0; j < startingRows; j++)
        {
            float oddRowOffset = isOddRow ? bubbleRadius : 0f;
            isOddRow = !isOddRow;

            for (int i = 0; i < gridWidth; i++)
            {
                yield return new WaitForSeconds(0.1f);

                GridBubble newSlot = bubbleGrid.Dequeue();
                float xPosition = horizontalStep * i + oddRowOffset;
                newSlot.ActivateBubble(new Vector3(xPosition, 0f, verticalStep));
                bubbleGrid.Enqueue(newSlot);
            }

            foreach (var bubble in bubbleGrid)
            {
                bubble.OffsetBubble(-verticalStep);
            }
        }

        onCompleted?.Invoke();
    }

    IEnumerator SpawnRowRoutine(OnRowSpawnResult onRowSpawnResult)
    {
        bool isGameOver = false;
        float oddRowOffset = isOddRow ? bubbleRadius : 0f;
        isOddRow = !isOddRow;

        for (int i = 0; i < gridWidth; i++)
        {
            yield return new WaitForSeconds(0.1f);

            // bubbleGrid.
            GridBubble lastSlot = bubbleGrid.Dequeue();

            if (lastSlot.IsActive)
            {
                //* Add a call to a dequeued Bubble, if it exists it could run some game over destruction anim 
                isGameOver = true;
                Debug.Log("game over");
            }
            else
            {
                //* activate new slot
                float xPosition = horizontalStep * i + oddRowOffset;
                lastSlot.ActivateBubble(new Vector3(xPosition, 0f, verticalStep));
                bubbleGrid.Enqueue(lastSlot);
            }
        }

        // * dirtyyy
        if (!isGameOver)
        {
            foreach (var bubble in bubbleGrid)
            {
                bubble.OffsetBubble(-verticalStep);
            }
        }

        onRowSpawnResult(isGameOver);
    }
}
