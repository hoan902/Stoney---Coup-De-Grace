using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySceneUIManager : MonoBehaviour
{
    [Header("--- References")]
    [SerializeField] private Image m_healthBarImg;
    [SerializeField] private Image m_avatarImg;
    [SerializeField] private Image m_battleEndTextImg;
    [SerializeField] private Image m_fadeMask;
    [SerializeField] private TextMeshProUGUI m_countdownTxt;
    [SerializeField] private TextMeshProUGUI m_levelTxt;
    [Header("--- Win Popup Refs")]
    [SerializeField] private Transform m_winPopup;
    [SerializeField] private Transform m_nextLevelBtn;
    [SerializeField] private Transform m_replayBtn;
    [SerializeField] private Transform m_modeBtnGroup;
    [Header("--- Sprites")]
    [SerializeField] private Sprite m_victorySprite;
    [SerializeField] private Sprite m_loseSprite;

    private Tweener m_stickHPTweener;
    private void Awake()
    {
        GameController.updatePlayerHpEvent += OnUpdateStickHP;
        GameController.battleEnded += OnBattleEnded;
        GameController.readyPlayEvent += OnGameReady;
        GameController.openMask += OpenFadeMask;
        GameController.countdownBattle += StartCountdownBattle;
    }

    private void OnDestroy()
    {
        GameController.updatePlayerHpEvent -= OnUpdateStickHP;
        GameController.battleEnded -= OnBattleEnded;
        GameController.readyPlayEvent -= OnGameReady;
        GameController.openMask -= OpenFadeMask;
        GameController.countdownBattle -= StartCountdownBattle;
    }

    private void OnGameReady()
    {
        OpenFadeMask();
        UpdateLevelInfoUI();
    }

    private void OnUpdateStickHP(int last, int current)
    {
        m_stickHPTweener?.Kill();
        float healthFraction = (float)current / (float)SaveModel.maxPlayerHP;
        m_healthBarImg.DOFillAmount(healthFraction, 0.5f);
    }
    private void OnBattleEnded(Team winingTeam)
    {
        OnPopupResult(true, winingTeam);
        m_battleEndTextImg.gameObject.SetActive(true);
        m_battleEndTextImg.transform.localScale = Vector3.zero;
        switch (winingTeam)
        {
            case Team.Team1:
                m_battleEndTextImg.sprite = m_victorySprite;
                m_battleEndTextImg.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBounce);
                break;
            case Team.Team2:
                m_battleEndTextImg.sprite = m_loseSprite;
                m_battleEndTextImg.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack);
                break;
        }
    }

    private void OnPopupResult(bool isActive, Team winingTeam = Team.Team1)
    {
        m_winPopup.gameObject.SetActive(isActive);
        if(isActive)
            StartCoroutine(IDelayShowMenu(winingTeam == Team.Team1));
        else
        {
            ResetUI();
            m_fadeMask.DOFade(1f, 0.4f).SetUpdate(true); ;
            UpdateLevelInfoUI();
        }
    }
    private void ResetUI()
    {
        m_replayBtn.localScale = Vector3.zero;
        m_modeBtnGroup.localScale = Vector3.zero;
        m_nextLevelBtn.localScale = Vector3.zero;
    }

    private void OpenFadeMask()
    {
        m_fadeMask.DOFade(0f, 0.4f).SetUpdate(true);
    }
    private void StartCountdownBattle(int countdownSec)
    {
        if (countdownSec > 0)
        {
            StartCoroutine(IStartCountdown(countdownSec));
            return;
        }
        m_countdownTxt.text = "";
    }

    private void UpdateLevelInfoUI()
    {
        int curLevel = -1;
        GameMode curMode = (GameMode)SaveModel.currentMode;
        switch (curMode)
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
        m_levelTxt.text = $"level {curLevel}\n mode\n{curMode}";
    }
    IEnumerator IStartCountdown(int countdownSec)
    {
        for (int i = countdownSec; i > 0; i--)
        {
            m_countdownTxt.text = $"{i}";
            yield return new WaitForSecondsRealtime(1f);
            if(i == 1)
            {
                m_countdownTxt.text = "FIGHT!";
                yield return new WaitForSecondsRealtime(0.5f);
                m_countdownTxt.text = "";
            }
        }
    }

    IEnumerator IDelayShowMenu(bool isWin)
    {
        yield return new WaitForSeconds(0.5f);
        m_replayBtn.gameObject.SetActive(true);
        m_modeBtnGroup.gameObject.SetActive(true);
        m_replayBtn.localScale = Vector3.zero;
        m_modeBtnGroup.localScale = Vector3.zero;
        m_replayBtn.DOScale(1f, 0.3f);
        m_modeBtnGroup.DOScale(1f, 0.3f);
        if (isWin)
        {
            m_nextLevelBtn.gameObject.SetActive(true);
            m_nextLevelBtn.localScale = Vector3.zero;
            m_nextLevelBtn.DOScale(1f, 0.3f);
        }
    }

    public void OnNextLevel()
    {
        GameMode curGameMode = (GameMode)SaveModel.currentMode;
        switch (curGameMode)
        {
            case GameMode.OneVsOne:
                SaveModel.currentLevel += 1;
                break;
            case GameMode.OneVsMany:
                SaveModel.currentLevelOneVMany += 1;
                break;
            case GameMode.ManyVsMany:
                SaveModel.currentLevelManyVMany += 1;
                break;
        }
        SaveModel.playerLevel++;
        SaveModel.SaveAllInfo();
        GameController.OnPlayLevel();
        OnPopupResult(false);
    }

    public void OnReplay()
    {
        GameController.OnPlayLevel();
        OnPopupResult(false);
    }

    public void OnModeOvO()
    {
        GameController.OnChangingMode(GameMode.OneVsOne);
        OnPopupResult(false);
    }

    public void OnModeOvM()
    {
        GameController.OnChangingMode(GameMode.OneVsMany);
        OnPopupResult(false);
    }

    public void OnModeMvM()
    {
        GameController.OnChangingMode(GameMode.ManyVsMany);
        OnPopupResult(false);
    }

    #region --- Test Func
#if UNITY_EDITOR
    public void TestWin()
    {
        OnBattleEnded(Team.Team1);
    }
    public void TestLose()
    {
        OnBattleEnded(Team.Team1);
    }
#endif

    #endregion
}
