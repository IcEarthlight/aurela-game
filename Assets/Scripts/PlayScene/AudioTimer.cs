using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AudioTimer : MonoBehaviour
{
    public TrackControler trackControler;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public bool IsPaused { get; private set; } = false;
    public bool IsEnded { get; private set; } = false;
    public delegate void pause();
    public static event pause Pause;
    public delegate void resume();
    public static event resume Resume;
    public delegate void backtrack(float newTimeMs);
    public static event backtrack Backtrack;
    private float playerPauseTime = 0f;

    void Start()
    {
        audioSource.playOnAwake = false;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void Update()
    {
        if (!IsPaused && !audioSource.isPlaying && audioSource.time == 0f)
        {
            IsEnded = true;
            // Debug.Log("Audio Ended");
        }
        if (audioSource == null)
            Debug.LogError("audio source is null!");
    }

    public void TimerPause()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            audioSource.Pause();
            Pause?.Invoke();
        }
    }

    public void TimerResume()
    {
        if (IsPaused)
        {
            IsPaused = false;
            audioSource.UnPause();
            Resume?.Invoke();
        }
    }

    public void TimerStop()
    {
        TimerPause();
        audioSource.time = 0f;
    }

    public void TimerBack(float ms, bool doPause)
    {
        audioSource.time = Math.Max(0f, audioSource.time - ms / 1000f);
        if (doPause)
        {
            TimerPause();
        }
        Backtrack?.Invoke(ms);
    }

    public void PlayerPause()
    {
        if (!IsPaused)
        {
            TimerPause();
            if (GetMs() > playerPauseTime)
                playerPauseTime = GetMs();
        }
    }

    public void PlayerResume()
    {
        if (IsPaused)
        {
            TimerBack(GetMs() - playerPauseTime + 2000f, false);
            TimerResume();
        }
    }

    public float GetMs()
    {
        return 1000f * audioSource.time;
        // return 100f * audioSource.time + 1000f;
    }

    public float GetProgress()
    {
        return audioSource.time / audioSource.clip.length;
    }

    void OnGUI()
    {
        // if (GUI.Button(new Rect(10, 10, 100, 30), "pause/resume"))
        //     if (IsPaused)
        //         TimerResume();
        //     else
        //         TimerPause();
        // if (GUI.Button(new Rect(10, 50, 100, 30), "back"))
        //     TimerBack(3000f, false);
        // if (GUI.Button(new Rect(10, 90, 100, 30), "stop"))
        //     TimerStop();
        // GUI.Box(new Rect(130, 10, 160, 30), "totalMs: " + GetMs().ToString());
        // GUI.Box(new Rect(130, 50, 160, 30), "startTime: ");
        // GUI.Box(new Rect(130, 90, 160, 30), "pauseTime: ");
        // GUI.Box(new Rect(130, 130, 160, 30), "totalPauseMs: ");
    }
}
