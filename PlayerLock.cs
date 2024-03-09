using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

namespace SolarFalcon
{
    public class PlayerLock : MonoBehaviour
    {
        PlayerStatus PlayerStatus;
        PlayerAnimationAttack PlayerAttack;
        PlayerMelee PlayerMelee;
        PlayerBlock PlayerBlock;
        PlayerCaster PlayerCaster;
        PlayerDash PlayerDash;
        public TurnTowardCameraDirection CameraDirection;
        public CameraController CameraController;
        float cachedCameraSpeed;

        [SerializeField]
        bool PlayerStatus_LockMove, PlayerStatus_LockAttack, PlayerStatus_LockCast, PlayerStatus_LockDash, PlayerStatus_LockBlock,
            PlayerStatus_CanInterrupt, PlayerStatus_LockRotation, PlayerStatus_LockMouseRotation, PlayerStatus_LockMelee;
        [SerializeField] bool PlayerAttack_LockMove, PlayerAttack_LockAttack, PlayerAttack_LockCast, PlayerAttack_LockDash, PlayerAttack_LockBlock, 
            PlayerAttack_CanInterrupt, PlayerAttack_LockRotation, PlayerAttack_LockMelee;
        [SerializeField] bool PlayerBlock_LockMove, PlayerBlock_LockAttack, PlayerBlock_LockCast, PlayerBlock_LockDash, PlayerBlock_LockBlock, 
            PlayerBlock_CanInterrupt, PlayerBlock_LockRotation, PlayerBlock_LockMelee;
        [SerializeField] bool PlayerCaster_LockMove, PlayerCaster_LockAttack, PlayerCaster_LockCast, PlayerCaster_LockDash, PlayerCaster_LockBlock, 
            PlayerCaster_CanInterrupt, PlayerCaster_LockRotation, PlayerCaster_LockMelee;
        [SerializeField] bool PlayerDash_LockMove, PlayerDash_LockAttack, PlayerDash_LockCast, PlayerDash_LockDash, PlayerDash_LockBlock, 
            PlayerDash_CanInterrupt, PlayerDash_LockRotation, PlayerDash_LockMouseRotation, PlayerDash_LockMelee;
        [SerializeField] bool PlayerHypermode_LockMove, PlayerHypermode_LockAttack, PlayerHypermode_LockCast, PlayerHypermode_LockDash, 
            PlayerHypermode_LockBlock, PlayerHypermode_CanInterrupt, PlayerHypermode_LockRotation, PlayerHypermode_LockMelee;
        [SerializeField]
        bool PlayerMelee_LockMove, PlayerMelee_LockAttack, PlayerMelee_LockCast, PlayerMelee_LockDash, PlayerMelee_LockBlock,
            PlayerMelee_CanInterrupt, PlayerMelee_LockRotation, PlayerMelee_LockMelee;

        private void Awake()
        {
            PlayerStatus = GetComponent<PlayerStatus>();
            PlayerAttack = GetComponentInChildren<PlayerAnimationAttack>();
            PlayerBlock = GetComponentInChildren<PlayerBlock>();
            PlayerCaster = GetComponentInChildren<PlayerCaster>();
            PlayerDash = GetComponent<PlayerDash>();
            PlayerMelee = GetComponentInChildren<PlayerMelee>();
        }

