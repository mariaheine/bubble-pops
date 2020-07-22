using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Game.Specifications;
using System.Linq;

public class ActionResolutionResult
{
    public bool isGameOver;
}

public class ActionResolutionLoop : LoopState
{
    Vector3[] shootPath;
    Player player;
    bool isGameOver;

    public ActionResolutionLoop(PlayLoop playLoop, TweenerLib tweenerLib) : base(playLoop, tweenerLib)
    {
        player = playLoop.Player;
    }

    public override void Begin(object pastStateResult)
    {
        PlayerActionResult result = pastStateResult as PlayerActionResult;

        shootPath = result.path;

        playLoop.StartCoroutine(MovePlayerBubble());
    }

    IEnumerator MovePlayerBubble()
    {
        float distance = shootPath.SumPathDistance();

        Tween bubblePathTween = tweenerLib.PathTransform(
            player.PlayerBubble.transform,
            shootPath,
            distance
        );

        yield return bubblePathTween.WaitForCompletion();

        player.ToggleAimerHologram(false);

        if (CheckIfPlayerAimedInLoseGameLocation(shootPath.Last()))
        {
            isGameOver = true;
            playLoop.SwitchState(PlayLoopState.NewRound);
        }
        else
        {
            isGameOver = false;
            playLoop.SwitchState(PlayLoopState.Scoring);
        }
    }

    // todo move to grid manager
    // *hacky, if aimmResult is below that location the bubble is overflowing the grid, change to less hardcoded version
    private bool CheckIfPlayerAimedInLoseGameLocation(Vector3 aimResult)
    {
        return aimResult.z < -3f;
    }

    public override object End()
    {
        return new ActionResolutionResult { isGameOver = this.isGameOver };
    }
}
