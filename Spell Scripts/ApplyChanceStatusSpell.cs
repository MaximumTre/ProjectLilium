using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class ApplyChanceStatusSpell : MonoBehaviour, IStartSpell
    {
        EntityStatus Caster;
        public Damage SpellDamage, StatusDamage;
        public List<string> DamageTags;
        Spell Spell;
        NegativeEffectType NegativeEffectType;
        public float activeTime = 10f, destroyTime = 3f;
        bool start = false;

        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            Caster = caster;
            Spell = spell;
            SpellDamage = spell.SpellDamage;
            SpellDamage.myEntityStats = Caster;
            StatusDamage = spell.StatusDamage;
            StatusDamage.myEntityStats = Caster;
            DamageTags = damageTags;
            NegativeEffectType = spell.StatusType;
            start = true;
        }

        void Update()
        {
            if (start)
            {
                activeTime -= Time.deltaTime;
                if(activeTime <= 0)
                {
                    Destroy(gameObject, destroyTime);
                    start = false;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (start)
            {
                if (DamageTags.Contains(other.tag) && other.GetComponent<DamageReceiver>())
                {
                    DamageReceiver hit = other.GetComponent<DamageReceiver>();
                    int result = CalculateDamage.instance.TakeDamage(SpellDamage, hit.MyStatus);
                    hit.MyStatus.TakeDamage(result);

                    hit.NegativeStatus.AddNegativeEffect(NegativeEffectType, Spell.statusAdd, StatusDamage);
                }
            }
        }
    }
}
