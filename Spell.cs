using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    [CreateAssetMenu(fileName ="NewSpell", menuName ="Spell Object")]
    public class Spell : ScriptableObject
    {
        public GameObject SpellPrefab;
        public ElementType ElementType;
        public SpellType SpellType;
        public int EnduranceCost;
        public float statusAdd;
        public NegativeEffectType StatusType;
        public Damage SpellDamage, StatusDamage;
        public List<Seals> Sequence;
        public CastType CastType;
        public bool knockdown, hypermodeOnly, useNormal; 
    }

    public enum Seals { North, South, East, West }
    public enum CastType { Snap, ForwardProjectile, TouchGround, AreaBurst, LongBeam, SelfHeal }
}
