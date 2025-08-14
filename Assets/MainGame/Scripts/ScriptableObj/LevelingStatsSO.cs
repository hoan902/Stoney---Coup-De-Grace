using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "stats-config", menuName = "CustomScriptable/StatsConfig", order = 1)]
public class LevelingStatsSO : ScriptableObject
{
    [SerializeField] private List<StatsConfig> m_botLevelingStats;
    [SerializeField] private List<StatsConfig> m_playerLevelingStats;

    public StatsConfig GetBotStatByLevel(int level)
    {
        return m_botLevelingStats.Find(stat => stat.level == level);
    }
    
    public StatsConfig GetPlayerStatByLevel(int level)
    {
        return m_playerLevelingStats.Find(stat => stat.level == level);
    }
    
    public int GetTotalBotDataCount()
    {
        return m_botLevelingStats.Count;
    }
    
    public int GetTotalPlayerDataCount()
    {
        return m_playerLevelingStats.Count;
    }
}
[Serializable]
public class StatsConfig
{
    public string characterTitle;
    public int level;
    public int health;
    public int attackDamage;
    public BaseComboSet comboSet;
}
