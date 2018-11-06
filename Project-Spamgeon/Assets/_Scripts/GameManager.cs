using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager instance_;
    public static GameManager Instance { get { return instance_; } }

    private static byte numOfPlayers_ = 0;
    public static byte NumOfPlayers { get { return numOfPlayers_; } }

    private static int currentDungeonDepth_ = 1;
    public static int CurrentDungeonDepth { get { return currentDungeonDepth_; } }

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
        GameStateHandler.ChangeState(GameStateHandler.States.MAIN);
    }

    // Use this for initialization
    void Start () {
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
            Instance.leftPlayer.ChangePlayerType(Players.SINGLE);
            Instance.rightPlayer.ChangePlayerType(Players.COMPUTER);
        } else
        {
            Instance.leftPlayer.ChangePlayerType(Players.FIRST);
            Instance.rightPlayer.ChangePlayerType(Players.SECOND);
        }
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


    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

}

