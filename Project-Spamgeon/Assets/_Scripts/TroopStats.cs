using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TroopStatsContainer {

    [SerializeField] private TroopStat[] stats_;

    public void Initialize()
    {
        SortStats();
        AdjustLevelingWeights();
    }

    /// <summary>
    /// Sorts the stats in order of smallest weight to largest, with smallest weight at index 0.
    /// </summary>
    private void SortStats()
    {
        TroopStat temp;

        for(int i = 0; i < stats_.Length; i++)
        {
            for (int j = 0; j < stats_.Length - 1; j++)
            {
                if (stats_[j].LevelingWeight > stats_[j + 1].LevelingWeight)
                {
                    temp = stats_[j + 1];
                    stats_[j + 1] = stats_[j];
                    stats_[j] = temp;
                }
            }
        }
    }

    /// <summary>
    /// Adjusts the levelingWeights of the stats so that they are normalized to their combined total.
    /// </summary>
    private void AdjustLevelingWeights()
    {
        for(int i = 1; i < stats_.Length; i++)
        {
            stats_[i].LevelingWeight += stats_[i - 1].LevelingWeight;
        }
    }

    /// <summary>
    /// Gets a stat based on its leveling weight, given a number between 0.0 and 1.0.
    /// </summary>
    /// <param name="normal">Number between 0.0 and 1.0</param>
    /// <returns></returns>
    public TroopStat GetStatFromNoramlized(float normal)
    {
        normal *= stats_[stats_.Length - 1].LevelingWeight;

        foreach(TroopStat ts in stats_)
        {
            if(ts.LevelingWeight >= normal) { return ts; }
        }

        return null;
    }


    /// <summary>
    /// Get a specific stat from the array of stats. If the stat does not exist in the collection, return null.
    /// </summary>
    /// <param name="statName">The name of the stat to find.</param>
    /// <returns></returns>
    public TroopStat GetStat(TroopStatNames statName)
    {
        foreach(TroopStat ts in stats_)
        {
            if(ts.StatName == statName)
            {
                return ts;
            }
        }

        return null;
    }

}

[Serializable]
public class TroopStat
{
    [SerializeField] private TroopStatNames statName_;
    public TroopStatNames StatName { get { return statName_; } }

    [SerializeField] private float currentValue_ = 1;
    public float CurrentValue { get { return currentValue_; } set { currentValue_ = value; } }

    [SerializeField] private float levelingWeight_ = 25.0f;
    public float LevelingWeight { get { return levelingWeight_; } set { levelingWeight_ = value; } }

    [SerializeField] private float levelingAmount_ = 0.1f;
    public float LevelingAmount { get { return levelingAmount_; } }

    public void LevelUp()
    {
        currentValue_ += levelingAmount_;
    }
}

public enum TroopStatNames
{
    INVALID = -1,
    MAX_HEALTH,
    ATTACK_SPEED,
    ATTACK_DAMAGE
}
