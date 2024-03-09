using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class EffectPooler : MonoBehaviour
    {
        public static EffectPooler current;

        public GameObject PhysicalHit;
        public List<GameObject> PhysicalHitEffectList;
        public int PhysicalHitAmount = 20;

        private void Awake()
        {
            if(current == null) current = this;
        }

        void Start()
        {
            InitializePhysicalHits();
        }
        
        void InitializePhysicalHits()
        {
            for (int i = 0; i < PhysicalHitAmount; i++)
            {
                GameObject go = Instantiate(PhysicalHit, transform, false);
                PhysicalHitEffectList.Add(go);
                go.SetActive(false);
            }
        }

        public void PlayPhysicalHit(Vector3 worldSpace)
        {
            for (int i = 0; i < PhysicalHitEffectList.Count; i++)
            {
                if (!PhysicalHitEffectList[i].activeInHierarchy)
                {
                    PhysicalHitEffectList[i].transform.SetParent(null);
                    PhysicalHitEffectList[i].transform.position = worldSpace;
                    PhysicalHitEffectList[i].SetActive(true);
                    break;
                }
            }
        }
    }
}
