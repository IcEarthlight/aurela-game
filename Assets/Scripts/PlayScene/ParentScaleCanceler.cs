using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentScaleCanceler : MonoBehaviour
{
    public Vector3 position;
    public Vector3 scale;

    void OnEnable()
    {
        Update();
    }

    void Update()
    {
        transform.localPosition = new Vector3(
            position.x / transform.parent.transform.localScale.x,
            position.y / transform.parent.transform.localScale.y,
            position.z / transform.parent.transform.localScale.z
        );
        transform.localScale = new Vector3(
            scale.x / transform.parent.transform.localScale.x,
            scale.y / transform.parent.transform.localScale.y,
            scale.z / transform.parent.transform.localScale.z
        );
    }
}
