using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class Detatch : MonoBehaviour
    {
        public Transform cachedParent;

        private void OnEnable()
        {
            transform.SetParent(null);
        }

        public void Reattach()
        {
            transform.SetParent(cachedParent);
        }
    }
}
