using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccUIControler : MonoBehaviour
{
    public AudioTimer timer;
    public TrackControler trackControler;
    public Text text;
    public float pauseAnimCoef;
    public float unpauseAnimCoef;
    private RectTransform rectTransform;
    private Vector2 pos;
    private Vector2 size;
    private readonly Vector2 pauseOffset = new(-234.44f, 0f);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        pos = rectTransform.offsetMin;
        size = rectTransform.offsetMax - pos;
    
        text = GetComponentInChildren<Text>();
    }

    void Update()
    {
        text.text = $"{100f * trackControler.judgeStat.GetAcc():F2}%";

        if (timer.IsPaused || timer.IsEnded)
        {
            rectTransform.offsetMin = Vector2.Lerp(
                rectTransform.offsetMin,
                pos + pauseOffset,
                pauseAnimCoef
            );
            rectTransform.offsetMax = Vector2.Lerp(
                rectTransform.offsetMax,
                pos + size + pauseOffset,
                pauseAnimCoef
            );
        }
        else
        {
            rectTransform.offsetMin = Vector2.Lerp(
                rectTransform.offsetMin,
                pos,
                unpauseAnimCoef
            );
            rectTransform.offsetMax = Vector2.Lerp(
                rectTransform.offsetMax,
                pos + size,
                unpauseAnimCoef
            );
        }
    }
}
