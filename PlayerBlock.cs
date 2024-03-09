using UnityEngine;
using UnityEngine.InputSystem;
using CMF;
using System.Collections.Generic;

namespace SolarFalcon
{
    public class PlayerBlock : MonoBehaviour
    {
        public delegate void PlayerParry();
        public event PlayerParry OnParry;

        public static PlayerBlock Instance;

        [SerializeField] bool debug;
        public Animator animator;

        PlayerLock PlayerLock;


        [SerializeField] bool isBlocking = false, parryWindowOpen = false;

        public float ParryWindowTime = 0.3f, delayParryCheck = 0.4f;
        public ParticleSystem ParryWindowEffect, ParryShockwaveEffect;

        public LayerMask ParryLayerMask;
        public List<string> ParryList;
        public ParticleSystem BlockEffect;

        private void Awake()
        {
            PlayerLock = transform.root.GetComponent<PlayerLock>();

            if(Instance == null )
            {
                Instance = this;
            }
        }

        private void Start()
        {
            parryWindowOpen = true;
        }

        public bool IsBlocking() {  return isBlocking; }

        public void Damaged(int amount)
        {
            if(parryWindowOpen)
            {
                // Delay parry check for a bit
                Invoke("DelayedParryCheck", delayParryCheck);

                animator.SetTrigger("Parry");
                isBlocking = true;
                animator.SetBool("Block Held", false);
                parryWindowOpen = true;

                PlayerLock.PlayerBlockLockAttack(false);
                PlayerLock.PlayerBlockLockCast(false);
                PlayerLock.PlayerBlockLockMove(false);
                PlayerLock.PlayerBlockLockDash(false);

                BlockEffect.Play();
            }
        }

        void DelayedParryCheck()
        {
            bool parryEvent = false;

            Collider[] hits = Physics.OverlapSphere(transform.position, 2f, ParryLayerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                if (ParryList.Contains(hits[i].tag))
                {
                    hits[i].GetComponent<DamageReceiver>().NegativeStatus.NegativeEffect(NegativeEffectType.Parried);
                }

                if(!parryEvent)
                {
                    if(OnParry != null)
                    {
                        OnParry.Invoke();
                    }

                    parryEvent = true;
                }
            }
        }

        public void ParryReady()
        {
            // Parry
            if (debug)
            {
                Debug.Log("PARRY");
            }

            parryWindowOpen = true;
            Invoke("CloseParry", ParryWindowTime);
        }

        void CloseParry()
        {
            parryWindowOpen = false;
        }

        public void OnBlock(InputAction.CallbackContext ctx)
        {
            if(ctx.performed)
            {
                if(!PlayerLock.GetBlockLock() && !isBlocking)
                {
                    if (debug)
                    {
                        Debug.Log("Player is Blocking");
                    }

                    PlayerLock.PlayerBlockLockAttack(true);
                    PlayerLock.PlayerBlockLockCast(true);
                    PlayerLock.PlayerBlockLockMove(true);
                    PlayerLock.PlayerBlockLockDash(true);

                    animator.SetTrigger("Block");
                    animator.SetBool("Block Held", true);
                    isBlocking = true;
                }
            }

            if(ctx.canceled)
            {
                animator.SetBool("Block Held", false);
                isBlocking = false;
                parryWindowOpen = true;

                PlayerLock.PlayerBlockLockAttack(false);
                PlayerLock.PlayerBlockLockCast(false);
                PlayerLock.PlayerBlockLockMove(false);
                PlayerLock.PlayerBlockLockDash(false);
            }
        }
    }
}
