using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SolarFalcon
{
    public class AnimationDrivenAgent : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] bool debug;
        NavMeshAgent agent;
        Vector2 smoothDeltaPosition = Vector2.zero;
        Vector2 velocity = Vector2.zero;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
        }

        void Update()
        {
            if(agent.isStopped)
            {
                animator.SetBool("move", false);
            }

            else
            {
                Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

                // Map 'worldDeltaPosition' to local space
                float dx = Vector3.Dot(transform.right, worldDeltaPosition);
                float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
                Vector2 deltaPosition = new Vector2(dx, dy);

                // Low-pass filter the deltaMove
                float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
                smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

                // Update velocity if delta time is safe
                if (Time.deltaTime > 1e-5f)
                    velocity = smoothDeltaPosition / Time.deltaTime;

                bool shouldMove = velocity.magnitude > 0.5f;

                // Update animation parameters
                animator.SetBool("move", shouldMove);
                animator.SetFloat("HorizontalMovement", velocity.x);
                animator.SetFloat("ForwardMovement", velocity.y);

                //LookAt lookAt = GetComponent<LookAt>();
                //if (lookAt)
                //lookAt.lookAtTargetPosition = agent.steeringTarget + transform.forward;

                //		// Pull character towards agent
                //		if (worldDeltaPosition.magnitude > agent.radius)
                //			transform.position = agent.nextPosition - 0.9f*worldDeltaPosition;

                //		// Pull agent towards character
                //if (worldDeltaPosition.magnitude > agent.radius)
                    agent.nextPosition = transform.position + 0.9f * worldDeltaPosition;

                if(debug)
                    Debug.Log("Remaining distance" + agent.remainingDistance);
            }
        }

        void OnAnimatorMove()
        {
            // Update postion to agent position
            //		transform.position = agent.nextPosition;

            // Update position based on animation movement using navigation surface height
            Vector3 position = animator.rootPosition;
            position.y = agent.nextPosition.y;
            transform.position = position;
        }
    }
}
