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
    punching_Head
}

public class GlobalConstants
{
    public const string CURRENT_LEVEL = "current-level";
    public const string CURRENT_PLAYER_LEVEL = "current-player-level";

}
