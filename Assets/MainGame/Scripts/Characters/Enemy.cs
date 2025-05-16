using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseCharacter
{
    
    private StatsConfig m_currentEnemyStat;
    private void Start()
    {
        OnInit();
    }

    void OnInit()
    {
        m_currentEnemyStat = ConfigsManagement.Instance.statsConfig.GetBotStatByLevel(1);
        if (m_currentEnemyStat != null)
            SetupStats(m_currentEnemyStat);
    }
    public void TakeDamage(int damageTaken, AnimatorParameter hitByAnimation)
    {
        if (currentHealth <= 0)
            return;
        currentHealth -= damageTaken;
        Debug.Log("Animation hit: " + hitByAnimation.ToString());
        switch (hitByAnimation)
        {
            case AnimatorParameter.punching_Stomach:
                animParamController.SetParameterTrigger(AnimatorParameter.getHit_Stomach);
                break;
            case AnimatorParameter.punching_KidneyRight:
                animParamController.SetParameterTrigger(AnimatorParameter.getHit_Kidney);
                break;
            case AnimatorParameter.punching_KidneyLeft:
                animParamController.SetParameterTrigger(AnimatorParameter.getHit_Kidney);
                break;
            case AnimatorParameter.punching_Head:
                animParamController.SetParameterTrigger(AnimatorParameter.getHit_Head);
                break;
        }
        if (currentHealth <= 0)
            Debug.LogError("DEAD BLOW!!");
    }
}
