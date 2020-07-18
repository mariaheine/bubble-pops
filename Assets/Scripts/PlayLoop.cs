using System.Collections;
using UnityEngine;

public enum PlayLoopState { StartGame, PlayerAction, ActionResolution }

public class PlayLoop : MonoBehaviour
{
    [SerializeField] GridSpawner gridSpawner;
    [SerializeField] ScreenBackgroundInput screenBackgroundInput;

    LoopState currentLoopState;
    StartGameLoop startGameLoop;
    PlayerActionLoop playerActionLoop;

    public GridSpawner GridSpawner => gridSpawner;
    public ScreenBackgroundInput ScreenBackgroundInput => screenBackgroundInput;

    void Awake()
    {
        // gridSpawner.SpawnRow((ddd) => {});
        startGameLoop = new StartGameLoop(this);
        playerActionLoop = new PlayerActionLoop(this);
    }

    public void SwitchState(PlayLoopState state)
    {
        object pastStateResult = null;
        if (currentLoopState != null) pastStateResult = currentLoopState.End();

        switch (state)
        {
            case PlayLoopState.StartGame:
                currentLoopState = startGameLoop;
                break;
            case PlayLoopState.PlayerAction:
                currentLoopState = playerActionLoop;
                break;
        }

        currentLoopState.Begin(pastStateResult);
    }

    // 0. Start Game
    // Spawn 4/5 rows of bubbles
    // Spawn player bubble
    // Spawn secondary player bubble
    void Start()
    {
        SwitchState(PlayLoopState.StartGame);
    }

    // 1. Player action
    // Listen to inputs
    // Draw aiming
    void Update()
    {
        currentLoopState.LogicUpdate();
    }

    // 2. Resolve player bubble throw
    // Destroy bubbles
    // Score points

    // 3. Spawn new bubble row
    // If overflow -> game over

    //! remove

    public void AddRow()
    {
        gridSpawner.SpawnRow(null);
    }
}