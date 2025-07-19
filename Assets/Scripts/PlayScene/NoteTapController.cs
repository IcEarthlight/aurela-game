using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteTapController : NoteController
{
    public override void Reposition()
    {
        base.Reposition();
        transform.localScale = new Vector3(
            1f, 1f,
            Math.Min(Math.Max(transform.position.z, 0f), trimDistance) / trimDistance / 2 + 1f
        );
    }
}
