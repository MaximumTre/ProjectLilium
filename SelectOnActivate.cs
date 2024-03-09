using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolarFalcon
{
    public class SelectOnActivate : MonoBehaviour
    {
        public Selectable SelectMe;

        private void OnEnable()
        {
            SelectMe.Select();
        }
    }
}
