using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyHistPoolManager : PoolManager
{
    public int yInterval;
    public int histHeight;

    public override GameObject GenerateObj()
    {
        return Instantiate(instance, transform);
    }

    static float ParabolicEaseOut(float x)
    {
        return -x * x + 2 * x;
    }

    static float CubicEaseOut(float x)
    {
        return x*x*x - 3*x*x + 3*x;
    }

    static float Pow15EaseOut(float x)
    {
        return 1 - Mathf.Pow(1 - x, 1.5f);
    }

    private void DrawTimePeriod(
        KeyInspector keyInspector,
        int x,
        float timeMs,
        float startTime,
        float endTime)
    {
        GameObject obj = GetAvailableObj();
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        float startY = keyInspector.startY - keyInspector.width - yInterval;

        rectTransform.offsetMin = new Vector2(
            x - keyInspector.width,
            startY - ParabolicEaseOut((timeMs - startTime) / KeyInspector.MAX_HIST_MS) * histHeight
        );
        rectTransform.offsetMax = new Vector2(
            x,
            startY - ParabolicEaseOut((timeMs - endTime) / KeyInspector.MAX_HIST_MS) * histHeight
        );
        obj.GetComponent<Image>().color = Color.Lerp(
            keyInspector.normalColor,
            new Color(1, 1, 1, 0),
            Pow15EaseOut((timeMs - (startTime + endTime) / 2) / KeyInspector.MAX_HIST_MS)
        );
        obj.SetActive(true);
    }

    public void UpdateKeyHist(
        KeyInspector keyInspector,
        float timeMs,
        Queue<KeyInspector.KeyEvent>[] keyEvents,
        bool[] lastKeyDown)
    {
        int x = keyInspector.startX;
        RecycleAll();

        for (int i = keyEvents.Length - 1; i >= 0; i--)
        {
            float downTime = timeMs - KeyInspector.MAX_HIST_MS;
            foreach (KeyInspector.KeyEvent keyEvent in keyEvents[i])
            {
                if (keyEvent.isKeyDown)
                {
                    downTime = keyEvent.timeMs;
                }
                else
                {
                    DrawTimePeriod(keyInspector, x, timeMs, downTime, keyEvent.timeMs);
                }
            }
            if (lastKeyDown[i])
            {
                DrawTimePeriod(keyInspector, x, timeMs, downTime, timeMs);
            }
            x -= keyInspector.width + keyInspector.spacing;
        }
    }
}
