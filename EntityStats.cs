using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

/*
EntityStats

    EntityStats class controls everything regarding every entity that has a stat block in the game.
    This class will add/remove weaknesses, and control the leveling up of a stat.

Physical/Magical Defense: Upon each level up, physical/magical defense is increased by 1% (0.01f)
and caps out at 50%. Since it starts at 5% (0.05f) this means a maximum of 45 points can be added
to these stats before the function will not allow any more points.
When LevelUp is called it returns true/false. If a false is received, the GUI program should 
alert the player in some way that they cannot raise the stat anymore.

Status Effects - Positive and Negative effects that can only be activated by calling them on each
EntityStats object. Some effects will be listed but may not be implemented in EntityStats.

Speed Up: Increases Movement Speed (and attack speed for enemies)

Speed Down: Decreases Movement Speed (and attack speed for enemies)

Curse: Prevents spell casting

Poison: Reduces all stats (battle stats only) by 30%.

Refreshing Wind: Regenerates EP each second

Drain: Degenerates LP each second.

Regeneration: Regenerates LP each second

Bleed: Degenerates LP each second.

Max LP Up: Increases Max LP by 20%
Max EP Up: Increases Max EP by 20%

Max LP Down: Decreases MaxLP by 20%
Max EP Down: Decreases MaxEP by 20%

Weapon Skill Up: Increases Weapon Skill by 20%
Evocation Up: Increases Evocation by 20%
Energy Projection Up: Increases Energy Projection by 20%

Weapon Skill Down: Decreases Weapon Skill by 20%
Evocation Down: Decreases Evocation by 20%
Energy Projection Down: Decreases Energy Projection by 20%

Physical Defense Down: Decreases Physical defense by 20%
Magical Defense Down: Decreases Magical defense by 20%

Physical Defense Up: Increases Physical Defense by 20%
Magical Defense Up: Increases Physical Defense by 20%

Exponential: Increases stats Exponentially. This skill is increased in a special way. 
Greatly reduces Physical/Magical Defense, causes LP to drop each second.
 
 */


namespace SolarFalcon
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewEntityStat", menuName = "Entity Stats")]
    public class EntityStats : ScriptableObject
    {
        public int CurrentLifePoints;
        public bool Alive;

        public CharacterStat MaxLifePoints, MaxEndurancePoints;
        public CharacterStat Evocation;
        public CharacterStat WeaponSkill;
        public CharacterStat EnergyProjection;
        public CharacterStat Thaumaturgy;
        public CharacterStat Artifice;
        public CharacterStat PhysicalDefense;
        public CharacterStat MagicalDefense;
        public CharacterStat AmbientEnduranceGain;

        [SerializeField]
        public List<ElementType> Weakness, Resistance, Immunity, Absorbtion;

        public float ExponentialPower;

        [SerializeField]
        bool PoisonImmune, BleedImmune, DrainImmune;

        [SerializeField]
        public int UltimaSparksDropped;

        StatModifier
            TwentyPercentAdditivePri = new StatModifier(.20f, StatModType.PercentAdd),
            TwentyPercentSubtractivePri = new StatModifier(.20f, StatModType.PercentSub),
            PoisonSub = new StatModifier(.30f, StatModType.PercentSub);

        public EntityStats()
        {
            Alive = true;
            MaxLifePoints = new CharacterStat();
            MaxLifePoints.BaseValue = 100;
            MaxEndurancePoints = new CharacterStat();
            MaxEndurancePoints.BaseValue = 100;
            Evocation = new CharacterStat();
            Evocation.BaseValue = 10;
            WeaponSkill = new CharacterStat();
            WeaponSkill.BaseValue = 10;
            EnergyProjection = new CharacterStat();
            EnergyProjection.BaseValue = 1;
            Thaumaturgy = new CharacterStat();
            Thaumaturgy.BaseValue = 1;
            Artifice = new CharacterStat();
            Artifice.BaseValue = 1;
            PhysicalDefense = new CharacterStat();
            PhysicalDefense.BaseValue = 0.05f;
            MagicalDefense = new CharacterStat();
            MagicalDefense.BaseValue = 0.05f;
            AmbientEnduranceGain = new CharacterStat();
            AmbientEnduranceGain.BaseValue = 1;

            Weakness = new List<ElementType>();
            Resistance = new List<ElementType>();
            Immunity = new List<ElementType>();
            Absorbtion = new List<ElementType>();

            CurrentLifePoints = (int)MaxLifePoints.Value;
            ExponentialPower = 1.1f;
        }    
    }
}
