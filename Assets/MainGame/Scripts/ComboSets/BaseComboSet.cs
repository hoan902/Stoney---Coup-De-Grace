using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseComboSet : MonoBehaviour
{
    public LayerMask enemyContactFilter;
    public List<AttackCombo> atkComboList;
    public BaseCharacter hostCharacter;
    public AudioClip[] m_clipFight;
    public AudioClip[] m_clipMissAtk;
    public int comboIndex { get; private set; }
    public string fightAnimation
    {
        get
        {
            return atkComboList[comboIndex].attackAnimtaion;
        }
    }

    protected bool canHit;

    protected DamageDealerInfo attackData
    {
        get
        {
            bool critical = Random.Range(0, 100) <= 20;
            DamageDealerInfo info = new DamageDealerInfo()
            {
                damage = critical ? (int)(m_damage * 4) : m_damage,
                critical = critical,
                attacker = hostCharacter.transform,
                AnimationAtkName = atkComboList[comboIndex].attackAnimtaion,
            };
            return info;
        }
    }
    private int m_damage
    {
        get
        {
            return atkComboList[comboIndex].damage;
        }
    }

    public virtual void SetupCombo(BaseCharacter hostChar, Collider[] bodyPartColliders)
    {
        hostCharacter = hostChar;
        foreach (var item in atkComboList)
        {
            switch (item.attackUsedBodyPart)
            {
                case AttackUsedBodyPart.RightHand:
                    item.impactCollider = bodyPartColliders[0];
                    break;
                case AttackUsedBodyPart.LeftHand:
                    item.impactCollider = bodyPartColliders[1];
                    break;
                case AttackUsedBodyPart.RightLegs:
                    item.impactCollider = bodyPartColliders[2];
                    break;
                case AttackUsedBodyPart.LeftLegs:
                    item.impactCollider = bodyPartColliders[3];
                    break;
            }
        }
    }


    public virtual void Init(GameObject characterObj)
    {
        hostCharacter = characterObj.GetComponent<BaseCharacter>();
        comboIndex = -1;
    }

    public virtual void StartPunching()
    {
        canHit = true;
        SoundManager.PlaySound3D(m_clipFight[Random.Range(0, m_clipFight.Length)], 100, false, transform.position);
    }
    public virtual void EndPunching()
    {
        canHit = false;
    }

    public virtual void ComboUpdate()
    {
        comboIndex++;
        if (comboIndex >= atkComboList.Count)
            comboIndex = 0;
        Punching();
    }

    public void Punching()
    {
        hostCharacter.OnPunching();
    }


    #region ---- Serializable Classes
    [System.Serializable]
    public class AttackCombo
    {
        public string attackAnimtaion;
        public int damage;
        public Collider impactCollider;
        public AttackUsedBodyPart attackUsedBodyPart;
    }
    #endregion
}
