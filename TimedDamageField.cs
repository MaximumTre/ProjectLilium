using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

namespace SolarFalcon
{
    public class TimedDamageField : MonoBehaviour, IStartAttack
    {
        public Transform HitLocation;
        public GameObject HitPrefab;
        public float radius;
        public float freq; /// Every x miliseconds, check damage.
        float currentFreq;
        public bool singleHit = true; /// This wil only allow a single instance of damage per object hit. 
        public List<Transform> HitTargets;

        public List<string> DamageTags;
        public LayerMask DamageMask;
        public Damage MyDamage;

        public float activeTime = 1, delayTime = 0; /// When this reaches zero, stop checking for damage.
        [SerializeField] bool start = false, delay = true, initialized = false;

        public void StartAttack(EntityStatus status, Damage damage, List<string> damageTags, LayerMask mask)
        {
            DamageTags = damageTags;
            DamageMask = mask;
            MyDamage = damage;
            initialized = true;
            if(HitLocation == null) { HitLocation = transform; }
        }

        public void Update()
        {
            if (!initialized)
            {
                return;
            }

            if (delay)
            {
                delayTime -= Time.deltaTime;
                if(delayTime <= 0 ) { start = true; delay = false; }
            }

            if(start)
            {
                activeTime -= Time.deltaTime;
                currentFreq += Time.deltaTime;
                if (currentFreq >= freq)
                {
                    Collider[] c = Physics.OverlapSphere(HitLocation.position, radius, DamageMask);
                    for (int i = 0; i < c.Length; i++)
                    {
                        if (DamageTags.Contains(c[i].tag))
                        {
                            if (singleHit)
                            {
                                if (!HitTargets.Contains(c[i].transform))
                                {
                                    int result = CalculateDamage.instance.TakeDamage(MyDamage, c[i].GetComponent<DamageReceiver>().MyStatus);
                                    c[i].GetComponent<DamageReceiver>().MyStatus.TakeDamage(result, MyDamage.myEntityStats.transform);
                                    HitTargets.Add(c[i].transform);
                                }
                            }

                            else
                            {
                                int result = CalculateDamage.instance.TakeDamage(MyDamage, c[i].GetComponent<DamageReceiver>().MyStatus);
                                c[i].GetComponent<DamageReceiver>().MyStatus.TakeDamage(result, MyDamage.myEntityStats.transform);
                            }

                            RaycastHit hitInfo;
                            Physics.Linecast(HitLocation.position, c[i].transform.position, out hitInfo, DamageMask);
                            if(DamageTags.Contains(hitInfo.collider.tag))
                                Instantiate(HitPrefab, hitInfo.point, Quaternion.Euler(hitInfo.normal));
                            else
                                Instantiate(HitPrefab, c[i].transform.position, Quaternion.identity);
                        }
                    }

                    currentFreq = 0;
                }

                if (activeTime <= 0)
                {
                    start = false;
                }
            }
        }

        void OnInterrupt()
        {
            activeTime = 0;
            gameObject.GetComponentInChildren<ParticleSystem>().Stop();
        }

        void OnDrawGizmos()
        {
            if(HitLocation == null)
            {
                HitLocation = transform;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(HitLocation.position, radius);
        }
    }
}