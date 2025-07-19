using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccUIControler2 : MonoBehaviour
{
    public TrackControler trackControler;
    public Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = $"{100f * trackControler.judgeStat.GetAcc():F2}%";
    }
}
