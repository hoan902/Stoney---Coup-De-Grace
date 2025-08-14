using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Team1,
    Team2
}
public enum GameMode
{
    OneVsOne,
    OneVsMany,
    ManyVsMany
}

public enum AttackUsedBodyPart
{
    RightHand,
    LeftHand,
    RightLegs,
    LeftLegs,
}

public enum AnimatorParameter
{
    move_Forward,
    move_Strafe,
    getHit_Head,
    getHit_Stomach,
    getHit_Kidney,
    punching_Stomach,
    punching_KidneyRight,
    punching_KidneyLeft,
    punching_Head,
    KnockedOut,
    Victory1
}

public class GlobalConstants
{
    public const string CURRENT_LEVEL = "current-level";
    public const string CURRENT_LEVEL_OVM = "current-level-OVM";
    public const string CURRENT_LEVEL_MVM = "current-level-MVM";
    public const string CURRENT_PLAYER_LEVEL = "current-player-level";
    public const string CURRENT_GAME_MODE = "current-game-mode";
    public const string MUSIC = "music-setting";
    public const string SOUND = "sound-setting";
}

[System.Serializable]
public class DamageDealerInfo
{
    public int damage;
    public bool critical;
    public Transform attacker;
    public string AnimationAtkName;
}
