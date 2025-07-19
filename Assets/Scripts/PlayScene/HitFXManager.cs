using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFXManager : PoolManager
{
    public void HitPerfect(Transform hitTransform)
    {
        GameObject hitFX = GetAvailableObj();
        hitFX.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            hitTransform.rotation
        );
        hitFX.SetActive(true);
    }

    public void HitGood(Transform hitTransform)
    {
        GameObject hitFX = GetAvailableObj();
        hitFX.transform.SetPositionAndRotation(
            new Vector3(hitTransform.position.x, hitTransform.position.y, 0f),
            hitTransform.rotation
        );
        hitFX.SetActive(true);
    }

    public void HitBad(Transform hitTransform) { }
}
