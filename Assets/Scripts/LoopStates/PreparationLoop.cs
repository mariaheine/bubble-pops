

public class PreparationLoop : LoopState
{
    Player player;

    public PreparationLoop(PlayLoop playLoop, TweenerLib tweenerLib) : base(playLoop, tweenerLib)
    {
        player = playLoop.Player;
    }

    public override void Begin(object pastStateResult)
    {
        player.CreatePlayerBubble();
        playLoop.SwitchState(PlayLoopState.PlayerAction);
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