using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChartIllustrationControler : MonoBehaviour, IPointerEnterHandler
{
    public ChartButtonHolder chartButtonHolder;
    public FocusManager focusManager;
    public Vector2 centerPos;
    private RectTransform rectTransform;
    private RawImage rawImage;
    public float diagonal;
    private Vector2 newPos, newSize;

    public Vector2 chartSelectOffset;
    public float animCoef;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rawImage = GetComponent<RawImage>();
    }

    void Start()
    {
        // centerPos = (rectTransform.offsetMax - rectTransform.offsetMin) / 2;
        Reposition();
    }

    public void SwitchIllustration(Texture2D texture)
    {
        rawImage.texture = texture;
        Reposition();
    }

    static float SaturationCurve(float x)
    {
        return 1 - Mathf.Exp(-x / 2);
    }

    public void Reposition()
    {
        newPos = centerPos + SaturationCurve(chartButtonHolder.focusChartIdx) * chartSelectOffset;
        newSize = diagonal * new Vector2(rawImage.texture.width, rawImage.texture.height).normalized;
        // rectTransform.offsetMin = centerPos - newSize / 2;
        // rectTransform.offsetMax = centerPos + newSize / 2;
    }

    void Update()
    {
        rectTransform.offsetMin = Vector2.Lerp(rectTransform.offsetMin, newPos - newSize / 2, animCoef);
        rectTransform.offsetMax = Vector2.Lerp(rectTransform.offsetMax, newPos + newSize / 2, animCoef);
        rectTransform.rotation = Quaternion.Euler(0, 0, 3 + 2 * Mathf.Sin(Time.time));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        focusManager.GetScrollFocusOn(ChartButtonHolder.FOCUS_ID);
    }
}
