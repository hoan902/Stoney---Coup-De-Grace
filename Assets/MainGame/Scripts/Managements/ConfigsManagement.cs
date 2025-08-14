using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

public class ConfigsManagement : ComponentSingleton<ConfigsManagement>
{
    public LevelingStatsSO statsConfig;
    public LevelConfigSO levelsRuleConfig;
    //HoanDN: This is for switching between mode and setup gamemode (applied stats for enemies)
}
