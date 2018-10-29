using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {

    [SerializeField] private ScreenWrapper[] screens;

    [SerializeField]private MenuScreen currentScreen;
    private MenuScreen screenLeaving;
    private MenuScreen screenComing;

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void TransitionToScreen(int index)
    {
        MenuScreen newScreen = GetScreen((ScreenTypes)index);
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

    private void ScreenComing_TransitionComplete(object sender, MenuScreen.TransitionCompleteArgs e)
    {
        e.screen.TransitionComplete -= ScreenComing_TransitionComplete;
        currentScreen = screenComing;
        screenComing = null;
    }

    private void ScreenLeaving_TransitionComplete(object sender, MenuScreen.TransitionCompleteArgs e)
    {
        e.screen.TransitionComplete -= ScreenLeaving_TransitionComplete;
        e.screen.gameObject.SetActive(false);
        screenLeaving = null;
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
