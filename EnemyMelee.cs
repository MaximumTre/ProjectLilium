using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayMaker;

namespace SolarFalcon
{
    public class EnemyMelee : MonoBehaviour
    {
        Animator animator;
        [SerializeField] bool lockMelee = false;
        public bool meleeLocked;
        [Tooltip("How much time to hold all movements after a melee attack")]
        public float meleeOverTime;

        public Transform MeleeRayPoint;
        public PlayMakerFSM FSM;
        public LayerMask DamageLayer;
        public List<string> DamageTags;

        public Damage WeaponDamage;

        [SerializeField] float gizmoRange, gizmoRadius;

        public float knockdownChance = 0.3f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }


        public void MeleeAttack()
        {
            if (!lockMelee)
            {
                animator.SetTrigger("Melee");
            }
        }

        public void HeavyMeleeAttack()
        {
            if (!lockMelee)
            {
                animator.SetTrigger("HeavyMelee");

            }
        }

        public void AttackOver()
        {
            animator.SetBool("HeavyMelee", false);
            animator.SetBool("Melee", false);
            FSM.SendEvent("Next Attack");
        }

        public void FinalAttack()
        {
            animator.SetBool("HeavyMelee", false);
            animator.SetBool("Melee", false);
            FSM.SendEvent("Final Attack");
        }

        public void ActivateHitBox(float radius, float maxDistance, bool heavy, bool isKnockdown = false)
        {
            RaycastHit[] hits;
            int result;
            hits = Physics.SphereCastAll(MeleeRayPoint.position, radius, transform.forward, maxDistance, DamageLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                if (DamageTags.Contains(hits[i].collider.tag))
                {
                    result = CalculateDamage.instance.TakeDamage(WeaponDamage, hits[i].collider.GetComponent<DamageReceiver>().MyStatus);

                    if(isKnockdown)
                    {
                        // Check knockdown change
                        float kChance = Random.Range(0f, 1f);
                        if(kChance < knockdownChance)
                        {
                            hits[i].collider.GetComponent<DamageReceiver>().MyStatus.TakeDamage(result, transform);
                        }

                        else
                        { 
                            hits[i].collider.GetComponent<DamageReceiver>().MyStatus.TakeDamage(result, transform);
                        }
                    }

                    else
                    {
                        hits[i].collider.GetComponent<DamageReceiver>().MyStatus.TakeDamage(result, transform);
                    }
                }
            }
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public void LockMelee(bool setting)
        {
            lockMelee = setting;
            meleeLocked = lockMelee;

            if(setting)
            {
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 end = MeleeRayPoint.position;
            end.z += gizmoRange;
            Gizmos.DrawLine(MeleeRayPoint.position, end);
            Gizmos.DrawWireSphere(end, gizmoRadius);
        }
    }
}
