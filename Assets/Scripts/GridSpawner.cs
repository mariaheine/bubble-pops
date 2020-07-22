using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Game.Specifications;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] GameObject DiskPrefab;
    [SerializeField] GameObject LeftWall;
    [SerializeField] GameObject RightWall;

    public delegate void OnRowSpawnResult(bool isGameOver);

    const float bubbleDiameter = 0.6f;

    bool isOddRow;
    float bubbleRadius;
    float horizontalStep;
    float verticalStep;
    Queue<GridBubble> bubbleGrid;

    public GridBubble[] BubbleGrid => bubbleGrid.ToArray();
    public bool IsCurrentOddRow => isOddRow; // odd ones are shifted to the right
    public List<GridBubble> ToplineBubbles;

    void Awake()
    {
        horizontalStep = bubbleDiameter + GRID_SPACING;
        float d = Mathf.Sin(Mathf.PI / 3) * bubbleDiameter;
        verticalStep = d + GRID_SPACING;
        bubbleRadius = bubbleDiameter / 2f;
        isOddRow = GRID_HEIGHT % 2 == 0 ? true : false;

        PositionWalls();
        ClearGrid();
    }

    public void CreateFirstRows(Action onCompleted)
    {
        StartCoroutine(CreateFirstRowsRoutine(onCompleted));
    }

    public void SpawnRow(OnRowSpawnResult onRowSpawn)
    {
        StartCoroutine(SpawnRowRoutine(onRowSpawn));
    }

    public void ClearGrid()
    {
        bubbleGrid = new Queue<GridBubble>(GRID_WIDTH * GRID_HEIGHT);
        ToplineBubbles = new List<GridBubble>(GRID_WIDTH);

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public GridBubble PutPlayerBubbleInAGrid(Bubble playerBubble)
    {
        Vector3 bubblePos = playerBubble.GetBubblePosition();

        for (int i = 0; i < BubbleGrid.Length; i++)
        {
            if (BubbleGrid[i].IsActive == false)
            {
                Vector3 otherBubblePos = BubbleGrid[i].Bubble.GetBubblePosition();
                
                if ((otherBubblePos - bubblePos).magnitude < 0.02)
                {
                    BubbleGrid[i].Bubble.UpdateBubbleValue(playerBubble.Value);
                    BubbleGrid[i].ActivateBubble(true);
                    playerBubble.ActivateBubble(false);
                    return BubbleGrid[i];
                }
            }
        }

        Debug.LogError("Failed to find a slot for player bubble");
        return null;
    }

    void PositionWalls()
    {
        LeftWall.transform.position = new Vector3(transform.position.x - BUBBLE_DIAMETER / 2, LeftWall.transform.position.y, LeftWall.transform.position.z);
        RightWall.transform.position = new Vector3(transform.position.x + GRID_WIDTH * (GRID_SPACING + BUBBLE_DIAMETER), RightWall.transform.position.y, RightWall.transform.position.z);
    }

    void GenerateInactiveGrid()
    {
        int bubbleID = 0;
        for (int j = GRID_HEIGHT - 1; j >= 0; j--)
        {
            for (int i = GRID_WIDTH - 1; i >= 0; i--)
            {
                Vector3 offset = new Vector3(horizontalStep * i, 0f, -verticalStep * j);
                offset.x += j % 2 * bubbleRadius;
                GameObject bubbleGO = GameObject.Instantiate(DiskPrefab, transform.position + offset, Quaternion.identity, transform);
                bubbleGrid.Enqueue(new GridBubble(bubbleGO, bubbleID++));
            }
        }
    }

    IEnumerator CreateFirstRowsRoutine(Action onCompleted)
    {
        GenerateInactiveGrid();

        for (int j = 0; j < STARTING_ROWS; j++)
        {
            float oddRowOffset = isOddRow ? bubbleRadius : 0f;
            isOddRow = !isOddRow;

            for (int i = 0; i < GRID_WIDTH; i++)
            {
                yield return new WaitForSeconds(0.1f);

                GridBubble newSlot = bubbleGrid.Dequeue();
                float xPosition = horizontalStep * i + oddRowOffset;
                newSlot.ActivateBubble(new Vector3(xPosition, 0f, verticalStep));
                bubbleGrid.Enqueue(newSlot);

                if (j == STARTING_ROWS - 1)
                {
                    ToplineBubbles.Clear();
                    ToplineBubbles.Add(newSlot);
                }
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

        Debug.Log("spawning");
        ToplineBubbles.Clear();

        for (int i = 0; i < GRID_WIDTH; i++)
        {
            yield return new WaitForSeconds(0.1f);

            GridBubble lastSlot = bubbleGrid.Dequeue();

            // todo change gmeover condition, it will not work for the player added bubbles
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
                lastSlot.Bubble.ResetBubbleScale();
                bubbleGrid.Enqueue(lastSlot);
                ToplineBubbles.Add(lastSlot);
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
