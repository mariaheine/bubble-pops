using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Game.Specifications;
using System.Linq;

public class ActionResolutionLoop : LoopState
{
    Vector3[] shootPath;
    Player player;

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
        float distance = 0f;
        for (int i = 0; i < shootPath.Length - 1; i++)
        {
            distance += (shootPath[i] - shootPath[i+1]).magnitude;
        }

        Tween bubblePathTween = tweenerLib.PathTransform(
            player.PlayerBubble.transform,
            shootPath,
            distance
        );

        yield return bubblePathTween.WaitForCompletion();

        player.ToggleAimerHologram(false);
        playLoop.SwitchState(PlayLoopState.Scoring);
    }

    public override void GizmoDraw()
    {
        
    }

    public override object End()
    {
        return null;
    }

    public override void LogicUpdate()
    {

    }
}
