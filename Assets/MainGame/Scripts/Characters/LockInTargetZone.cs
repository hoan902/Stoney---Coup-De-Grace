using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class LockInTargetZone : MonoBehaviour
{
    [SerializeField] private BaseCharacter m_char;
    private HashSet<BaseCharacter> m_enemiesInRange = new();
 
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<BaseCharacter>())
            return;
        BaseCharacter approchedEnemy = other.GetComponent<BaseCharacter>();
        if (approchedEnemy.charTeam == m_char.charTeam) 
            return;
        m_enemiesInRange.Add(approchedEnemy);
        CheckForNearestEnemy();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<BaseCharacter>())
            return;
        BaseCharacter approchedEnemy = other.GetComponent<BaseCharacter>();
        m_enemiesInRange.Remove(approchedEnemy);
        CheckForNearestEnemy();
    }

    void CheckForNearestEnemy()
    {
        switch (m_enemiesInRange.Count)
        {
            case 0:
                m_char.SetLockInTarget(null);
                return;
            case 1:
                m_char.SetLockInTarget(m_enemiesInRange.First().transform);
                return;
        }

        Transform nearestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (BaseCharacter enemy in m_enemiesInRange)
        {
            var distance = Vector3.Distance(m_char.GetCharacterPos().position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
        m_char.SetLockInTarget(nearestEnemy);
    }
}
