using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SolarFalcon
{
    public interface IStartAttack 
    {
        void StartAttack(EntityStatus status, Damage damage, List<string> damageTags, LayerMask mask);
    }
}