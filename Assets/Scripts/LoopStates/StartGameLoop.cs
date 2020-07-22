using UnityEngine;

public class StartGameLoop : LoopState
{

    public StartGameLoop(PlayLoop playLoop, TweenerLib tweenerLib) : base(playLoop, tweenerLib)
    {
    }

    public override void Begin(object pastStateResult)
    {
        playLoop.Player.rabbitParticles.Play(); // dirtyyy

        playLoop.GridSpawner.CreateFirstRows(
            onCompleted: () => playLoop.SwitchState(PlayLoopState.Preparation)
        );

    }

    public override object End()
    {
        return null;
    }

    public override void LogicUpdate()
    {
    }

    public override void PhysicsUpdate()
    {
    }
}
