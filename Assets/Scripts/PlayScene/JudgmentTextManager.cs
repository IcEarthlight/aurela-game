using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentTextManager : PoolManager
{
    public Camera mainCamera;
    public float initialAirYOffset;
    public float initialYOffset;

    public void HitPerfect(Transform hitTransform)
    {
        GameObject obj = GetAvailableObj();
        obj.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            mainCamera.transform.rotation
        );

        Vector3 cameraDirection = (mainCamera.transform.rotation * Vector3.forward).normalized;
        float distanceFactor = Vector3.Dot(
            cameraDirection,
            transform.position - mainCamera.transform.position
        ) / 5.467406f;
        float yOffset = Mathf.Lerp(initialYOffset, initialAirYOffset, hitTransform.position.y / 3.2f);
        obj.transform.position += yOffset * distanceFactor * obj.transform.up;

        obj.GetComponent<JudgmentTextAnimator>().SetSpritePerfect();
        obj.SetActive(true);
    }

    public void HitGreat(Transform hitTransform)
    {
        GameObject obj = GetAvailableObj();
        obj.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            mainCamera.transform.rotation
        );
        
        Vector3 cameraDirection = (mainCamera.transform.rotation * Vector3.forward).normalized;
        float distanceFactor = Vector3.Dot(
            cameraDirection,
            transform.position - mainCamera.transform.position
        ) / 5.467406f;
        float yOffset = Mathf.Lerp(initialYOffset, initialAirYOffset, hitTransform.position.y / 3.2f);
        obj.transform.position += yOffset * distanceFactor * obj.transform.up;

        obj.GetComponent<JudgmentTextAnimator>().SetSpriteGreat();
        obj.SetActive(true);
    }

    public void HitGood(Transform hitTransform)
    {
        GameObject obj = GetAvailableObj();
        obj.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            mainCamera.transform.rotation
        );
        
        Vector3 cameraDirection = (mainCamera.transform.rotation * Vector3.forward).normalized;
        float distanceFactor = Vector3.Dot(
            cameraDirection,
            transform.position - mainCamera.transform.position
        ) / 5.467406f;
        float yOffset = Mathf.Lerp(initialYOffset, initialAirYOffset, hitTransform.position.y / 3.2f);
        obj.transform.position += yOffset * distanceFactor * obj.transform.up;

        obj.GetComponent<JudgmentTextAnimator>().SetSpriteGood();
        obj.SetActive(true);
    }

    public void HitBad(Transform hitTransform)
    {
        GameObject obj = GetAvailableObj();
        obj.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            mainCamera.transform.rotation
        );
        
        Vector3 cameraDirection = (mainCamera.transform.rotation * Vector3.forward).normalized;
        float distanceFactor = Vector3.Dot(
            cameraDirection,
            transform.position - mainCamera.transform.position
        ) / 5.467406f;
        float yOffset = Mathf.Lerp(initialYOffset, initialAirYOffset, hitTransform.position.y / 3.2f);
        obj.transform.position += yOffset * distanceFactor * obj.transform.up;
        
        obj.GetComponent<JudgmentTextAnimator>().SetSpriteBad();
        obj.SetActive(true);
    }

    public void HitMiss(Transform hitTransform)
    {
        GameObject obj = GetAvailableObj();
        obj.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            mainCamera.transform.rotation
        );
        
        Vector3 cameraDirection = (mainCamera.transform.rotation * Vector3.forward).normalized;
        float distanceFactor = Vector3.Dot(
            cameraDirection,
            transform.position - mainCamera.transform.position
        ) / 5.467406f;
        float yOffset = Mathf.Lerp(initialYOffset, initialAirYOffset, hitTransform.position.y / 3.2f);
        obj.transform.position += yOffset * distanceFactor * obj.transform.up;

        obj.GetComponent<JudgmentTextAnimator>().SetSpriteMiss();
        obj.SetActive(true);
    }
}
