using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySelectSingleScreen : GameScreen {

    [SerializeField] private SelectionMeter mainSelectionMeter;
    [SerializeField] private Transform troopSelectableHolder;
    [SerializeField] private ImageSelectable troopSelectablePrefab;
    [SerializeField] private Text explorerNameText;
    [SerializeField] private string targetTroopPool;

    private TroopPool troopPool;


    // Use this for initialization
    protected override void Start()
    {
        base.Start();        

        troopPool = TroopPoolManager.GetPool(targetTroopPool);

        AddTroopSelectables();
        sbsManager.RefreshSelectables();
        sbsManager.AddSelectionMeter(0, mainSelectionMeter);
    }

    private void AddTroopSelectables()
    {
        if(troopPool == null)
        {
            Debug.LogWarning("Could not find TroopPool \"" + targetTroopPool + "\".");
            return;
        }

        for(int i = 0; i < troopPool.Count; i++)
        {
            ImageSelectable imgSel = Instantiate(troopSelectablePrefab.gameObject, troopSelectableHolder).GetComponent<ImageSelectable>();
            imgSel.SetImageSprite(troopPool[i].portrait);
            imgSel.SetTabIndex(i);
            imgSel.OnFocus.AddListener(delegate { UpdateTroopDisplayedData(imgSel.TabIndex); });
        }
    }

    public void UpdateTroopDisplayedData(int index)
    {
        if(index >= 0 && index < troopPool.Count)
        {
            explorerNameText.text = troopPool[index].Name;
        } else
        {
            explorerNameText.text = "";
        }
    }

}
