using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopSelectorPortrait : Selectable {

    private Troop troop_;
    public Troop Troop { get { return troop_; } }

    [SerializeField] private Image image;

	public void SetTroop(Troop t)
    {
        troop_ = t;
        image.sprite = troop_.portrait;
    }
}
