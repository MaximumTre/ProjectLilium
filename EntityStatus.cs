using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
using JetBrains.Annotations;
using UnityEngine.Events;

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
    public class EntityStatus : MonoBehaviour
    {

        public delegate void EntityStatusUpdate();
        public EntityStatusUpdate OnStatusUpdate;

        public delegate void DamageEvent(Transform self, int amount);
        public delegate void HealEvent(Transform self, int amount);
        public delegate void EnduranceUsed(Transform self, int amount);
        public delegate void EnduranceGained(Transform self, int amount);

        public DamageEvent OnDamaged;
        public DamageEvent OnHealed;
        public EnduranceUsed OnEnduranceUsed;
        public EnduranceGained OnEnduranceGained;

        public int CurrentLifePoints, CurrentEndurancePoints;
        public int CurrentFlinchShield;
        public bool Alive;

        public EntityStats stats;
        public Animator animator;

        [SerializeField] CharacterStat MaxLifePoints, MaxEndurancePoints;
        [SerializeField] CharacterStat Evocation;
        [SerializeField] CharacterStat WeaponSkill;
        [SerializeField] CharacterStat EnergyProjection;
        [SerializeField] CharacterStat Thaumaturgy;
        [SerializeField] CharacterStat Artifice;
        [SerializeField] CharacterStat PhysicalDefense;
        [SerializeField] CharacterStat MagicalDefense;
        [SerializeField] CharacterStat AmbientEnduranceGain;

        [SerializeField]
        private List<ElementType> Weakness, Resistance, Immunity, Absorbtion;

        [SerializeField]
        protected int CurrentUltimaSparks;
        public int CurrentSparks { get { return CurrentUltimaSparks; } }

        [SerializeField]
        float ExponentialPower;

        [SerializeField]
        public bool PoisonImmune, BleedImmune, DrainImmune, KnockdownImmune;

        StatModifier
            TwentyPercentAdditivePri = new StatModifier(.20f, StatModType.PercentAdd),
            TwentyPercentSubtractivePri = new StatModifier(.20f, StatModType.PercentSub),
            PoisonSub = new StatModifier(.30f, StatModType.PercentSub),
            CurseSub = new StatModifier(0.5f, StatModType.PercentSub),
            BlessingAdd = new StatModifier(0.5f, StatModType.PercentAdd);

        [Tooltip("Local position for middle of character")]
        public float midPointMax, midPointMin;

        public Transform DamageNumberPoint;

        public delegate void EntityDeath();
        public event EntityDeath OnMyDeath;

        public void Start()
        {
            Alive = true;
            //MaxLifePoints = new CharacterStat();
            MaxLifePoints.BaseValue = stats.MaxLifePoints.BaseValue;
            //MaxEndurancePoints = new CharacterStat();
            MaxEndurancePoints.BaseValue = stats.MaxEndurancePoints.BaseValue;
            //Evocation = new CharacterStat();
            Evocation.BaseValue = stats.Evocation.BaseValue;
            //WeaponSkill = new CharacterStat();
            WeaponSkill.BaseValue = stats.WeaponSkill.BaseValue;
            //EnergyProjection = new CharacterStat();
            EnergyProjection.BaseValue = stats.EnergyProjection.BaseValue;
            //Thaumaturgy = new CharacterStat();
            Thaumaturgy.BaseValue = stats.Thaumaturgy.BaseValue;
            //Artifice = new CharacterStat();
            Artifice.BaseValue = stats.Artifice.BaseValue;
            //PhysicalDefense = new CharacterStat();
            PhysicalDefense.BaseValue = stats.PhysicalDefense.BaseValue;
            //MagicalDefense = new CharacterStat();
            MagicalDefense.BaseValue = stats.MagicalDefense.BaseValue;
            //AmbientEnduranceGain = new CharacterStat();
            AmbientEnduranceGain.BaseValue = stats.AmbientEnduranceGain.BaseValue;


            //Weakness = new List<ElementType>();
            Weakness = stats.Weakness;
            //Resistance = new List<ElementType>();
            Resistance = stats.Resistance;
            //Immunity = new List<ElementType>();
            Immunity = stats.Immunity;
            //Absorbtion = new List<ElementType>();
            Absorbtion = stats.Absorbtion;

            CurrentLifePoints = (int)MaxLifePoints.Value;
            CurrentEndurancePoints = (int)MaxEndurancePoints.Value;
            ExponentialPower = stats.ExponentialPower;
        }

        private void OnEnable()
        {
            if(!Alive)
            {
                Alive = true;
                MaxLifePoints.BaseValue = stats.MaxLifePoints.BaseValue;
                MaxEndurancePoints.BaseValue = stats.MaxEndurancePoints.BaseValue;
                Evocation.BaseValue = stats.Evocation.BaseValue;
                WeaponSkill.BaseValue = stats.WeaponSkill.BaseValue;
                EnergyProjection.BaseValue = stats.EnergyProjection.BaseValue;
                Thaumaturgy.BaseValue = stats.Thaumaturgy.BaseValue;
                Artifice.BaseValue = stats.Artifice.BaseValue;
                PhysicalDefense.BaseValue = stats.PhysicalDefense.BaseValue;
                MagicalDefense.BaseValue = stats.MagicalDefense.BaseValue;

                Weakness = stats.Weakness;
                Resistance = stats.Resistance;
                Immunity = stats.Immunity;
                Absorbtion = stats.Absorbtion;

                CurrentLifePoints = (int)MaxLifePoints.Value;
                ExponentialPower = stats.ExponentialPower;
            }

        }

        public float GetCurrentLifePointsNormalized() { return CurrentLifePoints / MaxLifePoints.Value; }
        public float GetCurrentEndurancePointsNormalized() { return CurrentEndurancePoints / MaxEndurancePoints.Value; }

        public virtual void UpdateStatus()
        {
            MaxLifePoints.BaseValue = stats.MaxLifePoints.BaseValue;
            MaxEndurancePoints.BaseValue = stats.MaxEndurancePoints.BaseValue;
            Evocation.BaseValue = stats.Evocation.BaseValue;
            WeaponSkill.BaseValue = stats.WeaponSkill.BaseValue;
            EnergyProjection.BaseValue = stats.EnergyProjection.BaseValue;
            Thaumaturgy.BaseValue = stats.Thaumaturgy.BaseValue;
            Artifice.BaseValue = stats.Artifice.BaseValue;
            PhysicalDefense.BaseValue = stats.PhysicalDefense.BaseValue;
            MagicalDefense.BaseValue = stats.MagicalDefense.BaseValue;

            CurrentLifePoints = (int)MaxLifePoints.Value;
            ExponentialPower = stats.ExponentialPower;

            if(OnStatusUpdate != null)
            {
                OnStatusUpdate.Invoke();
            }
        }

        public virtual void UseEndurance(int amount)
        {
            CurrentEndurancePoints -= amount;

            if(OnEnduranceUsed != null)
            {
                OnEnduranceUsed.Invoke(transform, amount);
            }
        }

        public virtual void UseSpark()
        {
            CurrentUltimaSparks--;
        }

        #region Level Up
        public bool LevelUp(StatType type)
        {
            if (CurrentUltimaSparks == 0)
            {
                return false;
            }

            if (type == StatType.MaxLP)
            {
                int tempLP = (int)MaxLifePoints.BaseValue;
                tempLP *= (int)(1 * .15f);
                MaxLifePoints.BaseValue = tempLP;
            }

            if (type == StatType.MaxEP)
            {
                int tempEP = (int)MaxEndurancePoints.BaseValue;
                tempEP *= (int)(1 * .15f);
                MaxEndurancePoints.BaseValue = tempEP;
            }

            if (type == StatType.WeaponSkill)
            {
                WeaponSkill.BaseValue++;
            }

            if (type == StatType.Evocation)
            {
                Evocation.BaseValue++;
            }

            if (type == StatType.Thaumaturgy)
            {
                Thaumaturgy.BaseValue++;
            }

            if (type == StatType.Artifice)
            {
                Artifice.BaseValue++;
            }

            if (type == StatType.EnergyProjection)
            {
                EnergyProjection.BaseValue++;
            }

            if (type == StatType.PhysicalDefense)
            {
                if (PhysicalDefense.BaseValue >= 0.5f)
                {
                    return false;
                }

                PhysicalDefense.BaseValue += 0.01f;
            }

            if (type == StatType.MagicalDefense)
            {
                if (MagicalDefense.BaseValue >= 0.5f)
                {
                    return false;
                }

                MagicalDefense.BaseValue += 0.01f;
            }

            return true;
        }
        #endregion

        #region Get Private Data

        public float GetMaxLP() { return MaxLifePoints.Value; }
        public float GetMaxEP() { return MaxEndurancePoints.Value; }
        public float GetEvocation() { return Evocation.Value; }
        public float GetWeaponSkill() { return WeaponSkill.Value; }
        public float GetEnergyProjection() { return EnergyProjection.Value; }
        public float GetThaumaturgy() { return Thaumaturgy.Value; }
        public float GetArtifice() { return Artifice.Value; }
        public float GetPhysicalDefense() { return PhysicalDefense.Value; }
        public float GetMagicalDefense() { return MagicalDefense.Value; }

        public float GetEnduranceGain() {  return AmbientEnduranceGain.Value; }

        public bool HasWeakness(ElementType element)
        {
            if (Weakness.Contains(element))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public bool HasResistance(ElementType element)
        {
            if (Resistance.Contains(element))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public bool HasImmunity(ElementType element)
        {
            if (Immunity.Contains(element))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public bool HasAbsorbtion(ElementType element)
        {
            if (Absorbtion.Contains(element))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        #endregion

        #region Edit Private Data

        #region Add Status Effects
        public void MaxLPUp()
        {
            MaxLifePoints.AddModifier(TwentyPercentAdditivePri);
        }

        public void MaxEPUp()
        {
            MaxEndurancePoints.AddModifier(TwentyPercentAdditivePri);
        }

        public void MaxLPDown()
        {
            MaxLifePoints.AddModifier(TwentyPercentSubtractivePri);
        }

        public void MaxEPDown()
        {
            MaxEndurancePoints.AddModifier(TwentyPercentSubtractivePri);
        }

        public void WeaponSkillUp()
        {
            WeaponSkill.AddModifier(TwentyPercentAdditivePri);
        }

        public void EvocationUp()// Increases Evocation by 20%
        {
            Evocation.AddModifier(TwentyPercentAdditivePri);
        }

        public void EnergyProjectionUp()// Increases Energy Projection by 20%
        {
            EnergyProjection.AddModifier(TwentyPercentAdditivePri);
        }

        public void WeaponSkillDown()// Decreases Weapon Skill by 20%
        {
            WeaponSkill.AddModifier(TwentyPercentSubtractivePri);
        }

        public void EvocationDown()// Decreases Evocation by 20%
        {
            Evocation.AddModifier(TwentyPercentSubtractivePri);
        }

        public void EnergyProjectionDown()// Decreases Energy Projection by 20%
        {
            EnergyProjection.AddModifier(TwentyPercentSubtractivePri);
        }

        public void PhysicalDefenseDown()// Decreases Physical defense by 20%
        {
            PhysicalDefense.AddModifier(TwentyPercentSubtractivePri);
        }

        public void MagicalDefenseDown()// Decreases Magical defense by 20%
        {
            MagicalDefense.AddModifier(TwentyPercentSubtractivePri);
        }

        public void PhysicalDefenseUp()// Increases Physical Defense by 20%
        {
            WeaponSkill.AddModifier(TwentyPercentSubtractivePri);
        }

        public void MagicalDefenseUp()// Increases Physical Defense by 20%
        {
            WeaponSkill.AddModifier(TwentyPercentAdditivePri);
        }

        public void Poison()
        {
            if (PoisonImmune) return;

            MaxLifePoints.AddModifier(PoisonSub);
            MaxEndurancePoints.AddModifier(PoisonSub);
            WeaponSkill.AddModifier(PoisonSub);
            Evocation.AddModifier(PoisonSub);
            EnergyProjection.AddModifier(PoisonSub);
            PhysicalDefense.AddModifier(PoisonSub);
            MagicalDefense.AddModifier(PoisonSub);
        }
        public void Curse()
        {
            MaxLifePoints.AddModifier(CurseSub);
            MaxEndurancePoints.AddModifier(CurseSub);
            WeaponSkill.AddModifier(CurseSub);
            Evocation.AddModifier(CurseSub);
            EnergyProjection.AddModifier(CurseSub);
            PhysicalDefense.AddModifier(CurseSub);
            MagicalDefense.AddModifier(CurseSub);
        }

        public void Blessing()
        {
            MaxLifePoints.AddModifier(BlessingAdd);
            MaxEndurancePoints.AddModifier(BlessingAdd);
            WeaponSkill.AddModifier(BlessingAdd);
            Evocation.AddModifier(BlessingAdd);
            EnergyProjection.AddModifier(BlessingAdd);
            PhysicalDefense.AddModifier(BlessingAdd);
            MagicalDefense.AddModifier(BlessingAdd);
        }

        public void Exponential()
        {

        }

        #endregion

        #region Remove Status Effects
        public void RemoveMaxLPUp()
        {
            MaxLifePoints.RemoveModifier(TwentyPercentAdditivePri);
        }

        public void RemoveMaxEPUp()
        {
            MaxEndurancePoints.RemoveModifier(TwentyPercentAdditivePri);
        }

        public void RemoveMaxLPDown()
        {
            MaxLifePoints.RemoveModifier(TwentyPercentSubtractivePri);
        }

        public void RemoveMaxEPDown()
        {
            MaxEndurancePoints.RemoveModifier(TwentyPercentSubtractivePri);
        }

        public void RemoveWeaponSkillUp()
        {
            WeaponSkill.RemoveModifier(TwentyPercentAdditivePri);
        }

        public void RemoveEvocationUp()
        {
            Evocation.RemoveModifier(TwentyPercentAdditivePri);
        }

        public void RemoveEnergyProjectionUp()
        {
            EnergyProjection.RemoveModifier(TwentyPercentAdditivePri);
        }

        public void RemoveWeaponSkillDown()
        {
            WeaponSkill.RemoveModifier(TwentyPercentSubtractivePri);
        }

        public void RemoveEvocationDown()
        {
            Evocation.RemoveModifier(TwentyPercentSubtractivePri);
        }

        public void RemoveEnergyProjectionDown()
        {
            EnergyProjection.RemoveModifier(TwentyPercentSubtractivePri);
        }

        public void RemovePhysicalDefenseDown()
        {
            PhysicalDefense.RemoveModifier(TwentyPercentSubtractivePri);
        }

        public void RemoveMagicalDefenseDown()
        {
            MagicalDefense.RemoveModifier(TwentyPercentSubtractivePri);
        }

        public void RemovePhysicalDefenseUp()
        {
            WeaponSkill.RemoveModifier(TwentyPercentSubtractivePri);
        }

        public void RemoveMagicalDefenseUp()
        {
            WeaponSkill.RemoveModifier(TwentyPercentAdditivePri);
        }

        public void RemovePoison()
        {
            MaxLifePoints.RemoveModifier(PoisonSub);
            MaxEndurancePoints.RemoveModifier(PoisonSub);
            WeaponSkill.RemoveModifier(PoisonSub);
            Evocation.RemoveModifier(PoisonSub);
            EnergyProjection.RemoveModifier(PoisonSub);
            PhysicalDefense.RemoveModifier(PoisonSub);
            MagicalDefense.RemoveModifier(PoisonSub);
        }

        public void RemoveCurse()
        {
            MaxLifePoints.RemoveModifier(CurseSub);
            MaxEndurancePoints.RemoveModifier(CurseSub);
            WeaponSkill.RemoveModifier(CurseSub);
            Evocation.RemoveModifier(CurseSub);
            EnergyProjection.RemoveModifier(CurseSub);
            PhysicalDefense.RemoveModifier(CurseSub);
            MagicalDefense.RemoveModifier(CurseSub);
        }

        public void RemoveBlessing()
        {
            MaxLifePoints.RemoveModifier(BlessingAdd);
            MaxEndurancePoints.RemoveModifier(BlessingAdd);
            WeaponSkill.RemoveModifier(BlessingAdd);
            Evocation.RemoveModifier(BlessingAdd);
            EnergyProjection.RemoveModifier(BlessingAdd);
            PhysicalDefense.RemoveModifier(BlessingAdd);
            MagicalDefense.RemoveModifier(BlessingAdd);
        }

        public void RemoveAllEfects()
        {
            RemoveMaxLPUp();
            RemoveMaxEPUp();
            RemoveMaxLPDown();
            RemoveMaxEPDown();
            RemovePhysicalDefenseDown();
            RemovePhysicalDefenseUp();
            RemoveMagicalDefenseDown();
            RemoveMagicalDefenseUp();
            RemoveEvocationDown();
            RemoveEvocationUp();
            RemoveWeaponSkillUp();
            RemoveWeaponSkillDown();
            RemoveEnergyProjectionDown();
            RemoveEnergyProjectionUp();
            RemovePoison();
            RemoveCurse();
            RemoveBlessing();
        }

        public void RemoveExponential()//: Increases stats Exponentially
        {
            // Not implemented yet
        }

        #endregion

        public void SetPoisonImmune(bool setting) { PoisonImmune = setting; }

        public void SetBleedImmune(bool setting) { BleedImmune = setting; }

        public void SetDrainImmune(bool setting) { DrainImmune = setting; }

        public void AddWeakness(ElementType element)
        {
            if (Weakness.Contains(element))
            {
                return;
            }

            else
            {
                Weakness.Add(element);
            }
        }

        public void AddResistance(ElementType element)
        {
            if (Resistance.Contains(element))
            {
                return;
            }

            else
            {
                Resistance.Add(element);
            }
        }

        public void AddImmunity(ElementType element)
        {
            if (Immunity.Contains(element))
            {
                return;
            }

            else
            {
                Immunity.Add(element);
            }
        }

        public void AddAbsorbtion(ElementType element)
        {
            if (Absorbtion.Contains(element))
            {
                return;
            }

            else
            {
                Absorbtion.Add(element);
            }
        }

        //
        public void RemoveWeakness(ElementType element)
        {
            if (!Weakness.Contains(element))
            {
                return;
            }

            else
            {
                Weakness.Remove(element);
            }
        }

        public void RemoveResistance(ElementType element)
        {
            if (!Resistance.Contains(element))
            {
                return;
            }

            else
            {
                Resistance.Remove(element);
            }
        }

        public void RemoveImmunity(ElementType element)
        {
            if (!Immunity.Contains(element))
            {
                return;
            }

            else
            {
                Immunity.Remove(element);
            }
        }

        public void RemoveAbsorbtion(ElementType element)
        {
            if (!Absorbtion.Contains(element))
            {
                return;
            }

            else
            {
                Absorbtion.Remove(element);
            }
        }

        #endregion

        #region Damage Reaction

        public virtual void TakeDamage(int amount, Transform attacker)
        {
            
        }

        /// <summary>
        /// This version is used for simple damage from sources like status effects
        /// </summary>
        /// <param name="amount"></param>
        public virtual void TakeDamage(int amount)
        {
            CurrentLifePoints -= amount;
        }

        #endregion
    }
}
