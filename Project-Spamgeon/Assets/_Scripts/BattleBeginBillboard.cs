using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BattleBeginBillboard : MonoBehaviour {

    [SerializeField] private Text text;
    [SerializeField] private ButtonGraphic spaceBarGraphic;
    [SerializeField] private Vector3 pulseScale;
    [SerializeField] private float pulseTime;
    [SerializeField] private float fadeTime;
    private CanvasGroup canvasGroup;
    private Coroutine cr_TextPulse;
    private Coroutine cr_FadeOut;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        canvasGroup.alpha = 1;
    }

    public void SetText(string t, bool withPulse)
    {
        text.text = t;
        if (withPulse)
        {
            CoroutineManager.BeginCoroutine(CoroutineManager.ShrinkScaleFrom(text.transform, pulseScale, Vector3.one, pulseTime, cr_TextPulse), ref cr_TextPulse, this);
        }
    }

    public void SetSpaceBarEnabled(bool b)
    {
        spaceBarGraphic.gameObject.SetActive(b);
    }

    public void FadeOut()
    {
        CoroutineManager.BeginCoroutine(CoroutineManager.FadeAlphaTo(canvasGroup, 1.0f, 0.0f, fadeTime, true, cr_FadeOut), ref cr_FadeOut, this);
    }
}
