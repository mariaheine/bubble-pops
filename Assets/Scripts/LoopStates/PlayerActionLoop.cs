using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionLoop : LoopState
{
    const float shotAngleHalfRange = 60f;

    float lastHorizontalPressPosition;

    public PlayerActionLoop(PlayLoop playLoop) : base(playLoop)
    {
    }

    public override void Begin(object pastStateResult)
    {
        playLoop.ScreenBackgroundInput.HorizontalPressLocationChanged += UpdateAim;
    }

    private void UpdateAim(float horizontalPos)
    {
        lastHorizontalPressPosition = horizontalPos;

        float angle = horizontalPos.Remap(-1f,1f,-shotAngleHalfRange, shotAngleHalfRange);
        // Debug.Log(angle);
        Vector3 defaultDir = Vector3.forward;
        Vector3 aimDir = Quaternion.AngleAxis(angle, Vector3.up) * defaultDir;

        GameObject aimdot = GameObject.Find("AimDot");
        GameObject player = GameObject.Find("Player");
        Debug.Log(aimDir);
        Debug.Log(aimdot);
        aimdot.transform.position = player.transform.position + aimDir * 2f;
    }

    public override object End()
    {
        playLoop.ScreenBackgroundInput.HorizontalPressLocationChanged -= UpdateAim;

        return lastHorizontalPressPosition;
    }

    public override void LogicUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Debug.Log("mouse");
        }
    }

    public override void PhysicsUpdate()
    {
    }
}
