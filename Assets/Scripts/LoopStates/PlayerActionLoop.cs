using System;
using System.Collections;
using System.Collections.Generic;
using static Game.Specifications;
using UnityEngine;
using DG.Tweening;

public class PlayerActionResult
{
    public Vector3 aimDirection;
    public Vector3[] path;
    public bool isGameOver;
}

public class PlayerActionLoop : LoopState
{
    const float SHOOT_ANGLE_RANGE = 60f;
    const int MAX_BOUNCES = 8;

    Player player;
    Vector3 leftRaycastOrigin;
    Vector3 rightRaycastOrigin;
    Vector3 aimResult;
    Vector3 aimDirection;
    List<Vector3> aimPath;
    float aimAngle; // * not used
    float lastHorizontalPressPosition;
    bool foundAimResult;
    bool isGameOver;

    GameObject aimdot;
    GameObject hitdotleft;
    GameObject hitdotcenter;
    GameObject hitdotright;

    public PlayerActionLoop(PlayLoop playLoop, TweenerLib tweenerLib) : base(playLoop, tweenerLib)
    {
        player = playLoop.Player;

        aimPath = new List<Vector3>();

        aimdot = GameObject.Find("AimDot");
        hitdotleft = GameObject.Find("HitDotLeft");
        hitdotcenter = GameObject.Find("HitDotCenter");
        hitdotright = GameObject.Find("HitDotRight");
    }

    public override void Begin(object pastStateResult)
    {
        playLoop.ScreenBackgroundInput.HorizontalPressLocationChanged += UpdateAimDirection;
        playLoop.ScreenBackgroundInput.HorizontalDragEnded += CompleteAim;
        playLoop.ScreenBackgroundInput.HorizontalDragStarted += StartAiming;
    }

    private void StartAiming(float obj)
    {
        player.ToggleLineRenderer(true);
    }

    void UpdateAimDirection(float horizontalPos)
    {
        lastHorizontalPressPosition = horizontalPos;

        aimDirection = GetAimDirection(horizontalPos);

        aimdot.transform.position = player.transform.position + aimDirection * 2f;

        foundAimResult = false;
        aimPath.Clear();
        aimPath.Add(player.transform.position);
        UpdateAim(player.transform.position, aimDirection, 0);

        player.UpdateLineRenderer(aimPath.ToArray());
    }

    public override void LogicUpdate()
    {
    }

    void CompleteAim(float horizontalPos)
    {
        lastHorizontalPressPosition = horizontalPos;
        GetAimDirection(horizontalPos);

        if (foundAimResult)
        {
            // ! Tween ortho size
            player.ToggleLineRenderer(false);
            player.AimerBubbleHologram.SetActive(false);

            if (CheckIfPlayerAimedInLoseGameLocation())
            {
                isGameOver = true;
                playLoop.SwitchState(PlayLoopState.NewRound);
            }
            else
            {
                isGameOver = false;
                playLoop.SwitchState(PlayLoopState.ActionResolution);
            }
        }
    }

    Vector3 GetAimDirection(float horizontalPos)
    {
        aimAngle = horizontalPos.Remap(-1f, 1f, -SHOOT_ANGLE_RANGE, SHOOT_ANGLE_RANGE);
        return Quaternion.AngleAxis(aimAngle, Vector3.up) * Vector3.forward;
    }

    public override object End()
    {
        playLoop.ScreenBackgroundInput.HorizontalPressLocationChanged -= UpdateAimDirection;
        playLoop.ScreenBackgroundInput.HorizontalDragEnded -= CompleteAim;

        return new PlayerActionResult { path = aimPath.ToArray(), aimDirection = aimDirection, isGameOver = isGameOver };
    }


    void UpdateAim(Vector3 location, Vector3 direction, int currentStep)
    {
        if (currentStep > MAX_BOUNCES)
        {
            return;
        }

        currentStep++;

        RaycastHit hitcenter;
        if (Physics.Raycast(
            location,
            direction,
            out hitcenter,
            Mathf.Infinity))
        {
            Debug.DrawRay(location, direction * 8, Color.yellow);

            if (hitcenter.transform.gameObject.layer == 8)
            {
                hitdotcenter.transform.position = hitcenter.point;
                UpdateAimHit(location, direction, hitcenter);
            }
            else if (hitcenter.transform.gameObject.layer == 9)
            {
                aimPath.Add(hitcenter.point);
                ReflectAim(hitcenter.point, hitcenter.normal, direction, currentStep);
            }
            else
            {
                Debug.Log($"Umm, what did you hit? layer: {hitcenter.transform.gameObject.layer}");
            }
        }
    }

