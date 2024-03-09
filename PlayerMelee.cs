using SolarFalcon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : MonoBehaviour
{
    public delegate void PlayerWeaponSwitch(int animatorLayerIndex);
    public PlayerWeaponSwitch OnPlayerWeaponSwitch;

    public InputAction MeleeAttack;
    public InputAction SelectDagger;
    public InputAction SelectScythe;
    public InputAction SelectScepter;
    public InputAction SelectWhip;

    bool lockMelee = false;
    Animator animator;

    PlayerLock PlayerLock;

    public string BaseLayerName = "Base Layer";
    int baseLayerIndex;
    public string DaggerLayerName = "Dagger Layer";
    int daggerLayerIndex;
    public string ScytheLayerName = "Scythe Layer";
    int scytheLayerIndex;
    public string ScepterLayerName = "Scepter Layer";
    int scepterLayerIndex;
    public string WhipLayerName = "Whip Layer";
    int whipLayerIndex;

    [SerializeField] MeshDisolveController DaggerDissolve, ScytheDissolve, ScepterDissolve; 


    [SerializeField]
    private WeaponUsage CurrentWeapon;
    private int currentAttack = 1;
    private float currentAttackWait, attackWait, attackOver;        // attackWait is changed when weapon is changed
    private const int MAX_ATTACKS = 3; 
    
    [SerializeField]
    private bool attacking = false, canAttack = true, canSwitch = true;

    public WeaponInfo DaggerInfo, ScytheInfo, ScepterInfo, WhipInfo;

    public ParticleSystem DaggerFirstEffect, DaggerSecondEffect, DaggerFinalEffect;
    public ParticleSystem ScytheFirstEffect, ScytheSecondEffect, ScytheFinalEffect;
    public ParticleSystem ScepterFirstEffect, ScepterSecondEffect, ScepterFinalEffect;
    public ParticleSystem WhipFirstEffect, WhipSecondEffect, WhipFinalEffect;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        daggerLayerIndex = animator.GetLayerIndex(DaggerLayerName);
        scytheLayerIndex = animator.GetLayerIndex(ScytheLayerName);
        scepterLayerIndex = animator.GetLayerIndex(ScepterLayerName);
        whipLayerIndex = animator.GetLayerIndex(WhipLayerName);

        PlayerLock = GetComponent<PlayerLock>();

        SwitchWeapon(WeaponUsage.Dagger);
    }

    private void OnEnable()
    {
        MeleeAttack.Enable();
        SelectDagger.Enable();
        SelectScythe.Enable();
        SelectScepter.Enable();
        SelectWhip.Enable();

        MeleeAttack.performed += AttackButtonPressed;
        SelectDagger.performed += DaggerSwitch;
        SelectScythe.performed += ScytheSwitch;
        SelectScepter.performed += ScepterSwitch;
        SelectWhip.performed += WhipSwitch;
    }

    private void OnDisable()
    {
        MeleeAttack.Disable();
        SelectDagger.Disable();
        SelectScythe.Disable();
        SelectScepter.Disable();
        SelectWhip.Disable();

        MeleeAttack.performed -= AttackButtonPressed;
        SelectDagger.performed -= DaggerSwitch;
        SelectScythe.performed -= ScytheSwitch;
        SelectScepter.performed -= ScepterSwitch;
        SelectWhip.performed -= WhipSwitch;
    }

    // Update is called once per frame
    void Update()
    {
        if(attacking)
        {
            currentAttackWait += Time.deltaTime;
            if(currentAttackWait >= attackWait)
            {
                canAttack = true;
            }

            if(currentAttackWait >= attackOver)
            {
                PlayerLock.PlayerMeleeCanInterrupt(true);
                PlayerLock.PlayerMeleeLockBlock(false);
                PlayerLock.PlayerMeleeLockCast(false);
                PlayerLock.PlayerMeleeLockDash(false);
                PlayerLock.PlayerMeleeLockMove(false);

                currentAttack = 1;
                canSwitch = true;
                canAttack = true;
                attacking = false;
            }
        }
    }

    void DaggerSwitch(InputAction.CallbackContext ctx)
    {
        if (!canSwitch)
        {
            return;
        }

        if (ctx.performed)
        {
            SwitchWeapon(WeaponUsage.Dagger);
        }
    }

    void ScytheSwitch(InputAction.CallbackContext ctx)
    {
        if (!canSwitch)
        {
            return;
        }

        if (ctx.performed)
        {
            SwitchWeapon(WeaponUsage.Scythe);
        }
    }

    void ScepterSwitch(InputAction.CallbackContext ctx)
    {
        if (!canSwitch)
        {
            return;
        }

        if (ctx.performed)
        {
            SwitchWeapon(WeaponUsage.Scepter);
        }
    }

    void WhipSwitch(InputAction.CallbackContext ctx)
    {
        if (!canSwitch)
        {
            return;
        }

        if (ctx.performed)
        {
            SwitchWeapon(WeaponUsage.Whip);
        }
    }

    void SwitchWeapon(WeaponUsage weaponUsage)
    {
        CurrentWeapon = weaponUsage;
        switch (weaponUsage)
        {
            case WeaponUsage.Unarmed:
                animator.SetLayerWeight(baseLayerIndex, 1);
                animator.SetLayerWeight(daggerLayerIndex, 0);
                animator.SetLayerWeight(scytheLayerIndex, 0);
                animator.SetLayerWeight(scepterLayerIndex, 0);
                animator.SetLayerWeight(whipLayerIndex, 0);
                DaggerDissolve.dissolve = true;
                ScytheDissolve.dissolve = true;
                ScepterDissolve.dissolve = true;
                
                if(OnPlayerWeaponSwitch != null) { OnPlayerWeaponSwitch.Invoke(baseLayerIndex); }
                break;

            case WeaponUsage.Dagger:
                animator.SetLayerWeight(daggerLayerIndex, 1);
                animator.SetLayerWeight(baseLayerIndex, 0);
                animator.SetLayerWeight(scytheLayerIndex, 0);
                animator.SetLayerWeight(scepterLayerIndex, 0);
                animator.SetLayerWeight(whipLayerIndex, 0);
                DaggerDissolve.dissolve = false;
                ScytheDissolve.dissolve = true;
                ScepterDissolve.dissolve = true;
                DaggerDissolve.resolve = true;

                if (OnPlayerWeaponSwitch != null) { OnPlayerWeaponSwitch.Invoke(daggerLayerIndex); }
                break;

            case WeaponUsage.Scythe:
                animator.SetLayerWeight(scytheLayerIndex, 1);
                animator.SetLayerWeight(daggerLayerIndex, 0);
                animator.SetLayerWeight(baseLayerIndex, 0);
                animator.SetLayerWeight(scepterLayerIndex, 0);
                animator.SetLayerWeight(whipLayerIndex, 0);
                DaggerDissolve.dissolve = true;
                ScytheDissolve.dissolve = false;
                ScepterDissolve.dissolve = true;
                ScytheDissolve.resolve = true;

                if (OnPlayerWeaponSwitch != null) { OnPlayerWeaponSwitch.Invoke(scytheLayerIndex); }
                break;

            case WeaponUsage.Scepter:
                animator.SetLayerWeight(scepterLayerIndex, 1);
                animator.SetLayerWeight(scytheLayerIndex, 0);
                animator.SetLayerWeight(daggerLayerIndex, 0);
                animator.SetLayerWeight(baseLayerIndex, 0);
                animator.SetLayerWeight(whipLayerIndex, 0);
                DaggerDissolve.dissolve = true;
                ScytheDissolve.dissolve = true;
                ScepterDissolve.dissolve = false;
                ScepterDissolve.resolve = true;

                if (OnPlayerWeaponSwitch != null) { OnPlayerWeaponSwitch.Invoke(scepterLayerIndex); }
                break;

            case WeaponUsage.Whip:
                animator.SetLayerWeight(whipLayerIndex, 1);
                animator.SetLayerWeight(scytheLayerIndex, 0);
                animator.SetLayerWeight(daggerLayerIndex, 0);
                animator.SetLayerWeight(baseLayerIndex, 0);
                animator.SetLayerWeight(scepterLayerIndex, 0);
                DaggerDissolve.dissolve = true;
                ScytheDissolve.dissolve = true;
                ScepterDissolve.dissolve = true;

                if (OnPlayerWeaponSwitch != null) { OnPlayerWeaponSwitch.Invoke(whipLayerIndex); }
                break;
        }
    }

    void AttackButtonPressed(InputAction.CallbackContext ctx)
    {
        if (PlayerLock.GetMeleeLock() || !canAttack)
        {
            return;
        }

        if (ctx.performed)
        {
            switch(CurrentWeapon)
            {
                case WeaponUsage.Dagger:
                    if(currentAttack == 1)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = DaggerInfo.First_to_Second_Time;
                        attackOver = DaggerInfo.First_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 2;
                        attacking = true;
                        animator.CrossFade("First Attack", 0.02f, daggerLayerIndex);
                        DaggerFirstEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    if (currentAttack == 2)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = DaggerInfo.Second_To_Final;
                        attackOver = DaggerInfo.Second_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 3;
                        attacking = true;
                        animator.CrossFade("Second Attack", 0.02f, daggerLayerIndex);
                        DaggerSecondEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    if (currentAttack == 3)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = DaggerInfo.Final_End_Time;
                        attackOver = DaggerInfo.Final_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 1;
                        attacking = true;
                        animator.CrossFade("Final Attack", 0.02f, daggerLayerIndex);
                        DaggerFinalEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    break;

                case WeaponUsage.Scythe:
                    if (currentAttack == 1)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = ScytheInfo.First_to_Second_Time;
                        attackOver = ScytheInfo.First_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 2;
                        attacking = true;
                        animator.CrossFade("First Attack", 0.02f, scytheLayerIndex);
                        ScytheFirstEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    if (currentAttack == 2)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = ScytheInfo.Second_To_Final;
                        attackOver = ScytheInfo.Second_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 3;
                        attacking = true;
                        animator.CrossFade("Second Attack", 0.02f, scytheLayerIndex);
                        ScytheSecondEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    if (currentAttack == 3)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = ScytheInfo.Final_End_Time;
                        attackOver = ScytheInfo.Final_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 1;
                        attacking = true;
                        animator.CrossFade("Final Attack", 0.02f, scytheLayerIndex);
                        ScytheFinalEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    break;

                case WeaponUsage.Scepter:
                    if (currentAttack == 1)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = ScepterInfo.First_to_Second_Time;
                        attackOver = ScepterInfo.First_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 2;
                        attacking = true;
                        animator.CrossFade("First Attack", 0.02f, scepterLayerIndex);
                        ScepterFirstEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    if (currentAttack == 2)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = ScepterInfo.Second_To_Final;
                        attackOver = ScepterInfo.Second_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 3;
                        attacking = true;
                        animator.CrossFade("Second Attack", 0.02f, scepterLayerIndex);
                        ScepterSecondEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    if (currentAttack == 3)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = ScepterInfo.Final_End_Time;
                        attackOver = ScepterInfo.Final_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 1;
                        attacking = true;
                        animator.CrossFade("Final Attack", 0.02f, scepterLayerIndex);
                        ScepterFinalEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    break;

                case WeaponUsage.Whip:
                    if (currentAttack == 1)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = WhipInfo.First_to_Second_Time;
                        attackOver = WhipInfo.First_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 2;
                        attacking = true;
                        animator.CrossFade("First Attack", 0.02f, whipLayerIndex);
                        WhipFirstEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    if (currentAttack == 2)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = WhipInfo.Second_To_Final;
                        attackOver = WhipInfo.Second_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 3;
                        attacking = true;
                        animator.CrossFade("Second Attack", 0.02f, whipLayerIndex);
                        WhipSecondEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    if (currentAttack == 3)
                    {
                        canAttack = false;
                        canSwitch = false;
                        attackWait = WhipInfo.Final_End_Time;
                        attackOver = WhipInfo.Final_End_Time;
                        currentAttackWait = 0;
                        currentAttack = 1;
                        attacking = true;
                        animator.CrossFade("Final Attack", 0.02f, whipLayerIndex);
                        WhipFinalEffect?.Play();
                        PlayerLock.PlayerMeleeCanInterrupt(true);
                        PlayerLock.PlayerMeleeLockBlock(true);
                        PlayerLock.PlayerMeleeLockCast(true);
                        PlayerLock.PlayerMeleeLockDash(true);
                        PlayerLock.PlayerMeleeLockMove(true);

                        break;
                    }

                    break;
            }
        }
    }
}

public enum WeaponUsage { Unarmed, Scythe, Dagger, Whip, Scepter}
