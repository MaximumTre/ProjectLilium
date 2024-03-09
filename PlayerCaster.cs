using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using CMF;
using UnityEngine.InputSystem.HID;
using UnityEngine.Animations.Rigging;

namespace SolarFalcon
{
    public class PlayerCaster : MonoBehaviour
    {
        public delegate void CastedSpell(Transform caster);
        public CastedSpell OnSpellCast;
        Animator animator;
        public List<Spell> Spells;
        [SerializeField] List<Seals> CurrentSequence;
        [SerializeField] Spell CurrentSpell;
        [SerializeField] bool isCasting = false;
        public bool canCast;

        PlayerLock PlayerLock;
        HypermodeController HypermodeController;

        [SerializeField] PlayerDash PlayerDash;
        [SerializeField] PlayerAnimationAttack PlayerAttack;
        [SerializeField] PlayerBlock PlayerBlock;
        [SerializeField] Transform WindTransform;
        [SerializeField] PlayerStatus PlayerStatus;
        [SerializeField] CameraAimRay CameraAimRay;
        [SerializeField] Rig NeckRig;
        float neckRigCachedWeight;

        public Transform ForwardPoint, ForwardGroundPoint, CastAtPoint, BeamPoint, ReticleTransform, SelfTransform;

        public List<string> DamageTags;
        List<string> SelfTag;
        public LayerMask DamageMask;
        bool Hypermode = false, checkEndCasting = false;

        public GameObject NorthSealPrefab, SouthSealPrefab, EastSealPrefab, WestSealPrefab;
        public Transform SealTransform;
        public List<GameObject> NorthSeals, SouthSeals, EastSeals, WestSeals;
        public int initialSetting = 4;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            PlayerLock = transform.root.GetComponent<PlayerLock>();
            HypermodeController = transform.root.GetComponent<HypermodeController>();
        }

        private void Start()
        {
            SelfTag = new List<string>();
            SelfTag.Add("Player");

            HypermodeController.OnHypermodeActive += OnHypermodeActive;
            HypermodeController.OnHypermodeOver += OnHypermodeOver;

            // Create Effect Pools
            for (int i = 0; i < initialSetting; i++)
            {
                GameObject n = Instantiate(NorthSealPrefab, SealTransform);
                GameObject s = Instantiate(SouthSealPrefab, SealTransform);
                GameObject e = Instantiate(EastSealPrefab, SealTransform);
                GameObject w = Instantiate(WestSealPrefab, SealTransform);

                n.SetActive(false);
                s.SetActive(false);
                e.SetActive(false);
                w.SetActive(false);

                NorthSeals.Add(n);
                SouthSeals.Add(s);
                WestSeals.Add(w);
                EastSeals.Add(e);
            }
        }

        private void OnDestroy()
        {
            HypermodeController.OnHypermodeActive -= OnHypermodeActive;
            HypermodeController.OnHypermodeOver -= OnHypermodeOver;
        }

