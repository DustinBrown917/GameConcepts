using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troop : MonoBehaviour {

    public const float EXP_REQUIRED_PER_LEVEL = 60.0f;

    [SerializeField] private int level_ = 1;
    public int Level { get { return level_; } }
    [SerializeField] private int currentExp_;
    public float ExperienceThreshold { get { return EXP_REQUIRED_PER_LEVEL * level_; } }

    [SerializeField] private float maxHealth_;
    public float MaxHealth { get { return maxHealth_; } }
    [SerializeField] private float currentHealth_;
    public float CurrentHealth { get { return currentHealth_; } }

    [SerializeField] private float maxEnergy_;
    public float MaxEnergy { get { return maxEnergy_; } }
    [SerializeField] private float currentEnergy_;
    public float CurrentEnergy { get { return currentEnergy_; } }

    [SerializeField] private float attackDamage_ = 1;
    [SerializeField] private float attackSpeed_ = 0.1f;

    [SerializeField] private bool isAlive = true;

    private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    private Animator animator;

    [SerializeField] private SpriteRenderer graphic;
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
        defaultColor = graphic.color;
        defaultSpriteLocation = graphic.transform.localPosition;
        ps = GetComponent<ParticleSystem>();
        ResetTroop();
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

    public void AddEnergy()
    {
        if (!isAlive) { return; }

        currentEnergy_ += attackSpeed_;
        if(currentEnergy_ > maxEnergy_)
        {
            if(target != null)
            {
                if (Attack())
                {
                    currentEnergy_ = currentEnergy_ - maxEnergy_;
                }
                
            }
            else
            {
                currentEnergy_ = maxEnergy_;
            }
        }

        OnEnergyChanged();
    }

    private bool Attack()
    {
        if (target == null || !isAlive) { return false; } //If target is already null, then there are no enemies left.

        if (!target.isAlive) { RequestTarget(); } //If the target isn't alive, ask for another target.

        if (target == null) { return false; } //If target is null after the request, no enemies are left.

        audioSource.clip = attackSound;
        audioSource.Play();
        target.Damage(attackDamage_);
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
        currentHealth_ = maxHealth_;
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
        Transform graphicTransform = graphic.transform;
        float rumbleTime = 0.15f;
        float t = 0f;
        Vector3 rumblePosition = new Vector3(0, 0, graphic.transform.localPosition.z);

        while (t < rumbleTime)
        {
            t += Time.deltaTime;

            graphic.color = Color.Lerp(woundedColor, defaultColor, t / rumbleTime);
            rumblePosition.x += UnityEngine.Random.Range(-0.1f, 0.1f);

            graphic.transform.localPosition = defaultSpriteLocation + rumblePosition;

            yield return null;
        }

        graphic.transform.localPosition = defaultSpriteLocation;
        graphic.color = defaultColor;
    }



    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/

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
