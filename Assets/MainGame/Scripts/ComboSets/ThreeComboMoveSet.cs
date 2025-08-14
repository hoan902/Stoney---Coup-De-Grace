using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThreeComboMoveSet : BaseComboSet
{
    [SerializeField] private AudioClip m_criticalHitSFX;
    [SerializeField] private AudioClip m_criticalHitCrowd1SFX;
    [SerializeField] private AudioClip m_criticalHitCrowd2SFX;
    public override void StartPunching()
    {
        canHit = true;
        ActiveSkill();
    }

    void ActiveSkill()
    {
        if (comboIndex < 0)
            return;
        SoundManager.PlaySound3D(m_clipMissAtk[comboIndex], 100, false, transform.position);
        StartCoroutine(IBaseHit(comboIndex));
    }
    public int OverlapCollider3D(Collider collider, LayerMask layerMask, List<Collider> results)
    {
        Bounds bounds = collider.bounds;
        Collider[] hits = Physics.OverlapBox(
            bounds.center,
            bounds.extents,
            collider.transform.rotation,
            ~0
        );

        results.Clear();
        results.AddRange(hits);
        return hits.Length;
    }

    IEnumerator IBaseHit(int indexCombo)
    {
        List<Collider> recievedHits = new List<Collider>();
        float startTime = Time.realtimeSinceStartup;
        DamageDealerInfo data = attackData;
        bool characterHited = false;
        bool sent = false;
        data.damage += hostCharacter.atkDamage;
        while (canHit)
        {
            List<Collider> results = new List<Collider>();
            //enemy check
            OverlapCollider3D(atkComboList[indexCombo].impactCollider, enemyContactFilter, results);
            for (int i = 0; i < results.Count; i++)
            {
                //Already got hit by attack combo [i]
                if (recievedHits.Contains(results[i]))
                    continue;
                if (results[i].GetComponent<BaseCharacter>() == null)
                    continue;
                BaseCharacter hitscanChar = results[i].GetComponent<BaseCharacter>();
                recievedHits.Add(results[i]);
                characterHited = hitscanChar; //Just a reminder: characterHited = true if hitscanChar not null
                if (hitscanChar.charTeam == hostCharacter.charTeam)
                    continue;
                results[i].SendMessage("OnHit", data, SendMessageOptions.DontRequireReceiver);
                SoundManager.PlaySound3D(m_clipFight[comboIndex], 100, false, transform.position);
            }
            yield return new WaitForEndOfFrame();
            if (!sent && recievedHits.Count > 0)
            {
                sent = true;
                if (data.critical)
                {
                    data.attacker.SendMessage("SpawnCritHitFX");
                    SoundManager.PlaySound3D(m_criticalHitSFX, 100, false, transform.position); 
                    SoundManager.PlaySound3D(m_criticalHitCrowd1SFX, 100, false, transform.position); 
                    SoundManager.PlaySound3D(m_criticalHitCrowd2SFX, 100, false, transform.position); 
                }
                else
                {
                    data.attacker.SendMessage("SpawnNormalHitFX");
                }
            }
            if (data.attacker.TryGetComponent<PlayerManagement>(out var player))
            {
                if (data.critical && recievedHits.Count > 0 && characterHited)
                {
                    if (Time.realtimeSinceStartup - startTime > 0.3f)
                        Time.timeScale = 1;
                    else
                        Time.timeScale = 0.1f;
                }
            }
        }
        Time.timeScale = 1;
    }
}
