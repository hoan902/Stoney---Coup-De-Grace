using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        m_animator.SetTrigger(param.ToString());
    }
    public void SetParameterBool(AnimatorParameter param, bool isActive)
    {
        switch (param)
        {
            case AnimatorParameter.KnockedOut:
                m_animator.SetBool("isDead", isActive);
                break;
            case AnimatorParameter.Victory1:
                m_animator.SetBool("isVictory", isActive);
                break;
        }
    }

    public void SetParameterFloat(AnimatorParameter param, float value)
    {
        m_animator.SetFloat(param.ToString(), value);
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
