using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulHintControler : MonoBehaviour
{
    public AudioTimer timer;
    public TrackControler trackControler;
    public NoteController note1;
    public NoteController note2;
    public float lineWidth;
    public float trimDistance;
    public float hitTimeMs;

    private MeshRenderer mesh;

    public void Init(
        TrackControler trackControler,
        NoteController note1,
        NoteController note2
    )
    {
        this.trackControler = trackControler;
        timer = trackControler.timer;
        this.note1 = note1;
        this.note2 = note2;
        hitTimeMs = note1.hitTimeMs;
    }

    void OnEnable()
    {
        AudioTimer.Backtrack += Reposition;
    }

    void OnDisable()
    {
        AudioTimer.Backtrack -= Reposition;
    }

    public void Reposition(float newTimeMs) { Reposition(); }

    public void Reposition()
    {
        transform.localPosition = (note2.transform.position - note1.transform.position) / 2;
        transform.rotation = Quaternion.Euler(
            -Mathf.Sign(transform.localPosition.y) * Vector2.Angle(Vector2.right, transform.localPosition),
            90f, -90f
        );
        transform.localScale = new Vector3(
            lineWidth, 1f,
            Mathf.Sqrt(Mathf.Pow(transform.localPosition.x, 2) + Mathf.Pow(transform.localPosition.y, 2)) / 5f
        );
    }

    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        bool isHit = note1.IsHit || note1 is NoteHoldController hold1 && hold1.ToHold ||
                     note2.IsHit || note2 is NoteHoldController hold2 && hold2.ToHold;
        if (!timer.IsPaused && !isHit)
        {
            Reposition();
        }
        mesh.enabled = !isHit && transform.position.z <= trimDistance;
    }
}
