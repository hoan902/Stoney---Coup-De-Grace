using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BotCharacter : BaseCharacter
{
    [Header("============ Bot Configs ===============")]
    [SerializeField] private BillboardEff m_hpBillboard;
    [SerializeField] private float m_attackRange = 2f;
    [SerializeField] SpriteRenderer healthProgress;

    //======= Private
    private NavMeshAgent m_agent;
    private StatsConfig m_currentStat;
    private Vector3 m_pointDebug;

    private Coroutine m_hpTimer;
    private Tweener m_hpTweener;
    private SpriteRenderer m_healthDelay;
    private void Start()
    {
        if (m_agent == null)
            m_agent = GetComponent<NavMeshAgent>();
    }

    public void OnInit(int upgradeLevel,int bonusHealth = 0, bool isAlly = false)
    {
        if (isAlly)
            m_currentStat = ConfigsManagement.Instance.statsConfig.GetPlayerStatByLevel(upgradeLevel);
        else
            m_currentStat = ConfigsManagement.Instance.statsConfig.GetBotStatByLevel(upgradeLevel);
        if (m_currentStat != null)
            SetupStats(m_currentStat, bonusHealth);
        if (m_agent == null)
            m_agent = GetComponent<NavMeshAgent>();
        StartCoroutine(IStartFight());
    }

    #region ----- Override classes
    public override void InitSetup()
    {
        base.InitSetup();
        if (healthProgress != null)
        {
            m_healthDelay = healthProgress.transform.parent.Find("hp-progress-white").GetComponent<SpriteRenderer>();
            UpdateHealthProgress();
        }
    }
    public override void Dead()
    {
        base.Dead();
        StartCoroutine(IDead());
    }
    public override void OnHit(DamageDealerInfo attackerInfor)
    {
        base.OnHit(attackerInfor);
        // HP Bar Update 
        if (healthProgress != null && !isPlayer)
        {
            if (m_hpTimer != null)
                StopCoroutine(m_hpTimer);
            m_hpTimer = StartCoroutine(IHideHPBar());
            UpdateHealthProgress();
        }
    }
    #endregion

    #region ----- Private Classes
    void LookAtTarget()
    {
        if (isDead || isGameOver)
            return;
        if (target != null)
        {
            BaseCharacter currentTarget = target.GetComponent<BaseCharacter>();
            if (currentTarget.isDead)
                return;
            Vector3 lookPosition = new Vector3(target.position.x, characterVisual.position.y, target.position.z);
            characterVisual.DOLookAt(lookPosition, 0.3f);
        }
    }
    void UpdateHealthProgress()
    {
        Vector2 size = healthProgress.size;
        float oldVal = size.x;
        if (currentHealth < 0)
            currentHealth = 0;
        float newVal = 0.78125f * currentHealth / maxHealth;
        healthProgress.size = new Vector2(newVal, size.y);
        if (isDead)
            Dead();
        if (m_hpTweener != null)
            m_hpTweener.Kill();
        m_hpTweener = DOTween.To(() => oldVal, x => size.x = x, newVal, 0.2f).OnUpdate(() => {
            m_healthDelay.size = size;
        }).OnComplete(() => {
            if (isDead)
            {
                if (m_hpTimer != null)
                    StopCoroutine(m_hpTimer);
                healthProgress.transform.parent.gameObject.SetActive(false);
            }
        }).SetDelay(0.3f);
    }
    void SetNearestTarget(HashSet<BaseCharacter> enemiesHashset)
    {
        Transform nearestEnemy = null;
        float closestDistance = Mathf.Infinity;
        if (enemiesHashset.Count == 0)
            return;
        foreach (var enemyTarget in enemiesHashset)
        {
            if (enemyTarget.isDead)
                continue;
            var distance = Vector3.Distance(GetCharacterPos().position, enemyTarget.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemyTarget.transform;
            }
        }
        if (nearestEnemy != null)
            SetLockInTarget(nearestEnemy);
    }

    void StartAutoAttack()
    {
        if (isDead || isGameOver)
            return;
        //Check attacking status
        if (isAtking)
            return;
        isAtking = true;
        var currentState = animator.GetCurrentAnimatorStateInfo(2); //Get animator layer (2 which is Attack layer)
        isAtking = currentState.IsTag("Atk") && currentState.normalizedTime >= 0;
        equipedComboSet.ComboUpdate();
    }
    #endregion

    #region ----- IENumerators
    IEnumerator IHideHPBar()
    {
        healthProgress.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        healthProgress.transform.parent.gameObject.SetActive(false);
    }
    IEnumerator IDead()
    {
        characterController.enabled = false;
        m_agent.enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    IEnumerator IStartFight()
    {
        while (!isDead && !isGameOver)
        {
            switch (charTeam)
            {
                case Team.Team1:
                    SetNearestTarget(BattlefieldManagement.Instance.GetTeam2OnBoard());
                    break;
                case Team.Team2:
                    SetNearestTarget(BattlefieldManagement.Instance.GetTeam1OnBoard());
                    break;
            }
            if (!m_agent.enabled)
                break;
            if (isAtking)
                break;
            //
            LookAtTarget();
            //
            m_agent.stoppingDistance = m_attackRange;
            m_agent.speed = walkSpeed;
            //
            if(target != null)
            {
                target.TryGetComponent(out BaseCharacter curTargetChar);
                int listCount = curTargetChar.listOfEnemiesTargetingYou.Count;
                Vector3 newScatterSurroundPos = target.position;
                for (int i = 0; i < listCount; i++)
                {
                    if (curTargetChar.listOfEnemiesTargetingYou[i] == this)
                    {
                        newScatterSurroundPos = new Vector3(
                            target.position.x + curTargetChar.testRadiusScatter * Mathf.Cos(2 * Mathf.PI * i / listCount),
                            target.position.y,
                            target.position.z + curTargetChar.testRadiusScatter * Mathf.Sin(2 * Mathf.PI * i / listCount));
                    }
                }
                m_pointDebug = newScatterSurroundPos;
                m_agent.SetDestination(newScatterSurroundPos);

                float magSpeed = Mathf.Clamp01(m_agent.velocity.magnitude / m_agent.speed);
                animParamController.SetParameterFloat(AnimatorParameter.move_Forward, magSpeed);
                yield return new WaitForSeconds(0.5f);
                if (m_agent.enabled)
                {
                    if (m_agent.remainingDistance <= m_agent.stoppingDistance && !curTargetChar.isDead)
                    {
                        StartAutoAttack();
                        yield return new WaitForSeconds(0.7f);
                    }
                }
            }        
        }
        if (m_agent.enabled)
            m_agent.isStopped = true;
    }
    #endregion

    void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawSphere(m_pointDebug, 0.1f);
    }
}
