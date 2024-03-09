using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public interface IStartSpell
    {
        void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask);
    }
}
