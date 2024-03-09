using HutongGames.PlayMaker.Actions;
using Kryz.CharacterStats;
using System.Collections.Generic;
using UnityEngine;


namespace SolarFalcon
{
    [System.Serializable]
    public class GameData
    {
        public bool HypermodeUnlocked;
        public bool useGamePad, invertMouseH, invertMouseV, invertGamepadH, invertGamepadV;

        public int MaximumHypermodeAttacks;
        public int UltimaSparks;

        public int MaxLifePoints, MaxEndurancePoints;
        public int Evocation;
        public int WeaponSkill;
        public int EnergyProjection;
        public int Thaumaturgy;
        public int Artifice;
        public float PhysicalDefense;
        public float MagicalDefense;
        public int AmbientEnduranceGain;

        [SerializeField]
        public List<ElementType> Weakness, Resistance, Immunity, Absorbtion;

        public float ExponentialPower;

        [SerializeField]
        bool PoisonImmune, BleedImmune, DrainImmune;

        public bool DaggerUnlocked, ScytheUnlocked, ScepterUnlocked, WhipUnlocked;

        /*
        StatModifier
            TwentyPercentAdditivePri = new StatModifier(.20f, StatModType.PercentAdd),
            TwentyPercentSubtractivePri = new StatModifier(.20f, StatModType.PercentSub),
            PoisonSub = new StatModifier(.30f, StatModType.PercentSub);
        */

        public GameData() 
        {
            MaxLifePoints = 100;
            MaxEndurancePoints = 100;
            Evocation = 10;
            WeaponSkill = 10;
            EnergyProjection = 1;
            Thaumaturgy = 1;
            Artifice = 1;
            PhysicalDefense = 0.05f;
            MagicalDefense = 0.05f;
            AmbientEnduranceGain = 1;

            Weakness = new List<ElementType>();
            Resistance = new List<ElementType>();
            Immunity = new List<ElementType>();
            Absorbtion = new List<ElementType>();

            ExponentialPower = 1.1f;

            HypermodeUnlocked = false;
            useGamePad = false;
            invertMouseH = false;
            invertMouseV = false;
            invertGamepadH = false;
            invertGamepadV = false;

            MaximumHypermodeAttacks = 3;
            UltimaSparks = 5;

            DaggerUnlocked = true;
            WhipUnlocked = false;
            ScytheUnlocked = false;
            ScepterUnlocked = false;
    }
    }
}


