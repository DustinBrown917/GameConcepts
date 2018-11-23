using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class SingleButtonSelectable : MonoBehaviour {

    [SerializeField] private Behaviour[] focusBehaviours;
    [SerializeField] private Behaviour[] selectedBehaviours;
    [SerializeField] private AudioClip focusAudio;
    [SerializeField] private AudioClip selectedAudio;
    [SerializeField] private int group_ = 0;
    public int Group { get { return group_; } }
    [SerializeField] private int tabIndex_ = 0;
    public int TabIndex { get { return tabIndex_; } }
    private AudioSource audioSource;

    private bool hasFocus_ = false;
    public bool HasFocus { get { return hasFocus_; } }

    public UnityEvent OnSelect;
    public UnityEvent OnFocus;

	private void Awake () {
        SetSelectedBehaviourEnabled(false);
        SetFocusBehaviourEnabled(false);
        audioSource = GetComponent<AudioSource>();
	}

    public void SetTabIndex(int index)
    {
        tabIndex_ = index;
    }

    public void SetGroupIndex(int index)
    {
        group_ = index;
    }

    public void Focus(bool playNoise = true)
    {
        SetFocusBehaviourEnabled(true);
        if (playNoise)
        {
            audioSource.clip = focusAudio;
            audioSource.Play();
        }
        OnFocus.Invoke();
        //OnFocusReceived();
    }

    public void Defocus()
    {
        SetFocusBehaviourEnabled(false);
        OnFocusLost();
    }

    public void Select(bool playNoise = true)
    {
        SetSelectedBehaviourEnabled(true);
        if (playNoise)
        {
            audioSource.clip = selectedAudio;
            audioSource.Play();
        }
        OnSelect.Invoke();
    }

    private void SetFocusBehaviourEnabled(bool b)
    {
        hasFocus_ = b;
        for(int i = 0; i < focusBehaviours.Length; i++)
        {
            focusBehaviours[i].enabled = b;
        }
    }

    private void SetSelectedBehaviourEnabled(bool b)
    {
        for (int i = 0; i < selectedBehaviours.Length; i++)
        {
            selectedBehaviours[i].enabled = b;
        }
    }


    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

    //public event EventHandler FocusReceived;

    //private void OnFocusReceived()
    //{
    //    EventHandler handler = FocusReceived;

    //    if (handler != null)
    //    {
    //        handler(this, EventArgs.Empty);
    //    }
    //}

    public event EventHandler FocusLost;

    private void OnFocusLost()
    {
        EventHandler handler = FocusLost;

        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
