using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class EffectReset : MonoBehaviour
    {
        public bool started = false;
        public Transform parent;

        private void OnDisable()
        {
            transform.SetParent(parent, false);
        }
    }
}
