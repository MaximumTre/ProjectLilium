using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class TriggerDamageSpell : MonoBehaviour, IStartSpell
    {
        EntityStatus Caster;
        Spell Spell;
        public List<string> DamageTags;
        bool start = false, on = false;
        public Damage SpellDamage;
        public float delayTime = 0, activeTime = 4f, destroyTime = 3f, damageTime = 0.2f, pauseTime = 0.4f;
        float currentDamageTime = 0, currentPauseTime = 0;
        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            Caster = caster;
            Spell = spell;
            DamageTags = damageTags;
            SpellDamage = Spell.SpellDamage;
            SpellDamage.myEntityStats = caster;
            start = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (start)
            {
                if(on)
                {
                    if (DamageTags.Contains(other.tag) && other.GetComponent<DamageReceiver>())
                    {
                        EntityStatus receiver = other.GetComponent<DamageReceiver>().MyStatus;
                        int result = CalculateDamage.instance.TakeDamage(SpellDamage, receiver);
                        receiver.TakeDamage(result);
                    }
                }
            }
        }

        void Update()
        {
            if (start)
            {
                delayTime -= Time.deltaTime;

                if(delayTime <= 0)
                {
                    activeTime -= Time.deltaTime;
                    if (activeTime <= 0)
                    {
                        Destroy(gameObject, destroyTime);
                        start = false;
                    }

                    if (on)
                    {
                        currentDamageTime += Time.deltaTime;
                        if (currentDamageTime >= damageTime)
                        {
                            currentPauseTime = 0;
                            on = false;
                        }
                    }

                    if (!on)
                    {
                        currentPauseTime += Time.deltaTime;
                        if (currentPauseTime >= pauseTime)
                        {
                            currentDamageTime = 0;
                            on = true;
                        }
                    }
                }
            }
        }
    }
}
