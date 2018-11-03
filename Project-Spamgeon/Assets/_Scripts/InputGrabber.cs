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

    private void Awake()
    {
        if(instance_ != null)
        {
            Destroy(gameObject);
            return;
        }
        instance_ = this;

        playerTimers = new float[3];
        timerCoroutines = new Coroutine[3];
    }

    private void Update()
    {
        if (inputBlocked_) { return; }

        if (Input.GetKeyDown(playerKeys[0]))
        {
            StartSelectionTimer(Players.SINGLE);
            OnInputEventStart(new InputEventStartArgs(Players.SINGLE));
        }
        if (Input.GetKeyDown(playerKeys[1]))
        {
            StartSelectionTimer(Players.FIRST);
            OnInputEventStart(new InputEventStartArgs(Players.FIRST));
        }
        if (Input.GetKeyDown(playerKeys[2]))
        {
            StartSelectionTimer(Players.SECOND);
            OnInputEventStart(new InputEventStartArgs(Players.SECOND));
        }
    }

    public float GetSelectionTime(Players timer)
    {
        return playerTimers[(int)timer];
    }

    private void StartSelectionTimer(Players player)
    {
        int playerIndex = (int)player;
        if(timerCoroutines[playerIndex] != null)
        {
            StopCoroutine(timerCoroutines[playerIndex]);
            timerCoroutines[playerIndex] = null;
        }
        timerCoroutines[playerIndex] = StartCoroutine(SelectionTimer(player, timerCoroutines[playerIndex]));
    }

    private IEnumerator SelectionTimer(Players player, Coroutine container)
    {
        int playerIndex = (int)player;
        while (Input.GetKey(playerKeys[playerIndex]) && playerTimers[playerIndex] < timeToSelect)
        {
            playerTimers[playerIndex] += Time.deltaTime;
            yield return null;
        }

        if(playerTimers[playerIndex] >= timeToSelect)
        {
            OnSelectEvent(new SelectEventArgs(player));
        } else
        {
            OnTabEvent(new TabEventArgs(player));
        }
        playerTimers[playerIndex] = 0;
        container = null;
    }

    private void CancelInput(Players player)
    {
        StopCoroutine(timerCoroutines[(int)player]);
        timerCoroutines[(int)player] = null;
        playerTimers[(int)player] = 0;
    }



    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

    #region TabEvent Event
    public event EventHandler<TabEventArgs> TabEvent;

    public class TabEventArgs : EventArgs
    {
        public Players player;

        public TabEventArgs(Players p)
        {
            player = p;
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
        public Players player;

        public SelectEventArgs(Players p)
        {
            player = p;
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
        public Players player;

        public InputEventStartArgs(Players p)
        {
            player = p;
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


