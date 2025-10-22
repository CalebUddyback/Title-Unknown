using System.Collections;
using UnityEngine;

public class CoroutineWithData
{
    public Coroutine coroutine { get; private set; }
    public object result;
    private IEnumerator target;

    bool stop = false;

    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext() && !stop)
        {
            result = target.Current;
            yield return result;
        }
    }

    public void Stop() => stop = true;

}
