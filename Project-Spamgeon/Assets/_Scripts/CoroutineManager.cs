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

    public static IEnumerator ShrinkScaleFrom(Transform trans, Vector3 initialScale, Vector3 targetScale, float shrinkTime, Coroutine container)
    {
        trans.localScale = initialScale;
        float elapsedTime = 0;
        while(elapsedTime < shrinkTime)
        {
            elapsedTime += Time.deltaTime;
            trans.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / shrinkTime);
            yield return null;
        }
        trans.localScale = targetScale;
        container = null;
    }

    public static IEnumerator FadeAlphaTo(CanvasGroup cg, float initialAlpha, float targetAlpha, float fadeTime, bool disableWhenDone, Coroutine container)
    {
        cg.alpha = initialAlpha;
        float elapsedTime = 0;

        while(elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsedTime / fadeTime);
            yield return null;
        }
        cg.alpha = targetAlpha;

        if (disableWhenDone)
        {
            cg.gameObject.SetActive(false);
        }

        container = null;
    }
}
