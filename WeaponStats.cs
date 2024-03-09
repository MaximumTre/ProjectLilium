using Kryz.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    /*
     WeaponStats struct
    This struct is designed to hold instanced weapon stats for use in calculating damage.
    The weaponPotency member is a CharacterStat so it can receive buffs. The weaponPotency should
    never rise above 1.0f as this would ensure a critical hit.

    Weapon Potency
    This is basically the minimum percentage this weapon will deal in of its weaponPower stat.

    Weapon Power
    This is also a CharacterStat so it can be buffed/debuffed during play.
    */
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon Stats")]
    public class WeaponStats : ScriptableObject
    {
        public ElementType DamageType;
        public CharacterStat weaponPower;
        public CharacterStat weaponPotency;
        public int refinement;

        public WeaponStats(ElementType type, int pow, float pot, int refine)
        {
            DamageType = type;
            weaponPotency = new CharacterStat(pot);
            weaponPower = new CharacterStat(pow);
            refinement = refine;
        }
    }
}
