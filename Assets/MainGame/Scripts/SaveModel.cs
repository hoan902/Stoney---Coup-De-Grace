using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveModel 
{
    public static int currentLevel;
    public static int currentLevelOneVMany;
    public static int currentLevelManyVMany;
    public static int playerLevel;

    public static bool saveFileLoaded = false;

    private static int m_playerHP;
    private static int m_maxPlayerHP;
    public static int playerHP => m_playerHP;
    public static int maxPlayerHP => m_maxPlayerHP;

    public static void LoadCurrentSave() 
    {
        saveFileLoaded = true;
        LoadUserInfo();
        SaveAllInfo();
    }
    
    public static void LoadUserInfo()
    {
        currentLevel = PlayerPrefs.GetInt(GlobalConstants.CURRENT_LEVEL, 1);
        currentLevelOneVMany = PlayerPrefs.GetInt(GlobalConstants.CURRENT_LEVEL_OVM, 1);
        currentLevelManyVMany = PlayerPrefs.GetInt(GlobalConstants.CURRENT_LEVEL_MVM, 1);
        playerLevel = PlayerPrefs.GetInt(GlobalConstants.CURRENT_PLAYER_LEVEL, 1);
    }
    
    public static void SaveAllInfo()
    {
        PlayerPrefs.SetInt(GlobalConstants.CURRENT_LEVEL, currentLevel);
        PlayerPrefs.SetInt(GlobalConstants.CURRENT_LEVEL_OVM, currentLevelOneVMany);
        PlayerPrefs.SetInt(GlobalConstants.CURRENT_LEVEL_MVM, currentLevelManyVMany);
        PlayerPrefs.SetInt(GlobalConstants.CURRENT_PLAYER_LEVEL, playerLevel);
        PlayerPrefs.Save();
    }

    public static void UpdateHp(int value, int currentMaxHp)
    {
        m_playerHP += value;
        m_playerHP = Mathf.Clamp(m_playerHP, -1, currentMaxHp);
    }

    public static void UpdateMaxHp(int currentMaxHp)
    {
        m_maxPlayerHP = currentMaxHp;
    }

    public void UpdatePlayerLevel(int updatedLevel)
    {
        playerLevel = updatedLevel;
    }

    public void UpdateLevel(int updatedLevel)
    {
        currentLevel = updatedLevel;
    }
}
