using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TroopSelectorMenu : SelectorMenu {

    //[SerializeField] private bool forLeftPlayer = true;
    //[SerializeField] private TroopPool troopPool;
    //[SerializeField] private Transform troopPortraitContainer;
    //[SerializeField] private GameObject troopSelectionPortrait;


    //protected override void Start()
    //{
    //    base.Start();
    //    SpawnTroopPortraits();
    //}

    //private void SpawnTroopPortraits()
    //{
    //    //for(int i = 0; i < troopPool.troops.Count; i++)
    //    //{
    //    //    GameObject go = Instantiate(troopSelectionPortrait, troopPortraitContainer);
    //    //    TroopSelectorPortrait t = go.GetComponent<TroopSelectorPortrait>();
    //    //    AddSelectable(t);

    //    //    t.SetTroop(troopPool.troops[i].GetComponent<Troop>());
    //    //}
    //}

    //protected override void InputGrabber_SelectEvent(object sender, InputGrabber.SelectEventArgs e)
    //{
    //    base.InputGrabber_SelectEvent(sender, e);
    //    if(e.playerIndex != playerIndexToListenTo) { return; }
    //    ImageSelectable troopSelectorPortrait = selectables[currentlySelectedIndex_].GetComponent<ImageSelectable>();

    //    if(troopSelectorPortrait != null)
    //    {
    //        if (forLeftPlayer)
    //        {
    //            if (GameManager.GetLeftPlayer().ActiveTroopCount < 5 && troopSelectorPortrait.Adder)
    //            {
    //                if (troopSelectorPortrait.Adder)
    //                {
    //                    GameManager.GetLeftPlayer().AddActiveTroop(troopSelectorPortrait.Troop);
    //                } 
    //            } else if (!troopSelectorPortrait.Adder)
    //            {
    //                Troop t = troopSelectorPortrait.Troop;
    //                GameManager.GetLeftPlayer().RemoveActiveTroop(troopSelectorPortrait.Troop);
    //                t.CleanDestroy();
    //            }
    //        } else {
    //            if (GameManager.GetRightPlayer().ActiveTroopCount < 5 && troopSelectorPortrait.Adder)
    //            {

    //                GameManager.GetRightPlayer().AddActiveTroop(troopSelectorPortrait.Troop);
  
    //            } else if(!troopSelectorPortrait.Adder)
    //            {
    //                Troop t = troopSelectorPortrait.Troop;
    //                GameManager.GetRightPlayer().RemoveActiveTroop(troopSelectorPortrait.Troop);
    //                t.CleanDestroy();
    //            }
    //        }
    //        if(GameManager.NumOfPlayers == 1)
    //        {
    //            BattleManager.Instance.IntroduceBattle();
    //        }

    //        OnTroopSelected();
    //    }
    //}

    //public event EventHandler TroopSelected;

    //private void OnTroopSelected()
    //{
    //    EventHandler handler = TroopSelected;

    //    if (handler != null)
    //    {
    //        handler(this, EventArgs.Empty);
    //    }
    //}
}
