using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineWithData : MonoBehaviour {

    public Coroutine Coroutine { get; private set; }
    public object Result;
    private IEnumerator _target;
    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        _target = target;
        this.Coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (_target.MoveNext())
        {
            Result = _target.Current;
            yield return Result;
        }
    }
}
