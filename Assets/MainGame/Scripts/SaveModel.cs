using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveModel 
{
    public static int currentLevel;
    public static int playerLevel;
    
    public static bool saveFileLoaded = false;
    
    public static void LoadCurrentSave() 
    {
        saveFileLoaded = true;
        LoadUserInfo();
        SaveAllInfo();
    }
    
    public static void LoadUserInfo()
    {
        currentLevel = PlayerPrefs.GetInt(GlobalConstants.CURRENT_LEVEL, 1);
        playerLevel = PlayerPrefs.GetInt(GlobalConstants.CURRENT_PLAYER_LEVEL, 1);
    }
    
    public static void SaveAllInfo()
    {
        PlayerPrefs.SetInt(GlobalConstants.CURRENT_LEVEL, currentLevel);
        PlayerPrefs.SetInt(GlobalConstants.CURRENT_PLAYER_LEVEL, playerLevel);
        PlayerPrefs.Save();
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
