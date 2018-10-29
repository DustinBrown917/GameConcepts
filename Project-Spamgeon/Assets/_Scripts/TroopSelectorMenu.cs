using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TroopSelectorMenu : SelectorMenu {

    [SerializeField] private bool forLeftPlayer = true;
    [SerializeField] private TroopPool troopPool;
    [SerializeField] private Transform troopPortraitContainer;
    [SerializeField] private GameObject troopSelectionPortrait;


    protected override void Start()
    {
        base.Start();
        SpawnTroopPortraits();
    }

    private void SpawnTroopPortraits()
    {
        for(int i = 0; i < troopPool.troops.Count; i++)
        {
            GameObject go = Instantiate(troopSelectionPortrait, troopPortraitContainer);
            TroopSelectorPortrait t = go.GetComponent<TroopSelectorPortrait>();
            AddSelectable(t);

            t.SetTroop(troopPool.troops[i].GetComponent<Troop>());
        }
    }

    protected override void InputGrabber_SelectEvent(object sender, InputGrabber.SelectEventArgs e)
    {
        base.InputGrabber_SelectEvent(sender, e);

        TroopSelectorPortrait troopSelectorPortrait = selectables[currentlySelectedIndex_].GetComponent<TroopSelectorPortrait>();

        if(troopSelectorPortrait != null)
        {
            if (forLeftPlayer)
            {
                GameManager.GetLeftPlayer().AddActiveTroop(troopSelectorPortrait.Troop);
            } else
            {
                GameManager.GetRightPlayer().AddActiveTroop(troopSelectorPortrait.Troop);
            }
        }
    }
}
