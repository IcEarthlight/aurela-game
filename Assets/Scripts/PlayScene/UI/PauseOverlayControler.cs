using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseOverlayControler : MonoBehaviour
{
    public AudioTimer timer;
    public TrackControler trackControler;
    public RectTransform pauseBgRT;
    public Image pauseBgImg;
    public TextMeshProUGUI pauseTMP;
    public GameObject btnFocus, quitBtn, resumeBtn, retryBtn;
    public Color pauseColor, textColor, btnColor, btnTextColor;
    public float backColorAnimCoef, bgAnimCoef, textAnimCoef, btnAnimCoef;
    private Vector2 focusBtnPos, focusBtnSize;
    private RectTransform btnFocusTransform;
    private Image btnFocusImg, quitImg, resumeImg, retryImg;
    private Text quitText, resumeText, retryText;
    private int focusBtn = 1;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        btnFocusImg = btnFocus.GetComponent<Image>();
        btnFocusTransform = btnFocus.GetComponent<RectTransform>();
        focusBtnPos = btnFocusTransform.offsetMin;
        focusBtnSize = btnFocusTransform.offsetMax - focusBtnPos;

        quitImg = quitBtn.GetComponent<Image>();
        resumeImg = resumeBtn.GetComponent<Image>();
        retryImg = retryBtn.GetComponent<Image>();
        quitText = quitBtn.GetComponentInChildren<Text>();
        resumeText = resumeBtn.GetComponentInChildren<Text>();
        retryText = retryBtn.GetComponentInChildren<Text>();
    }

    public void PlayerPause()
    {
        // Debug.Log("Player Pause");
        focusBtn = 1;
        timer.PlayerPause();
    }
    public void PlayerQuit()
    {
        // Debug.Log("Player Quit");
        SceneManager.LoadScene("SelectSongScene");
    }

    public void PlayerResume()
    {
        // Debug.Log("Player Resume");
        timer.PlayerResume();
    }

    public void PlayerRetry()
    {
        // Debug.Log("Player Retry");
        ChartMetadataCarrier.Create(trackControler.metadata);
        SceneManager.LoadScene("PlayScene");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (timer.IsEnded)
                PlayerQuit();
            if (timer.IsPaused)
                PlayerResume();
            else
                PlayerPause();
        }

        if (timer.IsPaused && !timer.IsEnded)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) focusBtn = (focusBtn + 2) % 3;
            if (Input.GetKeyDown(KeyCode.RightArrow)) focusBtn = (focusBtn + 1) % 3;
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                switch (focusBtn)
                {
                    case 0: PlayerQuit(); break;
                    case 1: PlayerResume(); break;
                    case 2: PlayerRetry(); break;
                }
            }

            switch (focusBtn)
            {
                case 0: btnFocusTransform.offsetMin = focusBtnPos + 250 * Vector2.left; break;
                case 1: btnFocusTransform.offsetMin = focusBtnPos; break;
                case 2: btnFocusTransform.offsetMin = focusBtnPos + 250 * Vector2.right; break;
            }
            btnFocusTransform.offsetMax = btnFocusTransform.offsetMin + focusBtnSize;

            image.color = Color.Lerp(
                image.color,
                pauseColor,
                backColorAnimCoef
            );
            pauseBgRT.localScale = Vector3.Lerp(
                pauseBgRT.localScale,
                Vector3.one,
                bgAnimCoef / 2
            );
            pauseBgImg.color = Color.Lerp(
                pauseBgImg.color,
                Color.white,
                bgAnimCoef / 16
            );
            pauseTMP.characterSpacing = Mathf.Lerp(
                pauseTMP.characterSpacing,
                83.25f,
                textAnimCoef
            );
            pauseTMP.color = Color.Lerp(
                pauseTMP.color,
                textColor,
                textAnimCoef
            );
            btnFocusTransform.localScale = Vector3.one;
            btnFocusImg.color = Color.Lerp(
                btnFocusImg.color,
                Color.white,
                btnAnimCoef / 4
            );
            quitImg.color = Color.Lerp(
                quitImg.color,
                btnColor,
                btnAnimCoef / 8
            );
            resumeImg.color = quitImg.color;
            retryImg.color = resumeImg.color;
            quitText.color = btnTextColor;
            resumeText.color = quitText.color;
            retryText.color = resumeText.color;
        }
        else
        {
            image.color = Color.Lerp(
                image.color,
                Color.clear,
                backColorAnimCoef
            );
            pauseBgRT.localScale = Vector3.Lerp(
                pauseBgRT.localScale,
                new Vector3(1.5f, 1.25f, 1f),
                bgAnimCoef
            );
            pauseBgImg.color = Color.Lerp(
                pauseBgImg.color,
                new Color(1f, 1f, 1f, 0f),
                bgAnimCoef
            );
            pauseTMP.characterSpacing = Mathf.Lerp(
                pauseTMP.characterSpacing,
                300f,
                textAnimCoef * 2
            );
            pauseTMP.color = Color.Lerp(
                pauseTMP.color,
                new Color(1f, 1f, 1f, 0f),
                textAnimCoef * 4
            );
            btnFocusTransform.localScale = Vector3.Lerp(
                btnFocusTransform.localScale,
                new Vector3(1.5f, 1.25f, 1f),
                btnAnimCoef
            );
            btnFocusImg.color = Color.Lerp(
                btnFocusImg.color,
                new Color(1f, 1f, 1f, 0f),
                btnAnimCoef * 2
            );
            quitImg.color = Color.Lerp(
                quitImg.color,
                new Color(1f, 1f, 1f, 0f),
                btnAnimCoef * 2
            );
            resumeImg.color = quitImg.color;
            retryImg.color = resumeImg.color;
            quitText.color = Color.Lerp(
                quitText.color,
                new Color(btnTextColor.r, btnTextColor.g, btnTextColor.b, 0f),
                btnAnimCoef * 2
            );
            resumeText.color = quitText.color;
            retryText.color = resumeText.color;
        }

        if (quitImg.color.a < 0.01f)
        {
            quitBtn.SetActive(false);
            resumeBtn.SetActive(false);
            retryBtn.SetActive(false);
        }
        else
        {
            quitBtn.SetActive(true);
            resumeBtn.SetActive(true);
            retryBtn.SetActive(true);
        }
    }
}
