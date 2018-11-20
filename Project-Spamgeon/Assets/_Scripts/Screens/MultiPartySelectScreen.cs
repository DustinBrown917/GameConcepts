using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiPartySelectScreen : GameScreen {

    [SerializeField] private SelectionMeter mainSelectionMeter;
    [SerializeField] private SelectionMeter firstPlayerSelectionMeter;
    [SerializeField] private SelectionMeter secondPlayerSelectionMeter;

    [Space]

    [SerializeField] private Text firstPlayerText;
    private bool firstPlayerReady;
    [SerializeField] private Text secondPlayerText;
    private bool secondPlayerReady;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        sbsManager.AddSelectionMeter(0, mainSelectionMeter);
        sbsManager.AddSelectionMeter(1, firstPlayerSelectionMeter);
        sbsManager.AddSelectionMeter(2, secondPlayerSelectionMeter);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (firstPlayerReady) { ToggleReady(1); }
        if(secondPlayerReady) { ToggleReady(2); }
    }

    public void ToggleReady(int playerNumber)
    {
        if (playerNumber == 1)
        {
            firstPlayerReady = !firstPlayerReady;
            if (firstPlayerReady)
            {
                firstPlayerText.text = "Ready!";
            }
            else
            {
                firstPlayerText.text = "Not Ready";
            }
        }
        else if(playerNumber == 2)
        {
            secondPlayerReady = !secondPlayerReady;
            if (secondPlayerReady)
            {
                secondPlayerText.text = "Ready!";
            }
            else
            {
                secondPlayerText.text = "Not Ready";
            }
        }
    }

}
