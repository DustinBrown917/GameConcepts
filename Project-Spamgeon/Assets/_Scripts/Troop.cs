using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troop : MonoBehaviour {

    public const float EXP_REQUIRED_PER_LEVEL = 60.0f;

    [SerializeField] private int level_ = 1;
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

    private bool isAlive = true;

    private Troop target;


    private void Start()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }

    private void GameManager_GameStateChanged(object sender, GameManager.GameStateChangedArgs e)
    {
        throw new System.NotImplementedException();
    }

    public void FindTarget()
    {
        //Requires Player class
    }


    /*
     * Pre-Battle:
     * Deploy to battlefield
     * Reset health to max and energy to 0
     */

    /*
     * Battle:
     * Loop >>
     * Find target if needed
     * If target != null && target isAlive {attack target}
     * if !target.IsAlive {target = null;}
     * << Loop
     */

    /*
     * Post-Battle:
     * Clear target (for safety)
     */

    /*
     * Postgame screen (victory or defeat)
     * if(!isAlive) remove self
     */



    /************************************************************************************/
    /************************************** EVENTS **************************************/
    /************************************************************************************/


    public event EventHandler HealthChanged;

    private void OnHealthChanged()
    {
        EventHandler handler = HealthChanged;

        if(handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }

    public event EventHandler EnergyChanged;

    private void OnEnergyChanged()
    {
        EventHandler handler = EnergyChanged;

        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }

}
