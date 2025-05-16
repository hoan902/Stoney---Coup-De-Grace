using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class TriggerHelper : MonoBehaviour
{
    [SerializeField] private LayerMask m_layerMask;
    [SerializeField] private List<TriggerHandler> m_onTriggerEnter;
    [SerializeField] private List<TriggerHandler> m_onTriggerExit;
    [SerializeField] private List<TriggerHandler> m_onTriggerStay;
    [SerializeField] private List<TriggerHandler> m_onCollisionEnter;
    [SerializeField] private List<TriggerHandler> m_onCollisionExit;
    [SerializeField] private List<TriggerHandler> m_onCollisionStay;

    private void OnTriggerEnter(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) == 0 && m_layerMask != 0) 
            return;
        foreach (var handler in m_onTriggerEnter.Where(handler => handler != null))
        {
            Send(handler, collision);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) == 0 && m_layerMask != 0) 
            return;
        foreach (var handler in m_onTriggerExit.Where(handler => handler != null))
        {
            Send(handler, collision);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) == 0 && m_layerMask != 0) 
            return;
        foreach (var handler in m_onTriggerStay.Where(handler => handler != null))
        {
            Send(handler, collision);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) == 0 && m_layerMask != 0) 
            return;
        foreach (var handler in m_onCollisionEnter.Where(handler => handler != null))
        {
            Send(handler, collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) == 0 && m_layerMask != 0) return;
        foreach (var handler in m_onCollisionExit.Where(handler => handler != null))
        {
            Send(handler, collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) == 0 && m_layerMask != 0) return;
        foreach (var handler in m_onCollisionStay.Where(handler => handler != null))
        {
            Send(handler, collision);
        }
    }

    void Send(TriggerHandler handler, object data)
    {
        handler.receiver.SendMessage(handler.method,
            handler.sender == null ? data : new CollisionData() { sender = handler.sender, data = data });
    }

    [Serializable]
    private class TriggerHandler
    {
        public GameObject sender;
        public GameObject receiver;
        public string method;
    }
}

public class CollisionData
{
    public GameObject sender;
    public object data;
}
