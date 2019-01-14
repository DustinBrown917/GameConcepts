using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : GameScreen {

    [SerializeField] private SelectionMeter startMenuSelectionMeter;

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.ResetDungeonDepth();
        GameManager.SetNumOfPlayers(1);
        if(MusicManager.Instance.CurrentSong != MusicManager.Songs.MENU)
        {
            MusicManager.Instance.SetCurrentSong(MusicManager.Songs.MENU);
        }
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        sbsManager.AddSelectionMeter(0, startMenuSelectionMeter);
	}
	

}
