using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseCharacter : MonoBehaviour
{
    [Header("Refference variables")]
    public Animator animator;
    public CharacterController characterController;
    public Transform characterVisual;
    public AnimationParametersController animParamController;
    public Transform target;
    public Team charTeam;

    [Header("Character Stats Configs")] 
    [FoldoutGroup("Stats")]
    [FoldoutGroup("Stats")] public string titleRank;
    [FoldoutGroup("Stats")] public int maxHealth;
    [FoldoutGroup("Stats")] public int currentHealth;
    [FoldoutGroup("Stats")] public int atkDamage;
    [FoldoutGroup("Stats")] public int level;
    
    
    [Header("Movement Configs variables")]
    public float walkSpeed = 3f;
    public float movementThreshold = 0.1f;
    public bool lookToMovementDirection = true;//turn this off if you want to separate movement and aiming
    
    [Header("Animation variables")]
    [Tooltip("This will turn rotation towards the joystick direction")]
    public bool canStrafe = false;
    [HideInInspector] public float joystickMagnitude;
    [HideInInspector] public Vector3 vectorForward; //camera fwd,right
    [HideInInspector] public Vector3 vectorRight; //camera fwd,right
    [HideInInspector] public Vector3 move;
    [HideInInspector] public float parameterForward;//we will use them in animation variables
    [HideInInspector] public float parameterStrafe;//we will use them in animation variables

    private void Awake()
    {
        if(characterController == null)
            characterController = GetComponent<CharacterController>();
        if(characterVisual == null)
            characterVisual = transform;
    }
    
    public void SetupStats(StatsConfig stat)
    {
        maxHealth = stat.health;
        currentHealth = maxHealth;
        level = stat.level;
        atkDamage = stat.attackDamage;
        titleRank = stat.characterTitle;
    }
    
    public Transform GetCharacterPos()
    {
        return characterVisual;
    }

    public void SetLockInTarget(Transform target)
    {
        this.target = target;
    }
}
