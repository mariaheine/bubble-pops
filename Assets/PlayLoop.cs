using System.Collections;
using UnityEngine;

public class PlayLoop : MonoBehaviour
{
    [SerializeField] GridSpawner gridSpawner;

    LoopState currentLoopState;
    StartGameLoop startGameLoop;

    public GridSpawner GridSpawner => gridSpawner;

    void Awake()
    {
        startGameLoop = new StartGameLoop(this);
        // gridSpawner.SpawnRow((ddd) => {});
    }

    // 0. Start Game
    // Spawn 4/5 rows of bubbles
    // Spawn player bubble
    // Spawn secondary player bubble
    void Start()
    {
        currentLoopState = startGameLoop;
        // currentLoopState.Begin();
        StartCoroutine(delay());
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(1f);

        
        currentLoopState.Begin();
    }

    // 1. Player action
    // Listen to inputs
    // Draw aiming

    // 2. Resolve player bubble throw
    // Destroy bubbles
    // Score points

    // 3. Spawn new bubble row
    // If overflow -> game over
}

public interface LoopStateController
{
    void ChangeState();
}

public abstract class LoopState
{
    protected PlayLoop playLoop;

    public LoopState(PlayLoop playLoop)
    {
        this.playLoop = playLoop;
    }

    public abstract void Begin();
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();
    public abstract void End();
}

public class StartGameLoop : LoopState
{
    public StartGameLoop(PlayLoop playLoop) : base(playLoop)
    {
    }

    public override void Begin()
    {
        // playLoop.GridSpawner.SpawnRow(null);
        playLoop.StartCoroutine(playLoop.GridSpawner.CreateFirstRows());
    }

    public override void End()
    {
        throw new System.NotImplementedException();
    }

    public override void LogicUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void PhysicsUpdate()
    {
        throw new System.NotImplementedException();
    }
}
