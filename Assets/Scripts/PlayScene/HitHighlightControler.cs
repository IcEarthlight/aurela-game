using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitHighlightControler : MonoBehaviour
{
    public int TrackId;
    public Color normalColor;
    public Color touchColor;
    public Color highlightColor;
    public float touchLerpFactor = 0.5f;
    public float normalLerpFactor = 0.5f;
    public float scaleLerpFactor;
    public Vector3 targetScale;

    private TrackControler trackControler;
    private Material material;
    private Color color;
    private Vector3 originalScale;

    void Start()
    {
        trackControler = transform.parent.GetComponent<TrackControler>();
        material = GetComponent<MeshRenderer>().material;
        color = normalColor;
        material.SetColor("_Color", color);
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(trackControler.keyBindings[TrackId]))
            color = highlightColor;
        else if (Input.GetKey(trackControler.keyBindings[TrackId]))
            color = Color.Lerp(color, touchColor, touchLerpFactor);
        else
            color = Color.Lerp(color, normalColor, normalLerpFactor);

        material.SetColor("_Color", color);

        if (Input.GetKeyDown(trackControler.keyBindings[TrackId]))
            transform.localScale = originalScale;
        else
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleLerpFactor);
    }
}
