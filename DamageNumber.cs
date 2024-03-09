using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace SolarFalcon
{
    public class DamageNumber : MonoBehaviour
    {
        public static DamageNumber instance;


        public List<GameObject> DamageNumberList;
        [SerializeField] GameObject DMGNumberPrefab;

        private void Start()
        {
            if(this != instance)
            {
                instance = this;
            }

            for (int i = 0; i < 100; i++)
            {
                GameObject n = Instantiate(DMGNumberPrefab, transform);
                //n.GetComponent<DamageNumberPrefab>().CachedParent = transform;
                DamageNumberList.Add(n);
                n.SetActive(false);
            }
        }

        public void GetDamageNumber(Vector3 position, int number, bool damage, bool endurance)
        {
            for (int i = 0; i < DamageNumberList.Count; i++)
            {
                if (!DamageNumberList[i].activeInHierarchy)
                {
                    Vector3 rand;
                    rand.x = Random.Range(-0.5f, 0.5f);
                    rand.y = Random.Range(-0.5f, 0.5f);
                    rand.z = Random.Range(-0.5f, 0.5f);
                    DamageNumberList[i].transform.position = position + rand;
                    DamageNumberList[i].SetActive(true);
                    DamageNumberList[i].GetComponent<DamageNumberPrefab>().SetNumber(number, damage, endurance);
                    break;
                }
            }
        }

       

        
    }
}
