using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorMenu : MonoBehaviour {

    [SerializeField] private Selectable[] selectables;
    private int currentlySelectedIndex = 0;

    private void OnEnable()
    {
        InputGrabber.Instance.TabEvent += InputGrabber_TabEvent;
        InputGrabber.Instance.SelectEvent += InputGrabber_SelectEvent;
    }

    private void Start()
    {
        FocusOn(currentlySelectedIndex);
    }

    private void InputGrabber_SelectEvent(object sender, InputGrabber.SelectEventArgs e)
    {
        if(e.player == Players.SINGLE)
        {
            Debug.Log("Selected index " + currentlySelectedIndex.ToString());
        }
    }

    private void InputGrabber_TabEvent(object sender, InputGrabber.TabEventArgs e)
    {
        if(e.player == Players.SINGLE)
        {
            FocusNext();
        }
    }

    private void OnDisable()
    {
        InputGrabber.Instance.TabEvent -= InputGrabber_TabEvent;
        InputGrabber.Instance.SelectEvent -= InputGrabber_SelectEvent;
    }

    private void FocusNext()
    {
        selectables[currentlySelectedIndex].Defocus();
        currentlySelectedIndex++;
        if(currentlySelectedIndex >= selectables.Length)
        {
            currentlySelectedIndex = 0;
        }
        FocusOn(currentlySelectedIndex);
    }

    private void FocusOn(int index)
    {
        if(index < 0 || index > selectables.Length) { index = 0; }
        selectables[index].Focus();
    }
}
