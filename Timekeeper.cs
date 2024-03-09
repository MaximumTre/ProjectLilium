using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class Timekeeper : MonoBehaviour
    {
        public static Timekeeper instance;

        public float WorldTime = 1f;
        public float PlayerTime = 1f;

        private void Awake()
        {
            instance = this;
        }
    }
}
