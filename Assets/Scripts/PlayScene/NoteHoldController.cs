using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteHoldController : NoteController
{
    public float releaseTimeMs;
    public double judgeInterval;
    public double nextHoldJudgeTimeMs;

    public int missCount = 0, catchCount = 0;
    protected Material material;
    protected bool isLastCatch = false;
    protected float eraseTop = 0f;
    public override bool IsHit
    {
        get { return isHit; }
        protected set
        {
            if (!isHit && value)
                isHit = true;
            else if (isHit && !value)
                throw new InvalidOperationException("Cannot set isHit to false after it has been set to true.");
        }
    }
    protected bool toHold = false;
    public bool ToHold
    {
        get { return toHold; }
        protected set
        {
            if (!toHold && value)
            {
                toHold = true;
                trackControler.judgeStat.Stat(this);
                hitFXPoolControler.Hit(this);
            }
            else if (toHold && !value)
                throw new InvalidOperationException("Cannot set toHold to false after it has been set to true.");
        }
    }

    public override void Init(TrackControler trackControler, float hitTimeMs, float releaseTimeMs, int trackId, float x, float y)
    {
        base.Init(trackControler, hitTimeMs, releaseTimeMs, trackId, x, y);
        this.releaseTimeMs = releaseTimeMs;
        nextHoldJudgeTimeMs = hitTimeMs + judgeInterval;
    }

    public override void Reposition()
    {
        float hitZ = (hitTimeMs - timer.GetMs()) * trackControler.GetSpeed() / 1000f;
        float releaseZ = (releaseTimeMs - timer.GetMs()) * trackControler.GetSpeed() / 1000f;
        transform.position = new Vector3(x, 0.4f + 2.8f * y, hitZ);
        transform.localScale = new Vector3(1f, 1f, releaseZ - hitZ);

        if (hitZ < trimDistance && timer.GetMs() < releaseTimeMs + BAD_INTERVAL)
        {
            mesh.SetActive(true);
            float eraseBottom = (trimDistance - hitZ) / (releaseZ - hitZ);
            material.SetFloat("_EraseBottom", Mathf.Clamp(eraseBottom, 0f, 1f));
            if (isLastCatch) eraseTop = (-hitZ) / (releaseZ - hitZ);
            material.SetFloat("_EraseTop", Mathf.Clamp(eraseTop, 0f, 1f));
        }
        else
        {
            mesh.SetActive(false);
        }
    }

    public override bool Hit()
    {
        if (ToHold || IsHit) return false;

        float offset = hitTimeMs - timer.GetMs(); // early + late -

        if (offset < -BAD_INTERVAL) { judgment = -4; ToHold = true; isLastCatch = false; return false; }
        else if (offset < -GOOD_INTERVAL) { judgment = -3; ToHold = true; isLastCatch = true; return true; }
        else if (offset < -GREAT_INTERVAL) { judgment = -2; ToHold = true; isLastCatch = true; return true; }
        else if (offset < -PERFECT_INTERVAL) { judgment = -1; ToHold = true; isLastCatch = true; return true; }
        else if (offset < PERFECT_INTERVAL) { judgment = 0; ToHold = true; isLastCatch = true; return true; }
        else if (offset < GREAT_INTERVAL) { judgment = 1; ToHold = true; isLastCatch = true; return true; }
        else if (offset < GOOD_INTERVAL) { judgment = 2; ToHold = true; isLastCatch = true; return true; }
        else if (offset < BAD_INTERVAL) { judgment = 3; ToHold = true; isLastCatch = true; return true; }
        else return false;
    }

    public int CalculateAmount()
    {
        int amount = 1;
        double nhjt = nextHoldJudgeTimeMs;
        while (nhjt < releaseTimeMs - judgeInterval * 0.75f)
        {
            nhjt += judgeInterval;
            amount++;
        }
        return amount;
    }

    public void Hold()
    {
        if (IsHit) { return; }

        while (nextHoldJudgeTimeMs < releaseTimeMs - 20)
        {
            float offset = (float)nextHoldJudgeTimeMs - timer.GetMs();

            if (offset < -BAD_INTERVAL) { missCount++; isLastCatch = false; trackControler.judgeStat.AddUncatch(); hitFXPoolControler.UnCatch(this); }
            else if (offset < 0f) { catchCount++; isLastCatch = true; trackControler.judgeStat.AddCatch(); hitFXPoolControler.Catch(this); }
            else return;

            nextHoldJudgeTimeMs += judgeInterval;
        }

        IsHit = true;
    }

    public void CheckMiss()
    {
        if (IsHit) { return; }

        while (nextHoldJudgeTimeMs < releaseTimeMs - judgeInterval * 0.75f)
        {
            float offset = (float)nextHoldJudgeTimeMs - timer.GetMs();

            if (offset < -BAD_INTERVAL) { missCount++; isLastCatch = false; trackControler.judgeStat.AddUncatch(); hitFXPoolControler.UnCatch(this); }
            // else if (offset < GOOD_INTERVAL) { catchCount++; trackControler.judgeStat.AddCatch(); hitFXPoolControler.Catch(this); }
            else return;

            nextHoldJudgeTimeMs += judgeInterval;
        }

        IsHit = true;
    }

    protected override void Awake()
    {
        base.Awake();
        material = mesh.GetComponent<MeshRenderer>().material;
    }

    protected override void Update()
    {
        if (!timer.IsPaused)
        {
            Reposition();
        }
        // base.Update();
        if (toHold)
            CheckMiss();
    }
}
