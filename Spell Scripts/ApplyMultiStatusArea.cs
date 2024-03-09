using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class ApplyMultiStatusArea : MonoBehaviour, IStartSpell
    {
        public GameObject EffectPrefab;
        public float delay = 0f;
        public List<string> DamageTags;
        public LayerMask DamageMask;
        EntityStatus Caster;
        public NegativeEffectType[] NegativeEffects;
        public Damage SpellDamage, StatusDamage;
        public float radius, damageTick = 1f, damageTime = 5f, destroyTime = 15f;
        [SerializeField] bool start = false;
        public Spell Spell;
        float currentDamageTime;

        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            Caster = caster;
            Spell = spell;
            SpellDamage = Spell.SpellDamage;
            SpellDamage.myEntityStats = Caster;
            StatusDamage = Spell.StatusDamage;
            StatusDamage.myEntityStats = Caster;
            DamageTags = damageTags;
            DamageMask = damageMask;
            start = true;
            currentDamageTime = damageTime; // First hit is immediate
        }

        public void Update()
        {
            if (start)
            {
                if (delay > 0f)
                {
                    delay -= Time.deltaTime;
                }

                if (delay <= 0)
                {
                    damageTime -= Time.deltaTime;
                    currentDamageTime += Time.deltaTime;
                    if (currentDamageTime >= damageTick)
                    {
                        Collider[] hits = Physics.OverlapSphere(transform.position, radius, DamageMask);
                        if (hits.Length > 0)
                        {
                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (DamageTags.Contains(hits[i].tag))
                                {
                                    if (hits[i].GetComponent<DamageReceiver>())
                                    {
                                        DamageReceiver hit = hits[i].GetComponent<DamageReceiver>();
                                        int result = CalculateDamage.instance.TakeDamage(SpellDamage, hit.MyStatus);

                                        for (int j = 0; j < NegativeEffects.Length; j++)
                                        {
                                            hit.NegativeStatus.AddNegativeEffect(NegativeEffects[j], Spell.statusAdd, StatusDamage);
                                        }

                                        hit.MyStatus.TakeDamage(result, Caster.transform);

                                        if (EffectPrefab != null)
                                            Instantiate(EffectPrefab, hit.transform.root.position, Quaternion.identity);
                                    }
                                }
                            }
                        }

                        currentDamageTime = 0;
                    }

                    if (damageTime <= 0)
                    {
                        Destroy(gameObject, destroyTime);
                        start = false;
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
