
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class HitSpawnSpell : MonoBehaviour, IStartSpell
    {
        public GameObject FXSpawn;
        public bool DestoyOnHit = false;
        public bool FixRotation = false;
        public float LifeTimeAfterHit = 1;
        public float LifeTime = 0;
        public float radius = 10f;
        public LayerMask DamageMask;
        public List<string> DamageTags;
        public Damage SpellDamage, IgniteStatusDamage;
        public NegativeEffectType NegativeEffectType;
        public float statusChance = 0.5f;

        [SerializeField]bool start = false;

        void Start()
        {

        }

        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            SpellDamage = spell.SpellDamage;
            SpellDamage.myEntityStats = caster;
            DamageTags = damageTags;
            DamageMask = damageMask;
            start = true;
        }

        void Spawn()
        {
            if (FXSpawn != null)
            {
                Quaternion rotate = this.transform.rotation;
                if (!FixRotation)
                    rotate = FXSpawn.transform.rotation;
                GameObject fx = (GameObject)GameObject.Instantiate(FXSpawn, this.transform.position, rotate);
                if (LifeTime > 0)
                    GameObject.Destroy(fx.gameObject, LifeTime);
            }
            if (DestoyOnHit)
            {

                GameObject.Destroy(this.gameObject, LifeTimeAfterHit);
                if (this.gameObject.GetComponent<Collider>())
                    this.gameObject.GetComponent<Collider>().enabled = false;

            }

            Collider[] hits = Physics.OverlapSphere(this.transform.position, radius, DamageMask);
            for(int i = 0; i < hits.Length; i++)
            {
                if (DamageTags.Contains(hits[i].tag))
                {
                    if (hits[i].GetComponent<DamageReceiver>())
                    {
                        DamageReceiver receiver = hits[i].GetComponent<DamageReceiver>();
                        int result = CalculateDamage.instance.TakeDamage(SpellDamage, receiver.MyStatus);
                        receiver.MyStatus.TakeDamage(result, SpellDamage.myEntityStats.transform);

                        if(Random.Range(0f, 1f) <= statusChance)
                        {
                            if(NegativeEffectType != NegativeEffectType.Ignite)
                            {
                                receiver.NegativeStatus.NegativeEffect(NegativeEffectType);
                            }

                            else
                            {
                                receiver.NegativeStatus.NegativeEffect(NegativeEffectType, IgniteStatusDamage);
                            }
                        }
                    }
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Projectile"))
            {
                Spawn();
                Destroy(other.gameObject);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.CompareTag("Projectile"))
            {
                Spawn();
                Destroy(collision.collider.gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}