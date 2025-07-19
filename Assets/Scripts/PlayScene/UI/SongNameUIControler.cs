using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongNameUIControler : MonoBehaviour
{
    public AudioTimer timer;
    public TrackControler trackControler;
    public float animCoef;
    public float finalAnimCoef;
    private RectTransform rectTransform;
    private Vector2 pos;
    private Vector2 size;
    private TextMeshProUGUI songNameText;
    private TextMeshProUGUI artistText;
    private RectTransform songNameRectTransform;
    private RectTransform artistRectTransform;
    private readonly Vector2 pauseOffset = new(500f, -20f);
    private readonly Vector2 finalPos = new(0f, -141.5f);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        pos = rectTransform.offsetMin;
        size = rectTransform.offsetMax - pos;

        songNameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        artistText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        songNameRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        artistRectTransform = transform.GetChild(1).GetComponent<RectTransform>();
    }

    void Start()
    {
        songNameText.text = trackControler.metadata.TryGetStr("Metadata", "TitleUnicode");
        artistText.text = trackControler.metadata.TryGetStr("Metadata", "ArtistUnicode");
    }

    void Update()
    {
        // Debug.Log($"{songNameRectTransform.offsetMin} {songNameRectTransform.offsetMax} {artistRectTransform.offsetMin} {artistRectTransform.offsetMax}");
        if (timer.IsEnded)
        {
            rectTransform.offsetMin = Vector2.Lerp(
                rectTransform.offsetMin,
                finalPos,
                finalAnimCoef
            );
            rectTransform.offsetMax = Vector2.Lerp(
                rectTransform.offsetMax,
                finalPos + size,
                finalAnimCoef
            );
            songNameText.alignment = TextAlignmentOptions.Center;
            artistText.alignment = TextAlignmentOptions.Center;

            songNameRectTransform.offsetMin = new Vector2(
                Mathf.Lerp(songNameRectTransform.offsetMin.x, 283.9f, finalAnimCoef),
                songNameRectTransform.offsetMin.y
            );
            songNameRectTransform.offsetMax = new Vector2(
                Mathf.Lerp(songNameRectTransform.offsetMax.x, 668.1f, finalAnimCoef),
                songNameRectTransform.offsetMax.y
            );
            artistRectTransform.offsetMin = new Vector2(
                songNameRectTransform.offsetMin.x,
                artistRectTransform.offsetMin.y
            );
            artistRectTransform.offsetMax = new Vector2(
                songNameRectTransform.offsetMax.x,
                artistRectTransform.offsetMax.y
            );
        }
        else if (timer.IsPaused)
        {
            rectTransform.offsetMin = Vector2.Lerp(
                rectTransform.offsetMin,
                pos + pauseOffset,
                animCoef
            );
            rectTransform.offsetMax = Vector2.Lerp(
                rectTransform.offsetMax,
                pos + size + pauseOffset,
                animCoef
            );
        }
        else
        {
            rectTransform.offsetMin = Vector2.Lerp(
                rectTransform.offsetMin,
                pos,
                animCoef
            );
            rectTransform.offsetMax = Vector2.Lerp(
                rectTransform.offsetMax,
                pos + size,
                animCoef
            );
        }
    }
}
