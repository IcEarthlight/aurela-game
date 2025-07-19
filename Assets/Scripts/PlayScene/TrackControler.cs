using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class TrackControler : MonoBehaviour
{
    public AudioTimer timer;
    public ChartMetadata metadata;
    public TextAsset textAsset;
    public ComboCounter comboCounter;
    public HitFXPoolControler hitFXPoolControler;
    public GameObject tapPrefab;
    public GameObject holdPrefab;
    public GameObject airPrefab;
    public GameObject airHoldPrefab;
    public GameObject simulHintPrefab;
    public bool autoplay;
    public float noteSpeed;
    public float offset = 142f;
    public float trimDistance;
    public KeyCode[] keyBindings = new KeyCode[7] {
        KeyCode.W,
        KeyCode.E,
        KeyCode.F,
        KeyCode.Space,
        KeyCode.K,
        KeyCode.O,
        KeyCode.P
    };
    public JudgeStat judgeStat;

    private string chartContent;
    private readonly List<NoteController> notes = new();
    private readonly Queue<NoteController> tapJudgeSet = new();
    private readonly LinkedList<NoteHoldController> holdJudgeSet = new();
    private int enqueueIdx = 0;
    private int lastSimulHitIdx = 0;


    public float GetSpeed()
    {
        return 25 * noteSpeed;
    }

    [Obsolete]
    public void ReadChartStr()
    {
        string[] lines = chartContent.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.None
        );

        int i = 0;
        while (i < lines.Length)
        {
            if (lines[i].StartsWith("[TimingPoints]"))
            {
                for (; i < lines.Length; i++)
                {
                    if (lines[i].Trim().Length == 0) break;
                    // (lines[i]);
                }
            }
            else if (lines[i].StartsWith("[HitObjects]"))
            {
                for (; i < lines.Length; i++)
                {
                    if (lines[i].Trim().Length == 0) break;
                    CreateNoteFromLine(lines[i]);
                }
                CreateSimulHint(lastSimulHitIdx, notes.Count);
            }
            else i++;
        }
    }

    public void ReadChartStream()
    {
        try
        {
            using StreamReader reader = new(metadata.chartPath);
            string sectionName = "";

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim();

                if (line.StartsWith("//")) continue;

                Match match = Regex.Match(line, @"\[(.*?)\]");
                if (match.Success)
                {
                    sectionName = match.Groups[1].Value;
                    continue;
                }

                if (sectionName == "HitObjects")
                {
                    CreateNoteFromLine(line);
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"TrackControler.ReadChartStream: failed to parse path: {metadata.chartPath}\n{e.Message}");
        }

        CreateSimulHint(lastSimulHitIdx, notes.Count);
    }

    public bool CreateNoteFromLine(string line)
    {
        string[] parts = line.Split(',', ':');

        if (parts.Length < 3) return false;

        if (!float.TryParse(parts[0], out float x)) return false;
        if (!float.TryParse(parts[1], out float y)) return false;
        if (!float.TryParse(parts[2], out float hitTimeMs)) return false;

        int noteType = 1;
        float releaseTimeMs = hitTimeMs;
        if (parts.Length >= 4) int.TryParse(parts[3], out noteType);
        if (noteType == 128 && parts.Length >= 6) float.TryParse(parts[5], out releaseTimeMs);
        hitTimeMs += offset;
        releaseTimeMs += offset;

        int trackId = (int)(x / 72.5f);
        GameObject notePrefab;
        if (trackId == 0) { x = -3.675f; y = -1f / 7f; notePrefab = noteType == 128 ? holdPrefab : tapPrefab; }
        else if (trackId == 1) { x = -2.7f; y = 1f; notePrefab = noteType == 128 ? airHoldPrefab : airPrefab; }
        else if (trackId == 2) { x = -1.225f; y = -1f / 7f; notePrefab = noteType == 128 ? holdPrefab : tapPrefab; }
        else if (trackId == 3) { x = 0f; y = 15f / 28f; notePrefab = noteType == 128 ? airHoldPrefab : airPrefab; }
        else if (trackId == 4) { x = 1.225f; y = -1f / 7f; notePrefab = noteType == 128 ? holdPrefab : tapPrefab; }
        else if (trackId == 5) { x = 2.7f; y = 1f; notePrefab = noteType == 128 ? airHoldPrefab : airPrefab; }
        else if (trackId == 6) { x = 3.675f; y = -1f / 7f; notePrefab = noteType == 128 ? holdPrefab : tapPrefab; }
        else { x = 1.225f * (trackId - 3); y = trackId & 1; notePrefab = noteType == 128 ? airHoldPrefab : airPrefab; }

        NoteController note = Instantiate(
            notePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity,
            transform
        ).GetComponent<NoteController>();
        note.Init(this, hitTimeMs, releaseTimeMs, trackId, x, y);
        note.Reposition();
        notes.Add(note);

        if (notes[lastSimulHitIdx].hitTimeMs != hitTimeMs)
        {
            CreateSimulHint(lastSimulHitIdx, notes.Count - 1);
            lastSimulHitIdx = notes.Count - 1;
        }

        return true;
    }

    void CreateSimulHint(int startIdx, int endIdx)
    {
        if (endIdx - startIdx < 2) return;

        bool hasTrack3 = false;
        for (int i = startIdx; i < endIdx; i++)
            if (notes[i].trackId == 3)
            {
                hasTrack3 = true;
                break;
            }

        for (int i = startIdx; i < endIdx; i++)
            for (int j = i + 1; j < endIdx; j++)
            {
                // if ((notes[i].noteType & NoteType.ISGROUND) != 0 &&
                //     (notes[j].noteType & NoteType.ISGROUND) != 0)
                //     continue;

                if (Mathf.Abs(notes[i].y - notes[j].y) < 0.01f)
                    continue;

                if (hasTrack3 && (
                    notes[i].trackId == 0 && notes[j].trackId == 5 ||
                    notes[i].trackId == 1 && notes[j].trackId == 4 ||
                    notes[i].trackId == 1 && notes[j].trackId == 6 ||
                    notes[i].trackId == 2 && notes[j].trackId == 5 ||
                    notes[i].trackId == 4 && notes[j].trackId == 1 ||
                    notes[i].trackId == 5 && notes[j].trackId == 0 ||
                    notes[i].trackId == 5 && notes[j].trackId == 2 ||
                    notes[i].trackId == 6 && notes[j].trackId == 1))
                    continue;

                Instantiate(
                    simulHintPrefab,
                    notes[i].transform
                ).GetComponent<SimulHintControler>().Init(
                    this,
                    notes[i],
                    notes[j]
                );
            }

    }

    // void InsertJudgeSetInNextJudgeTimeOrder(NoteHoldController hold)
    // {
    //     if (hold == null) return;

    //     LinkedListNode<NoteHoldController> holdNode = holdJudgeSet.Last;
    //     while (holdNode != null && hold.nextHoldJudgeTimeMs < holdNode.Value.nextHoldJudgeTimeMs)
    //     {
    //         holdNode = holdNode.Previous;
    //     }
    //     if (holdNode == null)
    //         holdJudgeSet.AddFirst(hold);
    //     else
    //         holdJudgeSet.AddAfter(holdNode, hold);
    // }

    void RefreshJudgeSet(float newTimeMs) { RefreshJudgeSet(); }

    void RefreshJudgeSet()
    {
        tapJudgeSet.Clear();
        enqueueIdx = 0;

        for (int i = 0; i < notes.Count; i++)
        {
            if (notes[i].hitTimeMs - NoteController.BAD_INTERVAL <= timer.GetMs() &&
                timer.GetMs() < notes[i].hitTimeMs + NoteController.BAD_INTERVAL)
            {
                if (!notes[i].IsHit) tapJudgeSet.Enqueue(notes[i]);
                enqueueIdx = i;
            }
        }
    }

    void UpdateJudgeSet()
    {
        while (tapJudgeSet.Count > 0 &&
               tapJudgeSet.Peek().hitTimeMs < timer.GetMs() - NoteController.BAD_INTERVAL)
        {
            NoteController missedNote = tapJudgeSet.Dequeue();
            missedNote.Hit();
            if (missedNote is NoteHoldController missedHold)
                holdJudgeSet.AddLast(missedHold);
        }
        while (enqueueIdx < notes.Count &&
               notes[enqueueIdx].hitTimeMs < timer.GetMs() + NoteController.BAD_INTERVAL)
        {
            tapJudgeSet.Enqueue(notes[enqueueIdx]);
            enqueueIdx++;
        }
    }

    void AutoHit()
    {
        foreach (NoteController note in tapJudgeSet)
        {
            if (!note.IsHit && note.hitTimeMs < timer.GetMs())
            {
                note.Hit();
                if (note is NoteHoldController hold)
                    holdJudgeSet.AddLast(hold);
            }
        }

        LinkedListNode<NoteHoldController> currentNode = holdJudgeSet.First;
        while (currentNode != null)
        {
            LinkedListNode<NoteHoldController> nextNode = currentNode.Next;
            currentNode.Value.Hold();
            if (currentNode.Value.IsHit)
                holdJudgeSet.Remove(currentNode);
            currentNode = nextNode;
        }
    }

    bool TryHit()
    {
        for (int i = 0; i < keyBindings.Length; i++)
        {
            if (!Input.GetKeyDown(keyBindings[i])) continue;
            foreach (NoteController note in tapJudgeSet)
                if (note.Hit(i))
                    break;
        }

        return false;
    }

    void TryTouch()
    {
        LinkedListNode<NoteHoldController> currentNode = holdJudgeSet.First;
        while (currentNode != null)
        {
            LinkedListNode<NoteHoldController> nextNode = currentNode.Next;
            if (Input.GetKey(keyBindings[currentNode.Value.trackId]))
            {
                currentNode.Value.Hold();
                if (currentNode.Value.IsHit)
                    holdJudgeSet.Remove(currentNode);
            }
            currentNode = nextNode;
        }
    }

    bool IsTrackHold(int trackId)
    {
        return Input.GetKey(keyBindings[trackId]);
    }

    void OnEnable()
    {
        AudioTimer.Backtrack += RefreshJudgeSet;
    }

    void OnDisable()
    {
        AudioTimer.Backtrack -= RefreshJudgeSet;
    }

    void Awake()
    {
        metadata = ChartMetadataCarrier.TryRetrieve();
        if (metadata != null)
        {
            // textAsset = metadata.LoadTextAsset();
            // timer.audioClip = metadata.LoadAudioClip();
            StartCoroutine(metadata.LoadAudioClipAsync(clip =>
            {
                if (clip != null)
                {
                    timer.audioClip = clip;
                    timer.enabled = true;

                    int numOfNotes = 0;
                    foreach (NoteController note in notes)
                    {
                        // if (note is NoteHoldController hold)
                        //     numOfNotes += hold.CalculateAmount();
                        // else
                        //     numOfNotes++;
                        if (0f <= note.hitTimeMs && note.hitTimeMs < 1000f * timer.audioClip.length)
                            numOfNotes++;
                    }
                    judgeStat = new(comboCounter, numOfNotes);
                }
                else
                    Debug.LogError("Fail to load audioClip.");
            }));
        }
        else
        {
            metadata = new ChartMetadata(textAsset.text);
            chartContent = textAsset.text;
        }

        ReadChartStream();

    }

    void Update()
    {
        UpdateJudgeSet();
        if (autoplay) AutoHit();
        else { TryHit(); TryTouch(); }
    }

    // void OnGUI()
    // {
    //     GUI.Box(new Rect(Screen.width - 170, Screen.height / 2 - 140, 160, 140), "Perfect: " + judgeStat.perfect
    //     + "\nGreat: " + judgeStat.great
    //     + "\nGood: " + judgeStat.good
    //     + "\nBad: " + judgeStat.bad
    //     + "\nMiss: " + judgeStat.miss
    //     + "\n"
    //     + "\nCatch: " + judgeStat.holdCatch
    //     + "\nUncatch: " + judgeStat.holdUncatch
    //     );
    // }
}
