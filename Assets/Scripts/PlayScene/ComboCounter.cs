using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    public int Count { get; private set; } = 0;
    public int MaxCombo { get; private set; } = 0;
    private Text text;

    public void Combo()
    {
        Count++;
        if (Count > MaxCombo)
            MaxCombo = Count;
    }

    public void Break()
    {
        Count = 0;
    }

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = Count > 0 ? Count.ToString() : "";
    }
}
