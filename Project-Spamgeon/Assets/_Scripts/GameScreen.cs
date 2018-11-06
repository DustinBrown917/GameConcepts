using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Remove in favour of GameScreen
[RequireComponent(typeof(CanvasGroup))]
public class GameScreen : MonoBehaviour {

    private const float STANDARD_FADE_TIME = 0.5f;

    [SerializeField] private SelectorMenu[] selectorMenus;

    public string Name { get { return gameObject.name; } }

    private CanvasGroup canvasGroup;

    private Coroutine cr_Fading = null;

    private void Awake()
    {
        if(canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void HookUpSelectorMenus()
    {
        if(selectorMenus.Length == 0) { return; }
        for(int i = 0; i < selectorMenus.Length; i++)
        {
            selectorMenus[i].HookToInput();
        }
    }

    private void UnhookSelectorMenus()
    {
        if (selectorMenus.Length == 0) { return; }
        for (int i = 0; i < selectorMenus.Length; i++)
        {
            selectorMenus[i].UnHookFromInput();
        }
    }

    public void FadeIn()
    {
        CoroutineManager.BeginCoroutine(FadeTo(1.0f, STANDARD_FADE_TIME), ref cr_Fading, this);
    }

    public void FadeOut()
    {
        CoroutineManager.BeginCoroutine(FadeTo(0.0f, STANDARD_FADE_TIME), ref cr_Fading, this);
    }

    private IEnumerator FadeTo(float alphaTarget, float fadeTime)
    {
        float initialAlpha = canvasGroup.alpha;
        float elapsedTime = 0;

        if(alphaTarget == 0) { UnhookSelectorMenus(); }

        while (elapsedTime <= fadeTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(initialAlpha, alphaTarget, elapsedTime / fadeTime);
            yield return null;
        }

        if(alphaTarget == 1.0f) { HookUpSelectorMenus(); }

        OnTransitionComplete(new TransitionCompleteArgs(this));

        cr_Fading = null;
    }


    public event EventHandler<TransitionCompleteArgs> TransitionComplete;

    public class TransitionCompleteArgs : EventArgs
    {
        public GameScreen screen;

        public TransitionCompleteArgs(GameScreen s)
        {
            screen = s;
        }
    }

    public void OnTransitionComplete(TransitionCompleteArgs e)
    {
        EventHandler<TransitionCompleteArgs> handler = TransitionComplete;

        if(handler != null)
        {
            handler(this, e);
        }
    }
}
