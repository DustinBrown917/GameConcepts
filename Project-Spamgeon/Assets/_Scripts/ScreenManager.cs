using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {

    private static ScreenManager instance_;
    public static ScreenManager Instance { get { return instance_; } }

    [SerializeField] private ScreenWrapper[] screens;

    [SerializeField]private MenuScreen currentScreen;
    private MenuScreen screenLeaving;
    private MenuScreen screenComing;

    private bool screenLeavingTransitioning = false;
    private bool screenComingTransitioning = false;

    private void Awake()
    {
        instance_ = this;
    }

    private void Start()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }

    private void GameManager_GameStateChanged(object sender, GameManager.GameStateChangedArgs e)
    {
        
        if(e.newState == GameStates.POST_BATTLE)
        {
            if (GameManager.NumOfPlayers == 1)
            {
                
                if (GameManager.GetLeftPlayer().ActiveTroopCount > 0)
                {
                    TransitionToScreen((int)ScreenTypes.VICTORY_SINGLE);
                } else
                {
                    TransitionToScreen((int)ScreenTypes.DEFEAT_SINGLE);
                }
                
            } else
            {
                //Multi player stuff
            }
            
        }
    }

    public void TransitionToScreen(int index)
    {
        MenuScreen newScreen = GetScreen((ScreenTypes)index);
        if(currentScreen == newScreen) { return; }

        if(currentScreen != null)
        {
            screenLeavingTransitioning = true;
            screenLeaving = currentScreen;
            currentScreen = null;
            screenLeaving.TransitionComplete += ScreenLeaving_TransitionComplete;
            screenLeaving.FadeOut();
        }

        if(newScreen != null)
        {
            screenComingTransitioning = true;
            screenComing = newScreen;
            screenComing.gameObject.SetActive(true);
            screenComing.TransitionComplete += ScreenComing_TransitionComplete;
            screenComing.FadeIn();
        } else
        {
            GameManager.ChangeGameState(GameStates.PRE_BATTLE);
        }
    }

    private void ScreenComing_TransitionComplete(object sender, MenuScreen.TransitionCompleteArgs e)
    {
        e.screen.TransitionComplete -= ScreenComing_TransitionComplete;
        currentScreen = screenComing;
        screenComing = null;
        screenComingTransitioning = false;
    }

    private void ScreenLeaving_TransitionComplete(object sender, MenuScreen.TransitionCompleteArgs e)
    {
        e.screen.TransitionComplete -= ScreenLeaving_TransitionComplete;
        e.screen.gameObject.SetActive(false);
        screenLeaving = null;
        screenLeavingTransitioning = false;
        if(GameManager.GetCurrentState() == GameStates.PRE_BATTLE)
        {
            GameManager.ChangeGameState(GameStates.BATTLE);
        }
    }

    private MenuScreen GetScreen(ScreenTypes screen)
    {
        for(int i = 0; i < screens.Length; i++)
        {
            if(screens[i].ScreenType == screen)
            {
                return screens[i].MenuScreen;
            }
        }

        return null;
    }

    public event EventHandler TransitionsDone;
    
    private void OnTranstionsDone()
    {
        EventHandler handler = TransitionsDone;

        if(handler!= null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}

[System.Serializable]
public class ScreenWrapper
{
    [SerializeField] private ScreenTypes screenType_;
    public ScreenTypes ScreenType { get { return screenType_; } }
    [SerializeField] private MenuScreen menuScreen_;
    public MenuScreen MenuScreen { get { return menuScreen_; } }
}

public enum ScreenTypes
{
    INVALID = -1,
    START,
    PARTY_SELECT_SINGLE,
    PARTY_SELECT_MULTI,
    VICTORY_SINGLE,
    DEFEAT_SINGLE
}
