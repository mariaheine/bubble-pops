using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public enum PlayLoopState 
{ 
    MainMenu,
    StartGame, 
    Preparation, 
    PlayerAction, 
    ActionResolution,
    Scoring,
    NewRound,
    GameOver //todo
}

public class PlayLoop : MonoBehaviour
{
    [SerializeField] GridSpawner gridSpawner;
    [SerializeField] ScreenBackgroundInput screenBackgroundInput;
    [SerializeField] Player player;
    [SerializeField] TweenerLib tweenerLib;
    [SerializeField] TextMeshProUGUI debugText;

    LoopState currentLoopState;
    MainMenuLoop mainMenuLoop;
    StartGameLoop startGameLoop;
    PreparationLoop preparationLoop;
    PlayerActionLoop playerActionLoop;
    ActionResolutionLoop actionResolutionLoop;
    ScoringLoop scoringLoop;
    NewRoundLoop newRoundLoop;

    public GridSpawner GridSpawner => gridSpawner;
    public ScreenBackgroundInput ScreenBackgroundInput => screenBackgroundInput;
    public Player Player => player;

    void Awake()
    {
        mainMenuLoop = new MainMenuLoop(this, tweenerLib);
        startGameLoop = new StartGameLoop(this, tweenerLib);
        preparationLoop = new PreparationLoop(this, tweenerLib);
        playerActionLoop = new PlayerActionLoop(this, tweenerLib);
        actionResolutionLoop = new ActionResolutionLoop(this, tweenerLib);
        scoringLoop = new ScoringLoop(this, tweenerLib);
        newRoundLoop = new NewRoundLoop(this, tweenerLib);

        DOTween.Init(true, true, LogBehaviour.Verbose); 
    }

    public void SetDebugText(params object[] text)
    {
        string uglyprinter = null;
        foreach (var el in text)
        {
            uglyprinter = $"{uglyprinter} | {el.ToString()}";
        }
        debugText.text = uglyprinter;
    }

    public void SwitchState(PlayLoopState state)
    {
        object pastStateResult = null;
        if (currentLoopState != null) pastStateResult = currentLoopState.End();

        switch (state)
        {
            case PlayLoopState.MainMenu:
                currentLoopState = mainMenuLoop;
                break;
            case PlayLoopState.StartGame:
                currentLoopState = startGameLoop;
                break;
            case PlayLoopState.Preparation:
                currentLoopState = preparationLoop;
                break;
            case PlayLoopState.PlayerAction:
                currentLoopState = playerActionLoop;
                break;
            case PlayLoopState.ActionResolution:
                currentLoopState = actionResolutionLoop;
                break;
            case PlayLoopState.Scoring:
                currentLoopState = scoringLoop;
                break;
            case PlayLoopState.NewRound:
                currentLoopState = newRoundLoop;
                break;
        }

        currentLoopState.Begin(pastStateResult);
    }

    void Start()
    {
        SwitchState(PlayLoopState.MainMenu);
    }

    void Update()
    {
        currentLoopState.LogicUpdate();
    }

    void OnDrawGizmos()
    {
        if (currentLoopState != null)  currentLoopState.GizmoDraw();
    }
}