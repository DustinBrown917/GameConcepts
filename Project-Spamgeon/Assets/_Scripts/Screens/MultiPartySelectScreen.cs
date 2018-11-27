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
    [SerializeField] private Transform firstPlayerContainer;
    [SerializeField] private Transform secondPlayerContainer;

    [SerializeField] string firstPlayerTroopPoolName;
    private TroopPool firstPlayerTroopPool;
    [SerializeField] string secondPlayerTroopPoolName;
    private TroopPool secondPlayerTroopPool;
    [SerializeField] private Text firstPlayerTroopText;
    [SerializeField] private Text secondPlayerTroopText;
    [SerializeField] private ImageSelectable[] firstPlayerSelectedPartyPortraits;
    private int[] firstPlayerSelectedPartyIndeces;
    [SerializeField] private ImageSelectable[] secondPlayerSelectedPartyPortraits;
    private int[] secondPlayerSelectedPartyIndeces;

    [SerializeField] private ImageSelectable troopSelectablePrefab;


    protected override void Awake()
    {
        base.Awake();
        firstPlayerSelectedPartyIndeces = new int[firstPlayerSelectedPartyPortraits.Length];
        secondPlayerSelectedPartyIndeces = new int[secondPlayerSelectedPartyPortraits.Length];

    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        firstPlayerTroopPool = TroopPoolManager.GetPool(firstPlayerTroopPoolName);
        secondPlayerTroopPool = TroopPoolManager.GetPool(secondPlayerTroopPoolName);
        AddTroopSelectables(true);
        AddTroopSelectables(false);

        sbsManager.RefreshSelectables();

        sbsManager.AddSelectionMeter(0, mainSelectionMeter);
        sbsManager.AddSelectionMeter(1, firstPlayerSelectionMeter);
        sbsManager.AddSelectionMeter(2, secondPlayerSelectionMeter);


    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InitIndexArrays();
        InitSelectableArrays();

        if (firstPlayerReady) { ToggleReady(1); }
        if (secondPlayerReady) { ToggleReady(2); }
    }

    public void ToggleReady(int playerNumber)
    {
        if (playerNumber == 1)
        {
            
            firstPlayerReady = !firstPlayerReady;
            if (firstPlayerReady)
            {
                if (!FirstPlayerHasTroops()) {
                    firstPlayerReady = false;
                    return;
                }
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
                if (!SecondPlayerHasTroops()) {
                    secondPlayerReady = false;
                    return;
                }
                secondPlayerText.text = "Ready!";
            }
            else
            {
                secondPlayerText.text = "Not Ready";
            }
        }

        if(firstPlayerReady && secondPlayerReady)
        {
            ProceedToBattle();
        }
    }

    private void InitIndexArrays()
    {
        for(int i = 0; i < firstPlayerSelectedPartyIndeces.Length; i++)
        {
            firstPlayerSelectedPartyIndeces[i] = -1;
        }

        for (int i = 0; i < secondPlayerSelectedPartyIndeces.Length; i++)
        {
            secondPlayerSelectedPartyIndeces[i] = -1;
        }
    }

    private void InitSelectableArrays()
    {
        for(int i = 0; i < firstPlayerSelectedPartyPortraits.Length; i++)
        {
            firstPlayerSelectedPartyPortraits[i].SetImageSprite(null);
            firstPlayerSelectedPartyPortraits[i].OnFocus.AddListener(delegate { UpdateTroopDisplayedData(-1, firstPlayerTroopPool, firstPlayerTroopText); });
        }

        for (int i = 0; i < secondPlayerSelectedPartyPortraits.Length; i++)
        {
            secondPlayerSelectedPartyPortraits[i].SetImageSprite(null);
            secondPlayerSelectedPartyPortraits[i].OnFocus.AddListener(delegate { UpdateTroopDisplayedData(-1, secondPlayerTroopPool, secondPlayerTroopText); });
        }
    }

    private void AddTroopSelectables(bool leftPlayer)
    {
        TroopPool troopPool;
        string targetTroopPool;
        Transform troopSelectableHolder;
        Text nameText;
        int playerIndex;
        int[] indexArray;
        ImageSelectable[] selectablesArray;
        if (leftPlayer)
        {
            troopPool = firstPlayerTroopPool;
            targetTroopPool = firstPlayerTroopPoolName;
            troopSelectableHolder = firstPlayerContainer;
            playerIndex = 1;
            nameText = firstPlayerTroopText;
            indexArray = firstPlayerSelectedPartyIndeces;
            selectablesArray = firstPlayerSelectedPartyPortraits;
        } else
        {
            troopPool = secondPlayerTroopPool;
            targetTroopPool = secondPlayerTroopPoolName;
            troopSelectableHolder = secondPlayerContainer;
            playerIndex = 2;
            nameText = secondPlayerTroopText;
            indexArray = secondPlayerSelectedPartyIndeces;
            selectablesArray = secondPlayerSelectedPartyPortraits;
        }

        if (troopPool == null)
        {
            Debug.LogWarning("Could not find TroopPool \"" + targetTroopPool + "\".");
            return;
        }

        

        for (int i = 0; i < troopPool.Count; i++)
        {
            ImageSelectable imgSel = Instantiate(troopSelectablePrefab.gameObject, troopSelectableHolder).GetComponent<ImageSelectable>();
            imgSel.SetImageSprite(troopPool[i].portrait);
            imgSel.SetTabIndex(i);
            imgSel.SetGroupIndex(playerIndex);
            imgSel.OnFocus.AddListener(delegate { UpdateTroopDisplayedData(imgSel.TabIndex, troopPool, nameText); });
            imgSel.OnSelect.AddListener(delegate { AddPartyMember(imgSel.TabIndex, troopPool, indexArray, selectablesArray, leftPlayer); });
        }
    }

    private void AddPartyMember(int index, TroopPool troopPool, int[] indexArray, ImageSelectable[] selectables, bool forLeftPlayer)
    {
        if(forLeftPlayer && firstPlayerReady) { return; }
        else if(!forLeftPlayer && secondPlayerReady) { return; }
        int indexOfCurrentSelectableTarget = -1;
        for(int i = 0; i < indexArray.Length; i++)
        {
            if(indexArray[i] == -1)
            {
                indexOfCurrentSelectableTarget = i;
                break;
            }
        }

        if(indexOfCurrentSelectableTarget == -1) { return; }

        indexArray[indexOfCurrentSelectableTarget] = index;
        selectables[indexOfCurrentSelectableTarget].SetImageSprite(troopPool[index].portrait);
    }

    public bool FirstPlayerHasTroops()
    {
        for(int i = 0; i < firstPlayerSelectedPartyIndeces.Length; i++)
        {
            if(firstPlayerSelectedPartyIndeces[i] != -1) { return true; }
        }

        return false;
    }

    public bool SecondPlayerHasTroops()
    {
        for (int i = 0; i < secondPlayerSelectedPartyIndeces.Length; i++)
        {
            if (secondPlayerSelectedPartyIndeces[i] != -1) { return true; }
        }

        return false;
    }

    public void RemovePlayerFromLeft(int index)
    {
        if(index < 0 || index >= firstPlayerSelectedPartyIndeces.Length || firstPlayerReady)
        {
            return;
        }

        firstPlayerSelectedPartyIndeces[index] = -1;
        firstPlayerSelectedPartyPortraits[index].SetImageSprite(null);
            
    }

    public void RemovePlayerFromRight(int index)
    {
        if (index < 0 || index >= secondPlayerSelectedPartyIndeces.Length || secondPlayerReady) {
            return;
        }

        secondPlayerSelectedPartyIndeces[index] = -1;
        secondPlayerSelectedPartyPortraits[index].SetImageSprite(null);
    }

    public void ProceedToBattle()
    {

        for(int i = 0; i < firstPlayerSelectedPartyIndeces.Length; i++)
        {
            if(firstPlayerSelectedPartyIndeces[i] != -1)
            {
                GameManager.GetLeftPlayer().AddActiveTroop(firstPlayerTroopPool[firstPlayerSelectedPartyIndeces[i]]);
            }
        }

        for (int i = 0; i < secondPlayerSelectedPartyIndeces.Length; i++)
        {
            if (secondPlayerSelectedPartyIndeces[i] != -1)
            {
                GameManager.GetRightPlayer().AddActiveTroop(secondPlayerTroopPool[secondPlayerSelectedPartyIndeces[i]]);
            }
        }


        ScreenManager.Instance.TransitionToScreen("");
        BattleManager.Instance.IntroduceBattle();
    }

    public void UpdateTroopDisplayedData(int index, TroopPool troopPool, Text nameText)
    {
        if (index >= 0 && index < troopPool.Count) {
            nameText.text = troopPool[index].Name;
        }
        else {
            nameText.text = "";
        }
    }

    public void SetLeftPlayerLabelBlank()
    {
        firstPlayerTroopText.text = "";
    }

    public void SetRightPlayerLabelBlank()
    {
        secondPlayerTroopText.text = "";
    }
}
