using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class AfterImageController : MonoBehaviour
    {
        public GameObject AfterimagePrefab;
        public List<GameObject> Afterimages;
        public int AfterimagesCount;

        public bool constant;

        private void Start()
        {
            Afterimages = new List<GameObject>();
            for (int i = 0; i < AfterimagesCount; i++)
            {
                Afterimages.Add(Instantiate(AfterimagePrefab, transform)); 
            }
        }

        private void Update()
        {
            if(constant)
            {
                for(int i = 0; i < Afterimages.Count; i++)
                {
                    if (!Afterimages[i].activeInHierarchy)
                    {
                        Afterimages[i].SetActive(true);
                        break;
                    }
                }
            }
        }
    }
}
