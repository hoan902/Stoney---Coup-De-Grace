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
        if (approchedEnemy.charTeam == m_char.charTeam || approchedEnemy.isDead) 
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

    public void UpdateCurrentStatus(BaseCharacter currentTarget)
    {
        if (m_enemiesInRange.Contains(currentTarget))
        {
            if (currentTarget.isDead)
            {
                m_enemiesInRange.Remove(currentTarget);
                CheckForNearestEnemy();
            }
        }
    }

    void CheckForNearestEnemy()
    {
        Transform nearestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (BaseCharacter enemy in m_enemiesInRange)
        {
            if (enemy.isDead)
                continue;
            var distance = Vector3.Distance(m_char.GetCharacterPos().position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
        m_char.target = nearestEnemy;
    }
}
