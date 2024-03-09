using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

namespace SolarFalcon
{
    public class VacuumSpell : MonoBehaviour
    {
        public Raycaster Raycaster;
        public List<string> DamageTags;
        public LayerMask DamageMask;
        Spell Spell;
        public float radius, holdTime;
        public List<DamageReceiver> CapturedTargets;
        public Transform PullPoint;
        bool start = false;

        // Update is called once per frame
        void Update()
        {
            if(start)
            {
                RaycastHit[] hits = Raycaster.CastBeam(radius, 0, DamageMask);
                foreach (RaycastHit hit in hits)
                {
                    if (DamageTags.Contains(hit.collider.tag))
                    {
                        if (!CapturedTargets.Contains(hit.collider.GetComponent<DamageReceiver>()))
                        {
                            CapturedTargets.Add(hit.collider.GetComponent<DamageReceiver>());
                            hit.collider.transform.root.GetComponent<NavMeshAgent>().enabled = false;
                        }
                    }
                }

                foreach (DamageReceiver r in CapturedTargets)
                {
                    r.transform.root.position = Vector3.Lerp(r.transform.root.position, PullPoint.position, Time.deltaTime);
                }

                holdTime -= Time.deltaTime;
                if(holdTime <= 0 )
                {
                    foreach (DamageReceiver dr in CapturedTargets)
                    {
                        dr.transform.root.GetComponent<NavMeshAgent>().enabled = true;
                    }
                    start = false;
                }
            }
        }
    }
}
