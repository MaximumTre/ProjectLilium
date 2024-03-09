using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEditor.Experimental.GraphView;


namespace SolarFalcon
{
    public class NegativeStatus : MonoBehaviour
    {
        [SerializeField] bool debug, debugDirection;
        [SerializeField] bool canBeParried, canBeShocked, canBeSlowed, canBeCursed, canBeIgnited, canBeFrozen, canBeSealed;
        [SerializeField] Animator animator;

        [SerializeField] float TimeScale = 1;

        [SerializeField] bool shocked = false, slowed = false, cursed = false, ignited = false, frozen = false,
            unfreeze = false, seal = false;
        [SerializeField] float shockedTime = 4f, slowedTime = 10f, slowedScale = 0.5f, igniteTime = 8f,
            frozenTime = 4f, curseTime = 30f, sealTime = 5f;
        [SerializeField]
        float shockedLimit = 10, slowedLimit = 10, igniteLimit = 10f, frozenLimit = 10f, curseLimit = 10f, sealLimit = 10f;
        [SerializeField]
        float shockedCurrent = 0, slowedCurrent = 0, igniteCurrent = 0, frozenCurrent = 0, curseCurrent = 0, sealCurrent = 0;

        [Tooltip("Constant variable for ignite damage tick")]
        [SerializeField] const float IGNITE_TICK = 0.33f;
        float currentShocked, currentSlowed, currentFrozen, currentIgnite, currentCursed,
            currentIgniteTick, currentSealed;

        public PlayMakerFSM FSM;
        Rigidbody rb;


        [SerializeField] bool TestShocked = false, TestSlowed = false, TestFrozen, TestIgnite, TestCursed;

        public EntityStatus MyStatus;

        Damage IgniteDamage;
        [SerializeField] Damage TestIgniteDmg;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            PlayerStatus.PDeath += HaltMovement;
            shocked = slowed = cursed = ignited = frozen = unfreeze = seal = false;
            currentCursed = currentSlowed = currentFrozen = currentIgnite = currentIgniteTick = currentSealed = 0;
            TimeScale = 1;
            FSM?.FsmVariables.GetFsmFloat("Time Scale").SafeAssign(1);
            animator.speed = 1;
        }

        private void OnDisable()
        {
            PlayerStatus.PDeath -= HaltMovement;
        }

        void HaltMovement()
        {
            // Stop Everything
            TimeScale = 0;
            FSM?.FsmVariables.GetFsmFloat("Time Scale").SafeAssign(0);
            animator.speed = 0;
        }

        void ResumeMovement()
        {
            TimeScale = 1;
            FSM?.FsmVariables.GetFsmFloat("Time Scale").SafeAssign(1);
            animator.speed = 1;
        }

        void OverrideTimeScale(float amount)
        {
            Debug.Log("Time Scale Overriden!" + amount.ToString());
            TimeScale = amount;
            FSM?.FsmVariables.GetFsmFloat("Time Scale").SafeAssign(amount);
            animator.speed = amount;
        }

        public float GetShockedTime() { return shockedTime; }
        public float GetSlowedTime() { return slowedTime; }
        public float GetIgniteTime() { return igniteTime; }
        public float GetFrozenTime() { return frozenTime; }
        public float GetSealedTime() { return sealTime; }
        public float GetCurseTime() { return curseTime; }


        private void Update()
        {
            if(shocked)
            {
                currentShocked += Time.deltaTime * TimeScale;
                if(currentShocked >= shockedTime)
                {
                    // Check if any other movement locking effects are active
                    if(true) // None for now
                    {
                        Invoke("EndShocked", 4f * TimeScale);
                    }
                    animator.Play("Shocked End");
                    shocked = false;
                }
            }

            if (slowed)
            {
                currentSlowed += Time.deltaTime * TimeScale;
                if (currentSlowed >= slowedTime)
                {
                    TimeScale = 1;
                    animator.speed = 1;
                    FSM?.FsmVariables.GetFsmFloat("Time Scale").SafeAssign(1);
                    slowed = false;
                }
            }

            if(cursed)
            {
                currentCursed += Time.deltaTime * TimeScale;
                if(currentCursed >= curseTime)
                {
                    MyStatus.RemoveCurse();
                    cursed = false;
                }
            }

            if(ignited)
            {
                currentIgnite += Time.deltaTime * TimeScale;
                currentIgniteTick += Time.deltaTime * TimeScale; 
                
                if (currentIgniteTick >= IGNITE_TICK)
                {
                    // Deal ignite damage
                    int result = CalculateDamage.instance.TakeDamage(IgniteDamage, MyStatus);
                    MyStatus.TakeDamage(result);
                    currentIgniteTick = 0;
                }

                if (currentIgnite >= igniteTime)
                {
                    ignited = false;
                }
            }

            if(frozen)
            {
                if(TimeScale >= 0.1f) { TimeScale = 0; }
                animator.speed = TimeScale;
                currentFrozen += Time.deltaTime;
                if(currentFrozen >= frozenTime)
                {
                    unfreeze = true;
                    DOTween.To(() => TimeScale, x => TimeScale = x, 1, 1f);
                    currentFrozen = 0;
                    frozen = false;
                }
            }

            if(unfreeze && !frozen)
            {
                animator.speed = TimeScale;
                currentFrozen += Time.deltaTime;
                if(currentFrozen >= 1)
                {
                    TimeScale = 1;
                    animator.speed = 1;
                    unfreeze = false;
                }
                
            }

            if(seal)
            {
                currentSealed += Time.deltaTime * TimeScale;
                if(currentSealed >= sealTime)
                {
                    animator.SetBool("Sealed Over", true);
                    FSM?.SendEvent("Status Unlocked");
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                    seal = false;
                }
            }

            if(TestShocked)
            {
                NegativeEffect(NegativeEffectType.Shocked);
                TestShocked = false;
            }

            if(TestSlowed)
            {
                NegativeEffect(NegativeEffectType.Slowed);
                TestSlowed = false;
            }

            if(TestCursed)
            {
                NegativeEffect(NegativeEffectType.Cursed);
                TestCursed = false;
            }

            if(TestFrozen)
            {
                NegativeEffect(NegativeEffectType.Frozen);
                TestFrozen = false;
            }

            if(TestIgnite)
            {
                NegativeEffect(NegativeEffectType.Ignite, TestIgniteDmg);
                TestIgnite = false;
            }
        }

