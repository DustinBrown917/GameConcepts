using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Remove in favour of GameScreen
[RequireComponent(typeof(CanvasGroup), typeof(SBSManager))]
public class GameScreen : MonoBehaviour {

    private const float STANDARD_FADE_TIME = 0.5f;

    public string Name { get { return gameObject.name; } }

    protected SBSManager sbsManager;
    private CanvasGroup canvasGroup;
    private Coroutine cr_Fading = null;

    /************************************************************************************/
    /********************************* UNITY BEHAVIOURS *********************************/
    /************************************************************************************/

    protected virtual void Awake()
    {
        if(canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        sbsManager = GetComponent<SBSManager>();
        sbsManager.SetHookUpOnEnable(false);
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void Start()
    {
        sbsManager.HookToInput();
        sbsManager.enabled = true;
    }

    protected virtual void OnDisable() { }

    /************************************************************************************/
    /************************************ BEHAVIOURS ************************************/
    /************************************************************************************/

    /// <summary>
    /// Hooks the SBSManager to input.
    /// </summary>
    public void HookUpSBSManager() { sbsManager.HookToInput(); }

    /// <summary>
    /// Unhooks the SBSManager from input.
    /// </summary>
    protected void UnhookSBSManager() { sbsManager.UnHookFromInput(); }

    /// <summary>
    /// Fades the canvas group alpha to 1.0 over the STANDARD_FADE_TIME seconds.
    /// </summary>
    public void FadeIn() {
        TransitionComplete += HookSBSManagerByEvent;
        CoroutineManager.BeginCoroutine(FadeTo(1.0f, STANDARD_FADE_TIME), ref cr_Fading, this);
    }

    /// <summary>
    /// Fades the canvas group alpha to 0 over the STANDARD_FADE_TIME seconds.
    /// </summary>
    public void FadeOut() {
        CoroutineManager.BeginCoroutine(FadeTo(0.0f, STANDARD_FADE_TIME), ref cr_Fading, this);
        UnhookSBSManager();
    }

    /************************************************************************************/
    /************************************ COROUTINES ************************************/
    /************************************************************************************/

    /// <summary>
    /// Fades the canvas group to a specified alpha over a specified number of seconds.
    /// </summary>
    /// <param name="alphaTarget">The target alpha to fade to.</param>
    /// <param name="fadeTime">The time the fade should take.</param>
    /// <returns></returns>
    private IEnumerator FadeTo(float alphaTarget, float fadeTime)
    {
        float initialAlpha = canvasGroup.alpha;
        float elapsedTime = 0;

        while (elapsedTime <= fadeTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(initialAlpha, alphaTarget, elapsedTime / fadeTime);
            yield return null;
        }

        OnTransitionComplete(new TransitionCompleteArgs(this));

        cr_Fading = null;
    }

    /************************************************************************************/
    /********************************* EVENT LISTENERS **********************************/
    /************************************************************************************/

    /// <summary>
    /// Hooks up input after a transition has completed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void HookSBSManagerByEvent(object sender, TransitionCompleteArgs e)
    {
        TransitionComplete -= HookSBSManagerByEvent;
        HookUpSBSManager();
    }

    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/


    #region TransitionComplete Event
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
    #endregion
}
