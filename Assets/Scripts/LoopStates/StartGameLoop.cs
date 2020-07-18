public class StartGameLoop : LoopState
{
    public StartGameLoop(PlayLoop playLoop) : base(playLoop)
    {
    }

    public override void Begin(object pastStateResult)
    {
        playLoop.GridSpawner.CreateFirstRows(
            onCompleted: () => playLoop.SwitchState(PlayLoopState.PlayerAction)
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
