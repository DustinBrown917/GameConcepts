using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateHandler {
    private static States currentState_ = States.INVALID;
    public static States CurrentState { get { return currentState_; } }


    public static void ChangeState(States newState)
    {
        StateChangedArgs args = new StateChangedArgs(currentState_, newState);

        currentState_ = newState;

        if(currentState_ == States.BATTLE)
        {
            MusicManager.Instance.SetCurrentSong(MusicManager.Songs.BATTLE);
        } else
        {
            MusicManager.Instance.SetCurrentSong(MusicManager.Songs.MENU);           
        }

        OnStateChanged(args);
    }

    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

    public static event EventHandler<StateChangedArgs> StateChanged;

    public class StateChangedArgs : EventArgs
    {
        public States Previous;
        public States Current;

        public StateChangedArgs(States previous_, States current_)
        {
            Previous = previous_;
            Current = current_;
        }
    }

    private static void OnStateChanged(StateChangedArgs stateChangedArgs)
    {
        EventHandler<StateChangedArgs> handler = StateChanged;

        if(handler != null)
        {
            handler(typeof(GameStateHandler), stateChangedArgs);
        }
    }


    public enum States
    {
        INVALID = -1,
        MAIN,
        PRE_BATTLE,
        BATTLE,
        POST_BATTLE,
        BATTLE_SUMMARY,
        LOOT
    }
}
