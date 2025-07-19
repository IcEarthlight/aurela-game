using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class NoteController : MonoBehaviour
{
    public AudioTimer timer;
    public TrackControler trackControler;
    public HitFXPoolControler hitFXPoolControler;
    public NoteType noteType;
    public float x, y;
    public float hitTimeMs;
    public int trackId;
    public int judgment = 10;

    public const float PERFECT_INTERVAL = 40f;
    public const float GREAT_INTERVAL = 80f;
    public const float GOOD_INTERVAL = 120f;
    public const float BAD_INTERVAL = 160f;

    protected GameObject mesh;
    protected float trimDistance;
    public bool isHit = false;

    public virtual bool IsHit
    {
        get { return isHit; }
        protected set
        {
            if (!isHit && value)
            {
                isHit = true;
                trackControler.judgeStat.Stat(this);
                hitFXPoolControler.Hit(this);
            }
            else if (isHit && !value)
            {
                throw new InvalidOperationException("Cannot set isHit to false after it has been set to true.");
            }
        }
    }

    public virtual void Init(
        TrackControler trackControler,
        float hitTimeMs,
        float releaseTimeMs,
        int trackId, float x, float y)
    {
        // Debug.Log("NoteController.init called with z: " + transform.position.z + ", trackId: " + trackId);
        this.trackControler = trackControler;
        timer = trackControler.timer;
        hitFXPoolControler = trackControler.hitFXPoolControler;
        this.hitTimeMs = hitTimeMs;
        this.trackId = trackId;
        this.x = x;
        this.y = y;

        transform.position = new Vector3(x, 0.4f + 2.8f * y, float.MaxValue);
        trimDistance = trackControler.trimDistance;
    }

    public virtual void Reposition(float newTimeMs)
    {
        Reposition();
    }

    public virtual void Reposition()
    {
        transform.position = new Vector3(
            x, 0.4f + 2.8f * y,
            (hitTimeMs - timer.GetMs()) * trackControler.GetSpeed() / 1000f
        );
    }

    public virtual bool Hit()
    {
        if (IsHit) return false;

        float offset = hitTimeMs - timer.GetMs(); // early + late -

        if (offset < -BAD_INTERVAL) { judgment = -4; IsHit = true; return false; }
        else if (offset < -GOOD_INTERVAL) { judgment = -3; IsHit = true; return true; }
        else if (offset < -GREAT_INTERVAL) { judgment = -2; IsHit = true; return true; }
        else if (offset < -PERFECT_INTERVAL) { judgment = -1; IsHit = true; return true; }
        else if (offset < PERFECT_INTERVAL) { judgment = 0; IsHit = true; return true; }
        else if (offset < GREAT_INTERVAL) { judgment = 1; IsHit = true; return true; }
        else if (offset < GOOD_INTERVAL) { judgment = 2; IsHit = true; return true; }
        else if (offset < BAD_INTERVAL) { judgment = 3; IsHit = true; return true; }
        else return false;
    }

    public virtual bool Hit(int hitTrack)
    {
        if (hitTrack != trackId) return false;
        return Hit();
    }

    protected virtual void Awake()
    {
        mesh = transform.GetChild(0).gameObject;
    }

    protected virtual void OnEnable()
    {
        AudioTimer.Backtrack += Reposition;
    }

    protected virtual void OnDisable()
    {
        AudioTimer.Backtrack -= Reposition;
    }

    protected virtual void Update()
    {
        if (!timer.IsPaused && !IsHit)
        {
            Reposition();
        }
        mesh.SetActive(!IsHit && transform.position.z <= trimDistance);
    }

    public override string ToString()
    {
        return $"N({trackId}, {hitTimeMs}, {(IsHit ? "Hit" : "Unhit")})";
    }
}