        private void Start()
        {
            PlayerStatus_LockMove = false;
            PlayerStatus_LockAttack = false;
            PlayerStatus_LockCast = false;
            PlayerStatus_LockDash = false;
            PlayerStatus_LockBlock = false;
            PlayerStatus_CanInterrupt = true;
            PlayerStatus_LockMouseRotation = false;

            PlayerAttack_LockMove = false;
            PlayerAttack_LockAttack = false;
            PlayerAttack_LockCast = false;
            PlayerAttack_LockDash = false;
            PlayerAttack_LockBlock = false;
            PlayerAttack_CanInterrupt = true;

            PlayerBlock_LockMove = false;
            PlayerBlock_LockAttack = false;
            PlayerBlock_LockCast = false;
            PlayerBlock_LockDash = false;
            PlayerBlock_LockBlock = false;
            PlayerBlock_CanInterrupt = true;

            PlayerCaster_LockMove = false;
            PlayerCaster_LockAttack = false;
            PlayerCaster_LockCast = false;
            PlayerCaster_LockDash = false;
            PlayerCaster_LockBlock = false;
            PlayerCaster_CanInterrupt = true;

            PlayerDash_LockMove = false;
            PlayerDash_LockAttack = false;
            PlayerDash_LockCast = false;
            PlayerDash_LockDash = false;
            PlayerDash_LockBlock = false;
            PlayerDash_CanInterrupt = true;
            PlayerDash_LockMouseRotation = false;

            PlayerHypermode_LockMove = false;
            PlayerHypermode_LockAttack = false;
            PlayerHypermode_LockCast = false;
            PlayerHypermode_LockDash = false;
            PlayerHypermode_LockBlock = false;
            PlayerHypermode_CanInterrupt = true;

            PlayerStatus_LockRotation = false;
            PlayerAttack_LockRotation = false;
            PlayerBlock_LockRotation = false;
            PlayerCaster_LockRotation = false;
            PlayerDash_LockRotation = false;
            PlayerHypermode_LockRotation = false;

            PlayerMelee_LockMove = false;
            PlayerMelee_LockAttack = false;
            PlayerMelee_LockCast = false;
            PlayerMelee_LockDash = false;
            PlayerMelee_LockBlock = false;
            PlayerMelee_CanInterrupt = true;

            cachedCameraSpeed = CameraController.cameraSpeed;
        }

        private void Update()
        {
            CheckLockRotation();
            CheckMove();
        }

        public bool GetMoveLock()
        {
            if (!PlayerStatus_LockMove && !PlayerAttack_LockMove && !PlayerBlock_LockMove && !PlayerCaster_LockMove 
                && !PlayerDash_LockMove && !PlayerHypermode_LockMove && !PlayerMelee_LockMove)
                return false;
            else 
                return true;
        }

        public bool GetAttackLock()
        {
            if(!PlayerStatus_LockAttack && !PlayerAttack_LockAttack && !PlayerBlock_LockAttack && 
                !PlayerCaster_LockAttack && !PlayerDash_LockAttack && !PlayerHypermode_LockAttack && !PlayerMelee_LockAttack)
                return false;
            else
                return true;
        }

        public bool GetMeleeLock()
        {
            if (!PlayerStatus_LockMelee && !PlayerAttack_LockMelee && !PlayerBlock_LockMelee &&
                !PlayerCaster_LockMelee && !PlayerDash_LockMelee && !PlayerHypermode_LockMelee && !PlayerMelee_LockMelee)
                return false;
            else
                return true;
        }

        public bool GetCastLock()
        {
            if (!PlayerStatus_LockCast && !PlayerAttack_LockCast && !PlayerBlock_LockCast && !PlayerCaster_LockCast && !PlayerDash_LockCast 
                && !PlayerHypermode_LockCast && !PlayerMelee_LockCast)
                return false;
            else
                return true;
        }

        public bool GetDashLock()
        {
            if (!PlayerStatus_LockDash && !PlayerAttack_LockDash && !PlayerBlock_LockDash && !PlayerCaster_LockDash && !PlayerDash_LockDash 
                && !PlayerHypermode_LockDash && !PlayerMelee_LockDash)
                return false;
            else
                return true;
        }

        public bool GetBlockLock()
        {
            if (!PlayerStatus_LockBlock && !PlayerAttack_LockBlock && !PlayerBlock_LockBlock && !PlayerCaster_LockBlock && !PlayerDash_LockBlock 
                && !PlayerHypermode_LockBlock && !PlayerMelee_LockBlock)
                return false;
            else
                return true;
        }

