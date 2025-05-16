using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManagement : BaseCharacter
{
    [Header("Camera Reference")] 
    [SerializeField] private Camera m_mainCamera;
    
    [Header("Collider Reference")] 
    [SerializeField] private Collider m_leftHandCollider;
    [SerializeField] private Collider m_rightHandCollider;
    
    [Header("Joystick Reference")]
    [SerializeField] private FixedJoystick m_moveJoystick;
    
    private Transform m_camTransform;
    private Vector3 m_cameraForward;
    private bool m_isAtking;
    private int m_atkIndex = 5;
    private float m_currentFixedUpdateTime;
    private StatsConfig m_currentPlayerStat;

    private void Awake()
    {
        if(!m_mainCamera)
            m_mainCamera = Camera.main;
        m_camTransform = m_mainCamera.transform;
        m_currentFixedUpdateTime = Time.fixedDeltaTime;
    }
    private void Start(){
        characterController.detectCollisions = false;
        RecalculateCamera(m_mainCamera);
        OnInit();
    }
    private void Update()
    {
        joystickMagnitude = new Vector2(m_moveJoystick.Horizontal, m_moveJoystick.Vertical).sqrMagnitude;
        joystickMagnitude = Mathf.Clamp01(joystickMagnitude);
        if (!animator) 
            return;
        if(canStrafe)
            RelativeAnimations();
        else
            animParamController.SetParameterFloat(AnimatorParameter.move_Forward, base.joystickMagnitude);
    }
    private void FixedUpdate()
    {
        var joystickMagnitude = new Vector2(m_moveJoystick.Horizontal, m_moveJoystick.Vertical).sqrMagnitude;
        base.joystickMagnitude = Mathf.Clamp01(joystickMagnitude);
        if(canStrafe)
        {
            lookToMovementDirection = false;
            //use strafe when you look at certain object(target) for instance
        }
        //getting the magnitude
        if (base.joystickMagnitude >= movementThreshold) 
            MovementAndRotation();
        else
            characterController.Move(new Vector3(0,-9.8f,0));//gravity when idle
    }

    void OnInit()
    {
        m_currentPlayerStat = ConfigsManagement.Instance.statsConfig.GetPlayerStatByLevel(SaveModel.playerLevel);
        if (m_currentPlayerStat != null)
            SetupStats(m_currentPlayerStat);
    }
    
    void RelativeAnimations(){
        if (m_camTransform != null)
        {
            m_cameraForward = Vector3.Scale(m_camTransform.up, new Vector3(1, 0, 1)).normalized; //camera forwad
            move = m_moveJoystick.Vertical * m_cameraForward + m_moveJoystick.Horizontal * m_camTransform.right;//relative 
            //vector to camera forward and right
        }
        else
        {
            move = m_moveJoystick.Vertical * Vector3.forward + m_moveJoystick.Horizontal * Vector3.right;
            //if there is no camera transform(for any reason then we use joystick forward and right)
        }
        if (move.magnitude > 1)
            move.Normalize();//normalizing here
        MoveAnims(move);
    }
    void MoveAnims(Vector3 move)
    {
        Vector3 localMove = transform.InverseTransformDirection(move);//inversing local move from the input
        parameterStrafe = localMove.x;//x is right input relative to camera 
        parameterForward = localMove.z;//z is forward joystick input relative to camera
        animParamController.SetParameterFloat(AnimatorParameter.move_Forward,parameterForward * 2f, 0.01f, Time.deltaTime);
        animParamController.SetParameterFloat(AnimatorParameter.move_Strafe,parameterStrafe * 2f, 0.01f, Time.deltaTime);
    }
    void RecalculateCamera(Camera _cam)
    {
            Camera cam = _cam;
            m_camTransform = cam.transform;
            vectorForward = cam.transform.forward; //camera forward
            vectorForward.y = 0;
            vectorForward = Vector3.Normalize(vectorForward);
            vectorRight = Quaternion.Euler(new Vector3(0, 90, 0)) * vectorForward; //camera right
    }
    void MovementAndRotation()
    {
        Vector3 rightMovement = vectorRight * (walkSpeed * Time.deltaTime * m_moveJoystick.Horizontal);//getting right movement out of joystick(relative to camera)
        Vector3 upMovement = vectorForward * (walkSpeed * Time.deltaTime * m_moveJoystick.Vertical); //getting up movement out of joystick(relative to camera)
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement); //final movement vector
        heading.y = -9.8f;//gravity while moving
        characterController.Move(heading * (walkSpeed * Time.deltaTime));//move
        if (lookToMovementDirection) //look to movement direction
        {
            Vector3 forward = new Vector3(heading.x, characterVisual.forward.y, heading.z);
            characterVisual.DOLookAt(characterVisual.position + forward, 0.3f);
        }
        if (target != null)
        {
            Vector3 lookPosition = new Vector3(target.position.x, characterVisual.position.y, target.position.z);
            characterVisual.DOLookAt(lookPosition, 0.3f);
        }
    }

    #region --- OnButtonEvent
    public void OnAttack()
    {
        //Check attacking status
        //var currentState = animator.GetCurrentAnimatorStateInfo(2);
        //m_isAtking = currentState.IsTag("Atk") && currentState.normalizedTime > 0;
        if (m_isAtking)
            return;
        m_isAtking = true;
        var randAnimationAtk = (AnimatorParameter)m_atkIndex;
        if (m_atkIndex < 8)
            m_atkIndex++;
        else
            m_atkIndex = 5;
        //randAnimationAtk = (AnimatorParameter)Random.Range(5, 8);
        animParamController.SetParameterTrigger(randAnimationAtk);
    }
    #endregion

    #region --- Sending Events

    void OnDealDamage(Collider collider)
    {
        if (!collider)
            return;
        if (!collider.TryGetComponent<Enemy>(out Enemy hitEnemy))
            return;
        if (hitEnemy.currentHealth <= 0)
            return;
        AnimatorParameter hitAnimation = (AnimatorParameter)(m_atkIndex - 1);
        if (m_atkIndex - 1 < 5)
            hitAnimation = (AnimatorParameter)5;
        hitEnemy.TakeDamage(10, hitAnimation);
        m_leftHandCollider.enabled = false;
        m_rightHandCollider.enabled = false;
    }
    
    void OnStartAttacking()
    {
        m_leftHandCollider.enabled = true;
        m_rightHandCollider.enabled = true;
        Time.timeScale = 0.6f;
        Time.fixedDeltaTime = m_currentFixedUpdateTime * Time.timeScale;
        /*m_mainCamera.DOFieldOfView(40, 0.4f).OnComplete(() =>
        {
            m_mainCamera.DOFieldOfView(60, 0.2f);
        });*/
        m_mainCamera.DOShakeRotation(0.4f, 1f).OnComplete((() =>
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = m_currentFixedUpdateTime * Time.timeScale;
        }));
    }
    
    void OnFinishAttacking()
    {
        m_leftHandCollider.enabled = false;
        m_rightHandCollider.enabled = false;
        m_isAtking = false;
    }
    #endregion
    
    #region ---- Testing New Input System (not having enough knowledge to use it yet)
    void MovementAndRotation(float horizontal, float vertical)
    {
        Vector3 rightMovement = vectorRight * (walkSpeed * Time.deltaTime * horizontal);//getting right movement out of joystick(relative to camera)
        Vector3 upMovement = vectorForward * (walkSpeed * Time.deltaTime * vertical); //getting up movement out of joystick(relative to camera)
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement); //final movement vector
        heading.y = -9.8f;//gravity while moving
        characterController.Move(heading * (walkSpeed * Time.deltaTime));//move
        if (lookToMovementDirection) //look to movement direction
        {
            Vector3 forward = new Vector3(heading.x, characterVisual.forward.y, heading.z);
            characterVisual.DOLookAt(characterVisual.position + forward, 0.3f);
        }
        if (target != null)
        {
            Vector3 lookPosition = new Vector3(target.position.x, characterVisual.position.y, target.position.z);
            characterVisual.DOLookAt(lookPosition, 0.3f);
        }
    }
    public void OnMoving(InputAction.CallbackContext vector)
    {
        var joystickVector = vector.ReadValue<Vector2>();
        var joystickMagnitude = vector.ReadValue<Vector2>().sqrMagnitude;
        base.joystickMagnitude = Mathf.Clamp01(joystickMagnitude);
        //getting the magnitude
        if (base.joystickMagnitude >= movementThreshold) 
            MovementAndRotation(joystickVector.x,joystickVector.y);
        else
            characterController.Move(new Vector3(0,-9.8f,0));//gravity when idle
    }

    #endregion
}
