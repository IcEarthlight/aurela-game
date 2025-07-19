using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentTextAnimator : PooledObjectAnimator
{
    public Camera mainCamera;
    public SpriteRenderer spriteRenderer;
    public Sprite perfectSprite;
    public Sprite greatSprite;
    public Sprite goodSprite;
    public Sprite badSprite;
    public Sprite missSprite;
    public float scaleFactor = 1f;
    public float upSpeed;

    public override void Init(PoolManager poolManager)
    {
        base.Init(poolManager);
        mainCamera = ((JudgmentTextManager)poolManager).mainCamera;
    }

    public void SetSpritePerfect() { spriteRenderer.sprite = perfectSprite; }
    public void SetSpriteGreat() { spriteRenderer.sprite = greatSprite; }
    public void SetSpriteGood() { spriteRenderer.sprite = goodSprite; }
    public void SetSpriteBad() { spriteRenderer.sprite = badSprite; }
    public void SetSpriteMiss() { spriteRenderer.sprite = missSprite; }

    protected override void Update()
    {
        base.Update();
        transform.rotation = mainCamera.transform.rotation;

        Vector3 cameraDirection = (mainCamera.transform.rotation * Vector3.forward).normalized;
        float distanceFactor = Vector3.Dot(
            cameraDirection,
            transform.position - mainCamera.transform.position
        ) / 5.467406f;

        if (!timer.IsPaused)
            transform.position += upSpeed * distanceFactor * transform.up;
        
        float scale = distanceFactor * scaleFactor;
        transform.localScale = new Vector3(scale, scale, 1);
    }

    public override void AnimationEnded()
    {
        spriteRenderer.sprite = null;
        base.AnimationEnded();
    }
}