        public void NegativeEffect(NegativeEffectType type)
        {
            if(!MyStatus.Alive) { return; }

            switch(type)
            {
                case NegativeEffectType.Parried:
                    if(canBeParried)
                    {
                        FSM?.SendEvent("Parried");
                    }
                    break;

                case NegativeEffectType.Shocked:
                    if(canBeShocked && !shocked)
                    {
                        shocked = true;
                        FSM?.SendEvent("Status Locked");
                        animator.Play("Shock");
                        currentShocked = 0;
                    }
                    break;

                case NegativeEffectType.Slowed:
                    if(canBeSlowed && !slowed)
                    {
                        slowed = true;
                        TimeScale = slowedScale;
                        FSM?.FsmVariables.GetFsmFloat("Time Scale").SafeAssign(slowedScale);
                        animator.speed = slowedScale;
                        currentSlowed = 0;
                    }
                    break;

                case NegativeEffectType.Cursed:
                    if(canBeCursed && !cursed)
                    {
                        cursed = true;
                        MyStatus.Curse();
                        currentCursed = 0;
                    }
                    break;

                case NegativeEffectType.Frozen:
                    if (canBeFrozen && !frozen)
                    {
                        frozen = true;
                        currentFrozen = 0;
                        FSM?.FsmVariables.GetFsmFloat("Time Scale").SafeAssign(0);
                        DOTween.To(() => TimeScale, x => TimeScale = x, 0, 1f);
                    }
                    break;

                case NegativeEffectType.Sealed:
                    if(canBeSealed && !seal)
                    {
                        seal = true;
                        currentSealed = 0;
                        FSM?.SendEvent("Status Locked");
                        rb.constraints = RigidbodyConstraints.FreezeAll;
                        animator.SetBool("Sealed Over", false);
                        animator.CrossFade("Sealed", 0.0125f);
                    }
                    break;
            }            
        }

        public void NegativeEffect(NegativeEffectType type, Damage statusDamage)
        {
            if (!MyStatus.Alive) { return; }

            if(type == NegativeEffectType.Ignite)
            {
                if(canBeIgnited && !ignited)
                {
                    ignited = true;
                    currentIgniteTick = 0;
                    currentIgnite = 0;
                    IgniteDamage = statusDamage;
                }
            }
        }

        public void AddNegativeEffect(NegativeEffectType type, float amount, Damage statusDamage)
        {
            if (MyStatus.Alive)
            {
                switch(type)
                {
                    case NegativeEffectType.Ignite:
                        igniteCurrent += amount;
                        if(igniteCurrent >= igniteLimit)
                        {
                            NegativeEffect(NegativeEffectType.Ignite, statusDamage);
                            igniteCurrent = 0;
                        }
                        break;

                    case NegativeEffectType.Cursed:
                        curseCurrent += amount;
                        if (curseCurrent >= curseLimit)
                        {
                            NegativeEffect(NegativeEffectType.Cursed);
                            curseCurrent = 0;
                        }
                        
                        break;

                    case NegativeEffectType.Slowed:
                        slowedCurrent += amount;
                        if (slowedCurrent >= slowedLimit)
                        {
                            NegativeEffect(NegativeEffectType.Slowed);
                            slowedCurrent = 0;
                        }
                        break;

                    case NegativeEffectType.Frozen:
                        frozenCurrent += amount;
                        if (frozenCurrent >= frozenLimit)
                        {
                            NegativeEffect(NegativeEffectType.Frozen);
                            frozenCurrent = 0;
                        }
                        break;

                    case NegativeEffectType.Sealed:
                        sealCurrent += amount;
                        if (sealCurrent >= sealLimit)
                        {
                            NegativeEffect(NegativeEffectType.Sealed);
                            sealCurrent = 0;
                        }
                        break;

                    case NegativeEffectType.Shocked:
                        shockedCurrent += amount;
                        if (shockedCurrent >= shockedLimit)
                        {
                            NegativeEffect(NegativeEffectType.Shocked);
                            shockedCurrent = 0;
                        }
                        break;
                }
            }
        }

        void EndShocked()
        {
            FSM.SendEvent("Status Unlocked");
        }

        public void Flinch(Transform attacker)
        {
            // Calculate the direction from player to object
            var attackerDirection = attacker.position - transform.position;
            var myDirection = transform.forward;

            // Calculate the angle between player and object
            var angle = Vector3.Angle(myDirection, attackerDirection);
            // Determine if the object is to the front, rear, left, or right of the player
            if (angle <= 45f)
            {
                if (debugDirection)
                    Debug.Log("Object is in front of the enemy.");

                //canBlockThis = true;
            }

            else if (angle > 45f && angle <= 135f)
            {
                if (Vector3.Cross(myDirection, attackerDirection).y < 0)
                {
                    if (debugDirection)
                        Debug.Log("Object is to the left of the enemy.");
                }
                else
                {
                    if (debugDirection)
                        Debug.Log("Object is to the right of the enemy.");
                }
            }

            else
            {
                if (debugDirection)
                    Debug.Log("Object is behind the enemy.");
            }
        }
    }
}
