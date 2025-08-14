using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class GameManager : MonoBehaviour
{
    [Header("-- Base Characters --")]
    [SerializeField] GameObject m_playerPref;
    [SerializeField] GameObject m_botThugPref;
    [SerializeField] GameObject m_botChampionPref;
    [SerializeField] GameObject m_botAllyPref;

    private PlayerManagement m_player;
    private bool m_gameReady = false;

    private void Awake()
    {
        if(!SaveModel.saveFileLoaded)
            SaveModel.LoadCurrentSave();
        GameController.changeMode += SwitchGameMode;
        GameController.playNewLevel += OnNextLevel;
    }
    private void OnDestroy()
    {
        GameController.changeMode -= SwitchGameMode;
        GameController.playNewLevel -= OnNextLevel;
    }
    private void Start()
    {
        StartCoroutine(IStartGame());
    }

    //HoanDN: Setup countdown before battle start.
    //-- Pause Menu
    void InitLevel(GameMode currentGameMode)
    {
        BattlefieldManagement.Instance.CleanAll();
        StartCoroutine(ISetupGameField(currentGameMode));
    }
    void SetupPlayer()
    {
        //-- Setup Player
        PlayerManagement player = Instantiate(m_playerPref).GetComponent<PlayerManagement>();
        player.OnInit();
        BattlefieldManagement.Instance.AddCharToTeam1(player);
        SaveModel.UpdateMaxHp(player.maxHealth);
        SaveModel.UpdateHp(player.currentHealth, player.maxHealth);
        GameController.UpdatePlayerHp(0);
        m_player = player;
    }
    void SetupCharaPos(HashSet<BotCharacter> alliesBot, HashSet<BotCharacter> enemiesBot, int upgradeLevelBot, int bonusHealth)
    {
        //-- Setup positions
        foreach (var allyBot in alliesBot)
        {
            allyBot.OnInit(m_player.level > 1 ? m_player.level - 1 : 1, isAlly: true);
            //SetNearestTarget(allyBot, BattlefieldManagement.Instance.GetTeam2OnBoard());
        }
        foreach (var enemyBot in enemiesBot)
        {
            enemyBot.OnInit(upgradeLevelBot, bonusHealth);
            //SetNearestTarget(enemyBot, BattlefieldManagement.Instance.GetTeam1OnBoard()); 
        }
        BattlefieldManagement.Instance.SetupPosSpawnCharacters();
        GameController.SetupCamFollowTarget(m_player.transform);
    }
    void SwitchGameMode(GameMode gameMode)
    {
        BattlefieldManagement.Instance.CleanAll();
        m_gameReady = false;
        SaveModel.currentMode = (int)gameMode;
        StartCoroutine(IStartGame());
    }
    void OnNextLevel()
    {
        StartCoroutine(IStartGame());
    }
    void PauseGame()
    {
        SaveModel.paused = true;
        Time.timeScale = 0;
    }
    void ResumeGame()
    {
        SaveModel.paused = false;
        Time.timeScale = 1;
    }
    #region --- IEnumerators
    IEnumerator IStartGame()
    {
        BattlefieldManagement.Instance.CleanAll();
        m_gameReady = false;
        yield return new WaitForSeconds(1f);
        InitLevel((GameMode)SaveModel.currentMode);
        yield return new WaitForSeconds(0.2f);
        SoundManager.PlaySound("miss-punch-small", false);
        SoundManager.PlaySound("normalCrowNoise", true, true);
        SoundManager.MuteSound(false);
        SoundManager.MuteMusic(false);
        PauseGame();
        yield return new WaitForSecondsRealtime(0.2f);
        GameController.OpenMask();
        yield return new WaitForSecondsRealtime(0.4f);
        GameController.CountdownBattleStart(3);
        yield return new WaitForSecondsRealtime(3f);
        //Only when in game
        ResumeGame();
        SoundManager.PlaySound("boxing-bell", false);
        GameController.ReadyPlay();
    }
    IEnumerator ISetupGameField(GameMode currentGameMode)
    {
        SetupPlayer();
        yield return null;
        LevelConfig levelConfig = ConfigsManagement.Instance.levelsRuleConfig.GetLevelRule(currentGameMode);
        //-- Setup Bot
        // If there Ally
        HashSet<BotCharacter> alliesBot = new HashSet<BotCharacter>();
        if (levelConfig.isAllyAssist)
        {
            for (int i = 0; i < 2; i++)
            {
                BotCharacter botAlly = Instantiate(m_botAllyPref).GetComponent<BotCharacter>();
                botAlly.name = "Team-1-bot";
                botAlly.charTeam = Team.Team1;
                BattlefieldManagement.Instance.AddCharToTeam1(botAlly);
                alliesBot.Add(botAlly);
                yield return null;
            }
        }
        //
        HashSet<BotCharacter> enemiesBot = new HashSet<BotCharacter>();
        int bonusHealth = levelConfig.bonusHealthPerLevel;
        int curLevel = SaveModel.currentLevel;
        switch (currentGameMode)
        {
            case GameMode.OneVsOne:
                curLevel = SaveModel.currentLevel;
                break;
            case GameMode.OneVsMany:
                curLevel = SaveModel.currentLevelOneVMany;
                break;
            case GameMode.ManyVsMany:
                curLevel = SaveModel.currentLevelManyVMany;
                break;
        }
        int numOfAddTeam2Bot = levelConfig.botAddedPerLevel * curLevel;
        int upgradeLevelBot = curLevel / levelConfig.levelReqForBotLevelUp;
        int totalUpgradeDataCount = ConfigsManagement.Instance.statsConfig.GetTotalBotDataCount();
        if (upgradeLevelBot <= 0)
            upgradeLevelBot = 1;
        if (upgradeLevelBot > totalUpgradeDataCount)
            upgradeLevelBot = totalUpgradeDataCount;
        if (numOfAddTeam2Bot >= 0)
        {
            if (numOfAddTeam2Bot == 0)
                numOfAddTeam2Bot = 1;
            if (numOfAddTeam2Bot > 5)
                numOfAddTeam2Bot = 5;
            for (int i = 0; i < numOfAddTeam2Bot; i++)
            {
                BotCharacter bot;
                if (upgradeLevelBot > 3)
                    bot = Instantiate(m_botChampionPref).GetComponent<BotCharacter>();
                else
                    bot = Instantiate(m_botThugPref).GetComponent<BotCharacter>();
                bot.name = "Team-2-bot";
                BattlefieldManagement.Instance.AddCharToTeam2(bot);
                enemiesBot.Add(bot);
                yield return null;
            }
        }
        SetupCharaPos(alliesBot, enemiesBot, upgradeLevelBot, bonusHealth);
    }
    #endregion

}
