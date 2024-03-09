using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class MultiRandomAreaSpell : _ObjectsMakeBase, IStartSpell
    {
        public float m_startDelay;
        public int m_makeCount;
        public float m_makeDelay;
        public Vector3 m_randomPos;
        public Vector3 m_randomRot;
        public Vector3 m_randomScale;
        public bool isObjectAttachToParent = true;

        float m_Time;
        float m_Time2;
        float m_delayTime;
        float m_count;

        EntityStatus Caster;
        public Damage SpellDamage, StatusDamage;
        public NegativeEffectType NegativeEffect;
        public List<string> DamageTags;
        public LayerMask DamageMask;
        bool start = false;
        public float bulletForce = 1000f;
        Spell Spell;


        void Start()
        {
            m_Time = m_Time2 = Time.time;
        }


        void Update()
        {
            if (start && Time.time > m_Time + m_startDelay)
            {
                if (Time.time > m_Time2 + m_makeDelay && m_count < m_makeCount)
                {
                    Vector3 m_pos = transform.position + GetRandomVector(m_randomPos);
                    Quaternion m_rot = transform.rotation * Quaternion.Euler(GetRandomVector(m_randomRot));


                    for (int i = 0; i < m_makeObjs.Length; i++)
                    {
                        GameObject m_obj = Instantiate(m_makeObjs[i], m_pos, m_rot);
                        Vector3 m_scale = (m_makeObjs[i].transform.localScale + GetRandomVector2(m_randomScale));
                        if (isObjectAttachToParent)
                            m_obj.transform.parent = this.transform;
                        m_obj.transform.localScale = m_scale;

                        if (m_obj.GetComponent<IStartSpell>() != null)
                            m_obj.GetComponent<IStartSpell>().StartSpell(Caster, Spell, DamageTags, DamageMask);

                        if (m_obj.GetComponent<IBullet>() != null)
                        {
                            m_obj.GetComponent<IBullet>().InitBullet(SpellDamage, DamageTags, bulletForce);
                        }
                    }

                    m_Time2 = Time.time;
                    m_count++;
                }
            }
        }

        public void StartSpell(EntityStatus status, Spell spell, List<string> damageTags, LayerMask mask)
        {
            Caster = status;
            Spell = spell;
            SpellDamage = Spell.SpellDamage;
            SpellDamage.myEntityStats = Caster;
            StatusDamage = Spell.StatusDamage;
            StatusDamage.myEntityStats = Caster;
            DamageTags = damageTags;
            DamageMask = mask;
            start = true;
        }

        public void InitBullet(Damage dmg, List<string> damageTags, float force)
        {
            SpellDamage = dmg;
            DamageTags = damageTags;
            bulletForce = force;
            start = true;
        }

        void OnInterrupt()
        {
            start = false;
            transform.BroadcastMessage("Stop");
        }
    }

}