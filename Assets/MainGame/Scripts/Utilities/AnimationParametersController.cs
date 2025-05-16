using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimationParametersController : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    
    void Start()
    {
        if(!m_animator)
            m_animator = GetComponent<Animator>();
    }

    public void SetParameterTrigger(AnimatorParameter param)
    {
        switch (param)
        {
            case AnimatorParameter.getHit_Head:
                m_animator.SetTrigger("getHit_Head");
                break;
            case AnimatorParameter.getHit_Stomach:
                m_animator.SetTrigger("getHit_Stomach");
                break;
            case AnimatorParameter.getHit_Kidney:
                m_animator.SetTrigger("getHit_Kidney");
                break;
            case AnimatorParameter.punching_Stomach:
                m_animator.SetTrigger("punching_Stomach");
                break;
            case AnimatorParameter.punching_KidneyRight:
                m_animator.SetTrigger("punching_KidneyRight");
                break;
            case AnimatorParameter.punching_KidneyLeft:
                m_animator.SetTrigger("punching_KidneyLeft");
                break;
            case AnimatorParameter.punching_Head:
                m_animator.SetTrigger("punching_Head");
                break;
        }
    }

    public void SetParameterFloat(AnimatorParameter param, float value)
    {
        switch (param)
        {
            case AnimatorParameter.move_Forward:
                m_animator.SetFloat("move_Forward", value);
                break;
            case AnimatorParameter.move_Strafe:
                m_animator.SetFloat("move_Strafe", value);
                break;
        }
    }

    public void SetParameterFloat(AnimatorParameter param, float value, float dampTime, float deltaTime)
    {
        switch (param)
        {
            case AnimatorParameter.move_Forward:
                m_animator.SetFloat("move_Forward", value, dampTime, deltaTime);
                break;
            case AnimatorParameter.move_Strafe:
                m_animator.SetFloat("move_Strafe", value, dampTime, deltaTime);
                break;
        }
    }
}
