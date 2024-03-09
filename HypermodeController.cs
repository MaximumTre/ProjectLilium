
using UnityEngine;
using UnityEngine.VFX;

namespace SolarFalcon
{
    public class HypermodeController : MonoBehaviour
    {
        public delegate void HypermodeActive();
        public HypermodeActive OnHypermodeActive;

        public delegate void HypermodeOver();
        public HypermodeOver OnHypermodeOver;

        public ParticleSystem HypermodeTransformEffect;
        public VisualEffect[] HypermodeAura;

        public SkinnedMeshRenderer Eyes;

        MaterialPropertyBlock EyesPropBlock;

        [ColorUsage(false, true)]
        public Color NormalEyeColor, HypermodeEyeColor;

        public Animator Animator, HypermodeCameraAnimator;

        [SerializeField] bool testHyper, testReturnToNormal, canHypermode, skipAnimation;

        PlayerLock PlayerLock;
        PlayerDash PlayerDash;
        PlayerCaster PlayerCaster;
        PlayerAnimationAttack PlayerAttack;
        [SerializeField] GameObject HypermodeWillSeal;

        //public CameraController CamControl;

        public enum Mode { Normal, Hyper, Light, Dark, Dual };
        Mode CurrentMode = Mode.Normal;
        float cachedCamSpeed; 
        public float TransformTime = 10f, HypermodeMoveSpeed;
        public int HypermodeAttackLimit = 5;
        int HypermodeAttacksLeft;

        private void Awake()
        {
            PlayerLock = GetComponent<PlayerLock>();
            PlayerDash = GetComponent<PlayerDash>();
            PlayerAttack = transform.GetComponentInChildren<PlayerAnimationAttack>();
            PlayerCaster = transform.GetComponentInChildren<PlayerCaster>();
        }

        private void Start()
        {
            //cachedCamSpeed = CamControl.cameraSpeed;
            EyesPropBlock = new MaterialPropertyBlock();

            PlayerAttack.OnUsedAttack += OnAttack;
            PlayerAttack.OnAttackComplete += OnAttackComplete;

            PlayerCaster.OnSpellCast += OnSpellCast;

            foreach (VisualEffect vfx in HypermodeAura)
            {
                vfx.enabled = false;
            }
        }

        private void OnDestroy()
        {
            PlayerAttack.OnUsedAttack -= OnAttack;
            PlayerAttack.OnAttackComplete -= OnAttackComplete;
            PlayerCaster.OnSpellCast -= OnSpellCast;
        }

        private void Update()
        {
            if(testHyper)
            {
                if(CurrentMode != Mode.Normal) { testHyper = false; }

                UseHypermode(Mode.Hyper);
                testHyper = false;
            }
        }

        void UpdateStats(GameData CurrentSave)
        {
            HypermodeAttackLimit = CurrentSave.MaximumHypermodeAttacks;
        }

        public void GoHyper()
        {
            if (PlayerStatus.Instance.CurrentSparks <= 0)
            {
                canHypermode = false;
            }

            if (canHypermode && CurrentMode == Mode.Normal)
            {
                HypermodeAttacksLeft = HypermodeAttackLimit;
                UseHypermode(Mode.Hyper);
            }
        }

        public bool UseHeal()
        {
            if(PlayerStatus.Instance.CurrentSparks > 0)
            {
                PlayerStatus.Instance.UseSpark();

                return true;
            }

            return false;
        }

        void OnAttack()
        {
            if(CurrentMode == Mode.Hyper)
            {
                HypermodeAttacksLeft--;
            }
        }

        void OnSpellCast(Transform caster)
        {
            if(caster == transform)
            {
                if(CurrentMode == Mode.Hyper)
                {
                    HypermodeAttacksLeft--;
                }
            }
        }

        void OnAttackComplete()
        {
            if(CurrentMode == Mode.Hyper)
            {
                if (HypermodeAttacksLeft <= 0)
                {
                    EndHypermode();
                }
            }
        }

