using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashAnimator : PooledObjectAnimator
{
    public Camera mainCamera;
    public SpriteRenderer spriteRenderer;
    public Color perfectColor;
    public Color greatColor;
    public Color goodColor;
    public float perfectScale;
    public float greatScale;
    public float goodScale;
    public float scaleFactor = 1f;
    public float scaleFactor2 = 1f;

    public override void Init(PoolManager poolManager)
    {
        base.Init(poolManager);
        mainCamera = ((FlashManager)poolManager).mainCamera;
    }

    public void SetSpritePerfect() { spriteRenderer.color = perfectColor; scaleFactor2 = perfectScale; }
    public void SetSpriteGreat() { spriteRenderer.color = greatColor; scaleFactor2 = greatScale; }
    public void SetSpriteGood() { spriteRenderer.color = goodColor; scaleFactor2 = goodScale; }

    protected override void Update()
    {
        base.Update();
        transform.rotation = mainCamera.transform.rotation;

        Vector3 cameraDirection = (mainCamera.transform.rotation * Vector3.forward).normalized;
        float scale = Vector3.Dot(
            cameraDirection,
            transform.position - mainCamera.transform.position
        ) / 7.283543f * scaleFactor * scaleFactor2;
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
