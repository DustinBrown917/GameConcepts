using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGrabber : MonoBehaviour {

    private static InputGrabber instance_;
    public static InputGrabber Instance { get { return instance_; } }

    [SerializeField] private float[] playerTimers;
    [SerializeField] private KeyCode[] playerKeys;
    private Coroutine[] timerCoroutines;
    [SerializeField] private float timeToSelect = 1.0f;
    public float TimeToSelect { get { return timeToSelect; } }
    private bool inputBlocked_ = false;
    public bool InputBlocked { get { return inputBlocked_; } }
    /*
     * Need to set up an array that will store which players' input is blocked. 
     */

    /************************************************************************************/
    /********************************* UNITY BEHAVIOURS *********************************/
    /************************************************************************************/

    private void Awake()
    {
        if(instance_ != null)
        {
            Destroy(gameObject);
            return;
        }
        instance_ = this;

        playerTimers = new float[playerKeys.Length];
        timerCoroutines = new Coroutine[playerKeys.Length];
    }

    private void Update()
    {
        if (inputBlocked_) { return; }

        for(int i = 0; i < playerKeys.Length; i++)
        {
            if (Input.GetKeyDown(playerKeys[i]))
            {
                StartSelectionTimer(i);
            }
        }
    }

    /************************************************************************************/
    /************************************ BEHAVIOURS ************************************/
    /************************************************************************************/

    /// <summary>
    /// Returns the time accumulated in a specific timer.
    /// </summary>
    /// <param name="index">The index of the timer to get.</param>
    /// <returns>The time accumulated in the timer.</returns>
    public float GetSelectionTime(int index)
    {
        if(index < 0 || index >= playerTimers.Length) { return -1; }
        return playerTimers[index];
    }

    /// <summary>
    /// Begins running a selection timer.
    /// </summary>
    /// <param name="index">The timer to run.</param>
    private void StartSelectionTimer(int index)
    {
        CoroutineManager.HaltCoroutine(ref timerCoroutines[index], this);
        CoroutineManager.BeginCoroutine(SelectionTimer(index, timerCoroutines[index]), ref timerCoroutines[index], this);
        OnInputEventStart(new InputEventStartArgs(index));
    }

    /// <summary>
    /// Runs a timer. If the threshold is met, a selection event is fired. If it is not, a tab event is fired.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    private IEnumerator SelectionTimer(int index, Coroutine container)
    {
        int playerIndex = index;
        while (Input.GetKey(playerKeys[playerIndex]) && playerTimers[playerIndex] < timeToSelect) {
            playerTimers[playerIndex] += Time.deltaTime;
            yield return null;
        }

        if (playerTimers[playerIndex] >= timeToSelect)  {
            OnSelectEvent(new SelectEventArgs(index));
        }
        else {
            OnTabEvent(new TabEventArgs(index));
        }
        playerTimers[playerIndex] = 0;
        container = null;
    }

    /// <summary>
    /// Halts and resets the input timer at the given index.
    /// </summary>
    /// <param name="index">The player index to stop.</param>
    private void CancelInput(int index)
    {
        CoroutineManager.HaltCoroutine(ref timerCoroutines[index], this);
        playerTimers[index] = 0;
    }



    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

    #region TabEvent Event
    public event EventHandler<TabEventArgs> TabEvent;

    public class TabEventArgs : EventArgs
    {
        public int playerIndex;

        public TabEventArgs(int p)
        {
            playerIndex = p;
        }      
    }

    private void OnTabEvent(TabEventArgs t)
    {
        EventHandler<TabEventArgs> handler = TabEvent;

        if(handler != null)
        {
            handler(this, t);
        }

        //Debug.Log("Tab Event for " + t.player.ToString());
    }
    #endregion

    #region SelectEvent Event
    public event EventHandler<SelectEventArgs> SelectEvent;

    public class SelectEventArgs : EventArgs
    {
        public int playerIndex;

        public SelectEventArgs(int p)
        {
            playerIndex = p;
        }
    }

    private void OnSelectEvent(SelectEventArgs s)
    {
        EventHandler<SelectEventArgs> handler = SelectEvent;

        if (handler != null)
        {
            handler(this, s);
        }

        //Debug.Log("Select Event for " + s.player.ToString());
    }
    #endregion

    #region InputEventStart Event
    public event EventHandler<InputEventStartArgs> InputEventStart;

    public class InputEventStartArgs : EventArgs
    {
        public int playerIndex;

        public InputEventStartArgs(int p)
        {
            playerIndex = p;
        }
    }

    private void OnInputEventStart(InputEventStartArgs s)
    {
        EventHandler<InputEventStartArgs> handler = InputEventStart;

        if (handler != null)
        {
            handler(this, s);
        }

        //Debug.Log("Select Event for " + s.player.ToString());
    }
    #endregion
}


