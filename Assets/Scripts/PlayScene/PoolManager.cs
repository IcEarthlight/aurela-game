using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public AudioTimer timer;
    public GameObject instance;
    protected readonly List<GameObject> objList = new();
    protected readonly Queue<GameObject> idleObjs = new();

    public void OnEnable()
    {
        AudioTimer.Backtrack += RecycleAll;
    }

    public void OnDisable()
    {
        AudioTimer.Backtrack -= RecycleAll;
    }

    public virtual GameObject GenerateObj()
    {
        GameObject newObj = Instantiate(instance, transform);
        PooledObjectAnimator objAnimator = newObj.GetComponent<PooledObjectAnimator>();
        objAnimator.Init(this);
        return newObj;
    }

    public GameObject GetAvailableObj()
    {
        if (idleObjs.Count > 0)
        {
            GameObject obj = idleObjs.Dequeue();
            return obj;
        }
        GameObject newObj = GenerateObj();
        objList.Add(newObj);
        return newObj;
    }

    public void AnimationEndedCallback(GameObject obj)
    {
        idleObjs.Enqueue(obj);
    }

    public void RecycleAll()
    {
        idleObjs.Clear();
        for (int i = 0; i < objList.Count; i++)
        {
            objList[i].SetActive(false);
            idleObjs.Enqueue(objList[i]);
        }
    }

    public void RecycleAll(float t) { RecycleAll(); }

    public void Clear()
    {
        for (int i = 0; i < objList.Count; i++)
            Destroy(objList[i]);
        objList.Clear();
        idleObjs.Clear();
    }
}