using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace SolarFalcon
{
    public class WeaponDamageField : MonoBehaviour, IStartAttack
    {
        Damage WeaponDamage;
        public EntityStatus Attacker;
        public List<string> DamageTags;
        public LayerMask DamageMask;
        public GameObject HitPrefab;
        public bool singleHit;
        bool start = false;
        public float activeTime = 3f;

        public List<Transform> HitList;

        public Transform WeaponGraphics;

        public void StartAttack(EntityStatus status, Damage damage, List<string> damageTags, LayerMask mask)
        {
            Attacker = status;
            WeaponDamage = damage;
            DamageTags = damageTags;
            DamageMask = mask;
            start = true;
            
            if(WeaponGraphics != null)
            {
                WeaponGraphics.SendMessage("Resolve");
            }
        }

        void Update()
        {
            if(start)
            {
                activeTime -= Time.deltaTime;
                if(activeTime <= 0 )
                {
                    if (WeaponGraphics != null)
                    {
                        WeaponGraphics.SendMessage("Dissolve");
                    }

                    if (gameObject.GetComponentInChildren<ParticleSystem>())
                    {
                        gameObject.GetComponentInChildren<ParticleSystem>().Stop();
                    }

                    start = false;
                }
            }
        }

        void OnInterrupt()
        {
            activeTime = 0;
            gameObject.GetComponentInChildren<ParticleSystem>().Stop();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!start)
            {
                return;
            }

            int result;

            if (DamageTags.Contains(other.tag))
            {
                if(singleHit)
                {
                    if (HitList.Contains(other.transform))
                    {
                        return;
                    }

                    result = CalculateDamage.instance.TakeDamage(WeaponDamage, other.GetComponent<DamageReceiver>().MyStatus);
                    other.GetComponent<DamageReceiver>().MyStatus.TakeDamage(result, Attacker.transform);
                    Instantiate(HitPrefab, other.ClosestPoint(transform.position), Quaternion.identity);

                    HitList.Add(other.transform);
                }

                result = CalculateDamage.instance.TakeDamage(WeaponDamage, other.GetComponent<DamageReceiver>().MyStatus);
                other.GetComponent<DamageReceiver>().MyStatus.TakeDamage(result, Attacker.transform);
                Quaternion randomRot = Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
                Instantiate(HitPrefab, other.ClosestPoint(transform.position), randomRot);
            }
        }
    }
}
