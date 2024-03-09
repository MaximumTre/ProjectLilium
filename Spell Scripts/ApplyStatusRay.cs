using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class ApplyStatusRay : MonoBehaviour, IStartSpell
    {
        public Raycaster Raycaster;
        public GameObject Effect;
        public float delay = 0f;
        public List<string> DamageTags;
        public LayerMask DamageMask;
        EntityStatus Caster;
        public NegativeEffectType NegativeEffect;
        public float radius, range, destroyTime = 15f;
        public bool firstOnly = true;
        [SerializeField] bool start = false;
        Spell Spell;
        public Damage StatusDamage;
        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            Caster = caster;
            Spell = spell;
            StatusDamage = Spell.StatusDamage;
            StatusDamage.myEntityStats = Caster;
            DamageTags = damageTags;
            DamageMask = damageMask;
            start = true;
        }

        public void Update()
        {
            if (start)
            {
                if(delay > 0f)
                {
                    delay -= Time.deltaTime;
                }

                if(delay <= 0)
                {
                    RaycastHit[] hits = Raycaster.CastBeam(radius, range, DamageMask);
                    if(hits.Length > 0)
                    {
                        for(int i = 0; i < hits.Length; i++)
                        {
                            if (DamageTags.Contains(hits[i].collider.tag))
                            {
                                if (hits[i].collider.GetComponent<DamageReceiver>())
                                {
                                    DamageReceiver hit = hits[i].collider.GetComponent<DamageReceiver>();
                                    hit.NegativeStatus.AddNegativeEffect(NegativeEffect, Spell.statusAdd, StatusDamage);

                                    if (firstOnly)
                                    {
                                        if (Effect != null)
                                        {
                                            Effect.transform.position = hit.transform.root.position;
                                            Effect.SetActive(true);
                                        }

                                        Destroy(gameObject, destroyTime);
                                        start = false;
                                        return;
                                    }
                                }
                            }
                        }

                        start = false;
                    }
                }
            }
        }
    }
}
