using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class MultipleAttack : _ObjectsMakeBase, IStartAttack, IBullet
    {
        public float m_startDelay;
        public float m_makeDelay;
        public float endTime = 10f;
        public Vector3 m_randomPos;
        public Vector3 m_randomRot;
        public Vector3 m_randomScale;

        float m_Time;
        float m_Time2;
        float m_delayTime;
        [SerializeField] int m_count;
        float m_scalefactor;

        EntityStatus Attacker;
        public Damage AttackDamage;
        public List<string> DamageTags;
        public LayerMask DamageMask;
        bool start = false;
        public float bulletForce = 1000f;

        Vector3[] initialPositions;
        Quaternion[] initialRot;


        void OnEnable()
        {
            m_count = 0;
            m_Time = m_Time2 = Time.time;

            initialPositions = new Vector3[m_makeObjs.Length];
            initialRot = new Quaternion[m_makeObjs.Length];

            for (int i = 0; i < m_makeObjs.Length; i++)
            {
                initialPositions[i] = m_makeObjs[i].transform.localPosition;
                initialRot[i] = m_makeObjs[i].transform.localRotation;
            }
        }


        void Update()
        {
            if (start && Time.time > m_Time + m_startDelay)
            {
                if (Time.time > m_Time2 + m_makeDelay && m_count < m_makeObjs.Length)
                {
                    Vector3 m_pos = transform.position + GetRandomVector(m_randomPos) * m_scalefactor;
                    Quaternion m_rot = transform.rotation * Quaternion.Euler(GetRandomVector(m_randomRot));

                    m_makeObjs[m_count].transform.position = m_pos;
                    m_makeObjs[m_count].transform.rotation = m_rot;

                    Vector3 m_scale = (m_makeObjs[m_count].transform.localScale + GetRandomVector2(m_randomScale));

                    m_makeObjs[m_count].SetActive(true);

                    if (m_makeObjs[m_count].GetComponent<IStartAttack>() != null)
                        m_makeObjs[m_count].GetComponent<IStartAttack>().StartAttack(Attacker, AttackDamage, DamageTags, DamageMask);

                    if (m_makeObjs[m_count].GetComponent<IBullet>() != null)
                    {
                        m_makeObjs[m_count].GetComponent<IBullet>().InitBullet(AttackDamage, DamageTags, bulletForce);
                    }

                    m_Time2 = Time.time;
                    m_count++;
                }

                if (m_count >= m_makeObjs.Length)
                {
                    Invoke("EndAttack", endTime);
                    start = false;
                }
            }
        }

        void EndAttack()
        {
            Debug.Log("END ATTACK for " + gameObject.name);
            gameObject.SetActive(false);

            for (int i = 0; i < m_makeObjs.Length; i++)
            {
                m_makeObjs[i].gameObject.SetActive(false);
                m_makeObjs[i].transform.SetParent(transform);
                m_makeObjs[i].transform.localPosition = initialPositions[i];
                m_makeObjs[i].transform.localRotation = initialRot[i];
            }

            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }

        public void StartAttack(EntityStatus status, Damage damage, List<string> damageTags, LayerMask mask)
        {
            Attacker = status;
            AttackDamage = damage;
            DamageTags = damageTags;
            DamageMask = mask;
            start = true;
        }

        public void InitBullet(Damage dmg, List<string> damageTags, float force)
        {
            AttackDamage = dmg;
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