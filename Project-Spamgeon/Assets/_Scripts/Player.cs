using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private List<Troop> activeTroops;
    public int ActiveTroopCount{ get { return activeTroops.Count; } }
    [SerializeField] private string troopPoolName;
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private Player opponent;

    [SerializeField] private int playerToListenTo = -1;

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
                if (playerToListenTo == -1)
                {
                    GenerateActiveTroops(UnityEngine.Random.Range(1, Mathf.Clamp(GameManager.CurrentDungeonDepth / 2, 1, 5)));
                }
                DeployActiveTroops();
                break;

            case GameStateHandler.States.BATTLE:
                AssignTargetsToAll();
                HookUpInputGrabber();

                if (playerToListenTo == -1)
                {
                    CoroutineManager.BeginCoroutine(ComputerEnergyDistribution(), ref cr_ComputerEnergyDistrubution, this);
                }
                break;

            case GameStateHandler.States.POST_BATTLE:
                UnhookInputGrabber();
                if (playerToListenTo == -1)
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
        if(e.playerIndex == playerToListenTo)
        {
            AddEnergyToActiveTroops();
            AddEnergyToActiveTroops();
            AddEnergyToActiveTroops();
            AddEnergyToActiveTroops();
            AddEnergyToActiveTroops();
            AddEnergyToActiveTroops();
        }
    }

    public void ChangePlayerType(int playerIndex)
    {
        if(playerIndex == playerToListenTo) { return; }

        playerToListenTo = playerIndex;
        switch (playerToListenTo)
        {
            case 0:
                troopPoolName = "Explorers";
                break;
            case 1:
                troopPoolName = "Combined";
                break;
            case 2:
                troopPoolName = "Combined";
                break;
            default:
                troopPoolName = "Monsters";
                break;
        }
    }

    public Troop GetRandomTroopFromPool()
    {
        return TroopPoolManager.GetPool(troopPoolName).GetRandomTroop().GetComponent<Troop>();
    }

    public Troop InstantiateTroop(Troop troopPrefab)
    {
        return Instantiate(troopPrefab.gameObject).GetComponent<Troop>();
    }

    public void AddActiveTroop(Troop t)
    {
        if(activeTroops.Count >= spawnPoints.Count) { return; }

        t = InstantiateTroop(t);

        NewTroopAddedArgs args = new NewTroopAddedArgs(this, t);

        t.SetOwner(this);
        activeTroops.Add(t);
        t.Death += Troop_Death;

        OnNewTroopAdded(args);
    }

    public void GenerateActiveTroops(int count)
    {
        if(count > spawnPoints.Count) { count = 5; }

        for(int i = 0; i < count; i++)
        {
            AddActiveTroop(GetRandomTroopFromPool());
        }
    }

    public Troop GetRandomActiveTroop()
    {
        if(activeTroops.Count == 0) { return null; }
        return activeTroops[UnityEngine.Random.Range(0, activeTroops.Count)];
    }

    public Troop GetActiveTroop(int index)
    {
        if(index < 0 || index >= activeTroops.Count) { return null; }
        return activeTroops[index];
    }

    public Troop GetOpponentActiveTroop()
    {
        return opponent.GetRandomActiveTroop();
    }

    private void AssignTargetsToAll()
    {
        for(int i = 0; i<activeTroops.Count; i++)
        {
            activeTroops[i].AssignTarget(opponent.GetRandomActiveTroop());
        }
    }

    private void DeployActiveTroops()
    {
        if (activeTroops.Count == 0) { return; }
        for (int i = 0; (i < activeTroops.Count && i < spawnPoints.Count); i++)
        {
            activeTroops[i].transform.SetParent(spawnPoints[i].transform);
            activeTroops[i].transform.localPosition = Vector3.zero;
            activeTroops[i].transform.localScale = Vector3.one;
            portraitHolder.AssignTroop(activeTroops[i]);
        }
    }

    public void RemoveActiveTroop(Troop t)
    {
        t.Death -= Troop_Death;
        activeTroops.Remove(t);

        TroopRemovedArgs args = new TroopRemovedArgs(this, t);
        OnTroopRemoved(args);

        if(activeTroops.Count == 0 && GameStateHandler.CurrentState == GameStateHandler.States.BATTLE)
        {
            OnNoMoreTroopsLeft(new NoMoreTroopsLeftArgs(this));
        }
    }

    private void HookUpInputGrabber()
    {
        if(playerToListenTo != -1)
        {
            InputGrabber.Instance.TabEvent += InputGrabber_TabEvent;
        }
    }

    private void UnhookInputGrabber()
    {
        if (playerToListenTo != -1)
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


    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

    public event EventHandler<TroopRemovedArgs> TroopRemoved;

    public class TroopRemovedArgs : EventArgs
    {
        public Player player;
        public Troop AddedTroop;

        public TroopRemovedArgs(Player p, Troop t)
        {
            player = p;
            AddedTroop = t;
        }
    }

    private void OnTroopRemoved(TroopRemovedArgs e)
    {
        EventHandler<TroopRemovedArgs> handler = TroopRemoved;

        if (handler != null)
        {
            handler(this, e);
        }
    }



    public event EventHandler<NewTroopAddedArgs> NewTroopAdded;

    public class NewTroopAddedArgs : EventArgs
    {
        public Player player;
        public Troop AddedTroop;

        public NewTroopAddedArgs(Player p, Troop t)
        {
            player = p;
            AddedTroop = t;
        }
    }

    private void OnNewTroopAdded(NewTroopAddedArgs e)
    {
        EventHandler<NewTroopAddedArgs> handler = NewTroopAdded;

        if(handler != null)
        {
            handler(this, e);
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
        EventHandler<NoMoreTroopsLeftArgs> handler = NoMoreTroopsLeft;

        if(handler != null)
        {
            handler(this, args);
        }
    }

}