    void ReflectAim(Vector3 location, Vector3 normal, Vector3 aimDirection, int currentStep)
    {
        Vector3 newDir = Vector3.Reflect(aimDirection, normal);
        UpdateAim(location, newDir, currentStep);
    }

    void UpdateAimHit(Vector3 location, Vector3 direction, RaycastHit hitcenter)
    {
        LayerMask layerMask = LayerMask.GetMask("Arena");
        SetRaycastOriginPoint(out Vector3 leftRaycastOrigin, true, location, direction);
        RaycastHit hitleft;
        if (Physics.Raycast(
            leftRaycastOrigin,
            direction,
            out hitleft,
            Mathf.Infinity,
            layerMask))
        {
            hitdotleft.transform.position = hitleft.point;
            Debug.DrawRay(leftRaycastOrigin, direction * 8, Color.red);
        }

        SetRaycastOriginPoint(out rightRaycastOrigin, false, location, direction);
        RaycastHit hitright;
        if (Physics.Raycast(
            rightRaycastOrigin,
            direction,
            out hitright,
            Mathf.Infinity,
            layerMask))
        {
            hitdotright.transform.position = hitright.point;
            Debug.DrawRay(rightRaycastOrigin, direction * 8, Color.blue);
        }

        AnalyzeAimHitResults(hitleft, hitcenter, hitright);
    }

    void SetRaycastOriginPoint(out Vector3 originPoint, bool isLeft, Vector3 offset, Vector3 aimDirection)
    {
        float angle = Vector3.Angle(Vector3.right, aimDirection);

        angle += 90 * (isLeft ? 1 : -1);

        originPoint.x = Mathf.Cos(Mathf.Deg2Rad * (angle)) * BUBBLE_DIAMETER / 2f;
        originPoint.y = 0f;
        originPoint.z = Mathf.Sin(Mathf.Deg2Rad * (angle)) * BUBBLE_DIAMETER / 2f;
        originPoint += offset;
    }

    void AnalyzeAimHitResults(RaycastHit leftHit, RaycastHit cetnerHit, RaycastHit rightHit)
    {
        float centerhitangle = Vector3.SignedAngle(Vector3.right, cetnerHit.normal, Vector3.up);
        int hitSide = (int)(centerhitangle / 30);
        float angle = 0f;

        //* I bet there is a more clever way to do this <facepalm>
        switch (hitSide)
        {
            case 0:
                angle = 0f;
                break;
            case -1:
            case -2:
                angle = 60f;
                break;
            case -3:
            case -4:
                angle = 120f;
                break;
            case -5:
            case 5:
                angle = 180f;
                break;
            case 4:
            case 3:
                angle = 240f;
                break;
            case 2:
            case 1:
                angle = 300f;
                break;
        }

        angle *= Mathf.Deg2Rad;

        Vector3 freeSlotRaycastDir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

        RaycastHit hit;
        if (Physics.Raycast(
            cetnerHit.point,
            freeSlotRaycastDir,
            out hit,
            BUBBLE_DIAMETER))
        {
            player.AimerBubbleHologram.SetActive(false);
            Debug.DrawRay(cetnerHit.transform.position, freeSlotRaycastDir * 2, Color.black);
        }
        else
        {
            Vector3 aimResultGridOffset = freeSlotRaycastDir * (BUBBLE_DIAMETER + GRID_SPACING);
            aimResult = cetnerHit.transform.position + aimResultGridOffset;
            aimPath.Add(aimResult);
            player.AimerBubbleHologram.transform.position = aimResult;
            player.AimerBubbleHologram.SetActive(true);
            foundAimResult = true;
        }

        playLoop.SetDebugText(centerhitangle, hitSide, angle);
    }

    // *hacky, if aimmResult is below that location the bubble is overflowing the grid, change to less hardcoded version
    private bool CheckIfPlayerAimedInLoseGameLocation()
    {
        return aimResult.z < -3f;
    }
}
