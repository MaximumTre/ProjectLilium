using UnityEngine;
using Cinemachine;
using static UnityEditor.FilePathAttribute;
using UnityEditor.Build.Content;

namespace SolarFalcon
{
    public class PlayerStatus : EntityStatus
    {
        public static PlayerStatus Instance;
        [SerializeField] bool debugDirection;
        GameData CurrentSave;

        PlayerLock PlayerLock;
        PlayerBlock PlayerBlock;
        PlayerCaster PlayerCaster;

        public delegate void PlayerDeath();
        public static PlayerDeath PDeath;
        public CinemachineVirtualCamera DeathCamera;
        public Transform ModelRoot, PickupPoint;

        float currentEndGainTime;

        private void Awake()
        {
            Instance = this;
            PlayerLock = GetComponent<PlayerLock>();
            PlayerBlock = GetComponentInChildren<PlayerBlock>();
            PlayerCaster = GetComponentInChildren<PlayerCaster>();
        }

        private void Start()
        {
            Alive = true;
            InitializePlayer();

            DeathCamera.Priority = 0;

            if(OnStatusUpdate != null)
            {
                OnStatusUpdate();
            }
        }

        public override void UseSpark()
        {
            base.UseSpark();
        }

        void InitializePlayer()
        {
            CurrentSave = GameManager.instance.GetCurrentSave();
            stats.MaxLifePoints.BaseValue = CurrentSave.MaxLifePoints;
            stats.MaxEndurancePoints.BaseValue = CurrentSave.MaxEndurancePoints;
            stats.Evocation.BaseValue = CurrentSave.Evocation;
            stats.WeaponSkill.BaseValue = CurrentSave.WeaponSkill;
            stats.EnergyProjection.BaseValue = CurrentSave.EnergyProjection;
            stats.Thaumaturgy.BaseValue = CurrentSave.Thaumaturgy;
            stats.Artifice.BaseValue = CurrentSave.Artifice;
            stats.PhysicalDefense.BaseValue = CurrentSave.PhysicalDefense;
            stats.MagicalDefense.BaseValue = CurrentSave.MagicalDefense;
            stats.AmbientEnduranceGain.BaseValue = CurrentSave.AmbientEnduranceGain;


            stats.Weakness = CurrentSave.Weakness;
            stats.Resistance = CurrentSave.Resistance;
            stats.Immunity = CurrentSave.Immunity;
            stats.Absorbtion = CurrentSave.Absorbtion;

            CurrentLifePoints = (int)CurrentSave.MaxLifePoints;
            CurrentEndurancePoints = (int)CurrentSave.MaxEndurancePoints;
            stats.ExponentialPower = CurrentSave.ExponentialPower;
            CurrentUltimaSparks = CurrentSave.UltimaSparks;
        }

        private void Update()
        {
            if(CurrentEndurancePoints != GetMaxEP())
            {
                currentEndGainTime -= Time.deltaTime * animator.speed;
                if(currentEndGainTime <= 0)
                {
                    CurrentEndurancePoints += (int)GetEnduranceGain();
                    if(OnEnduranceGained != null)
                    {
                        OnEnduranceGained(transform, (int)GetEnduranceGain());
                    }
                    currentEndGainTime = 1;
                }
            }

            if(CurrentEndurancePoints > GetMaxEP())
            {
                CurrentEndurancePoints = (int)GetMaxEP();
            }

            if(CurrentLifePoints > GetMaxLP())
            {
                CurrentLifePoints = (int)GetMaxLP();
            }
        }

        public void EnduranceOrb(int amount)
        {
            CurrentEndurancePoints += amount;
            if (OnEnduranceGained != null)
            {
                OnEnduranceGained(transform, amount);
            }
        }

        public override void UpdateStatus()
        {
            if (OnStatusUpdate != null)
            {
                OnStatusUpdate();
            }
        }

