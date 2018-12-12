using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootScreenManager : GameScreen {

    [SerializeField] private SelectionMeter mainSelectionMeter;
    [SerializeField] private StartSpamTimer spamTimer;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Slider lootSlider;
    [SerializeField] private TroopAddedScreen troopAddedScreen;
    [SerializeField] private float spamSequenceDuration;
    private Coroutine cr_SpamSequence;
    [SerializeField] private float spamScore = 0;
    [SerializeField] private float targetScore;
    [SerializeField] private float maxDecayRate;
    [SerializeField] private string targetTroopPool;


    protected override void OnEnable()
    {
        base.OnEnable();
        spamTimer.gameObject.SetActive(true);
        sbsManager.UnHookFromInput();
        continueButton.SetActive(false);
        spamScore = 0;
    }

    // Use this for initialization
    protected override void Start () {
        sbsManager.AddSelectionMeter(0, mainSelectionMeter);
        spamTimer.TimerDone.AddListener(BeginSpamSequence);
        
    }

    private void BeginSpamSequence()
    {
        CoroutineManager.BeginCoroutine(SpamSequenceTimer(), ref cr_SpamSequence, this);
    }

    private IEnumerator SpamSequenceTimer()
    {
        HookToInputGrabber();

        float valueScore = 0;
        float currentDecay = 0;
        float t = 0.0f;
        while(t < spamSequenceDuration)
        {
            
            valueScore = spamScore / targetScore;
            currentDecay = valueScore * maxDecayRate;
            if(spamScore > 0)
            {
                spamScore -= currentDecay;
            }
            
            lootSlider.value = valueScore; 
            t += Time.deltaTime;
            yield return null;
        }

        UnHookFromInputGrabber();

        //Doll out experience here.
        continueButton.SetActive(true);
        sbsManager.HookToInput();
    }

    private int GetTargetTroopIndex()
    {
        TroopPool tp = TroopPoolManager.GetPool(targetTroopPool);

        return UnityEngine.Random.Range(0, tp.Count);
    }

    public void HandleNextScreen()
    {
        if(spamScore >= targetScore * 0.9f)
        {
            if(GameManager.GetLeftPlayer().ActiveTroopCount < 5)
            {
                troopAddedScreen.PointToPooledTroop(GetTargetTroopIndex(), targetTroopPool);
                ScreenManager.Instance.TransitionToScreen("TroopAddedScreen");
            } else
            {
                ScreenManager.Instance.TransitionToScreen("");
                BattleManager.Instance.IntroduceBattle();
            }
            
        } else
        {
            ScreenManager.Instance.TransitionToScreen("");
            BattleManager.Instance.IntroduceBattle();
        }
    }

    private void HookToInputGrabber()
    {
        InputGrabber.Instance.TabEvent += InputGrabber_TabEvent;
    }

    private void InputGrabber_TabEvent(object sender, InputGrabber.TabEventArgs e)
    {
        if(e.playerIndex != 0) { return; }
        spamScore = Mathf.Clamp(spamScore += 1, 0, targetScore);
    }

    private void UnHookFromInputGrabber()
    {
        InputGrabber.Instance.TabEvent -= InputGrabber_TabEvent;
    }





    protected override void HookSBSManagerByEvent(object sender, TransitionCompleteArgs e)
    {
        //Stop sbs manager from immediately hooking up.
    }
}
