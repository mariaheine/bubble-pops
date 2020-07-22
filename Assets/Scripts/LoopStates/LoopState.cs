public abstract class LoopState
{
    protected PlayLoop playLoop;
    protected TweenerLib tweenerLib;

    public LoopState(PlayLoop playLoop, TweenerLib tweenerLib)
    {
        this.playLoop = playLoop;
        this.tweenerLib = tweenerLib;
    }

    public abstract void Begin(object pastStateResult);
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void GizmoDraw() { }
    public abstract object End();
}
