using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {

    private static ScreenManager instance_;
    public static ScreenManager Instance { get { return instance_; } }

    [SerializeField] private GameScreen[] screens;

    [SerializeField]private GameScreen currentScreen;
    private GameScreen screenLeaving;
    private GameScreen screenComing;

    private void Awake()
    {
        instance_ = this;
    }

    private void Start()
    {
        GameStateHandler.StateChanged += GameStateHandler_StateChanged;
    }

    private void GameStateHandler_StateChanged(object sender, GameStateHandler.StateChangedArgs e)
    {
        if (e.Current == GameStateHandler.States.POST_BATTLE)
        {
            if (GameManager.NumOfPlayers == 1)
            {

                if (GameManager.GetLeftPlayer().ActiveTroopCount > 0)
                {
                    TransitionToScreen("Victory Screen");
                }
                else
                {
                    TransitionToScreen("Defeat Screen");
                }
            }
            else
            {
                //Multi player stuff
            }

        }
    }

    public void TransitionToScreen(string screenName)
    {
        GameScreen newScreen = GetGameScreen(screenName);
        if(currentScreen == newScreen) { return; }

        if(currentScreen != null)
        {
            screenLeaving = currentScreen;
            currentScreen = null;
            screenLeaving.TransitionComplete += ScreenLeaving_TransitionComplete;
            screenLeaving.FadeOut();
        }

        if(newScreen != null)
        {
            screenComing = newScreen;
            screenComing.gameObject.SetActive(true);
            screenComing.TransitionComplete += ScreenComing_TransitionComplete;
            screenComing.FadeIn();
        }
    }

    private void ScreenComing_TransitionComplete(object sender, GameScreen.TransitionCompleteArgs e)
    {
        e.screen.TransitionComplete -= ScreenComing_TransitionComplete;
        currentScreen = screenComing;
        screenComing = null;
    }

    private void ScreenLeaving_TransitionComplete(object sender, GameScreen.TransitionCompleteArgs e)
    {
        e.screen.TransitionComplete -= ScreenLeaving_TransitionComplete;
        e.screen.gameObject.SetActive(false);
        screenLeaving = null;
    }

    private GameScreen GetGameScreen(string name)
    {
        for(int i = 0; i < screens.Length; i++)
        {
            if(screens[i].Name == name)
            {
                return screens[i];
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
    [SerializeField] private GameScreen menuScreen_;
    public GameScreen MenuScreen { get { return menuScreen_; } }
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
