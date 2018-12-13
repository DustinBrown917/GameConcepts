using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TroopLootPortrait : MonoBehaviour {

    [SerializeField] private Text levelText;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private Image portrait;

    [SerializeField] private TroopLootPortraitLevelUpText[] LevelUpTextContainers;

    private AudioSource audioSource;
    private Coroutine cr_swell;
    [SerializeField] private Vector3 swellSize;
    [SerializeField] private float swellTime;

    private Troop troop;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        DisableLevelUpTextContainers();
    }

    private void OnDisable()
    {
        if(troop != null)
        {
            troop.LevelUp -= Troop_LevelUp;
            troop.ExpChanged -= Troop_ExpChanged;
            troop = null;
        }
    }

    private void DisableLevelUpTextContainers()
    {
        foreach(TroopLootPortraitLevelUpText tlplut in LevelUpTextContainers)
        {
            tlplut.gameObject.SetActive(false);
        }
    }

    public void AssignTroop(Troop t)
    {
        troop = t;
        levelText.text = "Level " + t.Level.ToString();
        portrait.sprite = t.portrait;
        experienceSlider.value = t.CurrentExp / t.ExperienceThreshold;
        t.LevelUp += Troop_LevelUp;
        t.ExpChanged += Troop_ExpChanged;
    }

    private void Troop_ExpChanged(object sender, System.EventArgs e)
    {
        experienceSlider.value = troop.CurrentExp / troop.ExperienceThreshold;
    }

    private void Troop_LevelUp(object sender, Troop.LevelUpArgs e)
    {
        levelText.text = "Level " + troop.Level.ToString();

        audioSource.Play();
        CoroutineManager.BeginCoroutine(CoroutineManager.ShrinkScaleFrom(transform, swellSize, Vector3.one, swellTime), ref cr_swell, this);

        foreach(TroopLootPortraitLevelUpText tplut in LevelUpTextContainers)
        {
            if (!tplut.gameObject.activeSelf)
            {
                tplut.gameObject.SetActive(true);
                string skillName = "";

                switch (e.statThatWasLeveled.StatName)
                {
                    case TroopStatNames.INVALID:
                        skillName = "INVALID";
                        break;
                    case TroopStatNames.MAX_HEALTH:
                        skillName = "Max Health";
                        break;
                    case TroopStatNames.ATTACK_SPEED:
                        skillName = "Attack Speed";
                        break;
                    case TroopStatNames.ATTACK_DAMAGE:
                        skillName = "Attack Damage";
                        break;
                    default:
                        skillName = "DEFAULT";
                        break;
                }
                tplut.SetStatText(skillName);
                break;
            }
        }
        
    }
}
