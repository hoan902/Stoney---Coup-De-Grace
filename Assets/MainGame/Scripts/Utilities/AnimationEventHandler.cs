using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_receiverGO;
    public void OnTriggerEvent(string method)
    {
        m_receiverGO.SendMessage(method);
    }
}
