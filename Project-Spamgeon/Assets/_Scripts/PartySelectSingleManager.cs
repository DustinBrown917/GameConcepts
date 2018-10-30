using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySelectSingleManager : MonoBehaviour {

    [SerializeField] private TroopSelectorMenu troopSelectorMenu;

	// Use this for initialization
	void Start () {
        troopSelectorMenu.TroopSelected += TroopSelectorMenu_TroopSelected;
	}

    private void TroopSelectorMenu_TroopSelected(object sender, System.EventArgs e)
    {
        ScreenManager.Instance.TransitionToScreen(-1);
        troopSelectorMenu.UnHookFromInput();
    }


}
