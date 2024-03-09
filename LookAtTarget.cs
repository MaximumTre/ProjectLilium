using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class LookAtTarget : MonoBehaviour
    {
        public Transform target;

        // Update is called once per frame
        void FixedUpdate()
        {
            if(target != null)
            {
                transform.LookAt(target, Vector3.up);
            }
        }
    }
}
