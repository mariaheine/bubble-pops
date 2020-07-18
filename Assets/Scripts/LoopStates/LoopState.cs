public abstract class LoopState
{
    protected PlayLoop playLoop;

    public LoopState(PlayLoop playLoop)
    {
        this.playLoop = playLoop;
    }

    public abstract void Begin(object pastStateResult);
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();
    public abstract object End();
}
