using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager instance_;
    public static GameManager Instance { get { return instance_; } }

    private static byte numOfPlayers_ = 0;
    public static byte NumOfPlayers { get { return numOfPlayers_; } }

    private static int currentDungeonDepth_ = 0;
    public static int CurrentDungeonDepth { get { return currentDungeonDepth_; } }

    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    [SerializeField] private TroopPool[] playerTroopPools;
    [SerializeField] private TroopPool computerTroopPool;


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
        Screen.SetResolution(1280, 720, false);
        
    }

    // Use this for initialization
    void Start () {
        GameStateHandler.ChangeState(GameStateHandler.States.MAIN);
        rightPlayer.NoMoreTroopsLeft += Player_NoMoreTroopsLeft;
        leftPlayer.NoMoreTroopsLeft += Player_NoMoreTroopsLeft;
    }

    private void Player_NoMoreTroopsLeft(object sender, Player.NoMoreTroopsLeftArgs e)
    {
        BattleManager.Instance.EndBattle();
    }

    /************************************************************************************/
    /************************************ BEHAVIOURS ************************************/
    /************************************************************************************/

    public static void SetNumOfPlayers(byte num)
    {
        if(numOfPlayers_ == num || num == 0) { return; }

        numOfPlayers_ = num;

        if(num == 1)
        {
            Instance.leftPlayer.ChangePlayerType(0);
            Instance.rightPlayer.ChangePlayerType(-1);
        } else
        {
            Instance.leftPlayer.ChangePlayerType(1);
            Instance.rightPlayer.ChangePlayerType(2);
        }
    }

    /// <summary>
    /// Returns a troop pool associated with a given player.
    /// </summary>
    /// <param name="playerIndex">The index of the player whose troop pool you wish to get. (-1 for computer)</param>
    /// <returns>The troop pool of the specified player.</returns>
    public static TroopPool GetPlayerTroopPool(int playerIndex)
    {
        if (!IsInitialized()) { return null; }

        if (playerIndex == -1) {
            return Instance.computerTroopPool;
        } else if (playerIndex >= 0 && playerIndex < Instance.playerTroopPools.Length) {
            return Instance.playerTroopPools[playerIndex];
        }

        return null;
    }

    public static void ResetDungeonDepth()
    {
        currentDungeonDepth_ = 0;
        OnDungeonDepthChanged(new DungeonDepthChangedArgs(currentDungeonDepth_));
    }

    public static void NextDungeonFloor()
    {
        currentDungeonDepth_++;
        OnDungeonDepthChanged(new DungeonDepthChangedArgs(currentDungeonDepth_));
    }

    public static Player GetLeftPlayer()
    {
        if (!IsInitialized()) { return null; }
        return instance_.leftPlayer;
    }

    public static Player GetRightPlayer()
    {
        if (!IsInitialized()) { return null; }
        return instance_.rightPlayer;
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

    public void lChangeNumOfPlayers(int n)
    {
        SetNumOfPlayers((byte)n);
    }

    public void Quit()
    {
        Application.Quit();
    }


    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

    public static event EventHandler<DungeonDepthChangedArgs> DungeonDepthChanged;

    public class DungeonDepthChangedArgs : EventArgs
    {
        public int currentDepth;

        public DungeonDepthChangedArgs(int newDepth)
        {
            currentDepth = newDepth;
        }
    }

    private static void OnDungeonDepthChanged(DungeonDepthChangedArgs e)
    {
        EventHandler<DungeonDepthChangedArgs> handler = DungeonDepthChanged;

        if(handler != null)
        {
            handler(typeof(GameManager), e);
        }
    }
}

