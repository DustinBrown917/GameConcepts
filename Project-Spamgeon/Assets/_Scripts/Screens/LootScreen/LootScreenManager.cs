using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootScreenManager : GameScreen {

    [SerializeField] private SelectionMeter mainSelectionMeter;
    [SerializeField] private StartSpamTimer spamTimer;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Slider lootSlider;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TroopAddedScreen troopAddedScreen;
    [SerializeField] private Text expText;
    [SerializeField] private Vector3 expTextShrinkFrom;
    [SerializeField] private GoalMark ninetyMark;
    [SerializeField] private GoalMark seventyMark;
    [SerializeField] private Sprite newPartyMemberSprite;
    [SerializeField] private Sprite timesFourSprite;
    private AudioSource audioSource;
    private int expGained = 0;
    private int expGain = 1;
    private Coroutine cr_expTextSwell;
    [SerializeField] private float spamSequenceDuration;
    private Coroutine cr_SpamSequence;
    [SerializeField] private float spamScore = 0;
    [SerializeField] private float targetScore;
    [SerializeField] private float maxDecayRate;
    [SerializeField] private string targetTroopPool;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        spamTimer.gameObject.SetActive(true);
        sbsManager.UnHookFromInput();
        continueButton.SetActive(false);
        lootSlider.value = 0;
        timeSlider.value = 1;
        spamScore = 0;
        expGain = 1;
        expGained = 0;
        expText.text = "+0 EXP";
        if(GameManager.GetLeftPlayer().ActiveTroopCount >= 5)
        {
            ninetyMark.SetImage(timesFourSprite);
        } else
        {
            ninetyMark.SetImage(newPartyMemberSprite);
        }

        ninetyMark.Unachieve();
        seventyMark.Unachieve();
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
        MusicManager.Instance.SetCurrentSong(MusicManager.Songs.LOOT_SPAM);
        HookToInputGrabber();

        float previousValueScore = 0;
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

            if(previousValueScore < 0.9f && valueScore >= 0.9f)
            {
                if (GameManager.GetLeftPlayer().ActiveTroopCount == 5)
                {
                    expGain = 4;
                }

                ninetyMark.Achieve();

            }else if (previousValueScore >= 0.9f && valueScore < 0.9f)
            {
                ninetyMark.Unachieve();
                expGain = 2;
            }
            else if(previousValueScore < 0.7f && valueScore >= 0.7f)
            {
                expGain = 2;

                seventyMark.Achieve();
            } else if(previousValueScore >= 0.7f && valueScore < 0.7f)
            {
                seventyMark.Unachieve();
                expGain = 1;
            }

            timeSlider.value = 1.0f - (t / spamSequenceDuration);
            previousValueScore = valueScore;
            t += Time.deltaTime;
            yield return null;
        }

        timeSlider.value = 0;
        spamTimer.SetButtonActive(false);

        UnHookFromInputGrabber();
        MusicManager.Instance.SetCurrentSong(MusicManager.Songs.MENU);
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
        expGained += expGain;
        expText.text = "+" + expGained.ToString() + " EXP";
        CoroutineManager.BeginCoroutine(CoroutineManager.ShrinkScaleFrom(expText.transform, expTextShrinkFrom, Vector3.one, 0.2f), ref cr_expTextSwell, this);

        audioSource.Play();

        for(int i = 0; i < GameManager.GetLeftPlayer().ActiveTroopCount; i++)
        {
            GameManager.GetLeftPlayer().GetActiveTroop(i).AddExperience(expGain);
        }
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
