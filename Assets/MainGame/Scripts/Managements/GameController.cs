using System;
using UnityEngine;

public class GameController
{
    public static Action readyPlayEvent;
    public static Action<bool> activeInputEvent;
    public static Action<Transform> cameraFollowTarget;
    public static Action cameraZoomEff;
    public static Action<int, int> updatePlayerHpEvent;

    public static void ReadyPlay()
    {
        readyPlayEvent?.Invoke();
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

    public static void UpdatePlayerHp(int value)
    {
        if (SaveModel.playerHP <= 0 && value <= 0)
            return;
        int last = SaveModel.playerHP;
        SaveModel.UpdateHp(value, SaveModel.maxPlayerHP);
        updatePlayerHpEvent?.Invoke(last, SaveModel.playerHP);
    }
}
