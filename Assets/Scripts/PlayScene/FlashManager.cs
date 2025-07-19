using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashManager : PoolManager
{
    public Camera mainCamera;

    public void HitPerfect(Transform hitTransform)
    {
        GameObject obj = GetAvailableObj();
        obj.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            mainCamera.transform.rotation
        );
        obj.GetComponent<FlashAnimator>().SetSpritePerfect();
        obj.SetActive(true);
    }

    public void HitGreat(Transform hitTransform)
    {
        GameObject obj = GetAvailableObj();
        obj.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            mainCamera.transform.rotation
        );
        obj.GetComponent<FlashAnimator>().SetSpriteGreat();
        obj.SetActive(true);
    }

    public void HitGood(Transform hitTransform)
    {
        GameObject obj = GetAvailableObj();
        obj.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            mainCamera.transform.rotation
        );
        obj.GetComponent<FlashAnimator>().SetSpriteGood();
        obj.SetActive(true);
    }

    public void HitBad(Transform hitTransform) { }
}
