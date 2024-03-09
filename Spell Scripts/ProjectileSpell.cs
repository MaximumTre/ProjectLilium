using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class ProjectileSpell : MonoBehaviour, IStartSpell
    {
        Rigidbody rb;
        Animator animator;
        public float Speed = 1;
        //public float Dampeen = 0;
        //public float MinSpeed = 1;
        public float TimeDelay = 0;
        public float RandomMoveRadius = 0;
        public float RandomMoveSpeedScale = 0;

        [SerializeField] bool BulletActive = false;

        public List<string> DamageTags;
        public LayerMask DamageMask;
        public bool explodesOnCollision;
        EntityStatus CasterStatus;

        public Raycaster[] Raycasters;
        public GameObject[] DeactivatedObjectsOnCollision, EffectsOnCollision;
        public List<GameObject> CurrentHitObjects;
        public Damage SpellDamage;

        Vector3 randomTimeOffset;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }


        private void FixedUpdate()
        {
            if(BulletActive)
            {
                UpdateWorldPosition();
            }
        }

        void UpdateWorldPosition()
        {
            Vector3 randomOffset = Vector3.zero;
            if (RandomMoveRadius > 0)
            {
                randomOffset = GetRadiusRandomVector() * RandomMoveRadius;
            }

            rb.velocity = transform.forward * Speed;
        }

        Vector3 GetRadiusRandomVector()
        {
            var x = Time.time * RandomMoveSpeedScale + randomTimeOffset.x;
            var vecX = Mathf.Sin(x / 7 + Mathf.Cos(x / 2)) * Mathf.Cos(x / 5 + Mathf.Sin(x));

            x = Time.time * RandomMoveSpeedScale + randomTimeOffset.y;
            var vecY = Mathf.Cos(x / 8 + Mathf.Sin(x / 2)) * Mathf.Sin(Mathf.Sin(x / 1.2f) + x * 1.2f);

            x = Time.time * RandomMoveSpeedScale + randomTimeOffset.z;
            var vecZ = Mathf.Cos(x * 0.7f + Mathf.Cos(x * 0.5f)) * Mathf.Cos(Mathf.Sin(x * 0.8f) + x * 0.3f);


            return new Vector3(vecX, vecY, vecZ);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (BulletActive)
            {
                if (DamageTags.Contains(collision.collider.tag) && collision.collider.GetComponent<DamageReceiver>())
                {
                    Debug.Log("I hit " + collision.collider.name);
                    EntityStatus es = collision.collider.GetComponent<DamageReceiver>().MyStatus;
                    int result = CalculateDamage.instance.TakeDamage(SpellDamage, es);
                    es.TakeDamage(result, CasterStatus.transform);
                }

                foreach (GameObject go in EffectsOnCollision)
                {
                    if (!go.activeInHierarchy)
                    {
                        ContactPoint contact = collision.contacts[0];
                        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                        Vector3 pos = contact.point;
                        go.transform.position = pos;
                        go.transform.rotation = rot;
                        go.SetActive(true);
                    }
                }

                foreach (GameObject o in DeactivatedObjectsOnCollision)
                {
                    o.SetActive(false);
                }

                if (explodesOnCollision) // Uses Raycasters
                {
                    for (int i = 0; i < Raycasters.Length; i++)
                    {
                        RaycastHit[] hits = Raycasters[i].CastBeam(DamageMask);
                        for (int ii = 0; ii < hits.Length; ii++)
                        {
                            if (DamageTags.Contains(hits[ii].collider.tag))
                            {
                                if (!CurrentHitObjects.Contains(hits[ii].collider.gameObject))
                                {
                                    if (CurrentHitObjects[ii].GetComponent<DamageReceiver>())
                                    {
                                        CurrentHitObjects.Add(hits[ii].collider.gameObject);
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < CurrentHitObjects.Count; i++)
                    {
                        EntityStatus es = CurrentHitObjects[i].GetComponent<DamageReceiver>().MyStatus;
                        int result = CalculateDamage.instance.TakeDamage(SpellDamage, es);
                        es.TakeDamage(result, CasterStatus.transform);
                    }

                    explodesOnCollision = false;
                }

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                BulletActive = false;
            }
        }

        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            DamageTags = damageTags;
            DamageMask = damageMask;
            animator.speed = 1;
            BulletActive = true;
            SpellDamage = spell.SpellDamage;
            SpellDamage.myEntityStats = caster;
            CasterStatus = caster;
        }
    }
}
