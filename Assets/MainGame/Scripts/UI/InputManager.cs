using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject m_joystickMove;
    [SerializeField] private GameObject m_buttonAttack;

    public static Action<Vector2> axisMovingAction;
    public static Action onAttackAction;

    private bool m_stop = true;
    private bool m_moveEnter;
    private bool m_attackEnter;

    void Awake()
    {
        GameController.readyPlayEvent += OnReadyPlay;
        GameController.activeInputEvent += OnActive;
        //
        gameObject.SetActive(false);
    }

    private void Start()
    {
        m_buttonAttack.SetActive(true);
        m_joystickMove.SetActive(true);
    }

    void OnEnable()
    {
        m_moveEnter = false;
        m_attackEnter = false;
        m_buttonAttack.SetActive(true);
        m_joystickMove.SetActive(true);
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        m_moveEnter = false;
        m_attackEnter = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    void OnDestroy()
    {
        GameController.readyPlayEvent -= OnReadyPlay;
        GameController.activeInputEvent -= OnActive;
    }

    private void OnActive(bool active)
    {
        m_stop = !active;
        gameObject.SetActive(active);
    }

    private void OnReadyPlay()
    {
        m_stop = false;
        gameObject.SetActive(true);
    }

    public void FightEnter()
    {
        if (m_stop || m_attackEnter)
            return;
        m_attackEnter = true;
        onAttackAction?.Invoke();
    }

    public void FightExit()
    {
        if (m_stop || !m_attackEnter)
            return;
        m_attackEnter = false;
        onAttackAction?.Invoke();
    }
    public void OnMove()
    {
        if (m_stop)
            return;
        m_moveEnter = true;
        var joystick = m_joystickMove.GetComponent<FixedJoystick>();
        axisMovingAction?.Invoke(new Vector2(joystick.Horizontal, joystick.Vertical));
    }

    public void OnMoveStop()
    {
        if (m_stop || !m_moveEnter)
            return;
        m_moveEnter = false;
        var joystick = m_joystickMove.GetComponent<FixedJoystick>();
        axisMovingAction?.Invoke(new Vector2(0, 0));
    }
}
