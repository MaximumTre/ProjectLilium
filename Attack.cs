using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.UI;

namespace SolarFalcon
{
    [CreateAssetMenu(fileName ="NewAttack", menuName = "Attack Object")]
    public class Attack : ScriptableObject
    {
        public Texture Icon;
        public GameObject ForwardPrefab;
        public Damage ForwardAttackDamage;
        public GameObject ProjectilePrefab;
        public Damage ProjectileAttackDamage;
        public float projectileForce;
        public GameObject GroundPrefab;
        public Damage GroundAttackDamage;
        public GameObject CenterPrefab;
        public Damage CenterAttackDamage;
        public GameObject LeftSwordPrefab;
        public Damage LeftSwordAttackDamage;
        public GameObject RightSwordPrefab;
        public Damage RightSwordAttackDamage;
        public GameObject TwoHandPrefab;
        public Damage TwoHandAttackDamage;

        public GameObject HypermodeForwardPrefab;
        public Damage HypermodeForwardAttackDamage;
        public GameObject HypermodeProjectilePrefab;
        public Damage HypermodeProjectileAttackDamage;
        public float HypermodeProjectileForce;
        public GameObject HypermodeGroundPrefab;
        public Damage HypermodeGroundAttackDamage;
        public GameObject HypermodeCenterPrefab;
        public Damage HypermodeCenterAttackDamage;
        public GameObject HypermodeLeftSwordPrefab;
        public Damage HypermodeLeftSwordAttackDamage;
        public GameObject HypermodeRightSwordPrefab;
        public Damage HypermodeRightSwordAttackDamage;
        public GameObject HypermodeTwoHandPrefab;
        public Damage HypermodeTwoHandAttackDamage;

        public AttackType AttackType;

        public bool CanBeInterrupted, lockMove, halfMove, lockRotation, parentAfterSpawn;
        public int enduranceCost;
        public float cooldown, effectPoint;
        public string animationClip;
    }
}
