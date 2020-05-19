using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHandler : Singleton<CoroutineHandler>
{
    public static Coroutine StartStaticCoroutine(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }
}