        private void Update()
        {
            if(checkEndCasting)
            {
                if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    EndCasting();
                    checkEndCasting = false;
                }
            }
        }

        private void LateUpdate()
        {
            BeamPoint.transform.LookAt(ReticleTransform);
        }

        void OnHypermodeActive()
        {
            Hypermode = true;
        }

        void OnHypermodeOver()
        {
            Hypermode = false;
        }

        public void BeginCasting(InputAction.CallbackContext ctx)
        {
            if(PlayerLock.GetCastLock())
            {
                return;
            }

            if(ctx.performed)
            {
                if(!PlayerLock.GetCastLock() && !isCasting)
                {
                    // Lock other movements
                    PlayerLock.PlayerCasterLockAttack(true);
                    PlayerLock.PlayerCasterLockBlock(true);
                    PlayerLock.PlayerCasterLockDash(true);
                    PlayerLock.PlayerCasterLockMove(true);

                    CameraAimRay.on = true;

                    WindTransform.localRotation = Quaternion.Euler(0, 0, 0);

                    // Play Casting animation
                    neckRigCachedWeight = NeckRig.weight;
                    NeckRig.weight = 0;
                    animator.SetBool("Casting", true);
                    //PlayerRanged.RigControl(false);
                    isCasting = true;
                }
            }
        }

        public void ReleaseCasting(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                if (!PlayerLock.GetCastLock())
                {
                    if (isCasting)
                    {
                        bool found = false;
                        // Check spell sequence
                        for (int i = 0; i < Spells.Count; i++)
                        {
                            if (CurrentSequence.SequenceEqual(Spells[i].Sequence))
                            {
                                CurrentSpell = Spells[i];
                                if(CurrentSpell.hypermodeOnly && Hypermode) // Only use this spell during Hypermode
                                {
                                    CurrentSequence.Clear();

                                    Invoke("DelayedSequenceCheck", 0.1f);

                                    found = true;
                                }

                                if(CurrentSpell.hypermodeOnly && !Hypermode)
                                {
                                    found = false;
                                }

                                if(!CurrentSpell.hypermodeOnly && !Hypermode)
                                {
                                    CurrentSequence.Clear();

                                    Invoke("DelayedSequenceCheck", 0.1f);

                                    found = true;
                                }

                                if (!CurrentSpell.hypermodeOnly && Hypermode)
                                {
                                    CurrentSequence.Clear();

                                    Invoke("DelayedSequenceCheck", 0.1f);

                                    found = true;
                                }
                            }
                        }

                        if(found) 
                        { 
                            return; 
                        }

                        else 
                        { 
                            EndCasting();
                            CurrentSequence.Clear();
                        }
                    }

                    else
                    {
                        CurrentSequence.Clear();
                        EndCasting();
                    }
                }
            }
        }

        void DelayedSequenceCheck()
        {
            animator.StopPlayback();
            WindTransform.localRotation = Quaternion.Euler(0, 180, 0);

            switch (CurrentSpell.CastType)
            {
                case CastType.Snap:
                    animator.CrossFade("Snap Cast", 0);
                    //PlayerRanged.RigControl(false);
                    break;

                case CastType.TouchGround:
                    animator.CrossFade("Touch Ground Cast", 0);
                    //PlayerRanged.RigControl(false);
                    break;

                case CastType.ForwardProjectile:
                    animator.CrossFade("Forward Projectile Cast", 0);
                    //PlayerRanged.RigControl(false);
                    break;

                case CastType.AreaBurst:
                    animator.CrossFade("Area Burst Cast", 0);
                    //PlayerRanged.RigControl(false);
                    break;

                case CastType.LongBeam:
                    animator.CrossFade("Long Beam", 0);
                    break;

            }
        }

        void EndCasting()
        {
            if(isCasting && !PlayerLock.GetCastLock())
            {
                PlayerLock.PlayerCasterLockAttack(false);
                PlayerLock.PlayerCasterLockBlock(false);
                PlayerLock.PlayerCasterLockDash(false);
                PlayerLock.PlayerCasterLockMove(false);

                //PlayerRanged.RigControl(true);
                animator.SetBool("Casting", false);
                animator.SetBool("North", false);
                animator.SetBool("South", false);
                animator.SetBool("East", false);
                animator.SetBool("West", false);
                animator.CrossFade("Idle", 0.3f);
                isCasting = false;
                CameraAimRay.on = false;
                NeckRig.weight = neckRigCachedWeight;
                //PlayerRanged.AimTracking(false);
                WindTransform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }

        void CastSpell()
        {
            // Instantiate Prefab at location
            GameObject go;
            RaycastHit hit;

            switch(CurrentSpell.SpellType)
            {
                case SpellType.ForwardGround:
                    Physics.Raycast(ForwardGroundPoint.position, -ForwardGroundPoint.up, out hit);
                    if(hit.collider.CompareTag("Environment"))
                    {
                        go = Instantiate(CurrentSpell.SpellPrefab, hit.point, ForwardGroundPoint.rotation);
                        go.GetComponent<IStartSpell>().StartSpell(PlayerStatus, CurrentSpell, DamageTags, DamageMask);
                    }


                    if (OnSpellCast != null)
                    {
                        OnSpellCast.Invoke(transform);
                    }

                    break;

                case SpellType.ForwardDamage:
                    go = Instantiate(CurrentSpell.SpellPrefab, ForwardPoint.position, ForwardPoint.rotation);
                    go.GetComponent<IStartSpell>().StartSpell(PlayerStatus, CurrentSpell, DamageTags, DamageMask);

                    if (OnSpellCast != null)
                    {
                        OnSpellCast.Invoke(transform);
                    }

                    break;

                case SpellType.Projectile:
                    go = Instantiate(CurrentSpell.SpellPrefab, ForwardPoint.position, ForwardPoint.rotation);
                    go.transform.LookAt(PlayerAttack.GetReticleTransform());
                    go.GetComponent<IStartSpell>().StartSpell(PlayerStatus, CurrentSpell, DamageTags, DamageMask);
                    //PlayerRanged.AimTracking(true);

                    if (OnSpellCast != null)
                    {
                        OnSpellCast.Invoke(transform);
                    }

                    break;

                case SpellType.CastAtPoint:
                    if(CurrentSpell.useNormal)
                    {
                        go = Instantiate(CurrentSpell.SpellPrefab, CastAtPoint.position, CastAtPoint.rotation);
                        go.GetComponent<IStartSpell>().StartSpell(PlayerStatus, CurrentSpell, DamageTags, DamageMask);
                    }

                    else
                    {
                        go = Instantiate(CurrentSpell.SpellPrefab, CastAtPoint.position, Quaternion.identity);
                        go.GetComponent<IStartSpell>().StartSpell(PlayerStatus, CurrentSpell, DamageTags, DamageMask);
                    }

                    if (OnSpellCast != null)
                    {
                        OnSpellCast.Invoke(transform);
                    }

                    break;

                case SpellType.Beam:
                    go = Instantiate(CurrentSpell.SpellPrefab, BeamPoint);
                    go.GetComponent<IStartSpell>().StartSpell(PlayerStatus, CurrentSpell, DamageTags, DamageMask);
                    //PlayerRanged.AimTracking(true);

                    if(OnSpellCast != null)
                    {
                        OnSpellCast.Invoke(transform);
                    }

                    break;

                case SpellType.Heal:
                    if(HypermodeController.UseHeal())
                    {
                        Physics.Raycast(transform.position + Vector3.up, -transform.up, out hit);
                        if (hit.collider.CompareTag("Environment"))
                        {
                            go = Instantiate(CurrentSpell.SpellPrefab, hit.point, Quaternion.identity);
                            go.GetComponent<IStartSpell>().StartSpell(PlayerStatus, CurrentSpell, SelfTag, DamageMask);
                        }

                        if (OnSpellCast != null)
                        {
                            OnSpellCast.Invoke(transform);
                        }
                    }
                    break;
            }

            checkEndCasting = true;
        }

        public void InterruptCasting()
        {
            if(isCasting)
            {
                EndCasting();
                CurrentSequence.Clear();
                isCasting = false;                
            }
        }

        public void NorthSeal(InputAction.CallbackContext ctx)
        {
            if(ctx.performed)
            {
                if(!PlayerLock.GetCastLock())
                {
                    if(isCasting)
                    {
                        animator.SetTrigger("North");
                        CurrentSequence.Add(Seals.North);

                        // Get First Seal
                        for (int i = 0; i < NorthSeals.Count; i++)
                        {
                            if (!NorthSeals[i].activeInHierarchy)
                            {
                                NorthSeals[i].SetActive(true);
                                return;
                            }
                        }

                        // if we reach this point, make a new seal
                        GameObject n = Instantiate(NorthSealPrefab, SealTransform);

                        NorthSeals.Add(n);
                    }
                }
            }
        }

        public void SouthSeal(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                if (!PlayerLock.GetCastLock())
                {
                    if (isCasting)
                    {
                        animator.SetTrigger("South");
                        CurrentSequence.Add(Seals.South);

                        // Get First Seal
                        for (int i = 0; i < SouthSeals.Count; i++)
                        {
                            if (!SouthSeals[i].activeInHierarchy)
                            {
                                SouthSeals[i].SetActive(true);
                                return;
                            }
                        }

                        // if we reach this point, make a new seal
                        GameObject n = Instantiate(SouthSealPrefab, SealTransform);

                        SouthSeals.Add(n);
                    }
                }
            }
        }

        public void EastSeal(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                if (!PlayerLock.GetCastLock())
                {
                    if (isCasting)
                    {
                        animator.SetTrigger("East");
                        CurrentSequence.Add(Seals.East);

                        // Get First Seal
                        for (int i = 0; i < EastSeals.Count; i++)
                        {
                            if (!EastSeals[i].activeInHierarchy)
                            {
                                EastSeals[i].SetActive(true);
                                return;
                            }
                        }

                        // if we reach this point, make a new seal
                        GameObject n = Instantiate(EastSealPrefab, SealTransform);

                        EastSeals.Add(n);
                    }
                }
            }
        }

        public void WestSeal(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                if (!PlayerLock.GetCastLock())
                {
                    if (isCasting)
                    {
                        animator.SetTrigger("West");
                        CurrentSequence.Add(Seals.West);

                        // Get First Seal
                        for (int i = 0; i < WestSeals.Count; i++)
                        {
                            if (!WestSeals[i].activeInHierarchy)
                            {
                                WestSeals[i].SetActive(true);
                                return;
                            }
                        }

                        // if we reach this point, make a new seal
                        GameObject n = Instantiate(WestSealPrefab, SealTransform);

                        WestSeals.Add(n);
                    }
                }
            }
        }
    }
}
