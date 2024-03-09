using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class EndurancePickupScript : MonoBehaviour
    {
        bool goToPlayer = false;
        public int enduranceAmount;
        public Transform target;
        public float rotationSpeed = 5f;
        public float collapseSpeed = 2f;

        private Rigidbody rb;
        private Vector3 initialPosition;

        public GameObject hitEffect;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            initialPosition = transform.position;
            target = PlayerStatus.Instance.PickupPoint;
        }

        private void FixedUpdate()
        {
            if (!goToPlayer)
                return;

            // Rotate around the target position
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);

            // Calculate the distance to the target
            float distance = Vector3.Distance(transform.position, target.position);

            // Calculate the collapse speed based on the distance
            float currentCollapseSpeed = collapseSpeed * Time.deltaTime;

            // Move towards the target with the collapse speed
            transform.position = Vector3.MoveTowards(transform.position, target.position, currentCollapseSpeed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.CompareTag("Environment"))
            {
                goToPlayer = true;
                rb.useGravity = false;
            }

            if(collision.collider.CompareTag("Player"))
            {
                collision.collider.GetComponent<PlayerStatus>().EnduranceOrb(enduranceAmount);
                goToPlayer = false;
                Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
