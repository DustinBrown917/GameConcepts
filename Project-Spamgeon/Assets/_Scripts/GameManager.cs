using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager instance_;
    public static GameManager Instance { get { return instance_; } }

    private static int numOfPlayers_ = 1;
    public static int NumOfPlayers { get { return numOfPlayers_; } }

    private GameStates currentState;

    public static GameStates GetCurrentState()
    {
        if (!IsInitialized()) { return GameStates.INVALID; }
        return instance_.currentState;
    }

    public static void ChangeGameState(GameStates state)
    {
        if (!IsInitialized()) { return; }

        instance_.lChangeGameState(state);
    }


    /************************************************************************************/
    /********************************* UNITY BEHAVIOURS *********************************/
    /************************************************************************************/

    private void Awake()
    {
        if(instance_ != this)
        {
            Destroy(gameObject);
            return;
        }
        instance_ = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /************************************************************************************/
    /************************************ BEHAVIOURS ************************************/
    /************************************************************************************/

    private static bool IsInitialized()
    {
        if(Instance == null)
        {
            Debug.LogError("GameManager not initialized!");
            return false;
        }
        return true;
    }

    private void lChangeGameState(GameStates state)
    {
        if(state == currentState) { return; }

        GameStateChangedArgs args = new GameStateChangedArgs();
        args.previousState = currentState;
        args.newState = state;

        currentState = state;

        OnGameStateChanged(args);
    }

    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

    #region GameStateChanged Event
    public static event EventHandler<GameStateChangedArgs> GameStateChanged;

    public class GameStateChangedArgs : EventArgs
    {
        public GameStates previousState;
        public GameStates newState;
    }

    private static void OnGameStateChanged(GameStateChangedArgs args)
    {
        EventHandler<GameStateChangedArgs> handler = GameStateChanged;

        if(handler != null)
        {
            handler(typeof(GameManager), args);
        }
    }
    #endregion
}


public enum GameStates
{
    INVALID,
    START_SREEN,
    PRE_BATTLE,
    BATTLE,
    POST_BATTLE,
    VICTORY,
    DEFEAT
}
