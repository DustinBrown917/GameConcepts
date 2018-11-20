using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : GameScreen {

    [SerializeField] private SelectionMeter startMenuSelectionMeter;

	// Use this for initialization
	protected override void Start () {
        base.Start();
        sbsManager.AddSelectionMeter(0, startMenuSelectionMeter);
	}
	

}