        public bool GetCanInterruptLock()
        {
            if (PlayerStatus_CanInterrupt && PlayerAttack_CanInterrupt && PlayerBlock_CanInterrupt && PlayerCaster_CanInterrupt && 
                PlayerDash_CanInterrupt && PlayerHypermode_CanInterrupt && PlayerMelee_CanInterrupt)
                return true;
            else
                return false;
        }

        void CheckLockRotation()
        {
            if (!PlayerStatus_LockRotation && !PlayerAttack_LockRotation && !PlayerBlock_LockRotation && !PlayerCaster_LockRotation 
                && !PlayerDash_LockRotation && !PlayerHypermode_LockRotation && !PlayerMelee_LockRotation)
            {
                CameraDirection.enabled = true; 
            }

            else { CameraDirection.enabled = false; }
        }

        void CheckMouseRotationLock()
        {
            if(!PlayerDash_LockMouseRotation && !PlayerStatus_LockMouseRotation)
            {
                CameraController.cameraSpeed = cachedCameraSpeed;
            }

            else
            {
                CameraController.cameraSpeed = 0;
            }
        }

        void CheckMove()
        {
            if(GetMoveLock())
            {
                PlayerDash.LockMove(true);
            }

            else
            {
                PlayerDash.LockMove(false);
            }
        }

        // PlayerStatus
        public void PlayerStatusLockMove(bool setting)
        {
            PlayerStatus_LockMove = setting;
            CheckMove();
        }

        public void PlayerStatusLockAttack(bool setting)
        {
            PlayerStatus_LockAttack = setting;
        }

        public void PlayerStatusLockCast(bool setting)
        {
            PlayerStatus_LockCast = setting;
        }

        public void PlayerStatusLockDash(bool setting)
        {
            PlayerStatus_LockDash = setting;
        }

        public void PlayerStatusLockBlock(bool setting)
        {
            PlayerStatus_LockBlock = setting;
        }

        public void PlayerStatusCanInterrupt(bool setting)
        {
            PlayerStatus_CanInterrupt = setting;
        }

        public void PlayerStatusLockRotation(bool setting)
        {
            PlayerStatus_LockRotation = setting;
            CheckLockRotation();
        }

        public void PlayerStatusLockMouseRotation(bool setting)
        {
            PlayerStatus_LockMouseRotation = setting;
            CheckMouseRotationLock();
        }

        // PlayerAttack
        public void PlayerAttackLockMove(bool setting)
        {
            PlayerAttack_LockMove = setting;
            CheckMove();
        }

        public void PlayerAttackLockAttack(bool setting)
        {
            PlayerAttack_LockAttack = setting;
        }

        public void PlayerAttackLockCast(bool setting)
        {
            PlayerAttack_LockCast = setting;
        }

        public void PlayerAttackLockDash(bool setting)
        {
            PlayerAttack_LockDash = setting;
        }

        public void PlayerAttackLockBlock(bool setting)
        {
            PlayerAttack_LockBlock = setting;
        }

        public void PlayerAttackCanInterrupt(bool setting)
        {
            PlayerAttack_CanInterrupt = setting;
        }

        public void PlayerAttackLockRotation(bool setting)
        {
            PlayerAttack_LockRotation = setting;
            CheckLockRotation();
        }

        // PlayerMelee
        public void PlayerMeleeLockMove(bool setting)
        {
            PlayerMelee_LockMove = setting;
            CheckMove();
        }

        public void PlayerMeleeLockAttack(bool setting)
        {
            PlayerMelee_LockAttack = setting;
        }

        public void PlayerMeleeLockCast(bool setting)
        {
            PlayerMelee_LockCast = setting;
        }

        public void PlayerMeleeLockDash(bool setting)
        {
            PlayerMelee_LockDash = setting;
        }

        public void PlayerMeleeLockBlock(bool setting)
        {
            PlayerMelee_LockBlock = setting;
        }

        public void PlayerMeleeCanInterrupt(bool setting)
        {
            PlayerMelee_CanInterrupt = setting;
        }

