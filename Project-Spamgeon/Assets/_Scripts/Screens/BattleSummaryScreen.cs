using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSummaryScreen : GameScreen {

    [SerializeField] private SelectionMeter mainSelectionMeter;
    [SerializeField] private Text primaryText;
    [SerializeField] private Text secondaryText;
    [SerializeField] private GameObject selectorButton;
    [SerializeField] private GameObject selectorButtonMulti;
    [SerializeField] private GameObject returnToStartButton;
    [SerializeField] private float delaySeconds;
    [SerializeField] private SelectionMeter multiSelectionMeter;
    private Coroutine cr_EnableSelectableAfterDelay = null;
    private Coroutine cr_EnableReturnToStartAfterDelay = null;
    private Text selectorButtonText;
    private Text selectorButtonMultiText;

    private bool started;

    protected override void Awake()
    {
        base.Awake();
        selectorButtonText = selectorButton.GetComponentInChildren<Text>();
        selectorButtonMultiText = selectorButtonMulti.GetComponentInChildren<Text>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        

        selectorButton.SetActive(false);
        selectorButtonMulti.SetActive(false);
        returnToStartButton.SetActive(false);

        if (GameManager.NumOfPlayers == 1)
        {
            CoroutineManager.BeginCoroutine(CoroutineManager.EnableAfterDelay(selectorButton, delaySeconds, cr_EnableSelectableAfterDelay), ref cr_EnableSelectableAfterDelay, this);
            CoroutineManager.BeginCoroutine(CoroutineManager.EnableAfterDelay(returnToStartButton, delaySeconds, cr_EnableReturnToStartAfterDelay), ref cr_EnableReturnToStartAfterDelay, this);
        }
        else
        {
            CoroutineManager.BeginCoroutine(CoroutineManager.EnableAfterDelay(selectorButtonMulti, delaySeconds, cr_EnableSelectableAfterDelay), ref cr_EnableSelectableAfterDelay, this);
        }

        InputGrabber.Instance.TabEvent += InputGrabber_TabEvent;

        if (GameManager.NumOfPlayers == 1)
        { //Single player battle end
            if (GameManager.GetLeftPlayer().ActiveTroopCount > 0)
            {
                primaryText.text = "Victory!";
                secondaryText.text = "Tend your wounds and gather your party, there are deeper depths yet to plumb.";
                selectorButtonText.text = "Claim Loot";
            }
            else
            {
                primaryText.text = "A Bitter End...";
                secondaryText.text = "Your party has been claimed by The Spamgeon. Their bodies lie forever more on floor " + GameManager.CurrentDungeonDepth.ToString() + ", a warning to any who might come after.";
                selectorButtonText.text = "Admit Defeat";
            }
        }
        else
        { //Multiplayer battle end
            if (GameManager.GetLeftPlayer().ActiveTroopCount > 0)
            {
                primaryText.text = "Player 1 Stands Victorious!";
                secondaryText.text = "Player 2's party was no match.";
                selectorButtonMultiText.text = "Return to Start";
            }
            else
            {
                primaryText.text = "Player 2 Claims the Day!";
                secondaryText.text = "Player 1 may find better luck in the afterlife.";
                selectorButtonMultiText.text = "Return to Start";
            }
        }

        if (started)
        {
            sbsManager.AddSelectionMeter(1, multiSelectionMeter);
        } 
    }

    protected override void Start () {
        base.Start();

        sbsManager.AddSelectionMeter(0, mainSelectionMeter);
        sbsManager.AddSelectionMeter(1, multiSelectionMeter);

        started = true;
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
                ScreenManager.Instance.TransitionToScreen("LootScreen"); //Change this to the loot screen
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
