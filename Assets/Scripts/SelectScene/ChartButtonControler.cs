using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ChartButtonControler : MonoBehaviour, IPointerEnterHandler
{
    public ChartButtonHolder chartButtonHolder;
    public int chartIdx;
    public ChartMetadata chartMetadata;
    public Image image;
    public Sprite normalSprite, onfocusSprite;
    public Button button;
    public TextMeshProUGUI text;
    public float alpha = 1f;
    public float alphaAnimCoef = 0.1f;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(ChartButtonHolder chartButtonHolder, ChartMetadata chartMetadata, int chartIdx)
    {
        this.chartButtonHolder = chartButtonHolder;
        this.chartMetadata = chartMetadata;
        this.chartIdx = chartIdx;
        text.text = chartMetadata.TryGetStr("Metadata", "Version");
    }

    public void Trigger()
    {
        chartButtonHolder.ChartBtnHit(chartIdx);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        chartButtonHolder.ChartBtnPtrEnter(chartIdx);
    }

    void Update()
    {
        image.sprite = chartButtonHolder.IsChartOnFocus(this) ? onfocusSprite : normalSprite;

        alpha = Mathf.Lerp(alpha, chartButtonHolder.GetBtnAlpha(this), alphaAnimCoef);
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 2 * alpha);

        if (alpha < 0.16f)
            button.interactable = false;
        else
            button.interactable = true;
    }

    // public void OnDrag(PointerEventData eventData)
    // {
    //     Debug.Log($"{eventData.delta}");
    // }
}
