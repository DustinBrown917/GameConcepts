using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class Selectable : MonoBehaviour {

    [SerializeField] private Outline focusOutline;
    public bool HasFocus { get { return focusOutline.enabled; } }

	private void Awake () {
		if(focusOutline == null)
        {
            focusOutline = GetComponent<Outline>();
        }
        focusOutline.enabled = false;
	}

    public void Focus()
    {
        focusOutline.enabled = true;
        OnFocusReceived();
    }

    public void Defocus()
    {
        focusOutline.enabled = false;
        OnFocusLost();
    }

    public void Select()
    {
        
    }

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
