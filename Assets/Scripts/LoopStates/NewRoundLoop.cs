using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NewRoundLoop : LoopState
{
    Player player;
    GridSpawner gridSpawner;

    public NewRoundLoop(PlayLoop playLoop, TweenerLib tweenerLib) : base(playLoop, tweenerLib)
    {
        player = playLoop.Player;
        gridSpawner = playLoop.GridSpawner;
    }

    public override void Begin(object pastStateResult)
    {
        if (pastStateResult is ActionResolutionResult actionResolutionResult)
        {
            if (actionResolutionResult.isGameOver)
            {
                playLoop.SwitchState(PlayLoopState.MainMenu);
                GameOverSequence();
            }
        }
        else
        {
            gridSpawner.SpawnRow((isGameOver) =>
            {
                if (isGameOver)
                {
                    playLoop.SwitchState(PlayLoopState.MainMenu);
                    GameOverSequence();
                }
                else
                {
                    playLoop.SwitchState(PlayLoopState.PlayerAction);

                    player.CreatePlayerBubble();
                }
            });
        }
    }

    void GameOverSequence()
    {
        Sequence mySequence = DOTween.Sequence();

        float gridY = gridSpawner.transform.position.y;

        for (int i = 0; i < gridSpawner.BubbleGrid.Length; i++)
        {
            Bubble bubble = gridSpawner.BubbleGrid[i].Bubble;
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

        mySequence.AppendCallback(() => playLoop.GridSpawner.ClearGrid());
    }

    public override object End()
    {
        return null;
    }
}
