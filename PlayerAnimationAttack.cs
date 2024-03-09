using CMF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SolarFalcon
{
    public class PlayerAnimationAttack : MonoBehaviour
    {
        public delegate void ChangedAttack(Texture icon);
        public ChangedAttack OnChangedAttack;
        public delegate void UseAttack();
        public UseAttack OnUsedAttack;
        public delegate void AttackComplete();
        public UseAttack OnAttackComplete;


        Animator animator;
        public List<Attack> AttackList;
        [SerializeField] bool effectPlayed = false, canInterrupt = true, Hypermode = false;
        [SerializeField] float currentCooldown;
        int currentAttackNumber;
        bool useAttack;

        public Damage ForwardAttackDamage;
        public Damage ProjectileAttackDamage;
        public Damage GroundAttackDamage;
        public Damage CenterAttackDamage;
        public Damage LeftSwordAttackDamage;
        public Damage RightSwordAttackDamage;
        public Damage TwoHandAttackDamage;

        public Damage HypermodeForwardAttackDamage;
        public Damage HypermodeProjectileAttackDamage;
        public Damage HypermodeGroundAttackDamage;
        public Damage HypermodeCenterAttackDamage;
        public Damage HypermodeLeftSwordAttackDamage;
        public Damage HypermodeRightSwordAttackDamage;
        public Damage HypermodeTwoHandAttackDamage;

        PlayerLock PlayerLock;
        PlayerStatus PlayerStatus;
        PlayerDash PlayerDash;

        public CameraAimRay CameraAim;
        public HypermodeController HypermodeController;
        [SerializeField] TurnTowardCameraDirection CameraDirection;
        [SerializeField] TurnTowardControllerVelocity ControllerVelocity;

        public List<string> DamageTags;
        public LayerMask DamageMask;

        public Transform ReticleTransform, ForwardSpawnPoint, ProjectileSpawnPoint, CenterSpawnPoint, GroundSpawnPoint, LeftSword, RightSword, TwoHand;

        public float returnToCamDirection = 3f;
        float weaponSwitchTime = 0.25f;

        public GameObject ForwardAttackCurrent;
        public GameObject ProjectileAttackCurrent;
        public GameObject GroundAttackCurrent;
        public GameObject CenterAttackCurrent;
        public GameObject LeftSwordAttackCurrent;
        public GameObject RightSwordAttackCurrent;
        public GameObject TwoHandAttackCurrent;


        private void Awake()
        {
            PlayerLock = transform.root.GetComponent<PlayerLock>();
            PlayerStatus = transform.root.GetComponent<PlayerStatus>();
            PlayerDash = transform.root.GetComponent<PlayerDash>();
        }

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            HypermodeController = transform.GetComponent<HypermodeController>();
            currentAttackNumber = 0;

            if (OnChangedAttack != null)
            {
                OnChangedAttack.Invoke(AttackList[currentAttackNumber].Icon);
            }

            canInterrupt = AttackList[currentAttackNumber].CanBeInterrupted;

            ForwardAttackDamage = AttackList[currentAttackNumber].ForwardAttackDamage;
            ForwardAttackDamage.myEntityStats = PlayerStatus;

            CenterAttackDamage = AttackList[currentAttackNumber].CenterAttackDamage;
            CenterAttackDamage.myEntityStats = PlayerStatus;

            ProjectileAttackDamage = AttackList[currentAttackNumber].ProjectileAttackDamage;
            ProjectileAttackDamage.myEntityStats = PlayerStatus;

            GroundAttackDamage = AttackList[currentAttackNumber].GroundAttackDamage;
            GroundAttackDamage.myEntityStats = PlayerStatus;

            LeftSwordAttackDamage = AttackList[currentAttackNumber].LeftSwordAttackDamage;
            LeftSwordAttackDamage.myEntityStats = PlayerStatus;

            RightSwordAttackDamage = AttackList[currentAttackNumber].RightSwordAttackDamage;
            RightSwordAttackDamage.myEntityStats = PlayerStatus;

            TwoHandAttackDamage = AttackList[currentAttackNumber].TwoHandAttackDamage;
            TwoHandAttackDamage.myEntityStats = PlayerStatus;


            HypermodeForwardAttackDamage = AttackList[currentAttackNumber].HypermodeForwardAttackDamage;
            HypermodeForwardAttackDamage.myEntityStats = PlayerStatus;

            HypermodeCenterAttackDamage = AttackList[currentAttackNumber].HypermodeCenterAttackDamage;
            HypermodeCenterAttackDamage.myEntityStats = PlayerStatus;

            HypermodeProjectileAttackDamage = AttackList[currentAttackNumber].HypermodeProjectileAttackDamage;
            HypermodeProjectileAttackDamage.myEntityStats = PlayerStatus;

            HypermodeGroundAttackDamage = AttackList[currentAttackNumber].HypermodeGroundAttackDamage;
            HypermodeGroundAttackDamage.myEntityStats = PlayerStatus;

            HypermodeLeftSwordAttackDamage = AttackList[currentAttackNumber].HypermodeLeftSwordAttackDamage;
            HypermodeLeftSwordAttackDamage.myEntityStats = PlayerStatus;

            HypermodeRightSwordAttackDamage = AttackList[currentAttackNumber].HypermodeRightSwordAttackDamage;
            HypermodeRightSwordAttackDamage.myEntityStats = PlayerStatus;

            HypermodeTwoHandAttackDamage = AttackList[currentAttackNumber].HypermodeTwoHandAttackDamage;
            HypermodeTwoHandAttackDamage.myEntityStats = PlayerStatus;

            PlayerStatus.OnDamaged += OnInterrupt;
            HypermodeController.OnHypermodeActive += OnHypermodeActive;
            HypermodeController.OnHypermodeOver += OnHypermodeOver;
        }

        public Transform GetReticleTransform() { return ReticleTransform; }

        public bool CanInterrupt() { return canInterrupt; }

        void OnHypermodeActive()
        {
            Hypermode = true;
        }

        void OnHypermodeOver()
        {
            Hypermode = false;
        }

        // Update is called once per frame
        void Update()
        {
            if(useAttack)
            {
                if (!effectPlayed && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= AttackList[currentAttackNumber].effectPoint)
                {
                    if(AttackList[currentAttackNumber].ForwardPrefab != null)
                    {
                        ForwardAttackCurrent = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].ForwardPrefab, ForwardSpawnPoint);
                        ForwardAttackCurrent.GetComponentInChildren<IStartAttack>().StartAttack(PlayerStatus, ForwardAttackDamage, DamageTags, DamageMask);
                    }

                    if(AttackList[currentAttackNumber].CenterPrefab  != null)
                    {
                        CenterAttackCurrent = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].CenterPrefab, CenterSpawnPoint);
                        CenterAttackCurrent.GetComponentInChildren<IStartAttack>().StartAttack(PlayerStatus, CenterAttackDamage, DamageTags, DamageMask);
                    }

                    if(AttackList[currentAttackNumber].ProjectilePrefab != null)
                    {
                        // Only use MultipleAttack with this
                        GameObject p = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].ProjectilePrefab, ProjectileSpawnPoint);
                        p.AddComponent<LookAtTarget>();
                        p.GetComponentInChildren<LookAtTarget>().target = ReticleTransform;
                        p.GetComponentInChildren<IBullet>().InitBullet(ProjectileAttackDamage, DamageTags, AttackList[currentAttackNumber].projectileForce);
                        ProjectileAttackCurrent = p;
                    }

                    if(AttackList[currentAttackNumber].GroundPrefab != null)
                    {
                        GroundAttackCurrent = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].GroundPrefab, GroundSpawnPoint);
                        GroundAttackCurrent.GetComponentInChildren<IStartAttack>().StartAttack(PlayerStatus, GroundAttackDamage, DamageTags, DamageMask);
                    }

                    if(AttackList[currentAttackNumber].LeftSwordPrefab != null)
                    {
                        LeftSwordAttackCurrent = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].LeftSwordPrefab, LeftSword);
                        LeftSwordAttackCurrent.GetComponentInChildren<IStartAttack>().StartAttack(PlayerStatus, LeftSwordAttackDamage, DamageTags, DamageMask);
                    }

                    if(AttackList[currentAttackNumber].RightSwordPrefab!= null)
                    {
                        RightSwordAttackCurrent = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].RightSwordPrefab, RightSword);
                        RightSwordAttackCurrent.GetComponentInChildren<IStartAttack>().StartAttack(PlayerStatus, RightSwordAttackDamage, DamageTags, DamageMask);
                    }

                    if (AttackList[currentAttackNumber].TwoHandPrefab != null)
                    {
                        TwoHandAttackCurrent = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].TwoHandPrefab, TwoHand);
                        TwoHandAttackCurrent.GetComponentInChildren<IStartAttack>().StartAttack(PlayerStatus, TwoHandAttackDamage, DamageTags, DamageMask);
                    }

                    if(Hypermode)
                    {
                        if (AttackList[currentAttackNumber].HypermodeForwardPrefab != null)
                        {
                            GameObject o = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].HypermodeForwardPrefab, ForwardSpawnPoint);
                            o.GetComponent<IStartAttack>().StartAttack(PlayerStatus, HypermodeForwardAttackDamage, DamageTags, DamageMask);
                        }

                        if (AttackList[currentAttackNumber].HypermodeCenterPrefab != null)
                        {
                            GameObject o = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].HypermodeCenterPrefab, CenterSpawnPoint);
                            o.GetComponent<IStartAttack>().StartAttack(PlayerStatus, HypermodeCenterAttackDamage, DamageTags, DamageMask);
                        }

                        if (AttackList[currentAttackNumber].HypermodeProjectilePrefab != null)
                        {
                            // Only use MultipleAttack with this
                            GameObject p = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].HypermodeProjectilePrefab, ProjectileSpawnPoint);
                            p.AddComponent<LookAtTarget>();
                            p.GetComponent<LookAtTarget>().target = ReticleTransform;
                            p.GetComponent<IBullet>().InitBullet(HypermodeProjectileAttackDamage, DamageTags, AttackList[currentAttackNumber].HypermodeProjectileForce);
                        }

                        if (AttackList[currentAttackNumber].HypermodeGroundPrefab != null)
                        {
                            GameObject o = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].HypermodeGroundPrefab, GroundSpawnPoint);
                            o.GetComponent<IStartAttack>().StartAttack(PlayerStatus, HypermodeGroundAttackDamage, DamageTags, DamageMask);
                        }

                        if (AttackList[currentAttackNumber].HypermodeLeftSwordPrefab != null)
                        {
                            GameObject o = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].HypermodeLeftSwordPrefab, LeftSword);
                            o.GetComponent<IStartAttack>().StartAttack(PlayerStatus, HypermodeLeftSwordAttackDamage, DamageTags, DamageMask);
                        }

                        if (AttackList[currentAttackNumber].HypermodeRightSwordPrefab != null)
                        {
                            GameObject o = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].HypermodeRightSwordPrefab, RightSword);
                            o.GetComponent<IStartAttack>().StartAttack(PlayerStatus, HypermodeRightSwordAttackDamage, DamageTags, DamageMask);
                        }

                        if (AttackList[currentAttackNumber].HypermodeTwoHandPrefab != null)
                        {
                            GameObject o = ObjectPoolManager.SpawnAObjectToParent(AttackList[currentAttackNumber].HypermodeTwoHandPrefab, TwoHand);
                            o.GetComponent<IStartAttack>().StartAttack(PlayerStatus, HypermodeTwoHandAttackDamage, DamageTags, DamageMask);
                        }
                    }

                    currentCooldown = AttackList[currentAttackNumber].cooldown;
                    effectPlayed = true;
                }

                currentCooldown -= Time.deltaTime;

                if (currentCooldown <= 0f)
                {
                    // Unlock other movements
                    PlayerLock.PlayerAttackLockAttack(false);
                    PlayerLock.PlayerAttackLockBlock(false);
                    PlayerLock.PlayerAttackLockCast(false);
                    PlayerLock.PlayerAttackLockDash(false);
                    PlayerLock.PlayerAttackLockMove(false);
                    PlayerLock.PlayerAttackCanInterrupt(true);
                    PlayerLock.PlayerAttackLockRotation(false);

                    effectPlayed = false;
                    useAttack = false;

                    ForwardAttackCurrent = null;
                    ProjectileAttackCurrent = null;
                    GroundAttackCurrent = null;
                    CenterAttackCurrent = null;
                    LeftSwordAttackCurrent = null;
                    RightSwordAttackCurrent = null;
                    TwoHandAttackCurrent = null;

                    if(OnAttackComplete != null)
                    {
                        OnAttackComplete.Invoke();
                    }

                    currentCooldown = 0;
                }
            }

            if(weaponSwitchTime > 0)
            {
                weaponSwitchTime -= Time.deltaTime;
            }
        }

        void OnInterrupt(Transform sender, int amount)
        {
            if(sender == transform.root)
            {
                if (AttackList[currentAttackNumber].CanBeInterrupted && !Hypermode)
                {
                    if(ProjectileAttackCurrent != null) { ProjectileAttackCurrent.SendMessage("OnInterrupt"); }
                    if (ForwardAttackCurrent != null) { ForwardAttackCurrent.SendMessage("OnInterrupt"); }
                    if (GroundAttackCurrent != null) { GroundAttackCurrent.SendMessage("OnInterrupt"); }
                    if (CenterAttackCurrent != null) { CenterAttackCurrent.SendMessage("OnInterrupt"); }
                    if (LeftSwordAttackCurrent != null) { LeftSwordAttackCurrent.SendMessage("OnInterrupt"); }
                    if (RightSwordAttackCurrent != null) { RightSwordAttackCurrent.SendMessage("OnInterrupt"); }
                    if (TwoHandAttackCurrent != null) { TwoHandAttackCurrent.SendMessage("OnInterrupt"); }
                }
            }
        }

        public void UseSingleAttack(InputAction.CallbackContext ctx)
        {
            if(ctx.performed)
            {
                if (PlayerLock.GetAttackLock() || useAttack)
                {
                    return;
                }

                if (!useAttack)
                {
                    // Lock other movements
                    CameraDirection.enabled = true;
                    ControllerVelocity.enabled = false;

                    PlayerLock.PlayerAttackLockAttack(true);
                    PlayerLock.PlayerAttackLockBlock(true);
                    PlayerLock.PlayerAttackLockCast(true);
                    PlayerLock.PlayerAttackLockDash(true);
                    PlayerLock.PlayerAttackCanInterrupt(AttackList[currentAttackNumber].CanBeInterrupted);
                    PlayerLock.PlayerAttackLockRotation(AttackList[currentAttackNumber].lockRotation);

                    if (AttackList[currentAttackNumber].lockMove)
                    {
                        PlayerLock.PlayerAttackLockMove(AttackList[currentAttackNumber].lockMove);
                    }

                    if (AttackList[currentAttackNumber].halfMove)
                    {
                        PlayerDash.HalfMovement(AttackList[currentAttackNumber].halfMove);
                    }

                    animator.Play(AttackList[currentAttackNumber].animationClip);
                    PlayerStatus.UseEndurance(AttackList[currentAttackNumber].enduranceCost);

                    if(OnUsedAttack != null)
                    {
                        OnUsedAttack.Invoke();
                    }

                    useAttack = true;
                }
            }
        }

        public void NextAttack()
        {
            if(weaponSwitchTime > 0)
            {
                return;
            }

            currentAttackNumber++;
            if (currentAttackNumber >= AttackList.Count)
            {
                currentAttackNumber = 0;
            }

            if (OnChangedAttack != null)
            {
                OnChangedAttack.Invoke(AttackList[currentAttackNumber].Icon);
            }

            canInterrupt = AttackList[currentAttackNumber].CanBeInterrupted;

            ForwardAttackDamage = AttackList[currentAttackNumber].ForwardAttackDamage;
            ForwardAttackDamage.myEntityStats = PlayerStatus;

            CenterAttackDamage = AttackList[currentAttackNumber].CenterAttackDamage;
            CenterAttackDamage.myEntityStats = PlayerStatus;

            ProjectileAttackDamage = AttackList[currentAttackNumber].ProjectileAttackDamage;
            ProjectileAttackDamage.myEntityStats = PlayerStatus;

            GroundAttackDamage = AttackList[currentAttackNumber].GroundAttackDamage;
            GroundAttackDamage.myEntityStats = PlayerStatus;

            LeftSwordAttackDamage = AttackList[currentAttackNumber].LeftSwordAttackDamage;
            LeftSwordAttackDamage.myEntityStats = PlayerStatus;

            RightSwordAttackDamage = AttackList[currentAttackNumber].RightSwordAttackDamage;
            RightSwordAttackDamage.myEntityStats = PlayerStatus;

            TwoHandAttackDamage = AttackList[currentAttackNumber].TwoHandAttackDamage;
            TwoHandAttackDamage.myEntityStats = PlayerStatus;

            HypermodeForwardAttackDamage = AttackList[currentAttackNumber].HypermodeForwardAttackDamage;
            HypermodeForwardAttackDamage.myEntityStats = PlayerStatus;

            HypermodeCenterAttackDamage = AttackList[currentAttackNumber].HypermodeCenterAttackDamage;
            HypermodeCenterAttackDamage.myEntityStats = PlayerStatus;

            HypermodeProjectileAttackDamage = AttackList[currentAttackNumber].HypermodeProjectileAttackDamage;
            HypermodeProjectileAttackDamage.myEntityStats = PlayerStatus;

            HypermodeGroundAttackDamage = AttackList[currentAttackNumber].HypermodeGroundAttackDamage;
            HypermodeGroundAttackDamage.myEntityStats = PlayerStatus;

            HypermodeLeftSwordAttackDamage = AttackList[currentAttackNumber].HypermodeLeftSwordAttackDamage;
            HypermodeLeftSwordAttackDamage.myEntityStats = PlayerStatus;

            HypermodeRightSwordAttackDamage = AttackList[currentAttackNumber].HypermodeRightSwordAttackDamage;
            HypermodeRightSwordAttackDamage.myEntityStats = PlayerStatus;

            HypermodeTwoHandAttackDamage = AttackList[currentAttackNumber].HypermodeTwoHandAttackDamage;
            HypermodeTwoHandAttackDamage.myEntityStats = PlayerStatus;

            weaponSwitchTime = 0.25f;
        }

        public void PrevAttack()
        {
            if (weaponSwitchTime > 0)
            {
                return;
            }
            currentAttackNumber--;

            if (currentAttackNumber < 0)
            {
                currentAttackNumber = AttackList.Count - 1;
            }

            if (OnChangedAttack != null)
            {
                OnChangedAttack.Invoke(AttackList[currentAttackNumber].Icon);
            }

            canInterrupt = AttackList[currentAttackNumber].CanBeInterrupted;

            ForwardAttackDamage = AttackList[currentAttackNumber].ForwardAttackDamage;
            ForwardAttackDamage.myEntityStats = PlayerStatus;

            CenterAttackDamage = AttackList[currentAttackNumber].CenterAttackDamage;
            CenterAttackDamage.myEntityStats = PlayerStatus;

            ProjectileAttackDamage = AttackList[currentAttackNumber].ProjectileAttackDamage;
            ProjectileAttackDamage.myEntityStats = PlayerStatus;

            GroundAttackDamage = AttackList[currentAttackNumber].GroundAttackDamage;
            GroundAttackDamage.myEntityStats = PlayerStatus;

            LeftSwordAttackDamage = AttackList[currentAttackNumber].LeftSwordAttackDamage;
            LeftSwordAttackDamage.myEntityStats = PlayerStatus;

            RightSwordAttackDamage = AttackList[currentAttackNumber].RightSwordAttackDamage;
            RightSwordAttackDamage.myEntityStats = PlayerStatus;

            TwoHandAttackDamage = AttackList[currentAttackNumber].TwoHandAttackDamage;
            TwoHandAttackDamage.myEntityStats = PlayerStatus;

            HypermodeForwardAttackDamage = AttackList[currentAttackNumber].HypermodeForwardAttackDamage;
            HypermodeForwardAttackDamage.myEntityStats = PlayerStatus;

            HypermodeCenterAttackDamage = AttackList[currentAttackNumber].HypermodeCenterAttackDamage;
            HypermodeCenterAttackDamage.myEntityStats = PlayerStatus;

            HypermodeProjectileAttackDamage = AttackList[currentAttackNumber].HypermodeProjectileAttackDamage;
            HypermodeProjectileAttackDamage.myEntityStats = PlayerStatus;

            HypermodeGroundAttackDamage = AttackList[currentAttackNumber].HypermodeGroundAttackDamage;
            HypermodeGroundAttackDamage.myEntityStats = PlayerStatus;

            HypermodeLeftSwordAttackDamage = AttackList[currentAttackNumber].HypermodeLeftSwordAttackDamage;
            HypermodeLeftSwordAttackDamage.myEntityStats = PlayerStatus;

            HypermodeRightSwordAttackDamage = AttackList[currentAttackNumber].HypermodeRightSwordAttackDamage;
            HypermodeRightSwordAttackDamage.myEntityStats = PlayerStatus;

            HypermodeTwoHandAttackDamage = AttackList[currentAttackNumber].HypermodeTwoHandAttackDamage;
            HypermodeTwoHandAttackDamage.myEntityStats = PlayerStatus;

            weaponSwitchTime = 0.25f;
        }
    }
}
