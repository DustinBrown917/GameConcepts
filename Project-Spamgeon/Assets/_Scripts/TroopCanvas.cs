using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This class handles the canvas attached to a troop in battle.
public class TroopCanvas : MonoBehaviour {

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider energySlider;

    [SerializeField] private Troop troop;

	// Use this for initialization
	void Start () {
        troop.HealthChanged += Troop_HealthChanged;
        troop.EnergyChanged += Troop_EnergyChanged;
        energySlider.value = troop.CurrentEnergy / troop.MaxEnergy;
        healthSlider.value = troop.CurrentHealth / troop.MaxHealth;
    }

    private void Troop_EnergyChanged(object sender, System.EventArgs e)
    {
        energySlider.value = troop.CurrentEnergy / troop.MaxEnergy;
    }

    private void Troop_HealthChanged(object sender, System.EventArgs e)
    {
        healthSlider.value = troop.CurrentHealth / troop.MaxHealth;
    }

    private void OnDestroy()
    {
        troop.HealthChanged -= Troop_HealthChanged;
        troop.EnergyChanged -= Troop_EnergyChanged;
    }
}