        void UseHypermode(Mode SwitchTo)
        {
            if (CurrentMode != Mode.Normal && !canHypermode) { return; }

            if(OnHypermodeActive != null)
            {
                OnHypermodeActive.Invoke();
            }

            PlayerStatus.Instance.UseSpark();


            switch (SwitchTo)
            {
                case Mode.Hyper:
                    if(!skipAnimation)
                    {
                        Animator.Play("Start Hypermode");
                        Animator.SetBool("Hypermode", true);
                        Invoke("StartEffect", 0.89f);
                        Invoke("SwitchHypermodeMats", 3);
                        Invoke("PlayHypermodeAnim", 4.5f);
                        Invoke("Unlock", 8.5f);

                        LockAll(true);
                        PlayerLock.PlayerHypermodeLockRotation(true);
                        PlayerLock.PlayerHypermodeCanInterrupt(false);
                        PlayerDash.EnableHypermode(true);
                        HypermodeWillSeal.SetActive(true);

                        foreach (VisualEffect vfx in HypermodeAura)
                        {
                            if (!vfx.enabled)
                            {
                                vfx.enabled = true;
                            }

                            vfx.SendEvent("OnPlay");
                        }

                        CurrentMode = Mode.Hyper;
                        canHypermode = false;
                    }

                    if(skipAnimation) 
                    {
                        SwitchHypermodeMats();

                        PlayerLock.PlayerHypermodeCanInterrupt(false);
                        PlayerDash.EnableHypermode(true);
                        HypermodeWillSeal.SetActive(true);

                        foreach (VisualEffect vfx in HypermodeAura)
                        {
                            if (!vfx.enabled)
                            {
                                vfx.enabled = true;
                            }

                            vfx.SendEvent("OnPlay");
                        }

                        CurrentMode = Mode.Hyper;
                        canHypermode = false;
                    }
                    break;
            }

        }

        void EndHypermode()
        {
            if(OnHypermodeOver != null)
            {
                OnHypermodeOver.Invoke();
            }

            // Needs to be an event or action
            transform.BroadcastMessage("OnInterrupt", SendMessageOptions.DontRequireReceiver);
            PlayerDash.EndDashAnimation();
            LockAll(true);
            PlayerLock.PlayerHypermodeLockRotation(true);
            Animator.SetBool("Hypermode", false);


            PlayerLock.PlayerHypermodeCanInterrupt(true);
            PlayerDash.EnableHypermode(false);
            HypermodeWillSeal.SetActive(false);

            foreach (VisualEffect vfx in HypermodeAura)
            {
                vfx.SendEvent("OnStop");
            }

            StartEffect();
            Invoke("SwitchNormalModeMats", 3);
            Invoke("Unlock", 4f);
            CurrentMode = Mode.Normal;
        }

        void LockAll(bool setting)
        {
            PlayerLock.PlayerHypermodeLockRotation(true);

            PlayerLock.PlayerHypermodeLockAttack(setting);
            PlayerLock.PlayerHypermodeLockBlock(setting);
            PlayerLock.PlayerHypermodeLockCast(setting);
            PlayerLock.PlayerHypermodeLockDash(setting);
            PlayerLock.PlayerHypermodeLockMove(setting);
        }

        void Unlock()
        {
            LockAll(false);
            PlayerLock.PlayerHypermodeLockRotation(false);
            //CamControl.ResetCamera = false;
        }

        void StartEffect()
        {
            HypermodeTransformEffect.Play();
        }

        void PlayHypermodeAnim()
        {
            Animator.Play("Hypermode");
            HypermodeCameraAnimator.Play("Hypermode Activate");
        }

        void SwitchHypermodeMats()
        {
            EyesPropBlock.SetColor("_EmissionColor", HypermodeEyeColor);
            Eyes.SetPropertyBlock(EyesPropBlock);
        }

        void SwitchNormalModeMats()
        {
            EyesPropBlock.SetColor("_EmissionColor", NormalEyeColor);
            Eyes.SetPropertyBlock(EyesPropBlock);
        }
    }
}
