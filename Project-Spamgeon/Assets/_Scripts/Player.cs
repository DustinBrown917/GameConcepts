using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private List<Troop> activeTroops;
    [SerializeField] private TroopPool troopPool;
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private Player opponent;

    [SerializeField] private Players playerType;

    [SerializeField] private PortraitHolder portraitHolder;

	// Use this for initialization
	protected void Start () {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
	}

    protected void OnDestroy()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

    protected void GameManager_GameStateChanged(object sender, GameManager.GameStateChangedArgs e)
    {
        switch (e.newState)
        {
            case GameStates.PRE_BATTLE:
                //If computer, generate new line of activeTroops
                //if(playerType == Players.COMPUTER) { GenerateActiveTroops(5); }
                GenerateActiveTroops(5);
                DeployActiveTroops();
                break;
            case GameStates.BATTLE:
                AssignTargetsToAll();
                HookUpInputGrabber();
                break;
            case GameStates.POST_BATTLE:
                UnhookInputGrabber();
                break;
            case GameStates.BATTLE_SUMMARY:
                
                break;
            default:
                break;
        }
    }

    private void Troop_Death(object sender, Troop.TroopDeathArgs e)
    {
        RemoveActiveTroop(e.troop);
    }

    private void InputGrabber_TabEvent(object sender, InputGrabber.TabEventArgs e)
    {
        if(e.player == playerType)
        {
            AddEnergyToActiveTroops();
        }
    }

    public void ChangePlayerType(Players type)
    {
        if(type == playerType) { return; }

        playerType = type;
        troopPool = GameManager.GetPlayerTroopPool(type);
    }

    public Troop GetRandomTroopFromPool()
    {
        return Instantiate(troopPool.GetRandomTroop()).GetComponent<Troop>();
    }

    public void AddActiveTroop(Troop t)
    {
        if(activeTroops.Count >= spawnPoints.Count) { return; }

        t.SetOwner(this);
        activeTroops.Add(t);
        t.Death += Troop_Death;
    }

    public void GenerateActiveTroops(int count)
    {
        if(count > spawnPoints.Count) { count = 5; }

        for(int i = 0; i < count; i++)
        {
            AddActiveTroop(GetRandomTroopFromPool());
        }
    }

    public Troop GetActiveTroop()
    {
        if(activeTroops.Count == 0) { return null; }
        return activeTroops[UnityEngine.Random.Range(0, activeTroops.Count)];
    }

    public Troop GetOpponentActiveTroop()
    {
        return opponent.GetActiveTroop();
    }

    private void AssignTargetsToAll()
    {
        for(int i = 0; i<activeTroops.Count; i++)
        {
            activeTroops[i].AssignTarget(opponent.GetActiveTroop());
        }
    }

    private void DeployActiveTroops()
    {
        if(activeTroops.Count == 0) { return; }
        for(int i = 0; (i < activeTroops.Count && i < spawnPoints.Count); i++)
        {
            activeTroops[i].transform.SetParent(spawnPoints[i].transform);
            activeTroops[i].transform.localPosition = Vector3.zero;
            activeTroops[i].transform.localScale = Vector3.one;
            portraitHolder.AssignTroop(activeTroops[i]);
        }
    }

    private void RemoveActiveTroop(Troop t)
    {
        t.Death -= Troop_Death;
        activeTroops.Remove(t);
    }

    private void HookUpInputGrabber()
    {
        if(playerType != Players.COMPUTER)
        {
            InputGrabber.Instance.TabEvent += InputGrabber_TabEvent;
        }
    }

    private void UnhookInputGrabber()
    {
        if (playerType != Players.COMPUTER)
        {
            InputGrabber.Instance.TabEvent -= InputGrabber_TabEvent;
        }
    }

    private void AddEnergyToActiveTroops()
    {
        for(int i = 0; i < activeTroops.Count; i++)
        {
            activeTroops[i].AddEnergy();
        }
    }


}

public enum Players
{
    INVALID = -1,
    SINGLE,
    FIRST,
    SECOND,
    COMPUTER
}
