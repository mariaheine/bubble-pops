using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuLoop : LoopState
{
    Button startGameButton;
    GameObject mainMenuCanvas;
    Transform leavesParent;

    bool isFirstRun = true;

    List<Vector3> startingPositions;
    List<Vector3> startingRotations;

    public MainMenuLoop(PlayLoop playLoop, TweenerLib tweenerLib) : base(playLoop, tweenerLib)
    {
        leavesParent = GameObject.Find("Leaves").transform;
        mainMenuCanvas = GameObject.Find("MainMenuCanvas");
        startGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();
    }

    public override void Begin(object pastStateResult)
    {
        if (!isFirstRun)
        {
            for (int i = 0; i < leavesParent.childCount; i++)
            {
                Transform leaf = leavesParent.GetChild(i);

                startingPositions.Add(leaf.transform.position);

                leaf
                    .DOMove(startingPositions[i], 2.5f)
                    .OnComplete(() =>
                    {
                        mainMenuCanvas.SetActive(true);
                    });
            }
        }
        else
        {
            isFirstRun = false;
            startingPositions = new List<Vector3>(leavesParent.childCount);
            startingRotations = new List<Vector3>(leavesParent.childCount);
        }

        startGameButton.onClick.AddListener(() =>
        {
            mainMenuCanvas.SetActive(false);
            playLoop.SwitchState(PlayLoopState.StartGame);
        });
    }

    public override object End()
    {
        startGameButton.onClick.RemoveAllListeners();

        for (int i = 0; i < leavesParent.childCount; i++)
        {
            Transform leaf = leavesParent.GetChild(i);

            startingPositions.Add(leaf.transform.position);

            leaf.DOMove(leaf.transform.position - leaf.transform.right * 15f, 2.5f);
        }

        return null;
    }
}
