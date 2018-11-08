using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChanger : MonoBehaviour {

    public GameStateHandler.States stateToChangeTo;

    private void OnEnable()
    {
        GameStateHandler.ChangeState(stateToChangeTo);
    }
}
