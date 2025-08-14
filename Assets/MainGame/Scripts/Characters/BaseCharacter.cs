using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseCharacter : MonoBehaviour
{
    [Header("============ Base Character Configs ===============")]
    [Header("Refference variables")]
    public Animator animator;
    public CharacterController characterController;
    public Transform characterVisual;
    public AnimationParametersController animParamController;
    public Transform target;
    public Team charTeam;
    public AudioClip[] audiosHit;
    public AudioClip audiosDead;

    [Header("Collider Reference")]
    public Collider m_leftHandCollider;
    public Collider m_rightHandCollider;

    [Header("Character Stats Configs")]
    [FoldoutGroup("Stats", 1)] public string titleRank = "";
    [FoldoutGroup("Stats", 2)] public int maxHealth;
    [FoldoutGroup("Stats", 3)] public int currentHealth;
    [FoldoutGroup("Stats", 4)] public int atkDamage;
    [FoldoutGroup("Stats", 5)] public int level;
    [FoldoutGroup("Stats", 5)] public BaseComboSet equipedComboSet;


    [Header("Movement Configs variables")]
    public float walkSpeed = 3f;
    public float movementThreshold = 0.1f;
    public bool lookToMovementDirection = true;//turn this off if you want to separate movement and aiming

    //======= Hidden Public
    [HideInInspector] public Vector3 vectorForward; //camera fwd,right
    [HideInInspector] public Vector3 vectorRight; //camera fwd,right
    [HideInInspector] public Vector3 move;
    [HideInInspector] public float parameterForward;//we will use them in animation variables
    [HideInInspector] public float parameterStrafe;//we will use them in animation variables

    //======= Public
    public bool isAtking;
    public int atkIndex = 5;
    public bool isDead = false;
    public bool isPlayer = false;
    public List<BaseCharacter> listOfEnemiesTargetingYou;
    public float testRadiusScatter = 1f;
    public bool isGameOver = false;

    private void Awake()
    {
        InitSetup();
    }

    #region --- Virtual Classes
    public virtual void InitSetup()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        if (characterVisual == null)
            characterVisual = transform;
    }
    public virtual void Dead()
    {
        isDead = true;
        animParamController.SetParameterBool(AnimatorParameter.KnockedOut, isDead);
        BattlefieldManagement.Instance.RemoveChar(this, charTeam);
        if (target != null)
        {
            target.GetComponent<BaseCharacter>().RemoveEnemyFromTargetingYouList(this);
        }
        listOfEnemiesTargetingYou.Clear();
        BattlefieldManagement.Instance.CheckingBattleStatus();
    }

    public virtual void OnUpdateTarget()
    {
        
    }
    public virtual void OnVictory()
    {
        isGameOver = true;
        animParamController.SetParameterBool(AnimatorParameter.Victory1, true);
    }

    public virtual void OnHit(DamageDealerInfo attackerInfor)
    {
        if (isDead || isGameOver)
            return;
        if (audiosHit.Length > 0)
        {
            int rand = UnityEngine.Random.Range(0, audiosHit.Length);
            SoundManager.PlaySound3D(audiosHit[rand], 100, false, transform.position);
        }
        currentHealth -= attackerInfor.damage;
        if (currentHealth <= 0)
        {
            SoundManager.PlaySound3D(audiosDead, 100, false, transform.position);
            isDead = true;
            Dead();
        }
        // animation hit
        if (!isDead)
        {
            switch (attackerInfor.AnimationAtkName)
            {
                case "Stomach Punch":
                    animParamController.SetParameterTrigger(AnimatorParameter.getHit_Stomach);
                    break;
                case "Kidney Punch Right":
                case "Kidney Punch Left":
                    animParamController.SetParameterTrigger(AnimatorParameter.getHit_Kidney);
                    break;
                case "Head Punch":
                    animParamController.SetParameterTrigger(AnimatorParameter.getHit_Head);
                    break;
            }
        }
        else
        {
            animParamController.SetParameterBool(AnimatorParameter.KnockedOut, true);
            attackerInfor.attacker.GetComponent<BaseCharacter>().OnUpdateTarget();
        }
    }
    #endregion

    #region --- Events call via Send Message
    void SpawnNormalHitFX()
    {

    }
    void SpawnCritHitFX()
    {

    }
    #endregion

    #region ---- Local Public Classes
    public void SetupStats(StatsConfig stat, int bonusHealth = 0)
    {
        listOfEnemiesTargetingYou = new();
        maxHealth = stat.health + bonusHealth;
        currentHealth = maxHealth;
        level = stat.level;
        atkDamage = stat.attackDamage;
        titleRank = stat.characterTitle;
        equipedComboSet = Instantiate(stat.comboSet, transform);
        Collider[] setOfBodyPart = new Collider[] { m_rightHandCollider, m_leftHandCollider };
        equipedComboSet.SetupCombo(this, setOfBodyPart);
    }

    public Transform GetCharacterPos()
    {
        return characterVisual;
    }

    public void SetLockInTarget(Transform target)
    {
        if (isGameOver || isDead)
            return;
        if (!target)
            return;
        this.target = target;
        target.GetComponent<BaseCharacter>().SetListOfEnemiesTargetYou(this);
    }

    public void SetListOfEnemiesTargetYou(BaseCharacter enemyTargetYou)
    {
        if (!listOfEnemiesTargetingYou.Contains(enemyTargetYou))
            listOfEnemiesTargetingYou.Add(enemyTargetYou);
    }

    public void RemoveEnemyFromTargetingYouList(BaseCharacter enemyTargetYou)
    {

        if (listOfEnemiesTargetingYou.Contains(enemyTargetYou))
            listOfEnemiesTargetingYou.Remove(enemyTargetYou);
    }

    public void OnPunching()
    {
        if (isGameOver)
            return;
        animator.Play(equipedComboSet.fightAnimation, 2);
    }

    public void OnStartAttacking()
    {
        if (isGameOver)
            return;
        equipedComboSet.StartPunching();
        isAtking = true;
    }

    public void OnFinishAttacking()
    {
        if (isGameOver)
            return;
        equipedComboSet.EndPunching();
        isAtking = false;
    }
    #endregion
}
