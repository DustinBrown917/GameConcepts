using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopSelectorPortrait : SingleButtonSelectable {

    private Troop troop_;
    public Troop Troop { get { return troop_; } }

    [SerializeField] private Image image;
    [SerializeField] private bool adder_ = true;
    public bool Adder { get { return adder_; } }

	public void SetTroop(Troop t)
    {
        troop_ = t;
        image.sprite = troop_.portrait;
    }
}
