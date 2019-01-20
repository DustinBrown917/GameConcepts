using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputGrabber : MonoBehaviour {

    private static InputGrabber instance_;
    public static InputGrabber Instance { get { return instance_; } }

    [SerializeField] private float[] playerTimers;
    [SerializeField] private KeyCode[] playerKeys;
    [SerializeField] private string[] playerButtons;
    private float[] previousAxisValues;
    private float[] currentAxisValues;
    private Coroutine[] timerCoroutines;
    [SerializeField] private float timeToSelect = 1.0f;
    public float TimeToSelect { get { return timeToSelect; } }
    private bool[] inputBlocked;


    private Rewired.Player[] players;
    

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

        players = new Rewired.Player[3];
        for(int i = 0; i < players.Length; i++)
        {
            players[i] = ReInput.players.GetPlayer(i);
        }

        playerTimers = new float[playerButtons.Length];
        timerCoroutines = new Coroutine[playerButtons.Length];
        previousAxisValues = new float[playerButtons.Length];
        currentAxisValues = new float[playerButtons.Length];
        inputBlocked = new bool[playerButtons.Length];

        GameManager.NumberOfPlayersChanged += GameManager_NumberOfPlayersChanged;

        foreach(Controller c in ReInput.controllers.Joysticks)
        {
            Debug.Log(c.name);
        }

    }

    private void Start()
    {
        AssignControllers();
    }

    private void GameManager_NumberOfPlayersChanged(object sender, GameManager.NumberOfPlayersChangedArgs e)
    {
        Debug.Log("Num of players changed");
    }

    private void AssignControllers()
    {
        if(ReInput.controllers.joystickCount == 0) { return; }
        foreach(Rewired.Player p in players)
        {
            p.controllers.ClearControllersOfType<Joystick>();
        }

        players[0].controllers.AddController(ReInput.controllers.Joysticks[0], false);
        players[1].controllers.AddController(ReInput.controllers.Joysticks[0], false);

        if (ReInput.controllers.joystickCount > 1)
        {
            players[2].controllers.AddController(ReInput.controllers.Joysticks[1], true);
        }
    }

    private void Update()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (inputBlocked[i]) { continue; }

            if (players[i].GetButtonDown("SpamLeft"))
            {
                StartSelectionTimer(i, SpamSides.LEFT);
            } else if (players[i].GetButtonDown("SpamRight"))
            {
                StartSelectionTimer(i, SpamSides.RIGHT);
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
    /// Get whether or not a player's input is blocked.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool GetInputBlocked(int index)
    {
        if(index < 0 || index > inputBlocked.Length) { return false; }
        return inputBlocked[index];
    }

    /// <summary>
    /// Set whether or not a player's input is blocked.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="blocked"></param>
    public void SetInputBlocked(int index, bool blocked)
    {
        if (index < 0 || index > inputBlocked.Length) { return; }
        inputBlocked[index] = blocked;
    }

    /// <summary>
    /// Begins running a selection timer.
    /// </summary>
    /// <param name="index">The timer to run.</param>
    private void StartSelectionTimer(int index, SpamSides spamSide)
    {
        CoroutineManager.HaltCoroutine(ref timerCoroutines[index], this);
        if(playerTimers[index] > 0)
        {
            playerTimers[index] = 0;
            OnTabEvent(new TabEventArgs(index));
        }
        CoroutineManager.BeginCoroutine(SelectionTimer(index, spamSide, timerCoroutines[index]), ref timerCoroutines[index], this);
        OnInputEventStart(new InputEventStartArgs(index));
    }

    /// <summary>
    /// Runs a timer. If the threshold is met, a selection event is fired. If it is not, a tab event is fired.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    private IEnumerator SelectionTimer(int index, SpamSides spamSide, Coroutine container)
    {
        int playerIndex = index;

        while (playerTimers[playerIndex] < timeToSelect) {
            if((spamSide == SpamSides.LEFT && players[playerIndex].GetButtonUp("SpamLeft")) || (spamSide == SpamSides.RIGHT && players[playerIndex].GetButtonUp("SpamRight")))
            {
                break;
            }
            playerTimers[playerIndex] += Time.deltaTime;
            yield return null;
        }

        if (playerTimers[playerIndex] >= timeToSelect) {
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

    }
    #endregion





    private enum SpamSides
    {
        LEFT,
        RIGHT
    }
}


