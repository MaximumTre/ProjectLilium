using SolarFalcon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayMaker;


public class AttackAnimation : StateMachineBehaviour
{
    public bool isPlayer = true;
    [Range(0.0f, 1.0f)]
    public float HitBoxStartPoint;
    public bool hitboxActive = false;

    [Range(0.0f, 1.0f)]
    public float MoveForwardTime;
    public float MoveForwardForce;
    public bool hasMovedForward = false;

    [Range(0.0f, 1.0f)]
    public float MoveBackwardTime;
    public float MoveBackwardForce;
    public bool hasMovedBackward = false;

    [Range (0.0f, 1.0f)]
    public float EnemyAttackOverTime;
    public bool enemyAttackOver = false;
    public bool enemyFinalAttack = false;

    public float attackRadius, attackRange, hitStun;

    public bool HeavyAttack;

    [SerializeField] bool lockHitBox, lockRapidHitbox;

    public bool knockdown;

    EnemyMelee EM;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hitboxActive = false;
        hasMovedForward = false;
        hasMovedBackward = false;
        lockHitBox = false;
        lockRapidHitbox = false;
        enemyAttackOver = false;


        if(!isPlayer && EM == null)
        {
            EM = animator.transform.GetComponent<EnemyMelee>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if(!lockHitBox && HitBoxStartPoint != 0)
        {
            if (!hitboxActive)
            {
                if (stateInfo.normalizedTime >= HitBoxStartPoint)
                {
                    EM.ActivateHitBox(attackRadius, attackRange, HeavyAttack, knockdown);

                    hitboxActive = true;
                }
            }
        }

        if (!isPlayer && !enemyAttackOver)
        {
            if (!enemyFinalAttack && stateInfo.normalizedTime >= EnemyAttackOverTime)
            {
                EM.AttackOver();
                enemyAttackOver = true;
            }

            if (enemyFinalAttack && stateInfo.normalizedTime >= EnemyAttackOverTime)
            {
                EM.FinalAttack();
                enemyAttackOver = true;
            }
        }

        if(!hasMovedForward && MoveForwardForce > 0)
        {
            if(stateInfo.normalizedTime >= MoveForwardTime)
            {
                if (isPlayer)
                    animator.transform.root.SendMessage("AddMomentum", animator.transform.forward * MoveForwardForce);
                hasMovedForward = true;
            }
        }

        if (!hasMovedBackward && MoveBackwardForce > 0)
        {
            if (stateInfo.normalizedTime >= MoveBackwardTime)
            {
                if (isPlayer)
                    animator.transform.root.SendMessage("AddMomentum", -animator.transform.forward * MoveBackwardForce);
                hasMovedBackward = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
