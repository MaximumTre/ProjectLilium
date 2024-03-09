using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    [System.Serializable]
    public struct Damage 
    {
        public ElementType ElementType;
        public EntityStatus myEntityStats;
        public int attackPower;
        public float attackPotencyMin, attackPotencyMax, explosiveForce, pushForce;
        [Tooltip("Denotes if this attack will knockdown those struck")]
        public bool knockdown, explosive, push;

        public Damage(EntityStatus sender, ElementType attackElement, int atkPower, float potencyMin, float potencyMax = 1f, int expFrc = 0, int pushFrc = 0)
        {
            myEntityStats = sender;
            ElementType = attackElement;
            attackPower = atkPower;
            attackPotencyMin = potencyMin;
            attackPotencyMax = potencyMax;
            explosiveForce = expFrc;
            pushForce = pushFrc;
            knockdown = false;
            explosive = false;
            push = false;
        }
    }
}