        public override void TakeDamage(int amount, Transform attacker)
        {
            if(!Alive) { return; }

            Debug.Log("PlayerLock - Can Interrupt " + !PlayerLock.GetCanInterruptLock());

            bool canBlockThis = false;
            // Calculate the direction from player to object
            var attackerDirection = attacker.position - ModelRoot.position;
            var playerDirection = ModelRoot.forward;

            // Calculate the angle between player and object
            var angle = Vector3.Angle(playerDirection, attackerDirection);

            if(amount < 0)
            {
                if (OnDamaged != null)
                {
                    OnDamaged.Invoke(transform, amount);
                }
            }

            else
            {
                if (OnHealed != null)
                {
                    OnHealed.Invoke(transform, amount);
                }
            }

            #region Direction of Attacker

            // Determine if the object is to the front, rear, left, or right of the player
            if (angle <= 45f)
            {
                if(debugDirection)
                Debug.Log("Object is in front of the player.");

                canBlockThis = true;
                animator.SetFloat("Horizontal Damage", 0);
                animator.SetFloat("Vertical Damage", 1);
            }

            else if (angle > 45f && angle <= 135f)
            {
                if (Vector3.Cross(playerDirection, attackerDirection).y < 0)
                {
                    if (debugDirection)
                        Debug.Log("Object is to the left of the player.");

                    animator.SetFloat("Horizontal Damage", -1);
                    animator.SetFloat("Vertical Damage", 0);
                }
                else
                {
                    if (debugDirection)
                        Debug.Log("Object is to the right of the player.");

                    animator.SetFloat("Horizontal Damage", 1);
                    animator.SetFloat("Vertical Damage", 0);
                }
            }
            else
            {
                if (debugDirection)
                    Debug.Log("Object is behind the player.");

                animator.SetFloat("Horizontal Damage", 0);
                animator.SetFloat("Vertical Damage", -1);
            }
            #endregion

            if (PlayerBlock.IsBlocking() && canBlockThis)
            {
                PlayerBlock.Damaged(amount);
                return;
            }

            if (PlayerBlock.IsBlocking() && !canBlockThis)
            {
                if (!DeathCheck(amount) | !PlayerLock.GetCanInterruptLock()) { return; }

                animator.StopPlayback();
                PlayerCaster.InterruptCasting();
                PlayerLock.PlayerStatusLockAttack(true);
                PlayerLock.PlayerStatusLockBlock(true);
                PlayerLock.PlayerStatusLockCast(true);
                PlayerLock.PlayerStatusLockDash(true);
                PlayerLock.PlayerStatusLockMove(true);

                // Knockdown?

                return;
            }

            else
            {
                if (!DeathCheck(amount) | !PlayerLock.GetCanInterruptLock()) { return; }

                animator.StopPlayback();
                PlayerCaster.InterruptCasting();
                PlayerLock.PlayerStatusLockAttack(true);
                PlayerLock.PlayerStatusLockBlock(true);
                PlayerLock.PlayerStatusLockCast(true);
                PlayerLock.PlayerStatusLockDash(true);
                PlayerLock.PlayerStatusLockMove(true); 

                //if (!knockdown)
                {
                   
                }

                //else
                {
                    
                }
            }            
        }

        bool DeathCheck(int amount)
        {
            CurrentLifePoints += amount;

            if(CurrentLifePoints <= 0)
            {
                Alive = false;

                if(PDeath != null)
                {
                    PDeath.Invoke();
                }

                PlayerCaster.InterruptCasting();

                PlayerLock.PlayerStatusLockAttack(true);
                PlayerLock.PlayerStatusLockBlock(true);
                PlayerLock.PlayerStatusLockCast(true);
                PlayerLock.PlayerStatusLockDash(true);
                PlayerLock.PlayerStatusLockMove(true);
                PlayerLock.PlayerStatusCanInterrupt(false);
                PlayerLock.PlayerStatusLockMouseRotation(true);
                PlayerLock.PlayerStatusLockRotation(true);

                animator.StopPlayback();
                animator.Play("Death");
                DeathCamera.Priority = 100;
            }
            return Alive;
        }

        public void GetUpAnimationOver()
        {
            //if (KnockedDownFront)
            {
                //KnockedDownFront = false;
            }

            PlayerLock.PlayerStatusLockAttack(false);
            PlayerLock.PlayerStatusLockBlock(false);
            PlayerLock.PlayerStatusLockCast(false);
            PlayerLock.PlayerStatusLockDash(false);
            PlayerLock.PlayerStatusLockMove(false);
            PlayerLock.PlayerStatusCanInterrupt(true);

            // Do something on getup
        }

        public void DamageAnimationOver()
        {
            animator.SetBool("Heavy Damage", false);
            animator.SetBool("Normal Damage", false);

            PlayerLock.PlayerStatusLockAttack(false);
            PlayerLock.PlayerStatusLockBlock(false);
            PlayerLock.PlayerStatusLockCast(false);
            PlayerLock.PlayerStatusLockDash(false);
            PlayerLock.PlayerStatusLockMove(false);
            PlayerLock.PlayerStatusCanInterrupt(true);
            animator.CrossFade("Idle", 0.0125f);
        }
    }
}
