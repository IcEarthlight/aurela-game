using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObjectAnimator : MonoBehaviour
{
    public PoolManager manager;
    public AudioTimer timer;
    public float animationSpeed;

    protected Animator animator;

    public virtual void Init(PoolManager poolManager)
    {
        this.manager = poolManager;
        this.timer = poolManager.timer;
    }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        animator.speed = timer.IsPaused ? 0f : animationSpeed;
    }

    public virtual void AnimationEnded()
    {
        gameObject.SetActive(false);
        manager.AnimationEndedCallback(gameObject);
    }
}