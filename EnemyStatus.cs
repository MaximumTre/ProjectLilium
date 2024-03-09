using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SolarFalcon
{
    public class EnemyStatus : EntityStatus
    {
        [SerializeField] bool debugDirection, debugFlinch, debugKnockdown, dummy = false;
        [SerializeField] int lastDamage;

        [UnityEngine.Tooltip("This should only be set by the FSM.")]
        [SerializeField] bool isBlocking = false; 
        // The purpose of this class is to facilitate messages between collisions received from attacks etc
        // and the FSM. The only thing that should happen inside here is sending messages to the FSM, handling 
        // actually updating the LP lost, and sending death messages when enemies die. 
        // This is to prevent animation glitches and logic errors because Unity's animator is annoying as all hell

        public PlayMakerFSM FSM;

        public void Blocking(bool setting) { isBlocking = setting; }

        public delegate void EntityDeath();
        public event EntityDeath OnMyDeath;

        [SerializeField]
        int DamageShield = 0;


        public override void TakeDamage(int amount, Transform attacker)
        {
            if (!Alive)
            {
                return;
            }

            lastDamage = amount;

            if (!isBlocking)
            {
                CurrentLifePoints += amount;

                DamageNumber.instance.GetDamageNumber(DamageNumberPoint.position, amount, amount < 0, false);

                if(CurrentLifePoints <= 0 && !dummy)
                {
                    FSM.SendEvent("Death");
                    // Send Death Event
                    if(OnMyDeath != null)
                    {
                        OnMyDeath.Invoke();
                    }

                    Alive = false;
                    return;
                }

                else if (CurrentLifePoints <= 0 && dummy)
                {
                    CurrentLifePoints = (int)GetMaxLP();
                    Alive = true;
                }

                else
                {
                    if(OnDamaged != null)
                    {
                        OnDamaged.Invoke(transform, amount);
                    }
                }
            }


            bool canBlockThis = false;
            
            if (DamageShield <= 0)
            {
                               
            }
        }

        public override void TakeDamage(int amount)
        {
            CurrentLifePoints += amount;

            DamageNumber.instance.GetDamageNumber(DamageNumberPoint.position, amount, amount < 0, false); 
            
            if (CurrentLifePoints <= 0 && !dummy)
            {
                FSM.SendEvent("Death");
                // Send Death Event
                if (OnMyDeath != null)
                {
                    OnMyDeath.Invoke();
                }

                Alive = false;
                return;
            }

            if(CurrentLifePoints <= 0 && dummy)
            {
                CurrentLifePoints = (int)GetMaxLP();
                Alive = true;
            }
        }
    }
}
