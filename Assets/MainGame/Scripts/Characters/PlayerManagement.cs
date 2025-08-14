using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class PlayerManagement : BaseCharacter
{
    //=======================
    [Header("============ Player Configs ===============")]
    [Header("Camera Reference")] 
    [SerializeField] 
    private Camera m_followingCamera;

    [Header("Auto-lock Zone Reference")]
    [SerializeField]
    private LockInTargetZone m_autoLockZone;

    private StatsConfig m_currentPlayerStat;
    private float m_joystickMagnitude;
    private Vector2 m_movingAxis;
    //-------------------------------------------
    #region --- Unity Default MonoBehaviour Classes
    private void Awake()
    {
        InputManager.onAttackAction += OnAttack;
        InputManager.axisMovingAction += OnPhysicalMove;
    }

    private void OnDestroy()
    {
        InputManager.onAttackAction -= OnAttack;
        InputManager.axisMovingAction -= OnPhysicalMove;
    }

    private void Start(){
        characterController.detectCollisions = false;
        RecalculateCamera(Camera.main);
    }
    private void Update()
    {
        if (isDead || isGameOver)
            return;
        m_joystickMagnitude = new Vector2(m_movingAxis.x, m_movingAxis.y).sqrMagnitude;
        m_joystickMagnitude = Mathf.Clamp01(m_joystickMagnitude);
        if (!animator)
            return;
        else
        {
            animParamController.SetParameterFloat(AnimatorParameter.move_Forward, m_joystickMagnitude);
        }
    }
    private void FixedUpdate()
    {
        if (isDead || isGameOver)
            return;
        var tempJoystickMagnitude = new Vector2(m_movingAxis.x, m_movingAxis.y).sqrMagnitude;
        m_joystickMagnitude = Mathf.Clamp01(tempJoystickMagnitude);
        if (m_joystickMagnitude >= movementThreshold)
            MovementAndRotation(m_movingAxis.x, m_movingAxis.y);
        else
            characterController.Move(new Vector3(0, -9.8f, 0));
    }
    #endregion

    #region --- Override Classes
    public override void OnUpdateTarget()
    {
        base.OnUpdateTarget();
        if (target.GetComponent<BaseCharacter>() != null) 
        {
            BaseCharacter currentTarget = target.GetComponent<BaseCharacter>();
            m_autoLockZone.UpdateCurrentStatus(currentTarget);
            LookAtTarget();
        }
    }
    public override void OnHit(DamageDealerInfo attackerInfor)
    {
        base.OnHit(attackerInfor);
        if (isGameOver)
            return;
        GameController.UpdatePlayerHp(-attackerInfor.damage);
    }
    public override void Dead()
    {
        base.Dead();
        StartCoroutine(IDead());
    }
    #endregion

    #region --- IEnumerator
    IEnumerator IDead()
    {
        characterController.enabled = false;
        yield return new WaitForSeconds(2f);
        if (!isGameOver && isDead && BattlefieldManagement.Instance.GetTeam1OnBoard().Count > 0)
        {
            BaseCharacter ally = BattlefieldManagement.Instance.GetTeam1OnBoard().FirstOrDefault();
            GameController.SetupCamFollowTarget(ally.transform);
        }
        GameController.ActiveInput(false);
        Destroy(gameObject);
    }
    #endregion

    #region --- Controller Events
    void OnPhysicalMove(Vector2 axis)
    {
        if (isDead || isGameOver)
            return;
        m_movingAxis = axis;
    }
    void OnAttack()
    {
        if (isDead || isGameOver)
            return;
        var currentState = animator.GetCurrentAnimatorStateInfo(2);
        isAtking = currentState.IsTag("Atk") && currentState.normalizedTime >= 0;
        if (isAtking)
            return;
        isAtking = true;
        equipedComboSet.ComboUpdate();
    }
    #endregion

    #region --- Private Classes
    void RecalculateCamera(Camera currentCam)
    {
            Camera cam = currentCam;
            vectorForward = cam.transform.forward; 
            vectorForward.y = 0;
            vectorForward = Vector3.Normalize(vectorForward);
            vectorRight = Quaternion.Euler(new Vector3(0, 90, 0)) * vectorForward; 
    }
    void MovementAndRotation(float horizontal, float vertical)
    {
        Vector3 rightMovement = vectorRight * (walkSpeed * Time.deltaTime * horizontal);
        Vector3 upMovement = vectorForward * (walkSpeed * Time.deltaTime * vertical);
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        heading.y = -9.8f;
        characterController.Move(heading * (walkSpeed * Time.deltaTime));
        if (lookToMovementDirection) 
        {
            Vector3 forward = new Vector3(heading.x, characterVisual.forward.y, heading.z);
            characterVisual.DOLookAt(characterVisual.position + forward, 0.3f);
        }
        LookAtTarget();
    }
    void LookAtTarget()
    {
        if (target != null)
        {
            BaseCharacter currentTarget = target.GetComponent<BaseCharacter>();
            if (currentTarget.isDead)
            {
                m_autoLockZone.UpdateCurrentStatus(currentTarget);
                return;
            }
            Vector3 lookPosition = new Vector3(target.position.x, characterVisual.position.y, target.position.z);
            characterVisual.DOLookAt(lookPosition, 0.3f);
        }
    }
    #endregion

    #region --- Public Classes
    public void OnInit()
    {
        isPlayer = true;
        int totalUpgradeDataCount = ConfigsManagement.Instance.statsConfig.GetTotalPlayerDataCount();
        if (SaveModel.playerLevel > totalUpgradeDataCount)
            SaveModel.playerLevel = totalUpgradeDataCount;
        m_currentPlayerStat = ConfigsManagement.Instance.statsConfig.GetPlayerStatByLevel(SaveModel.playerLevel);
        if (m_currentPlayerStat != null)
            SetupStats(m_currentPlayerStat);
    }
    #endregion
}
