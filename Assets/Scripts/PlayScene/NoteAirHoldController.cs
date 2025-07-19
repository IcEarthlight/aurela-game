using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteAirHoldController : NoteHoldController
{
    protected Material sideMaterial1, sideMaterial2;
    protected GameObject tail;

    public override void Reposition()
    {
        float hitZ = (hitTimeMs - timer.GetMs()) * trackControler.GetSpeed() / 1000f;
        float releaseZ = (releaseTimeMs - timer.GetMs()) * trackControler.GetSpeed() / 1000f;
        transform.position = new Vector3(x, 0.4f + 2.8f * y, hitZ);
        transform.localScale = new Vector3(1f, 1f, releaseZ - hitZ);

        if (hitZ < trimDistance && timer.GetMs() < releaseTimeMs + BAD_INTERVAL)
        {
            mesh.SetActive(true);
            float eraseBottom = Mathf.Clamp((trimDistance - hitZ) / (releaseZ - hitZ), 0f, 1f);
            material.SetFloat("_EraseBottom", eraseBottom);
            sideMaterial1.SetFloat("_EraseBottom", eraseBottom);
            sideMaterial2.SetFloat("_EraseBottom", eraseBottom);

            material.SetFloat("_EraseBottom", Mathf.Clamp(eraseBottom, 0f, 1f));
            if (isLastCatch) eraseTop = Mathf.Clamp((-hitZ) / (releaseZ - hitZ), 0f, 1f);
            material.SetFloat("_EraseTop", eraseTop);
            sideMaterial1.SetFloat("_EraseTop", eraseTop);
            sideMaterial2.SetFloat("_EraseTop", eraseTop);
        }
        else
        {
            mesh.SetActive(false);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        tail = transform.GetChild(1).gameObject;
        sideMaterial1 = mesh.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        sideMaterial2 = mesh.transform.GetChild(1).GetComponent<MeshRenderer>().material;
    }

    protected override void Update()
    {
        base.Update();
        tail.SetActive(!ToHold && transform.position.z <= trimDistance);
    }
}
