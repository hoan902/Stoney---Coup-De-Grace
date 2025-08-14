using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "level-rule-config", menuName = "CustomScriptable/LevelConfig", order = 1)]
public class LevelConfigSO : ScriptableObject
{
    [SerializeField] private List<LevelConfig> m_LevelGenRule;

    public LevelConfig GetLevelRule(GameMode gameMode)
    {
        return m_LevelGenRule.Find(levelRule => levelRule.gameMode == gameMode);
    }
}
[Serializable]
public class LevelConfig
{
    [ShowInInspector, LabelText("gameMode")]
    public GameMode gameMode;
    public int botAddedPerLevel;
    public int levelReqForBotLevelUp;
    public int bonusHealthPerLevel;
    public bool isAllyAssist; 
}