using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private List<Troop> activeTroops;
    public int ActiveTroopCount{ get { return activeTroops.Count; } }
    [SerializeField] private TroopPool troopPool;
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private Player opponent;

    [SerializeField] private Players playerType;

    [SerializeField] private PortraitHolder portraitHolder;

    private Coroutine cr_ComputerEnergyDistrubution;
    [SerializeField] private float computerEnergyDistributionFrequency = 0.25f;

	// Use this for initialization
	protected void Start () {
        GameStateHandler.StateChanged += GameStateHandler_StateChanged;
	}

    protected void OnDestroy()
    {
        GameStateHandler.StateChanged -= GameStateHandler_StateChanged;
    }

    private void GameStateHandler_StateChanged(object sender, GameStateHandler.StateChangedArgs e)
    {
        switch (e.Current)
        {
            case GameStateHandler.States.PRE_BATTLE:
                if (playerType == Players.COMPUTER)
                {
                    GenerateActiveTroops(UnityEngine.Random.Range(1, Mathf.Clamp(GameManager.CurrentDungeonDepth / 2, 1, 5)));
                }
                DeployActiveTroops();
                break;

            case GameStateHandler.States.BATTLE:
                AssignTargetsToAll();
                HookUpInputGrabber();

                if (playerType == Players.COMPUTER)
                {
                    CoroutineManager.BeginCoroutine(ComputerEnergyDistribution(), ref cr_ComputerEnergyDistrubution, this);
                }
                break;

            case GameStateHandler.States.POST_BATTLE:
                UnhookInputGrabber();
                if (playerType == Players.COMPUTER)
                {
                    CoroutineManager.HaltCoroutine(ref cr_ComputerEnergyDistrubution, this);
                }
                break;

            case GameStateHandler.States.BATTLE_SUMMARY:
                break;

            default:
                break;
        }
    }

    private IEnumerator ComputerEnergyDistribution()
    {
        float frequency = computerEnergyDistributionFrequency - Mathf.Clamp(GameManager.CurrentDungeonDepth * 0.01f, 0.1f, 99.0f);
        while (true){
            foreach(Troop t in activeTroops)
            {
                t.AddEnergy();
            }
            yield return new WaitForSeconds(frequency);
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
        return troopPool.GetRandomTroop().GetComponent<Troop>();
    }

    public Troop InstantiateTroop(Troop troopPrefab)
    {
        return Instantiate(troopPrefab.gameObject).GetComponent<Troop>();
    }

    public void AddActiveTroop(Troop t)
    {
        if(activeTroops.Count >= spawnPoints.Count) { return; }

        t = InstantiateTroop(t);

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
        if(activeTroops.Count == 0)
        {
            OnNoMoreTroopsLeft(new NoMoreTroopsLeftArgs(this));
        }
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

    public event EventHandler<NoMoreTroopsLeftArgs> NoMoreTroopsLeft;

    public class NoMoreTroopsLeftArgs : EventArgs
    {
        public Player player;

        public NoMoreTroopsLeftArgs(Player p)
        {
            player = p;
        }
    }

    private void OnNoMoreTroopsLeft(NoMoreTroopsLeftArgs args)
    {
        Debug.Log(gameObject.name + " no more troops.");
        EventHandler<NoMoreTroopsLeftArgs> handler = NoMoreTroopsLeft;

        if(handler != null)
        {
            handler(this, args);
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
