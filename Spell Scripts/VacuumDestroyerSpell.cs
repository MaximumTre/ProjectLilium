using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SolarFalcon
{
    public class VacuumDestroyerSpell : MonoBehaviour, IStartSpell
    {
        public Raycaster Raycaster;
        public List<DamageReceiver> CapturedTargets;
        public float radius;
        public Transform PullPoint;
        public LayerMask DamageMask;
        public List<string> DamageTags;

        bool start = false;

        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            DamageMask = damageMask;
            start = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(CapturedTargets.Contains(other.GetComponent<DamageReceiver>()))
            {
                other.GetComponent<DamageReceiver>().MyStatus.TakeDamage(-1000000000);
                CapturedTargets.Remove(other.GetComponent<DamageReceiver>());
            }
        }

        private void Update()
        {
            if(start)
            {
                RaycastHit[] hits = Raycaster.CastBeam(radius, 0, DamageMask);
                foreach (RaycastHit hit in hits)
                {
                    if(DamageTags.Contains(hit.collider.tag))
                    {
                        if (!CapturedTargets.Contains(hit.collider.GetComponent<DamageReceiver>()))
                        {
                            CapturedTargets.Add(hit.collider.GetComponent<DamageReceiver>()); 
                            hit.collider.GetComponent<DamageReceiver>().NegativeStatus.NegativeEffect(NegativeEffectType.Shocked);
                            hit.collider.transform.root.GetComponent<Rigidbody>().useGravity = false;
                            hit.collider.transform.root.GetComponent<NavMeshAgent>().enabled = false;
                            hit.collider.transform.root.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                            hit.collider.transform.root.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.value, Random.value, Random.value) * 10f, ForceMode.Impulse);
                        }
                    }
                }

                foreach(DamageReceiver r in CapturedTargets)
                {
                    r.transform.root.position = Vector3.Lerp(r.transform.root.position, PullPoint.position, Time.deltaTime);
                }
            }
        }
    }
}
