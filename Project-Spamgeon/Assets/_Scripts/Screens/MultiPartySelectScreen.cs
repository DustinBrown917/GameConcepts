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

    [SerializeField] private ImageSelectable troopSelectablePrefab;


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

    private void AddTroopSelectables(bool leftPlayer)
    {
        TroopPool troopPool;
        string targetTroopPool;
        Transform troopSelectableHolder;
        Text nameText;
        int playerIndex;
        if (leftPlayer)
        {
            troopPool = firstPlayerTroopPool;
            targetTroopPool = firstPlayerTroopPoolName;
            troopSelectableHolder = firstPlayerContainer;
            playerIndex = 1;
            nameText = firstPlayerTroopText;
        } else
        {
            troopPool = secondPlayerTroopPool;
            targetTroopPool = secondPlayerTroopPoolName;
            troopSelectableHolder = secondPlayerContainer;
            playerIndex = 2;
            nameText = secondPlayerTroopText;
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
            //imgSel.OnSelect.AddListener(delegate { ConfirmSelection(imgSel.TabIndex); });
        }
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
