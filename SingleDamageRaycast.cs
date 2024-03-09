using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class SingleDamageRaycast : MonoBehaviour, IStartAttack
    {
        public GameObject HitPrefab;
        public float radius;
        public float length;
        public Transform cachedParent;

        public float hitEffectChance = 1;

        public List<string> DamageTags;
        public LayerMask DamageMask;
        public Damage MyDamage;

        public float delayTime = 0; /// When this reaches zero, stop checking for damage.
        public float endTime = 10f;
        [SerializeField] bool start = false, delay = true, initialized = false;

        void OnEnable()
        {
            initialized = false;
            start = false;
        }

        public void StartAttack(EntityStatus status, Damage damage, List<string> damageTags, LayerMask mask)
        {
            transform.SetParent(null);
            DamageTags = damageTags;
            DamageMask = mask;
            MyDamage = damage;
            initialized = true;
            start = true;
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
                if (delayTime <= 0) { start = true; delay = false; }
            }

            if (start)
            {
                RaycastHit[] rH = Physics.SphereCastAll(transform.position, radius, transform.forward, length);
                for (int i = 0; i < rH.Length; i++)
                {
                    if (DamageTags.Contains(rH[i].collider.tag) && rH[i].collider.GetComponent<DamageReceiver>())
                    {
                        int result = CalculateDamage.instance.TakeDamage(MyDamage, rH[i].collider.GetComponent<DamageReceiver>().MyStatus);
                        rH[i].collider.GetComponent<DamageReceiver>().MyStatus.TakeDamage(result, MyDamage.myEntityStats.transform);

                        if(Random.Range(0f, 1f) <= hitEffectChance)
                            Instantiate(HitPrefab, rH[i].point, Quaternion.LookRotation(rH[i].normal));
                    }
                }

                Invoke("EndEffect", endTime);
                start = false;                
            }
        }

        void EndEffect()
        {
            if(cachedParent != null)
            {
                transform.SetParent(cachedParent);
            }

            gameObject.SetActive(false);
        }

        void OnInterrupt()
        {
            start = false;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (Vector3.forward * length), radius);
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.forward * length));
        }
    }
}