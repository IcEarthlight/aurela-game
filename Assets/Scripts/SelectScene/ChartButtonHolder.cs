using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ChartButtonHolder : MonoBehaviour
{
    public const int FOCUS_ID = 1;
    public FocusManager focusManager;
    public ChartMetadata[] charts;
    public GameObject chartButtonPrefab;
    public float slope;
    public int focusChartIdx;
    public float animCoef;
    private RectTransform rectTransform;
    private readonly List<ChartButtonControler> chartButtonControlers = new();
    private readonly List<RectTransform> chartButtonRTs = new();
    private Vector2 offset = Vector2.zero;
    private int lastFocusChartIdx = -1;
    private int upKeyDownFrameCount = 0, downKeyDownFrameCount = 0;

    public ChartIllustrationControler chartIllustrationControler;

    private Vector2 centerPos, holderSize;

    public bool IsChartOnFocus(ChartButtonControler chartButtonControler)
    {
        return focusManager.IsKeyFocusOn(FOCUS_ID) && focusChartIdx == chartButtonControler.chartIdx;
    }

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        centerPos = rectTransform.offsetMin;
        holderSize = rectTransform.offsetMax - centerPos;
    }

    public static Vector2 GetCenter(RectTransform rt)
    {
        return (rt.offsetMin + rt.offsetMax) / 2;
    }

    void AddChart(ChartMetadata chart)
    {
        GameObject chartButton = Instantiate(chartButtonPrefab, transform);
        ChartButtonControler chartButtonControler = chartButton.GetComponent<ChartButtonControler>();
        chartButtonControler.Init(this, chart, chartButtonControlers.Count);
        chartButtonControlers.Add(chartButtonControler);

        RectTransform rt = chartButton.GetComponent<RectTransform>();
        chartButtonRTs.Add(rt);

        rt.offsetMin += offset;
        rt.offsetMax += offset;

        rectTransform.offsetMin = Vector2.Min(rectTransform.offsetMin, rt.offsetMin);
        rectTransform.offsetMax = Vector2.Max(rectTransform.offsetMax, rt.offsetMax);

        float height = rt.offsetMax.y - rt.offsetMin.y;
        offset -= new Vector2(height * slope, height);
    }

    void AddOver()
    {
        rectTransform.offsetMin = centerPos - GetCenter(chartButtonRTs[focusChartIdx]);
        rectTransform.offsetMax = rectTransform.offsetMin + holderSize;

        foreach (RectTransform rt in chartButtonRTs)
            rt.gameObject.SetActive(true);
    }

    public void SwitchSong(SongButtonControler songButtonControler)
    {
        if (charts == songButtonControler.charts) return;

        charts = songButtonControler.charts;
        focusChartIdx = Math.Clamp(focusChartIdx, 0, charts.Length - 1);

        int i;
        for (i = 0; i < charts.Length; i++)
        {
            if (i < chartButtonControlers.Count)
            {
                chartButtonControlers[i].Init(this, charts[i], i);
            }
            else
            {
                AddChart(charts[i]);
            }
        }
        AddOver();
        for (; i < chartButtonControlers.Count; i++)
        {
            chartButtonControlers[i].gameObject.SetActive(false);
        }

        OnChartSwitch();
    }

    public void OnChartSwitch()
    {
        chartIllustrationControler.gameObject.SetActive(true);
        chartIllustrationControler.SwitchIllustration(
            chartButtonControlers[focusChartIdx].chartMetadata.LoadBgTexture()
        );
    }

    void HandleScrollInput()
    {
        if (!focusManager.IsScrollFocusOn(FOCUS_ID)) return;

        focusChartIdx = Math.Clamp(
            focusChartIdx - Mathf.RoundToInt(10 * Input.GetAxis("Mouse ScrollWheel")),
            0, charts.Length - 1
        );
    }

    void HandleKeyInput()
    {
        if (!focusManager.IsKeyFocusOn(FOCUS_ID)) return;
        if (Input.GetKeyDown(KeyCode.UpArrow)) if (focusChartIdx > 0) focusChartIdx--;
        if (Input.GetKeyDown(KeyCode.DownArrow)) if (focusChartIdx < charts.Length - 1) focusChartIdx++;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            upKeyDownFrameCount++;
            if (upKeyDownFrameCount >= 32 && ((upKeyDownFrameCount & 7) == 0))
                if (focusChartIdx > 0) focusChartIdx--;
        }
        else
            upKeyDownFrameCount = 0;


        if (Input.GetKey(KeyCode.DownArrow))
        {
            downKeyDownFrameCount++;
            if (downKeyDownFrameCount >= 40 && ((downKeyDownFrameCount & 7) == 0))
                if (focusChartIdx < charts.Length - 1) focusChartIdx++;
        }
        else
            downKeyDownFrameCount = 0;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            focusManager.GetKeyFocusOn(SongButtonHolder.FOCUS_ID);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            ChartBtnHit(focusChartIdx);
    }

    public void ChartBtnHit(int chartBtnId)
    {
        focusManager.GetKeyFocusOn(FOCUS_ID);
        focusChartIdx = chartBtnId;

        ChartMetadataCarrier.Create(charts[chartBtnId]);
        SceneManager.LoadScene("PlayScene");
    }

    public void ChartBtnPtrEnter(int songBtnId)
    {
        focusManager.GetScrollFocusOn(FOCUS_ID);
    }

    public float GetBtnAlpha(ChartButtonControler button)
    {
        float distance = Vector3.Distance(transform.parent.position, button.transform.position);
        return 1 / (1 + Mathf.Pow(distance / 180, 6));
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"{eventData.delta}");
    }

    void Update()
    {
        if (charts == null || charts.Length <= 0) return;
        HandleScrollInput();
        HandleKeyInput();

        if (lastFocusChartIdx != focusChartIdx)
        {
            lastFocusChartIdx = focusChartIdx;
            OnChartSwitch();
        }

        rectTransform.offsetMin = Vector2.Lerp(
            rectTransform.offsetMin,
            centerPos - GetCenter(chartButtonRTs[focusChartIdx]),
            animCoef
        );
        rectTransform.offsetMax = rectTransform.offsetMin + holderSize;
    }
}
