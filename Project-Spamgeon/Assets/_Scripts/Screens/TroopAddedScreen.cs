using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopAddedScreen : GameScreen {

    [SerializeField] private SelectionMeter mainSelectionMeter;
    [SerializeField] private Image image;
    [SerializeField] private Text troopNameText;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private float delaySeconds;
    private int troopLevel;
    private int troopIndex;
    private string poolName;
    private Coroutine cr_EnableSelectableAfterDelay;


    protected override void OnEnable()
    {
        base.OnEnable();
        continueButton.SetActive(false);
        UpdateDisplayInfo();
        CoroutineManager.BeginCoroutine(CoroutineManager.EnableAfterDelay(continueButton, delaySeconds, cr_EnableSelectableAfterDelay), ref cr_EnableSelectableAfterDelay, this);
        InputGrabber.Instance.TabEvent += InputGrabber_TabEvent;
    }

    private void InputGrabber_TabEvent(object sender, InputGrabber.TabEventArgs e)
    {
        if(e.playerIndex != 0) { return; }
        if(cr_EnableSelectableAfterDelay != null)
        {
            CoroutineManager.HaltCoroutine(ref cr_EnableSelectableAfterDelay, this);
            continueButton.SetActive(true);
        }
        if (!sbsManager.HookedToInput)
        {
            sbsManager.FocusNext(0);
            sbsManager.HookToInput();
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

    public void PointToPooledTroop(int index, string poolName, int troopLevel = 1)
    {
        troopIndex = index;
        this.poolName = poolName;
        this.troopLevel = troopLevel;
    }

    private void UpdateDisplayInfo()
    {
        Troop t = TroopPoolManager.GetPool(poolName)[troopIndex];
        image.sprite = t.transform.Find("Graphic").GetComponent<SpriteRenderer>().sprite;
        troopNameText.text = "A new " + t.Name + " joins your party...";
    }

    public void ProceedToBattle()
    {
        GameManager.GetLeftPlayer().AddActiveTroop(TroopPoolManager.GetPool(poolName)[troopIndex]);
        ScreenManager.Instance.TransitionToScreen("");
        BattleManager.Instance.IntroduceBattle();
    }

}
