using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Selectable : MonoBehaviour {

    [SerializeField] private Behaviour[] focusBehaviours;
    [SerializeField] private Behaviour[] selectedBehaviours;
    private bool hasFocus_ = false;
    public bool HasFocus { get { return hasFocus_; } }

    public UnityEvent OnSelect;

	private void Awake () {
        SetSelectedBehaviourEnabled(false);
        SetFocusBehaviourEnabled(false);
	}

    public void Focus()
    {
        SetFocusBehaviourEnabled(true);
        OnFocusReceived();
    }

    public void Defocus()
    {
        SetFocusBehaviourEnabled(false);
        OnFocusLost();
    }

    public void Select()
    {
        SetSelectedBehaviourEnabled(true);
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

    public event EventHandler FocusReceived;

    private void OnFocusReceived()
    {
        EventHandler handler = FocusReceived;

        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }

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
