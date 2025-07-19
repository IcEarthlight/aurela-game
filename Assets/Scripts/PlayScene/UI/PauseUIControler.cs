using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIControler : MonoBehaviour
{
    public AudioTimer timer;
    public float pauseAnimCoef;
    public float unpauseAnimCoef;
    private RectTransform rectTransform;
    private Vector2 pos;
    private Vector2 size;
    private readonly Vector2 pauseOffset = new(-100f, 0f);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        pos = rectTransform.offsetMin;
        size = rectTransform.offsetMax - pos;
    }

    void Update()
    {
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
