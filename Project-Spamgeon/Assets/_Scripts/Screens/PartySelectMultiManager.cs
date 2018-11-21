using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PartySelectMultiManager : MonoBehaviour {

    //[SerializeField] private TroopSelectorMenu leftTroopSelectorMenu;
    //[SerializeField] private TroopSelectorMenu rightTroopSelectorMenu;

    //[SerializeField] private Text leftPlayerReadyText;
    //[SerializeField] private Text rightPlayerReadyText;

    //[SerializeField] private ImageSelectable[] leftPartyPortraits;
    //[SerializeField] private ImageSelectable[] rightPartyPortraits;

    //private bool leftPlayerReady = false;
    //private bool rightPlayerReady = false;

    //private void OnEnable()
    //{
    //    UpdateLeftPlayerPortraits();
    //    UpdateRightPlayerPortraits();

    //    leftPlayerReady = false;
    //    rightPlayerReady = false;

    //    leftPlayerReadyText.text = "Not Ready";
    //    rightPlayerReadyText.text = "Not Ready";
    //}

    //// Use this for initialization
    //void Start()
    //{
    //    GameManager.GetLeftPlayer().NewTroopAdded += LeftPlayer_NewTroopAdded;
    //    GameManager.GetRightPlayer().NewTroopAdded += RightPlayer_NewTroopAdded;
    //    GameManager.GetLeftPlayer().TroopRemoved += LeftPlayer_TroopRemoved;
    //    GameManager.GetRightPlayer().TroopRemoved += RightPlayer_TroopRemoved;

        
    //}



    //private void LeftPlayer_NewTroopAdded(object sender, Player.NewTroopAddedArgs e)
    //{
    //    UpdateLeftPlayerPortraits();
    //}

    //private void RightPlayer_NewTroopAdded(object sender, Player.NewTroopAddedArgs e)
    //{
    //    UpdateRightPlayerPortraits();
    //}

    //private void LeftPlayer_TroopRemoved(object sender, Player.TroopRemovedArgs e)
    //{
    //    UpdateLeftPlayerPortraits();
    //}

    //private void RightPlayer_TroopRemoved(object sender, Player.TroopRemovedArgs e)
    //{
    //    UpdateRightPlayerPortraits();
    //}

    //private void UpdateLeftPlayerPortraits()
    //{
    //    for(int i = 0; i < leftPartyPortraits.Length; i++)
    //    {
    //        Troop t = GameManager.GetLeftPlayer().GetActiveTroop(i);
    //        if (t != null)
    //        {
    //            leftPartyPortraits[i].gameObject.SetActive(true);
    //            leftPartyPortraits[i].SetTroop(t);
    //        } else
    //        {
    //            leftPartyPortraits[i].gameObject.SetActive(false);
    //        }
    //    }
    //}

    //private void UpdateRightPlayerPortraits()
    //{
    //    for (int i = 0; i < rightPartyPortraits.Length; i++)
    //    {
    //        Troop t = GameManager.GetRightPlayer().GetActiveTroop(i);
    //        if (t != null)
    //        {
    //            rightPartyPortraits[i].gameObject.SetActive(true);
    //            rightPartyPortraits[i].SetTroop(t);
    //        }
    //        else
    //        {
    //            rightPartyPortraits[i].gameObject.SetActive(false);
    //        }
    //    }
    //}

    //private void HandlePlayersReady()
    //{
    //    if (leftPlayerReady && rightPlayerReady)
    //    {
    //        ScreenManager.Instance.TransitionToScreen("");
    //        BattleManager.Instance.IntroduceBattle();
    //        leftTroopSelectorMenu.UnHookFromInput();
    //        rightTroopSelectorMenu.UnHookFromInput();
    //    }
    //}

    //public void ToggleLeftPlayerReady()
    //{
    //    if(GameManager.GetLeftPlayer().ActiveTroopCount == 0) { return; }
    //    leftPlayerReady = !leftPlayerReady;
    //    if (leftPlayerReady)
    //    {
    //        leftPlayerReadyText.text = "Ready!";
    //        HandlePlayersReady();
    //    } else
    //    {
    //        leftPlayerReadyText.text = "Not Ready";
    //    }
    //}

    //public void ToggleRightPlayerReady()
    //{
    //    if(GameManager.GetRightPlayer().ActiveTroopCount == 0) { return; }
    //    rightPlayerReady = !rightPlayerReady;

    //    if (rightPlayerReady)
    //    {
    //        rightPlayerReadyText.text = "Ready!";
    //        HandlePlayersReady();
    //    }
    //    else
    //    {
    //        rightPlayerReadyText.text = "Not Ready";
    //    }
    //}
}
