using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class Detection : MonoBehaviour
    {
        [SerializeField] bool debug;
        public Collider DetectionCollider;
        public PlayMakerFSM FSM;

        public List<string> DetectTags;
        [Tooltip("If a target with this tag enters, then the AI will immediately begin attacking this target.")]
        public List<string> OverrideTags;

        public EntityStatus CurrentTarget;
        [SerializeField] bool isOveride = false, hasTarget = false, ResetSent = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!hasTarget)
            {
                if(DetectTags.Contains(other.tag))
                {                    
                    if (other.transform.root.GetComponent<EntityStatus>().Alive)
                    {
                        if(debug)
                        {
                            Debug.Log("Target spotted!");
                        }

                        CurrentTarget = other.GetComponent<EntityStatus>();
                        isOveride = false;
                        CurrentTarget.OnMyDeath += OnTargetDeath;
                        FSM.FsmVariables.GetFsmGameObject("Current Target").SafeAssign(CurrentTarget.gameObject);
                        FSM.SendEvent("Target Found");
                        hasTarget = true;
                    }
                }

                else if(OverrideTags.Contains(other.tag))
                {
                    if (other.transform.root.GetComponent<EntityStatus>().Alive)
                    {
                        if (debug)
                        {
                            Debug.Log("Overide Target spotted!");
                        }
                        CurrentTarget = other.GetComponent<EntityStatus>();
                        isOveride = true;
                        CurrentTarget.OnMyDeath += OnTargetDeath;
                        FSM.FsmVariables.GetFsmGameObject("Current Target").SafeAssign(CurrentTarget.gameObject);
                        FSM.SendEvent("Target Found");
                        hasTarget = true;
                    }
                }
            }

            else if(hasTarget && !isOveride)
            {
                if (OverrideTags.Contains(other.tag))
                {
                    if (other.transform.root.GetComponent<EntityStatus>().Alive)
                    {
                        if (debug)
                        {
                            Debug.Log("Overide Target spotted!");
                        }
                        CurrentTarget = other.GetComponent<EntityStatus>();
                        isOveride = true;
                        CurrentTarget.OnMyDeath += OnTargetDeath;
                        FSM.FsmVariables.GetFsmGameObject("Current Target").SafeAssign(CurrentTarget.gameObject);
                        FSM.SendEvent("Target Found");
                        hasTarget = true;
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (hasTarget && !isOveride)
            {
                if (OverrideTags.Contains(other.tag))
                {
                    if (other.transform.root.GetComponent<EntityStatus>().Alive)
                    {
                        if (debug)
                        {
                            Debug.Log("Overide Target spotted!");
                        }
                        CurrentTarget = other.GetComponent<EntityStatus>();
                        isOveride = true;
                        CurrentTarget.OnMyDeath += OnTargetDeath;
                        FSM.FsmVariables.GetFsmGameObject("Current Target").SafeAssign(CurrentTarget.gameObject);
                        FSM.SendEvent("Target Found");
                        hasTarget = true;
                    }
                }
            }
        }

        private void Update()
        {
            if(CurrentTarget == null)
            {
                if(hasTarget)
                {
                    if(!ResetSent)
                    {
                        FSM.SendEvent("Target Lost");
                    }

                    if(ResetSent)
                    {
                        hasTarget = false;
                        ResetSent = false;
                    }
                }
            }
        }

        public void TargetLost()
        {
            ResetSent = true;
        }

        void OnTargetDeath()
        {
            CurrentTarget.OnMyDeath -= OnTargetDeath;
            CurrentTarget = null;
            isOveride = false;
            FSM.SendEvent("Target Lost");
            hasTarget = false;
        }
    }
}
