using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class Enemy : MonoBehaviour
    {
        public bool alive = true;

        public Action EnemyDefeated;

        public GameObject Model;


        public void Death()
        {
            if(EnemyDefeated != null) { EnemyDefeated.Invoke(); }

            // Change this to allow for death animations
            gameObject.SetActive(false);

            alive = false;
        }

        public void OnEnable()
        {
            alive = true;
        }
    }
}
