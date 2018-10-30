﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorMenu : MonoBehaviour {

    [SerializeField] protected List<Selectable> selectables;
    [SerializeField] private SelectionMeter selectionMeter;
    [SerializeField] private AudioClip focusNoise;
    [SerializeField] private AudioClip selectNoise;
    [SerializeField] protected Players playerToListenTo;
    private AudioSource audioSource;
    protected int currentlySelectedIndex_ = 0;
    private Coroutine cr_MeterFill;

    private bool hookedToInput = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = true;
    }

    protected virtual void Start()
    {
        FocusOn(currentlySelectedIndex_);
        StartCoroutine(DelayedSelectionMeterUpdate());
        HookToInput();
    }

    protected void OnDisable()
    {
        UnHookFromInput();
    }

    protected void OnDestroy()
    {
        UnHookFromInput();
    }

    /// <summary>
    /// One off call to properly align the meter after Start()
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DelayedSelectionMeterUpdate()
    {
        yield return new WaitForEndOfFrame();
        audioSource.mute = false;
        
    }

    protected virtual void InputGrabber_SelectEvent(object sender, InputGrabber.SelectEventArgs e)
    {
        if(e.player != playerToListenTo) { return; }
        selectables[currentlySelectedIndex_].Select();
        audioSource.clip = selectNoise;
        audioSource.Play();
    }

    protected void InputGrabber_TabEvent(object sender, InputGrabber.TabEventArgs e)
    {
        if(e.player == playerToListenTo)
        {
            FocusNext();
        }
    }

    protected void InputGrabber_InputEventStart(object sender, InputGrabber.InputEventStartArgs e)
    {
        if (e.player == playerToListenTo)
        {
            CoroutineManager.BeginCoroutine(FillInputMeter(), ref cr_MeterFill, this);
        }
    }

    protected IEnumerator FillInputMeter()
    {
        while(selectionMeter.Value < 1)
        {
            selectionMeter.Value = InputGrabber.Instance.GetSelectionTime(playerToListenTo) / InputGrabber.Instance.TimeToSelect;
            selectionMeter.Alpha = selectionMeter.Value;
            yield return null;
        }

        selectionMeter.Value = 0;
        selectionMeter.Alpha = selectionMeter.Value;
        cr_MeterFill = null;
    }


    private void FocusNext()
    {
        selectables[currentlySelectedIndex_].Defocus();
        currentlySelectedIndex_++;
        if(currentlySelectedIndex_ >= selectables.Count)
        {
            currentlySelectedIndex_ = 0;
        }
        FocusOn(currentlySelectedIndex_);
    }

    private void FocusOn(int index)
    {
        if(index < 0 || index > selectables.Count) { index = 0; }
        audioSource.clip = focusNoise;
        audioSource.Play();
        selectables[index].Focus();
    }

    public void HookToInput()
    {
        if (hookedToInput) { return; }

        InputGrabber.Instance.TabEvent += InputGrabber_TabEvent;
        InputGrabber.Instance.SelectEvent += InputGrabber_SelectEvent;
        InputGrabber.Instance.InputEventStart += InputGrabber_InputEventStart;

        hookedToInput = true;
    }

    public void UnHookFromInput()
    {
        if (!hookedToInput) { return; }

        InputGrabber.Instance.TabEvent -= InputGrabber_TabEvent;
        InputGrabber.Instance.SelectEvent -= InputGrabber_SelectEvent;
        InputGrabber.Instance.InputEventStart -= InputGrabber_InputEventStart;

        hookedToInput = false;
    }

    public void AddSelectable(Selectable s)
    {
        selectables.Add(s);
    }

    public void RemoveSelectable(Selectable s)
    {
        selectables.Remove(s);
    }
}