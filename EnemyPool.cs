using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] bool debug = false;

        public List<Enemy> EnemyList;
        public bool spawned = false, allDown = false;
        public float PoolCooldownTime = 10f; 

        [SerializeField] int EnemiesDownCount = 0;

        public Action EnemyPackDefeated;


        private void Start()
        {
            for (int i = 0; i < EnemyList.Count; i++)
            {
                EnemyList[i].EnemyDefeated += OnEnemyDefeated;
                EnemyList[i].gameObject.SetActive(false);
            }
        }

        void OnEnemyDefeated()
        {
            if(spawned)
            {
                if(!allDown)
                {
                    if (debug)
                    {
                        Debug.Log("Enemy defeated.");
                    }

                    EnemiesDownCount++;

                    if (EnemiesDownCount == EnemyList.Count)
                    {   
                        Invoke("Reset", PoolCooldownTime);
                        allDown = true;
                        spawned = false;
                        if (debug)
                        {
                            Debug.Log("All enemies defeated.");
                        }
                    }
                }
            }
        }

        public void SpawnEnemies(Transform[] points)
        {
            if(!spawned)
            {
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    int randPoint = UnityEngine.Random.Range(0, points.Length);
                    //EnemyList[i].transform.SetParent(EnemyList[i].transform);
                    EnemyList[i].transform.position = points[randPoint].position;
                    EnemyList[i].transform.rotation = points[randPoint].rotation;
                    EnemyList[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                    EnemyList[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                    if(debug)
                    {
                        Debug.Log("Enemy List " + i + "  is spawning");
                    }

                    EnemyList[i].gameObject.SetActive(true);
                }

                EnemiesDownCount = 0;
                spawned = true;
            }
        }

        private void Reset()
        {
            allDown = false;
            spawned = false;

            if (EnemyPackDefeated != null) { EnemyPackDefeated.Invoke(); }
        }
    }
}