        public void PlayerMeleeLockRotation(bool setting)
        {
            PlayerMelee_LockRotation = setting;
            CheckLockRotation();
        }

        // PlayerBlock
        public void PlayerBlockLockMove(bool setting)
        {
            PlayerBlock_LockMove = setting;
            CheckMove();
        }

        public void PlayerBlockLockAttack(bool setting)
        {
            PlayerBlock_LockAttack = setting;
        }

        public void PlayerBlockLockCast(bool setting)
        {
            PlayerBlock_LockCast = setting;
        }

        public void PlayerBlockLockDash(bool setting)
        {
            PlayerBlock_LockDash = setting;
        }

        public void PlayerBlockLockBlock(bool setting)
        {
            PlayerBlock_LockBlock = setting;
        }

        public void PlayerBlockCanInterrupt(bool setting)
        {
            PlayerBlock_CanInterrupt = setting;
        }

        public void PlayerBlockLockRotation(bool setting)
        {
            PlayerBlock_LockRotation = setting;
            CheckLockRotation();
        }

        // PlayerCaster
        public void PlayerCasterLockMove(bool setting)
        {
            PlayerCaster_LockMove = setting;
            CheckMove();
        }

        public void PlayerCasterLockAttack(bool setting)
        {
            PlayerCaster_LockAttack = setting;
        }

        public void PlayerCasterLockCast(bool setting)
        {
            PlayerCaster_LockCast = setting;
        }

        public void PlayerCasterLockDash(bool setting)
        {
            PlayerCaster_LockDash = setting;
        }

        public void PlayerCasterLockBlock(bool setting)
        {
            PlayerCaster_LockBlock = setting;
        }

        public void PlayerCasteranInterrupt(bool setting)
        {
            PlayerCaster_CanInterrupt = setting;
        }

        public void PlayerCasterLockRotation(bool setting)
        {
            PlayerCaster_LockRotation = setting;
            CheckLockRotation();
        }

        // PlayerDash
        public void PlayerDashLockMove(bool setting)
        {
            PlayerDash_LockMove = setting;
            CheckMove();
        }

        public void PlayerDashLockAttack(bool setting)
        {
            PlayerDash_LockAttack = setting;
        }

        public void PlayerDashLockCast(bool setting)
        {
            PlayerDash_LockCast = setting;
        }

        public void PlayerDashLockDash(bool setting)
        {
            PlayerDash_LockDash = setting;
        }

        public void PlayerDashLockBlock(bool setting)
        {
            PlayerDash_LockBlock = setting;
        }

        public void PlayerDashCanInterrupt(bool setting)
        {
            PlayerDash_CanInterrupt = setting;
        }

        public void PlayerDashLockRotation(bool setting)
        {
            PlayerDash_LockRotation = setting;
            CheckLockRotation();
        }

        public void PlayerDashLockMouseRotation(bool setting)
        {
            PlayerDash_LockMouseRotation = setting;
            CheckMouseRotationLock();
        }

        // PlayerHypermode
        public void PlayerHypermodeLockMove(bool setting)
        {
            PlayerHypermode_LockMove = setting;
            CheckMove();
        }

        public void PlayerHypermodeLockAttack(bool setting)
        {
            PlayerHypermode_LockAttack = setting;
        }

        public void PlayerHypermodeLockCast(bool setting)
        {
            PlayerHypermode_LockCast = setting;
        }

        public void PlayerHypermodeLockDash(bool setting)
        {
            PlayerHypermode_LockDash = setting;
        }

        public void PlayerHypermodeLockBlock(bool setting)
        {
            PlayerHypermode_LockBlock = setting;
        }

        public void PlayerHypermodeCanInterrupt(bool setting)
        {
            PlayerHypermode_CanInterrupt = setting;
        }

        public void PlayerHypermodeLockRotation(bool setting)
        {
            PlayerHypermode_LockRotation = setting;
            CheckLockRotation();
        }
    }
}
