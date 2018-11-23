using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSummaryManager : MonoBehaviour {

    [SerializeField] private Text primaryText;
    [SerializeField] private Text secondaryText;
    [SerializeField] private GameObject selectorMenu;
    [SerializeField] private GameObject selectorButton;
    private Text selectorButtonText;

    private void Awake()
    {
        selectorButtonText = selectorButton.GetComponentInChildren<Text>();
        selectorButton.GetComponent<SingleButtonSelectable>().OnSelect.AddListener(HandleContinue);
    }

    private void HandleContinue()
    {
        ScreenManager.Instance.TransitionToScreen("");

        if(GameManager.NumOfPlayers == 1)
        {
            if (GameManager.GetLeftPlayer().ActiveTroopCount > 0)
            {
                ScreenManager.Instance.TransitionToScreen("StartScreen"); //Change this to the loot screen
            }
            else
            {   //If player lost
                ScreenManager.Instance.TransitionToScreen("StartScreen");
            }
        } else
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

    private void OnEnable()
    {
        selectorMenu.SetActive(false);
        StartCoroutine(ShowAfterDelay(2.0f, selectorMenu));

        if(GameManager.NumOfPlayers == 1)
        { //Single player battle end
            if(GameManager.GetLeftPlayer().ActiveTroopCount > 0)
            {
                primaryText.text = "You Won!";
                secondaryText.text = "Good job, you shrub.";
                selectorButtonText.text = "Claim Loot";
            } else
            {
                primaryText.text = "You lost!";
                secondaryText.text = "You suck, you knob!";
                selectorButtonText.text = "Admit Defeat";
            }
        } else
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

    private IEnumerator ShowAfterDelay(float delayTime, GameObject itemToShow)
    {
        yield return new WaitForSeconds(delayTime);

        itemToShow.SetActive(true);
    }
}
