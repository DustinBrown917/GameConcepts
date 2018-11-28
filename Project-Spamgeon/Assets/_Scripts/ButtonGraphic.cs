using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonGraphic : MonoBehaviour {

    [SerializeField] private Sprite pressedGraphic;
    [SerializeField] private Sprite depressedGraphic;
    [SerializeField] private float depressedPhaseTime;
    [SerializeField] private float pressedPhaseTime;
    private Image graphic;
    private Coroutine cr_phase;


    private void Awake()
    {
        graphic = GetComponent<Image>();
    }

    private void OnEnable()
    {
        CoroutineManager.BeginCoroutine(DepressedPhase(depressedPhaseTime), ref cr_phase, this);
    }

    private void OnDisable()
    {
        CoroutineManager.HaltCoroutine(ref cr_phase, this);
    }

    private IEnumerator PressedPhase(float time)
    {
        graphic.sprite = pressedGraphic;
        yield return new WaitForSeconds(time);
        CoroutineManager.BeginCoroutine(DepressedPhase(depressedPhaseTime), ref cr_phase, this);
    }

    private IEnumerator DepressedPhase(float time)
    {
        graphic.sprite = depressedGraphic;
        yield return new WaitForSeconds(time);
        CoroutineManager.BeginCoroutine(PressedPhase(pressedPhaseTime), ref cr_phase, this);
    }
}
