

using UnityEngine;

namespace SolarFalcon
{
    public  class CalculateDamage : MonoBehaviour
    {
        public static CalculateDamage instance;

        private void Awake()
        {
            instance = this;
        }

        public int BalanceDamage(float damageAmount)
        {
            int result = Mathf.RoundToInt(damageAmount);
            return result;
        }

        public int TakeDamage(Damage attack, EntityStatus receiver)
        {
            float pot = Random.Range(attack.attackPotencyMin, attack.attackPotencyMax);
            int result = (int)(attack.attackPower * pot);

            if(pot == 1) // critical, roll damage again and add to current result
            {
                pot = Random.Range(attack.attackPotencyMin, attack.attackPotencyMax);
                result += (int)(attack.attackPower * pot);
            }

            if(receiver.HasWeakness(attack.ElementType))
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    result -= (int)(2 * receiver.GetPhysicalDefense());
                }

                else
                {
                    result -= (int)(2 * receiver.GetMagicalDefense());
                }

                result = -result;
                return result;
            }

            if(receiver.HasResistance(attack.ElementType))
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    result -= (int)(0.5f * receiver.GetPhysicalDefense());
                }

                else
                {
                    result -= (int)(0.5f * receiver.GetMagicalDefense());
                }

                result = -result;
                return result;
            }

            if(receiver.HasAbsorbtion(attack.ElementType))
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    result += (int)(1 * receiver.GetPhysicalDefense());
                }

                else
                {
                    result += (int)(1 * receiver.GetMagicalDefense());
                }

                result = +result;
                return result;
            }

            if(receiver.HasImmunity(attack.ElementType))
            {
                return 0;
            }

            if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
            {
                result += (int)(1 * receiver.GetPhysicalDefense());
            }

            else
            {
                result += (int)(1 * receiver.GetMagicalDefense());
            }

            result = -result;
            return result;
        }

        public int TakeDamageFixed(Damage attack, EntityStatus receiver, float pot, float critPot)
        {            
            int result = (int)(attack.attackPower * pot);

            if (pot == 1) // critical, roll damage again and add to current result
            {
                result += (int)(attack.attackPower * critPot);
            }

            if (receiver.HasWeakness(attack.ElementType))
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    result -= (int)(2 * receiver.GetPhysicalDefense());
                }

                else
                {
                    result -= (int)(2 * receiver.GetMagicalDefense());
                }

                result = -result;
            }

            if (receiver.HasResistance(attack.ElementType))
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    result -= (int)(0.5f * receiver.GetPhysicalDefense());
                }

                else
                {
                    result -= (int)(0.5f * receiver.GetMagicalDefense());
                }

                result = -result;
            }

            if (receiver.HasAbsorbtion(attack.ElementType))
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    result += (int)(1 * receiver.GetPhysicalDefense());
                }

                else
                {
                    result += (int)(1 * receiver.GetMagicalDefense());
                }

                result = +result;
            }

            if (receiver.HasImmunity(attack.ElementType))
            {
                return 0;
            }

            else
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    result += (int)(1 * receiver.GetPhysicalDefense());
                }

                else
                {
                    result += (int)(1 * receiver.GetMagicalDefense());
                }

                result = -result;
            }

            // Value is returned for testing purposes
            return result;
        }

        public void TakeDamageTest(Damage attack, EntityStatus receiver, ref ActualDamage actDmg)
        {
            actDmg.potential = Random.Range(attack.attackPotencyMin, attack.attackPotencyMax);
            actDmg.damage = (int)(attack.attackPower * actDmg.potential);

            if (actDmg.potential == 1) // critical, roll damage again and add to current result
            {
                actDmg.potentialCrit = Random.Range(attack.attackPotencyMin, attack.attackPotencyMax);
                actDmg.damage += (int)(attack.attackPower * actDmg.potential);
            }

            if (receiver.HasWeakness(attack.ElementType))
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    actDmg.damage -= (int)(2 * receiver.GetPhysicalDefense());
                }

                else
                {
                    actDmg.damage -= (int)(2 * receiver.GetMagicalDefense());
                }

                actDmg.damage = -actDmg.damage;
            }

            if (receiver.HasResistance(attack.ElementType))
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    actDmg.damage -= (int)(0.5f * receiver.GetPhysicalDefense());
                }

                else
                {
                    actDmg.damage -= (int)(0.5f * receiver.GetMagicalDefense());
                }

                actDmg.damage = -actDmg.damage;
            }

            if (receiver.HasAbsorbtion(attack.ElementType))
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    actDmg.damage += (int)(1 * receiver.GetPhysicalDefense());
                }

                else
                {
                    actDmg.damage += (int)(1 * receiver.GetMagicalDefense());
                }

                actDmg.damage = +actDmg.damage;
            }

            if (receiver.HasImmunity(attack.ElementType))
            {
                actDmg.damage = 0;
            }

            else
            {
                if (attack.ElementType == ElementType.Physical ||
                attack.ElementType == ElementType.Earth)  // Physical Damage
                {
                    actDmg.damage += (int)(1 * receiver.GetPhysicalDefense());
                }

                else
                {
                    actDmg.damage += (int)(1 * receiver.GetMagicalDefense());
                }

                actDmg.damage = -actDmg.damage;
            }
        }
    }
}
