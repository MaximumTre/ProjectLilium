using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SolarFalcon
{
    public class HandGrabSpell : MonoBehaviour, IStartSpell
    {
        public Raycaster Raycaster;
        public EntityStatus Caster;
        public Damage SpellDamage;
        public Animator Animator;
        public List<string> DamageTags;
        public LayerMask DamageMask;
        public float radius, length, grabTime;
        public Transform GrabedEnemyTransform;
        public SkinnedMeshRenderer SkinnedMeshRenderer;
        public FX_Mover Mover;
        [SerializeField] bool start;

        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            Caster = caster;
            SpellDamage = spell.SpellDamage;
            DamageTags = damageTags;
            DamageMask = damageMask;
            start = true;
        }

        void Start()
        {
        
        }
        

        void Update()
        {
            if (start)
            {
                RaycastHit[] hits = Raycaster.CastBeam(radius, length, DamageMask);
                if(hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (DamageTags.Contains(hits[i].collider.tag))
                        {
                            if (hits[i].collider.GetComponent<DamageReceiver>())
                            {
                                // Grab
                                hits[i].collider.transform.root.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                                StartCoroutine("Grab", hits[i].collider.transform);
                                Mover.enabled = false;
                                start = false;
                                return;
                            }
                        }
                    }
                }
            }
            
        }

        IEnumerator Grab(Transform target)
        {
            DamageReceiver enemy = target.GetComponent<DamageReceiver>();
            enemy.NegativeStatus.NegativeEffect(NegativeEffectType.Shocked);
            target.root.transform.position = GrabedEnemyTransform.position;
            Animator.SetBool("Grab", true);
            yield return new WaitForSeconds(enemy.NegativeStatus.GetShockedTime() - 0.2f);
            SkinnedMeshRenderer.material.DOColor(Color.clear, "_TintColor", 1f);
            target.transform.root.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            target.transform.root.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
