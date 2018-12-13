using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troop : MonoBehaviour {

    public const float EXP_REQUIRED_PER_LEVEL = 60.0f;

    [SerializeField] private string name_;
    public string Name { get { return name_; } }
    [SerializeField] private int level_ = 1;
    public int Level { get { return level_; } }
    [SerializeField] private float currentExp_;
    public float CurrentExp { get { return currentExp_; } }
    public float ExperienceThreshold { get { return EXP_REQUIRED_PER_LEVEL * level_; } }

    [SerializeField] private TroopStatsContainer stats;

    public float MaxHealth { get { return stats.GetStat(TroopStatNames.MAX_HEALTH).CurrentValue; } }
    [SerializeField] private float currentHealth_;
    public float CurrentHealth { get { return currentHealth_; } }

    public float MaxEnergy { get { return 10.0f; } }
    [SerializeField] private float currentEnergy_;
    public float CurrentEnergy { get { return currentEnergy_; } }

    public float AttackDamage { get { return stats.GetStat(TroopStatNames.ATTACK_DAMAGE).CurrentValue; } }

    public float AttackSpeed { get { return stats.GetStat(TroopStatNames.ATTACK_SPEED).CurrentValue; } }

    [SerializeField] private bool isAlive = true;

    private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    private Animator animator;

    [SerializeField] private SpriteRenderer graphic_;
    public SpriteRenderer Graphic { get { return graphic_; } }
    [SerializeField] private Color woundedColor;
    private Color defaultColor;
    private Vector3 defaultSpriteLocation;

    private ParticleSystem ps;

    private Troop target = null;

    private Player owner = null;

    public Sprite portrait;



    /************************************************************************************/
    /********************************* UNITY BEHAVIOURS *********************************/
    /************************************************************************************/

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        defaultColor = graphic_.color;
        defaultSpriteLocation = graphic_.transform.localPosition;
        ps = GetComponent<ParticleSystem>();
        ResetTroop();
        stats.Initialize();
    }

    private void Start()
    {
        GameStateHandler.StateChanged += GameStateHandler_StateChanged;
        
    }



    private void OnDestroy()
    {
        owner = null;
        GameStateHandler.StateChanged -= GameStateHandler_StateChanged;
    }

    /************************************************************************************/
    /************************************ BEHAVIOURS ************************************/
    /************************************************************************************/

    private void GameStateHandler_StateChanged(object sender, GameStateHandler.StateChangedArgs e)
    {
        switch (e.Current)
        {
            case GameStateHandler.States.PRE_BATTLE:
                ResetTroop();
                break;
            case GameStateHandler.States.BATTLE:
                RequestTarget();
                break;
            case GameStateHandler.States.POST_BATTLE:
                target = null;
                break;
            case GameStateHandler.States.BATTLE_SUMMARY:
                if (!isAlive) { Destroy(gameObject); }
                break;
            case GameStateHandler.States.MAIN:
                Kill(false, false);
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }

    public void SetOwner(Player p)
    {
        owner = p;
    }

    public void RequestTarget()
    {
        target = owner.GetOpponentActiveTroop();
    }

    public void AssignTarget(Troop t)
    {
        target = t;
    }

    public void Damage(float amount)
    {
        if (!isAlive) { return; }

        currentHealth_ -= amount;
        StartCoroutine(DamageRumble());
        ps.Emit(15);
        OnHealthChanged();
        if(currentHealth_ <= 0)
        {
            currentHealth_ = 0;
            Kill();
        }
    }

    /// <summary>
    /// Add energy to the troop. Trigger attack and cycle back to zero if energy is greater than max energy.
    /// </summary>
    public void AddEnergy()
    {
        if (!isAlive) { return; }

        currentEnergy_ += AttackSpeed;
        if(currentEnergy_ > MaxEnergy) {
            if(target != null) {
                if (Attack()) {
                    currentEnergy_ = currentEnergy_ - MaxEnergy;
                }  
            }
            else {
                currentEnergy_ = MaxEnergy;
            }
        }
        OnEnergyChanged();
    }

    /// <summary>
    /// Adds experience to the troop. Increments level if the exp threshold is passed, then resets the currenExp with the remainder added.
    /// </summary>
    /// <param name="exp">The amount of exp to add.</param>
    public void AddExperience(int exp)
    {
        currentExp_ += exp;
        
        while(currentExp_ >= ExperienceThreshold)
        {
            currentExp_ -= ExperienceThreshold;
            IncrementLevel();
        }

        OnExpChanged();
    }

    /// <summary>
    /// Increments the troops level, increases a random weighted stat and dispatches the LevelUp event.
    /// </summary>
    public void IncrementLevel()
    {
        level_++;

        float levelStatAtNormalWeight = UnityEngine.Random.value;

        TroopStat leveledStat = stats.GetStatFromNoramlized(levelStatAtNormalWeight);

        leveledStat.LevelUp();

        LevelUpArgs args = new LevelUpArgs(this, Level, leveledStat);
        OnLevelUp(args);
    }

    /// <summary>
    /// Levels up the troop to the target level.
    /// </summary>
    /// <param name="targetLevel">The level to push the troop to.</param>
    public void PushToLevel(int targetLevel)
    {
        while(level_ < targetLevel)
        {
            IncrementLevel();
        }
    }

    private bool Attack()
    {
        if (target == null || !isAlive) { return false; } //If target is already null, then there are no enemies left.

        if (!target.isAlive) { RequestTarget(); } //If the target isn't alive, ask for another target.

        if (target == null) { return false; } //If target is null after the request, no enemies are left.

        audioSource.clip = attackSound;
        audioSource.Play();
        target.Damage(AttackDamage);
        animator.SetTrigger("Attack");

        return true;
    }

    public void Kill(bool playSound = true, bool animate = true, bool passEvents = true)
    {
        if(currentHealth_ != 0 && passEvents)
        {
            currentHealth_ = 0;
            OnHealthChanged();
        }

        if (playSound)
        {
            audioSource.clip = deathSound;
            audioSource.Play();
        }
        if (animate)
        {
            animator.SetTrigger("Death");
        }

        if (passEvents)
        {
            OnDeath();
        }
        
    }

    public void ResetTroop()
    {
        isAlive = true;
        currentHealth_ = MaxHealth;
        currentEnergy_ = 0.0f;
        OnHealthChanged();
        OnEnergyChanged();
    }

    public void CleanDestroy()
    {
        Kill(false, false, false);
        Destroy(gameObject);
    }

    private IEnumerator DamageRumble()
    {
        Transform graphicTransform = graphic_.transform;
        float rumbleTime = 0.15f;
        float t = 0f;
        Vector3 rumblePosition = new Vector3(0, 0, graphic_.transform.localPosition.z);

        while (t < rumbleTime)
        {
            t += Time.deltaTime;

            graphic_.color = Color.Lerp(woundedColor, defaultColor, t / rumbleTime);
            rumblePosition.x += UnityEngine.Random.Range(-0.1f, 0.1f);

            graphic_.transform.localPosition = defaultSpriteLocation + rumblePosition;

            yield return null;
        }

        graphic_.transform.localPosition = defaultSpriteLocation;
        graphic_.color = defaultColor;
    }



    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

    #region LevelUp Event
    public event EventHandler<LevelUpArgs> LevelUp;
    
    public class LevelUpArgs : EventArgs
    {
        public Troop troop;
        public int newLevel;
        public TroopStat statThatWasLeveled;

        public LevelUpArgs(Troop t, int newLevel_, TroopStat statThatWasLeveled_)
        {
            troop = t;
            newLevel = newLevel_;
            statThatWasLeveled = statThatWasLeveled_;
        }
    }

    private void OnLevelUp(LevelUpArgs e)
    {
        EventHandler<LevelUpArgs> handler = LevelUp;

        if(handler != null)
        {
            handler(this, e);
        }
    }
    #endregion

    #region HealthChanged Event
    public event EventHandler HealthChanged;

    private void OnHealthChanged()
    {
        EventHandler handler = HealthChanged;

        if(handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
    #endregion

    #region EnergyChanged Event
    public event EventHandler EnergyChanged;

    private void OnEnergyChanged()
    {
        EventHandler handler = EnergyChanged;

        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
    #endregion

    #region ExpChanged Event
    public event EventHandler ExpChanged;

    private void OnExpChanged()
    {
        EventHandler handler = ExpChanged;

        if(handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
    #endregion

    #region Death Event
    public event EventHandler<TroopDeathArgs> Death;

    public class TroopDeathArgs : EventArgs
    {
        public Troop troop;

        public TroopDeathArgs(Troop troop)
        {
            this.troop = troop;
        }
    }

    private void OnDeath()
    {
        isAlive = false;
        EventHandler<TroopDeathArgs> handler = Death;

        if (handler != null)
        {
            handler(this, new TroopDeathArgs(this));
        }
    }
    #endregion
}

