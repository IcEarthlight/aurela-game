using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreStatControler : MonoBehaviour
{
    public string desc;
    public int n = 0;
    public float fn = 0f;
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetN(int n)
    {
        this.n = n;
        text.text = desc + n;
    }

    public void SetFn(float fn)
    {
        this.fn = fn;
        n = Mathf.RoundToInt(this.fn);
        text.text = desc + n;
    }

    public void SetLerpFn(float fn, float k)
    {
        this.fn = Mathf.Lerp(this.fn, fn, k);
        n = Mathf.RoundToInt(this.fn);
        text.text = desc + n;
    }

    public int GetN(int n)
    {
        return n;
    }

    public float GetFn(float fn)
    {
        return fn;
    }
}
