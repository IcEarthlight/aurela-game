using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreOverlayControler : MonoBehaviour
{
    public AudioTimer timer;
    public TrackControler trackControler;
    private bool? isHighScore = null;
    private float accuracy, highScoreDiff;
    private ScoreStatControler[] scoreStatControlers;
    private Image scoreBg;
    public GameObject scorePanel;
    private RectTransform scorePanelRT;
    private Image scorePanelImg;
    private Vector2 scorePanelPos, scorePanelSize;
    public Vector2 scorePanelOffset;
    public float bgAnimCoef, scorePanelAnimCoef;

    public GameObject backBtn, retryBtn;
    private RectTransform backBtnRT, retryBtnRT;
    private Vector2 backBtnPos, backBtnSize, retryBtnPos, retryBtnSize;
    public Vector2 btnOffset;
    public float btnAnimCoef;

    public GameObject accObj, bestScoreObj;
    private TextMeshProUGUI accText, bestScoreText;
    private float accTextFn = 0f;
    private RectTransform accRT, bestScoreRT;
    private Vector2 accPos, bestScorePos, accSize, bestScoreSize;
    public Vector2 accOffset, accOffset2, bestScoreOffset2;
    public float accAnimCoef;


    void Awake()
    {
        scoreBg = GetComponent<Image>();
        scorePanelRT = scorePanel.GetComponent<RectTransform>();
        scorePanelImg = scorePanel.GetComponent<Image>();
        scorePanelPos = scorePanelRT.offsetMin;
        scorePanelSize = scorePanelRT.offsetMax - scorePanelPos;

        backBtnRT = backBtn.GetComponent<RectTransform>();
        retryBtnRT = retryBtn.GetComponent<RectTransform>();
        backBtnPos = backBtnRT.offsetMin;
        backBtnSize = backBtnRT.offsetMax - backBtnPos;
        retryBtnPos = retryBtnRT.offsetMin;
        retryBtnSize = retryBtnRT.offsetMax - retryBtnPos;

        accText = accObj.GetComponent<TextMeshProUGUI>();
        bestScoreText = bestScoreObj.GetComponent<TextMeshProUGUI>();
        accRT = accObj.GetComponent<RectTransform>();
        bestScoreRT = accObj.GetComponent<RectTransform>();
        accPos = accRT.offsetMin;
        accSize = accRT.offsetMax - accPos;
        bestScorePos = bestScoreRT.offsetMin;
        bestScoreSize = bestScoreRT.offsetMax;

        scoreStatControlers = new ScoreStatControler[] {
            scorePanel.transform.GetChild(0).GetComponent<ScoreStatControler>(),
            scorePanel.transform.GetChild(1).GetComponent<ScoreStatControler>(),
            scorePanel.transform.GetChild(2).GetComponent<ScoreStatControler>(),
            scorePanel.transform.GetChild(3).GetComponent<ScoreStatControler>(),
            scorePanel.transform.GetChild(4).GetComponent<ScoreStatControler>(),
            scorePanel.transform.GetChild(5).GetComponent<ScoreStatControler>(),
            scorePanel.transform.GetChild(6).GetComponent<ScoreStatControler>(),
            scorePanel.transform.GetChild(7).GetComponent<ScoreStatControler>(),
            scorePanel.transform.GetChild(8).GetComponent<ScoreStatControler>()
        };
    }

    void Update()
    {
        if (timer.IsEnded)
        {
            scorePanel.SetActive(true);
            backBtn.SetActive(true);
            retryBtn.SetActive(true);
            accObj.SetActive(true);
            bestScoreObj.SetActive(true);

            scoreStatControlers[0].SetLerpFn(trackControler.judgeStat.perfect, scorePanelAnimCoef);
            scoreStatControlers[1].SetLerpFn(trackControler.judgeStat.great, scorePanelAnimCoef);
            scoreStatControlers[2].SetLerpFn(trackControler.judgeStat.good, scorePanelAnimCoef);
            scoreStatControlers[3].SetLerpFn(trackControler.judgeStat.bad, scorePanelAnimCoef);
            scoreStatControlers[4].SetLerpFn(trackControler.judgeStat.miss, scorePanelAnimCoef);
            scoreStatControlers[5].SetLerpFn(trackControler.judgeStat.holdCatch, scorePanelAnimCoef);
            scoreStatControlers[6].SetLerpFn(trackControler.judgeStat.holdUncatch, scorePanelAnimCoef);
            scoreStatControlers[7].SetLerpFn(trackControler.judgeStat.early, scorePanelAnimCoef);
            scoreStatControlers[8].SetLerpFn(trackControler.judgeStat.late, scorePanelAnimCoef);

            scoreBg.color = Color.Lerp(
                scoreBg.color,
                new Color(1f, 1f, 1f, 0.875f),
                bgAnimCoef
            );
            scorePanelRT.offsetMin = Vector2.Lerp(
                scorePanelRT.offsetMin,
                scorePanelPos,
                scorePanelAnimCoef
            );
            scorePanelRT.offsetMax = Vector2.Lerp(
                scorePanelRT.offsetMax,
                scorePanelPos + scorePanelSize,
                scorePanelAnimCoef
            );
            scorePanelImg.color = Color.Lerp(
                scorePanelImg.color,
                Color.white,
                scorePanelAnimCoef
            );
            backBtnRT.offsetMin = Vector2.Lerp(
                backBtnRT.offsetMin,
                backBtnPos,
                btnAnimCoef
            );
            backBtnRT.offsetMax = Vector2.Lerp(
                backBtnRT.offsetMax,
                backBtnPos + backBtnSize,
                btnAnimCoef
            );
            retryBtnRT.offsetMin = Vector2.Lerp(
                retryBtnRT.offsetMin,
                retryBtnPos,
                btnAnimCoef
            );
            retryBtnRT.offsetMax = Vector2.Lerp(
                retryBtnRT.offsetMax,
                retryBtnPos + retryBtnSize,
                btnAnimCoef
            );
            accText.color = Color.Lerp(
                accText.color,
                Color.white,
                accAnimCoef
            );

            if (!isHighScore.HasValue)
            {
                accuracy = 100f * trackControler.judgeStat.GetAcc();
                isHighScore = trackControler.metadata.TrySetHighScore(accuracy, out highScoreDiff);
            }
            accTextFn = Mathf.Lerp(accTextFn, accuracy, accAnimCoef * 2);
            accText.text = $"{accTextFn:F2}%";
            // print($"{accuracy} {highScoreDiff} {accTextFn}");
            if (isHighScore.Value && Mathf.Abs(accuracy - accTextFn) < 0.01f)
            {
                accRT.offsetMin = Vector2.Lerp(
                    accRT.offsetMin,
                    accPos + accOffset2,
                    accAnimCoef
                );
                accRT.offsetMax = Vector2.Lerp(
                    accRT.offsetMax,
                    accPos + accOffset2 + accSize,
                    accAnimCoef
                );
                bestScoreText.color = Color.Lerp(
                    bestScoreText.color,
                    Color.white,
                    accAnimCoef
                );
                bestScoreText.text = $"{accuracy - highScoreDiff:F2}% + {accuracy - accTextFn + highScoreDiff:F2}%";
                if (accTextFn >= 99.995f) bestScoreText.text += " <sup>_</sup>";
                // bestScoreRT.offsetMin = Vector2.Lerp(
                //     bestScoreRT.offsetMin,
                //     bestScorePos + bestScoreOffset2,
                //     accAnimCoef
                // );
                // bestScoreRT.offsetMax = Vector2.Lerp(
                //     bestScoreRT.offsetMax,
                //     bestScorePos + bestScoreOffset2 + bestScoreSize,
                //     accAnimCoef
                // );
            }
            else
            {
                accRT.offsetMin = Vector2.Lerp(
                    accRT.offsetMin,
                    accPos,
                    accAnimCoef
                );
                accRT.offsetMax = Vector2.Lerp(
                    accRT.offsetMax,
                    accPos + accSize,
                    accAnimCoef
                );
            }
        }
        else
        {
            scoreBg.color = new Color(1f, 1f, 1f, 0f);
            scorePanelRT.offsetMin = scorePanelPos + scorePanelOffset;
            scorePanelRT.offsetMax = scorePanelPos + scorePanelSize + scorePanelOffset;
            scorePanelImg.color = new Color(1f, 1f, 1f, 0f);
            backBtnRT.offsetMin = backBtnPos + btnOffset;
            backBtnRT.offsetMax = backBtnPos + backBtnSize + btnOffset / 2;
            retryBtnRT.offsetMin = retryBtnPos + btnOffset;
            retryBtnRT.offsetMax = retryBtnPos + retryBtnSize + btnOffset;
            accText.color = new Color(1f, 1f, 1f, 0f);
            bestScoreText.color = new Color(1f, 1f, 1f, 0f);
            accRT.offsetMin = accPos + accOffset;
            accRT.offsetMax = accPos + accSize + accOffset;
        }
    }
}
