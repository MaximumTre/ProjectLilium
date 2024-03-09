using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class EnemySpawner : MonoBehaviour
    {
        public List<EnemyPool> PoolList;

        List<EnemyPool> ActiveList;

        public Transform[] spawnPoints;

        public bool canSpawn = true;

        public Transform HiddenSpawnPoint;
        int currentList;

        private void Start()
        {
            ActiveList = new List<EnemyPool>();

            for (int i = 0; i < PoolList.Count; i++)
            {
                ActiveList.Add(Instantiate(PoolList[i], HiddenSpawnPoint.position, HiddenSpawnPoint.rotation));
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < PoolList.Count; i++)
            {
                ActiveList[i].EnemyPackDefeated -= CanSpawnAgain;
            }
        }

        public void Spawn()
        {
            if(canSpawn)
            {
                int randomPool = Random.Range(0, PoolList.Count);

                ActiveList[randomPool].SpawnEnemies(spawnPoints);
                ActiveList[randomPool].EnemyPackDefeated += CanSpawnAgain;
                currentList = randomPool;
                canSpawn = false;
            }
        }

        void CanSpawnAgain()
        {
            ActiveList[currentList].EnemyPackDefeated -= CanSpawnAgain;
            canSpawn = true;
        }
    }
}
