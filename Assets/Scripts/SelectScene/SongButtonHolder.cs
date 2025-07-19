using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SongButtonHolder : MonoBehaviour
{
    public const int FOCUS_ID = 0;
    public FocusManager focusManager;
    public ChartButtonHolder chartButtonHolder;
    public GameObject songButtonPrefab;
    public float slope;
    public int focusSongIdx;
    public float animCoef;
    private RectTransform rectTransform;
    private readonly List<SongButtonControler> songButtonControlers = new();
    private readonly List<RectTransform> songButtonRTs = new();
    private Vector2 offset = Vector2.zero;
    private int songCount = 0;
    private int upKeyDownFrameCount = 0, downKeyDownFrameCount = 0;

    private Vector2 centerPos, holderSize;

    public bool IsSongOnFocus(SongButtonControler songButtonControler)
    {
        return focusSongIdx == songButtonControler.songIdx;
    }

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        centerPos = rectTransform.offsetMin;
        holderSize = rectTransform.offsetMax - centerPos;

        focusSongIdx = PlayerPrefs.GetInt("SelectSongIdx", 0);
    }

    public static Vector2 GetCenter(RectTransform rt)
    {
        return (rt.offsetMin + rt.offsetMax) / 2;
    }

    public void AddSong(ChartMetadata[] charts)
    {
        GameObject songButton = Instantiate(songButtonPrefab, transform);
        SongButtonControler songButtonControler = songButton.GetComponent<SongButtonControler>();
        songButtonControler.Init(this, charts, songCount);
        songButtonControlers.Add(songButtonControler);

        RectTransform rt = songButton.GetComponent<RectTransform>();
        songButtonRTs.Add(rt);

        rt.offsetMin += offset;
        rt.offsetMax += offset;

        rectTransform.offsetMin = Vector2.Min(rectTransform.offsetMin, rt.offsetMin);
        rectTransform.offsetMax = Vector2.Max(rectTransform.offsetMax, rt.offsetMax);

        float height = rt.offsetMax.y - rt.offsetMin.y;
        offset -= new Vector2(height * slope, height);
        songCount++;
    }

    public void AddOver()
    {
        rectTransform.offsetMin = centerPos - GetCenter(songButtonRTs[focusSongIdx]);
        rectTransform.offsetMax = rectTransform.offsetMin + holderSize;

        foreach (RectTransform rt in songButtonRTs)
            rt.gameObject.SetActive(true);

        focusSongIdx = Math.Clamp(focusSongIdx, 0, songButtonControlers.Count);
    }

    void HandleScrollInput()
    {
        if (!focusManager.IsScrollFocusOn(FOCUS_ID)) return;

        focusSongIdx = Math.Clamp(
            focusSongIdx - Mathf.RoundToInt(10 * Input.GetAxis("Mouse ScrollWheel")),
            0, songCount - 1
        );
        chartButtonHolder.SwitchSong(songButtonControlers[focusSongIdx]);
    }

    void HandleKeyInput()
    {
        if (!focusManager.IsKeyFocusOn(FOCUS_ID)) return;
        if (Input.GetKeyDown(KeyCode.UpArrow)) if (focusSongIdx > 0) focusSongIdx--;
        if (Input.GetKeyDown(KeyCode.DownArrow)) if (focusSongIdx < songCount - 1) focusSongIdx++;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            upKeyDownFrameCount++;
            if (upKeyDownFrameCount >= 32 && ((upKeyDownFrameCount & 3) == 0))
                if (focusSongIdx > 0) focusSongIdx--;
        }
        else
            upKeyDownFrameCount = 0;


        if (Input.GetKey(KeyCode.DownArrow))
        {
            downKeyDownFrameCount++;
            if (downKeyDownFrameCount >= 40 && ((downKeyDownFrameCount & 3) == 0))
                if (focusSongIdx < songCount - 1) focusSongIdx++;
        }
        else
            downKeyDownFrameCount = 0;

        chartButtonHolder.SwitchSong(songButtonControlers[focusSongIdx]);
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.RightArrow))
            focusManager.GetKeyFocusOn(ChartButtonHolder.FOCUS_ID);
    }

    public void SongBtnHit(int songBtnId)
    {
        // focusManager.GetFocus(FOCUS_ID);
        focusSongIdx = songBtnId;
        chartButtonHolder.SwitchSong(songButtonControlers[songBtnId]);

        if (focusManager.IsKeyFocusOn(FOCUS_ID))
            focusManager.GetKeyFocusOn(ChartButtonHolder.FOCUS_ID);
        else
            focusManager.GetKeyFocusOn(FOCUS_ID);
    }

    public void SongBtnPtrEnter(int songBtnId)
    {
        focusManager.GetScrollFocusOn(FOCUS_ID);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"{eventData.delta}");
    }

    void Update()
    {
        HandleScrollInput();
        HandleKeyInput();

        rectTransform.offsetMin = Vector2.Lerp(
            rectTransform.offsetMin,
            centerPos - GetCenter(songButtonRTs[focusSongIdx]),
            animCoef
        );
        rectTransform.offsetMax = rectTransform.offsetMin + holderSize;
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt("SelectSongIdx", focusSongIdx);
    }
}
