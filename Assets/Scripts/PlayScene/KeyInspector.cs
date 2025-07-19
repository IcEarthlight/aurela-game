using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyInspector : MonoBehaviour
{
    public struct KeyEvent
    {
        public float timeMs;
        public bool isKeyDown;
        public KeyEvent(float timeMs, bool isKeyDown) =>
            (this.timeMs, this.isKeyDown) = (timeMs, isKeyDown);
    }


    public AudioTimer timer;
    public TrackControler trackControler;
    public KeyHistPoolManager keyHistPoolManager;

    public int startX, width, spacing, startY, textSize;
    public Color normalColor;
    public Color textActivateColor;
    public GameObject[] inspectors;
    public GameObject[] halos;
    public Image[] labels;
    public Text[] texts;
    public const float MAX_HIST_MS = 500f;
    private bool[] lastKeyDown;
    private Queue<KeyEvent>[] keyEvents;

    public static string KeyCodeToSymbol(KeyCode key)
    {
        if (key >= KeyCode.A && key <= KeyCode.Z)
            return key.ToString(); // A-Z

        if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
            return ((char)('0' + (key - KeyCode.Alpha0))).ToString(); // 0-9

        return key switch
        {
            KeyCode.Space => "␣",
            KeyCode.Tab => "⇥",
            KeyCode.Return or KeyCode.KeypadEnter => "↵",
            KeyCode.Backspace => "⌫",
            KeyCode.Escape => "⎋",
            KeyCode.LeftArrow => "←",
            KeyCode.RightArrow => "→",
            KeyCode.UpArrow => "↑",
            KeyCode.DownArrow => "↓",
            _ => key.ToString(),
        };
    }

    // void ClearKeyEvents(float newTimeMs)
    // {
    //     for (int i = 0; i < keyEvents.Length; i++)
    //         keyEvents[i].Clear();
    // }

    // void OnEnable()
    // {
    //     AudioTimer.Backtrack += ClearKeyEvents;
    // }

    // void OnDisable()
    // {
    //     AudioTimer.Backtrack -= ClearKeyEvents;
    // }

    void Awake()
    {
        halos = new GameObject[inspectors.Length];
        for (int i = 0; i < halos.Length; i++)
        {
            halos[i] = inspectors[i].transform.Find("Halo").gameObject;
        }

        labels = new Image[inspectors.Length];
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i] = inspectors[i].GetComponent<Image>();
            labels[i].color = normalColor;
            labels[i].fillCenter = false;
        }

        int x = startX;
        for (int i = labels.Length - 1; i >= 0; i--)
        {
            RectTransform rectTransform = labels[i].GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(x - width, startY - width);
            rectTransform.offsetMax = new Vector2(x, startY);
            x -= width + spacing;
        }

        texts = new Text[inspectors.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i] = inspectors[i].transform.GetComponentInChildren<Text>();
            texts[i].color = normalColor;
            texts[i].fontSize = textSize;
        }
        for (int i = 0; i < trackControler.keyBindings.Length; i++)
        {
            texts[i].text = KeyCodeToSymbol(trackControler.keyBindings[i]);
        }

        lastKeyDown = new bool[trackControler.keyBindings.Length];

        keyEvents = new Queue<KeyEvent>[trackControler.keyBindings.Length];
        for (int i = 0; i < keyEvents.Length; i++)
        {
            keyEvents[i] = new();
        }
    }

    void Update()
    {
        float timeMs = Time.time * 1000f;
        for (int i = 0; i < trackControler.keyBindings.Length; i++)
        {
            bool keyDown = Input.GetKey(trackControler.keyBindings[i]);

            labels[i].fillCenter = keyDown;
            halos[i].SetActive(!keyDown && !timer.IsEnded);
            texts[i].color = keyDown ? textActivateColor : normalColor;

            if (keyDown != lastKeyDown[i])
            {
                keyEvents[i].Enqueue(new KeyEvent(timeMs, keyDown));
            }

            lastKeyDown[i] = keyDown;

            while (keyEvents[i].Count > 0 && keyEvents[i].Peek().timeMs <= timeMs - MAX_HIST_MS)
            {
                keyEvents[i].Dequeue();
            }
        }
        keyHistPoolManager.UpdateKeyHist(this, timeMs, keyEvents, lastKeyDown);
    }
}
