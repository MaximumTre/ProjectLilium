using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class ChildObjectSpell : MonoBehaviour, IStartSpell
    {
        public GameObject Object;

        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            Object.GetComponent<IStartSpell>().StartSpell(caster, spell, damageTags, damageMask);
        }
    }
}
