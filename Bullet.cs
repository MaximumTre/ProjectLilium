using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class Bullet : MonoBehaviour, IBullet
    {
        [SerializeField] float deactivateTime = 3f;
        Rigidbody rb;

        public Damage BulletDamage;

        public List<string> DamageTags;

        public GameObject Graphics, HitGraphics;
        public ParticleSystem[] StopOnHit;

        bool hit = false, setup = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            Graphics?.SetActive(true);
            HitGraphics?.SetActive(false);
            hit = false;
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Invoke("LifetimeOver", 6f);
            setup = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(!hit && setup)
            {
                Debug.Log("HIT");
                CancelInvoke("LifetimeOver");

                if (DamageTags.Contains(collision.collider.tag) && collision.collider.GetComponent<DamageReceiver>())
                {
                    EntityStatus es = collision.collider.GetComponent<DamageReceiver>().MyStatus;
                    int result = CalculateDamage.instance.TakeDamage(BulletDamage, es);
                    es.TakeDamage(result);
                }

                Graphics?.SetActive(false);
                HitGraphics?.SetActive(true);
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                for (int i = 0; i < StopOnHit.Length; i++)
                {
                    StopOnHit[i].Stop();
                }
                Invoke("Deactivate", deactivateTime);
                rb.isKinematic = true;
                hit = true;
            }
        }

        public void InitBullet(Damage damage, List<string> damageTags, float force)
        {
            BulletDamage = damage;
            DamageTags = damageTags;
            rb.AddForce(transform.forward * force, ForceMode.Force);
            setup = true;
        }

        void Deactivate()
        {
            HitGraphics?.SetActive(false);
            gameObject.SetActive(false);
        }

        void LifetimeOver()
        {
            Graphics?.SetActive(false);
            HitGraphics?.SetActive(true);
            for (int i = 0; i < StopOnHit.Length; i++)
            {
                StopOnHit[i].Stop();
            }
            Invoke("Deactivate", deactivateTime);
            rb.isKinematic = true;
            hit = true;
        }
    }
}
