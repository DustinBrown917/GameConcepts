using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSummaryScreen : GameScreen {

    [SerializeField] private SelectionMeter mainSelectionMeter;
    [SerializeField] private Text primaryText;
    [SerializeField] private Text secondaryText;
    [SerializeField] private GameObject selectorButton;
    [SerializeField] private float delaySeconds;
    private Coroutine cr_EnableSelectableAfterDelay = null;
    private Text selectorButtonText;

    protected override void Awake()
    {
        base.Awake();
        selectorButtonText = selectorButton.GetComponentInChildren<Text>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        selectorButton.SetActive(false);
        CoroutineManager.BeginCoroutine(CoroutineManager.EnableAfterDelay(selectorButton, delaySeconds, cr_EnableSelectableAfterDelay), ref cr_EnableSelectableAfterDelay, this);
        InputGrabber.Instance.TabEvent += InputGrabber_TabEvent;

        if (GameManager.NumOfPlayers == 1)
        { //Single player battle end
            if (GameManager.GetLeftPlayer().ActiveTroopCount > 0)
            {
                primaryText.text = "You Won!";
                secondaryText.text = "Good job, you shrub.";
                selectorButtonText.text = "Claim Loot";
            }
            else
            {
                primaryText.text = "You lost!";
                secondaryText.text = "You suck, you knob!";
                selectorButtonText.text = "Admit Defeat";
            }
        }
        else
        { //Multiplayer battle end
            if (GameManager.GetLeftPlayer().ActiveTroopCount > 0)
            {
                primaryText.text = "Player 1 Won!";
                secondaryText.text = "Player 2, you're a dolt.";
                selectorButtonText.text = "Return to Start";
            }
            else
            {
                primaryText.text = "Player 2 Won!";
                secondaryText.text = "Player 1, you're a pest.";
                selectorButtonText.text = "Return to Start";
            }
        }
    }

    protected override void Start () {
        base.Start();

        sbsManager.AddSelectionMeter(0, mainSelectionMeter);
	}

    protected override void OnDisable()
    {
        base.OnDisable();
        InputGrabber.Instance.TabEvent -= InputGrabber_TabEvent;
    }

    public void HandleContinue()
    {
        if (GameManager.NumOfPlayers == 1)
        {
            if (GameManager.GetLeftPlayer().ActiveTroopCount > 0)
            {
                ScreenManager.Instance.TransitionToScreen("StartScreen"); //Change this to the loot screen
            }
            else
            {   //If player lost
                ScreenManager.Instance.TransitionToScreen("StartScreen");
            }
        }
        else
        { //Multiplayer battle end
            if (GameManager.GetLeftPlayer().ActiveTroopCount > 0)
            { //Left player won
                ScreenManager.Instance.TransitionToScreen("StartScreen");
            }
            else
            { //Right player won
                ScreenManager.Instance.TransitionToScreen("StartScreen");
            }
        }
    }

    private void InputGrabber_TabEvent(object sender, InputGrabber.TabEventArgs e)
    {
        if (e.playerIndex != 0) { return; }
        if (cr_EnableSelectableAfterDelay != null)
        {
            CoroutineManager.HaltCoroutine(ref cr_EnableSelectableAfterDelay, this);
            selectorButton.SetActive(true);
        }
        if (!sbsManager.HookedToInput)
        {
            sbsManager.FocusNext(0);
            sbsManager.HookToInput();
        }

    }
}
