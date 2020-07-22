using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using static Game.Specifications;

public class ScoringLoop : LoopState
{
    Player player;
    GridSpawner gridSpawner;
    GridBubble[] bubbleGrid;
    Action propagationComplete;

    public ScoringLoop(PlayLoop playLoop, TweenerLib tweenerLib) : base(playLoop, tweenerLib)
    {
        player = playLoop.Player;
        gridSpawner = playLoop.GridSpawner;
    }

    public override void Begin(object pastStateResult)
    {
        bubbleGrid = gridSpawner.BubbleGrid;

        GridBubble gridBubble = gridSpawner.PutPlayerBubbleInAGrid(player.PlayerBubble);

        propagationComplete = () =>
            {
                playLoop.StartCoroutine(
                    SeekoutSeparatedBubbles(
                        onSeparatedIslandsFound: () =>
                        {
                            playLoop.SwitchState(PlayLoopState.NewRound);
                        }));
            };

        PropagateCollapse(bubbleToPropagateFrom: gridBubble);

    }

    void PropagateCollapse(GridBubble bubbleToPropagateFrom)
    {
        ResetGridPropagationStatus();
        List<GridBubble> bubbleCluster = CollectMatchingNeighbours(
            originBubble: bubbleToPropagateFrom,
            matchCondition: (gridBubble, index) => FilterByMatchingValue(bubbleToPropagateFrom, index));
        AnalyzeClusterScore(bubbleCluster);
    }

    private bool FilterByMatchingValue(GridBubble collapseBubble, int index)
    {
        return bubbleGrid[index].Bubble.Value == collapseBubble.Bubble.Value;
    }

    List<GridBubble> CollectMatchingNeighbours(GridBubble originBubble, Func<GridBubble, int, bool> matchCondition)
    {
        if (originBubble is null)
        {
            throw new ArgumentNullException("originBubble");
        }

        List<GridBubble> bubbleCluster = new List<GridBubble>(bubbleGrid.Length);
        originBubble.HasBeenPropagatedTo = true;
        bubbleCluster.Add(originBubble);

        for (int i = 0; i < bubbleGrid.Length; i++)
        {
            if (bubbleGrid[i].HasBeenPropagatedTo) continue;

            if (bubbleGrid[i].IsActive == true && matchCondition(originBubble, i))
            {
                float d = (bubbleGrid[i].Bubble.GetBubblePosition() - originBubble.Bubble.GetBubblePosition()).magnitude;
                if (d < BUBBLE_DIAMETER + 2 * GRID_SPACING)
                {
                    bubbleCluster.AddRange(CollectMatchingNeighbours(bubbleGrid[i], matchCondition));
                }
            }
        }

        return bubbleCluster;
    }

    // Todo I really need to switch to async await, coroutines and tweens are truly a nightmare
    void AnalyzeClusterScore(List<GridBubble> bubbleCluster)
    {
        //* There is no points if a bubble formed no cluster
        if (bubbleCluster.Count == 1)
        {
            propagationComplete.Invoke();
            return;
        }

        //* Get new bubble value
        int clusterValue = RaisedBubbleValue(bubbleCluster[0].Bubble, bubbleCluster.Count);

        //* Proceed with explosion if max score was reached
        if (clusterValue >= EXPLOSION_SCORE)
        {
            playLoop.StartCoroutine(ExplodeCluster(bubbleCluster));
        }
        else
        {
            //* Find collapse slot while prefering one that would allow further bubble merging
            if (IsThereBubbleThatCanPropagateCollapse(bubbleCluster, clusterValue, out GridBubble collapseBubble))
            {
                //* A bubble to propagate collapse has been found
                playLoop.StartCoroutine(TweenBubbleGathering(
                    bubbleCluster,
                    collapseBubble,
                    onBubblesGathered: () =>
                    {
                        collapseBubble.Bubble.UpdateBubbleValue(clusterValue);
                        PropagateCollapse(collapseBubble);
                    }));
            }
            else
            {
                //* Cluster cannot propagate, collapse and continue to next game stage
                collapseBubble = bubbleCluster[UnityEngine.Random.Range(0, bubbleCluster.Count)];
                collapseBubble.Bubble.UpdateBubbleValue(clusterValue);

                playLoop.StartCoroutine(TweenBubbleGathering(
                    bubbleCluster,
                    collapseBubble,
                    onBubblesGathered: () =>
                    {
                        propagationComplete.Invoke();
                    }));

            }
        }

    }

    bool IsThereBubbleThatCanPropagateCollapse(List<GridBubble> bubbleCluster, int soughtCollapseValue, out GridBubble collapseBubble)
    {
        collapseBubble = null
        ;
        foreach (var bubble in bubbleCluster)
        {
            if (collapseBubble == null && CheckIfHasNeighbourMatchingValue(bubble, soughtCollapseValue))
            {
                // Debug.Log("found bubble 2 collapse", b.Bubble.transform);
                collapseBubble = bubble;
                return true;
            }
        }

        return false;
    }

