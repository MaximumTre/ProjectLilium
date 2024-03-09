using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class ShareActiveStatus : MonoBehaviour
    {
        public GameObject target;

        private void OnEnable()
        {
            if(target != null)
            {
                target.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (target != null)
            {
                target.SetActive(false);
            }
        }
    }
}
