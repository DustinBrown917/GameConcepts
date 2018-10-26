using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager instance_;
    public static GameManager Instance { get { return instance_; } }

    private static int numOfPlayers_ = 1;
    public static int NumOfPlayers { get { return numOfPlayers_; } }

    private static int currentDungeonDepth_ = 1;
    public static int CurrentDungeonDepth { get { return currentDungeonDepth_; } }

    private GameStates currentState;
    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    [SerializeField] private TroopPool[] troopPools = new TroopPool[4];



    /************************************************************************************/
    /********************************* UNITY BEHAVIOURS *********************************/
    /************************************************************************************/

    private void Awake()
    {
        if(instance_ != null && instance_ != this)
        {
            Destroy(gameObject);
            return;
        }
        instance_ = this;
    }

    // Use this for initialization
    void Start () {
        leftPlayer.ChangePlayerType(Players.FIRST);
        rightPlayer.ChangePlayerType(Players.SECOND);
        StartCoroutine(DEBUG_DELAY());
	}
	
    private IEnumerator DEBUG_DELAY()
    {
        yield return new WaitForSeconds(1.0f);
        ChangeGameState(GameStates.PRE_BATTLE);

        yield return new WaitForSeconds(1.0f);
        ChangeGameState(GameStates.BATTLE);
    }

	// Update is called once per frame
	void Update () {
		
	}

    /************************************************************************************/
    /************************************ BEHAVIOURS ************************************/
    /************************************************************************************/

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

    public static TroopPool GetPlayerTroopPool(Players playerType)
    {
        if (!IsInitialized()) { return null; }
        return Instance.troopPools[(int)playerType];
    }

    public static void ResetDungeonDepth()
    {
        currentDungeonDepth_ = 1;
    }

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
    BATTLE_SUMMARY
}