    IEnumerator ExplodeCluster(List<GridBubble> bubbleCluster)
    {
        if (bubbleCluster.Count == 1)
        {
            Debug.LogError("what");
            yield break;
        }

        //* Exploding the topmost bubble is more fun
        GridBubble topMostBubble = bubbleCluster[0];
        for (int i = 1; i < bubbleCluster.Count; i++)
        {
            if (bubbleCluster[i].Bubble.GetBubblePosition().z > topMostBubble.Bubble.GetBubblePosition().z)
            {
                topMostBubble = bubbleCluster[i];
            }
        }

        //* Gather cluster into the top mostbubble
        yield return playLoop.StartCoroutine(TweenBubbleGathering(bubbleCluster, topMostBubble));

        List<GridBubble> neighbours = GetAllActiveNeighbours(topMostBubble);
        yield return playLoop.StartCoroutine(TweenBubbleGathering(neighbours, topMostBubble));

        // ! add explosion anim

        Camera.main.DOShakePosition(0.3f, 0.03f, 20, 90, false);

        topMostBubble.ActivateBubble(false);

        propagationComplete.Invoke();
    }

    List<GridBubble> GetAllActiveNeighbours(GridBubble originBubble)
    {
        List<GridBubble> neighbours = new List<GridBubble>(6);
        for (int i = 0; i < bubbleGrid.Length; i++)
        {
            if (bubbleGrid[i].IsActive == true)
            {
                float d = (bubbleGrid[i].Bubble.GetBubblePosition() - originBubble.Bubble.GetBubblePosition()).magnitude;
                if (d < BUBBLE_DIAMETER + 2 * GRID_SPACING)
                {
                    neighbours.Add(bubbleGrid[i]);
                }
            }
        }
        return neighbours;
    }

    IEnumerator SeekoutSeparatedBubbles(Action onSeparatedIslandsFound)
    {
        GridBubble mainlandOrigin = null;
        mainlandOrigin = gridSpawner.ToplineBubbles.Find(b => b.IsActive == true);

        if (mainlandOrigin == null)
        {
            onSeparatedIslandsFound.Invoke();
            yield break;
        }

        IEnumerable<GridBubble> separatedBubbles = FindSeparatedBubbles(mainlandOrigin);

        float gridY = gridSpawner.transform.position.y;

        Sequence mySequence = DOTween.Sequence();

        for (int i = 0; i < separatedBubbles.Count(); i++)
        {
            Bubble bubble = separatedBubbles.ElementAt(i).Bubble;
            Vector3 originalPosition = bubble.transform.localPosition;

            mySequence.Insert(0f, bubble.transform
                .DOLocalMoveY(
                    bubble.transform.localPosition.y - gridY,
                    0.5f)
                .SetEase(Ease.OutBounce)
                .OnComplete(() =>
                {
                    bubble.ActivateBubble(false);
                    bubble.transform.localPosition = originalPosition;
                }));

            mySequence.Insert(0f, bubble.transform
                .DOScale(
                    0.2f,
                    0.5f)
                .SetEase(Ease.OutBounce))
                .OnComplete(() =>
                {
                    bubble.transform.localScale = Vector3.one;
                });
        }

        mySequence.AppendCallback(() => onSeparatedIslandsFound.Invoke());
    }

    private IEnumerable<GridBubble> FindSeparatedBubbles(GridBubble mainlandOrigin)
    {
        ResetGridPropagationStatus();

        List<GridBubble> activeBubbles = new List<GridBubble>();

        for (int i = 0; i < bubbleGrid.Length; i++)
        {
            if (bubbleGrid[i].IsActive) activeBubbles.Add(bubbleGrid[i]);
        }

        //* condition is true because we just want to collect all the neigbours
        //* who have connection to the first "mainland" bubble
        List<GridBubble> mainlandNeighbours = CollectMatchingNeighbours(
            originBubble: mainlandOrigin,
            matchCondition: (gridBubble, index) =>
            {
                return true;
            }
        );

        var separatedBubbles = activeBubbles.Except(mainlandNeighbours);
        return separatedBubbles;
    }

    IEnumerator TweenBubbleGathering(List<GridBubble> bubbleCluster, GridBubble collapseBubble, Action onBubblesGathered = null)
    {
        Sequence mySequence = DOTween.Sequence();
        
        foreach (var b in bubbleCluster)
        {
            if (b == collapseBubble) continue;

            Vector3 bubbleStartingPos = b.Bubble.transform.position;

            mySequence.Insert(0f, b.Bubble.transform
                .DOMove(
                    collapseBubble.Bubble.GetBubblePosition(),
                    0.3f
                )
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    b.ActivateBubble(false);
                    b.Bubble.transform.position = bubbleStartingPos;
                }));
        }

        yield return mySequence.WaitForCompletion();

        onBubblesGathered?.Invoke();
    }

    int RaisedBubbleValue(Bubble bubble, int factor)
    {
        return bubble.Value << (factor - 1);
    }

    bool CheckIfHasNeighbourMatchingValue(GridBubble gridBubble, int value)
    {
        for (int i = 0; i < bubbleGrid.Length; i++)
        {
            if (bubbleGrid[i].IsActive == true && bubbleGrid[i].Bubble.Value == value)
            {
                float d = (bubbleGrid[i].Bubble.GetBubblePosition() - gridBubble.Bubble.GetBubblePosition()).magnitude;
                if (d < BUBBLE_DIAMETER + 2 * GRID_SPACING)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void ResetGridPropagationStatus()
    {
        for (int i = 0; i < bubbleGrid.Length; i++)
        {
            bubbleGrid[i].HasBeenPropagatedTo = false;
        }
    }

    public override object End()
    {
        return null;
    }
}
