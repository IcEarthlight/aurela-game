using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SongButtonControler : MonoBehaviour, IPointerEnterHandler
{
    public SongButtonHolder songButtonHolder;
    public int songIdx;
    public string title, artist;
    public ChartMetadata[] charts;
    public TextMeshProUGUI text;
    public GameObject songIllustration;
    private RectTransform rectTransform;
    private RawImage songIllustrationImage;

    public Image buttonFrameImage;
    public Sprite normalSprite, onfocusSprite;

    public void Init(SongButtonHolder songButtonHolder, ChartMetadata[] charts, int songIdx)
    {
        this.songButtonHolder = songButtonHolder;
        this.charts = charts;
        this.songIdx = songIdx;
        title = charts[0].TryGetStr("Metadata", "TitleUnicode");
        artist = charts[0].TryGetStr("Metadata", "ArtistUnicode");
        text.text = title;

        rectTransform = GetComponent<RectTransform>();
        
        songIllustrationImage = songIllustration.GetComponent<RawImage>();

        songIllustrationImage.texture = charts[0].LoadBgTexture();

        Vector2 originalSize = new(songIllustrationImage.texture.width, songIllustrationImage.texture.height);
        Vector2 newSize = rectTransform.offsetMax - rectTransform.offsetMin;

        if (newSize.y / newSize.x < originalSize.y / originalSize.x)
        {
            Rect uvRect = songIllustrationImage.uvRect;
            uvRect.height = newSize.y / newSize.x / (originalSize.y / originalSize.x);
            uvRect.y = (1f - uvRect.height) / 3 * 2;
            songIllustrationImage.uvRect = uvRect;
        }
        else
        {
            Rect uvRect = songIllustrationImage.uvRect;
            uvRect.width = originalSize.y / originalSize.x / (newSize.y / newSize.x);
            uvRect.x = (1f - uvRect.width) / 2;
            songIllustrationImage.uvRect = uvRect;
        }
    }

    public void Trigger()
    {
        songButtonHolder.SongBtnHit(songIdx);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        songButtonHolder.SongBtnPtrEnter(songIdx);
    }

    void Update()
    {
        buttonFrameImage.sprite = songButtonHolder.IsSongOnFocus(this) ? onfocusSprite : normalSprite;
    }
}
