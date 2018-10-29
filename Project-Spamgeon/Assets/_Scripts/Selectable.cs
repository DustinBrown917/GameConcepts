using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Selectable : MonoBehaviour {

    [SerializeField] private SelectionMeter selectionMeter_;
    public SelectionMeter SelectionSlider { get { return selectionMeter_; } }
    [SerializeField] private Outline focusOutline;
    [SerializeField] private Color selectedColor;
    private Color defaultOutlineColor;
    public bool HasFocus { get { return focusOutline.enabled; } }

    public UnityEvent OnSelect;

	private void Awake () {
		if(focusOutline == null)
        {
            focusOutline = GetComponent<Outline>();
        }
        defaultOutlineColor = focusOutline.effectColor;
        focusOutline.enabled = false;
	}

    public void Focus()
    {
        focusOutline.enabled = true;
        focusOutline.effectColor = defaultOutlineColor;
        OnFocusReceived();
    }

    public void Defocus()
    {
        focusOutline.enabled = false;
        OnFocusLost();
    }

    public void Select()
    {
        focusOutline.effectColor = selectedColor;
        OnSelect.Invoke();
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
