using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarControler : MonoBehaviour
{
    public AudioTimer timer;
    private RectTransform rectTransform;
    private float screenWidth;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        screenWidth = GetComponentInParent<CanvasScaler>().referenceResolution.x;
    }

    void Update()
    {
        rectTransform.offsetMax = new Vector2(
            screenWidth * (timer.GetProgress() - 1f),
            rectTransform.offsetMax.y
        );
    }
}
