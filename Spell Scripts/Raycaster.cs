using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class Raycaster : MonoBehaviour
    {
        [SerializeField] bool debug, testHit;
        [SerializeField] float dRadius, dLength;
        [SerializeField] LayerMask dMask;

        public float radius, length;
        public LayerMask mask;
        Ray Ray;

        public RaycastHit[] hits;

        public bool active = false;

        private void Start()
        {
            Ray.origin = transform.position;
            Ray.direction = transform.forward;
        }

        public RaycastHit[] CastBeam(float radius, float length, LayerMask mask)
        {
            Ray.origin = transform.position;
            Ray.direction = transform.forward;
            this.radius = radius;
            this.length = length;
            this.mask = mask;
            RaycastHit[] hits = Physics.SphereCastAll(Ray, this.radius, this.length, this.mask);

            return hits;
        }

        public RaycastHit[] CastBeam(LayerMask mask)
        {
            Ray.origin = transform.position;
            Ray.direction = transform.forward;
            RaycastHit[] hits = Physics.SphereCastAll(Ray, radius, length, mask);

            return hits;
        }

        private void Update()
        {
            if(debug)
            {
                if(testHit)
                {
                    CastBeam(dRadius, dLength, dMask);
                    testHit = false;
                }
            }
        }

        public void TestCast()
        {
            debug = true;
            CastBeam(dRadius, dLength, dMask);
        }

        private void OnDrawGizmos()
        {
            Ray.origin = transform.position;
            Ray.direction = transform.forward;

            if (!active)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawLine(Ray.origin, Ray.GetPoint(length));
            Gizmos.DrawWireSphere(Ray.origin, radius);
            Gizmos.DrawWireSphere(Ray.GetPoint(length), radius);
            Gizmos.DrawWireSphere(Ray.GetPoint(length * 0.5f), radius);
        }
    }
}
