using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineManager {

	
    public static void BeginCoroutine(IEnumerator routine, ref Coroutine container, MonoBehaviour parentBehaviour)
    {
        HaltCoroutine(ref container, parentBehaviour);

        container = parentBehaviour.StartCoroutine(routine);
    }

    public static void HaltCoroutine(ref Coroutine container, MonoBehaviour parentBehaviour)
    {
        if(container != null)
        {
            parentBehaviour.StopCoroutine(container);
            container = null;
        }
    }

    public static IEnumerator EnableAfterDelay(GameObject obj, float delay, Coroutine container)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
        container = null;
    }

    public static IEnumerator EnableAfterDelay(MonoBehaviour behaviour, float delay, Coroutine container)
    {
        yield return new WaitForSeconds(delay);
        behaviour.enabled = true;
        container = null;
    }
}
