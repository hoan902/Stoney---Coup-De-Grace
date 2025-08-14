using System;
using UnityEngine;

public class GameController
{
    public static Action readyPlayEvent;
    public static Action openMask;
    public static Action<int> countdownBattle;
    public static Action<bool> activeInputEvent;
    public static Action<Transform> cameraFollowTarget;
    public static Action cameraZoomEff;
    public static Action shakeCameraEvent;
    public static Action<int, int> updatePlayerHpEvent;
    public static Action<Team> battleEnded;
    public static Action<GameMode> changeMode;
    public static Action playNewLevel;

    public static void ReadyPlay()
    {
        readyPlayEvent?.Invoke();
    }
    public static void OpenMask()
    {
        openMask?.Invoke();
    }
    public static void CountdownBattleStart(int countdownSec)
    {
        countdownBattle?.Invoke(countdownSec);
    }
    public static void ActiveInput(bool active)
    {
        activeInputEvent?.Invoke(active);
    }

    public static void SetupCamFollowTarget(Transform player)
    {
        cameraFollowTarget?.Invoke(player);
    }

    public static void CameraZoomEff()
    {
        cameraZoomEff?.Invoke();
    }
    public static void ShakeCamera()
    {
        shakeCameraEvent?.Invoke();
    }

    public static void OnEndedMatch(Team winingTeam)
    {
        battleEnded?.Invoke(winingTeam);
    }

    public static void OnChangingMode(GameMode gameMode)
    {
        changeMode?.Invoke(gameMode);
    }

    public static void OnPlayLevel()
    {
        playNewLevel?.Invoke();
    }

    public static void UpdatePlayerHp(int value)
    {
        if (SaveModel.playerHP <= 0 && value <= 0)
            return;
        int last = SaveModel.playerHP;
        SaveModel.UpdateHp(value, SaveModel.maxPlayerHP);
        updatePlayerHpEvent?.Invoke(last, SaveModel.playerHP);
    }
}
