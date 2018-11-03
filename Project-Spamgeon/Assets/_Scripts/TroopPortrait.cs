using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopPortrait : MonoBehaviour {

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Image portrait;
    [SerializeField] private Text levelText;
    [SerializeField] private Sprite deathSprite;

    private Troop troop;
	
	void Start () {
        GameStateHandler.StateChanged += GameStateHandler_StateChanged;
        gameObject.SetActive(false);
	}

    private void GameStateHandler_StateChanged(object sender, GameStateHandler.StateChangedArgs e)
    {
        if (e.Current == GameStateHandler.States.BATTLE_SUMMARY)
        {
            ClearTroop();
            gameObject.SetActive(false);
        }
    }

    public void AssignTroop(Troop t)
    {
        if(troop != null) { ClearTroop(); }

        troop = t;
        troop.HealthChanged += Troop_HealthChanged;
        troop.EnergyChanged += Troop_EnergyChanged;
        troop.Death += Troop_Death;

        portrait.sprite = troop.portrait;
        levelText.text = "Level: " + troop.Level.ToString();
        energySlider.value = troop.CurrentEnergy / troop.MaxEnergy;
        healthSlider.value = troop.CurrentHealth / troop.MaxHealth;
    }

    private void Troop_Death(object sender, Troop.TroopDeathArgs e)
    {
        portrait.sprite = deathSprite;
        ClearTroop();
    }

    private void Troop_EnergyChanged(object sender, System.EventArgs e)
    {
        energySlider.value = troop.CurrentEnergy / troop.MaxEnergy;
    }

    private void Troop_HealthChanged(object sender, System.EventArgs e)
    {
        healthSlider.value = troop.CurrentHealth / troop.MaxHealth;
    }

    public void ClearTroop()
    {
        troop.Death -= Troop_Death;
        troop.HealthChanged -= Troop_HealthChanged;
        troop.EnergyChanged -= Troop_EnergyChanged;
        troop = null;
    }

}
