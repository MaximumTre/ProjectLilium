
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolarFalcon
{
    public class CameraAimRay : MonoBehaviour
    {
        public GameObject ReticleObject;
        public LayerMask layerMask;
        public float maxDistance = 500, defaultDistance = 30, radius = 0.25f;
        public Camera mainCamera;
        Vector3 initialScreenRetPos;
        public List<string> DamageTags;
        public bool on = false, useNormal = false;

        private void Start()
        {
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if(on)
            {
                if (!ReticleObject.activeInHierarchy)
                {
                    ReticleObject.SetActive(true);
                }

                Ray ray = new Ray();
                ray.origin = transform.position;
                ray.direction = transform.forward;

                RaycastHit hit;

                if (Physics.SphereCast(ray, radius, out hit, maxDistance, layerMask))
                {
                    if (DamageTags.Contains(hit.collider.tag))
                    {
                        ReticleObject.transform.position = hit.point - (Vector3.forward * 0.25f);

                        if(useNormal)
                        {
                            Quaternion targetRotation = Quaternion.FromToRotation(ReticleObject.transform.up, hit.normal) * ReticleObject.transform.rotation;
                            ReticleObject.transform.rotation = targetRotation;
                        }
                    }

                    else
                    {
                        ReticleObject.transform.position = ray.GetPoint(defaultDistance);

                        if (useNormal)
                        {
                            Quaternion targetRotation = Quaternion.FromToRotation(ReticleObject.transform.up, hit.normal) * ReticleObject.transform.rotation;
                            ReticleObject.transform.rotation = targetRotation;
                        }
                    }
                }

                else
                {
                    ReticleObject.transform.position = ray.GetPoint(defaultDistance);

                    if (useNormal)
                    {
                        Quaternion targetRotation = Quaternion.FromToRotation(ReticleObject.transform.up, hit.normal) * ReticleObject.transform.rotation;
                        ReticleObject.transform.rotation = targetRotation;
                    }
                }
            }

            if(!on)
            {
                if(ReticleObject.activeInHierarchy)
                {
                    ReticleObject.SetActive(false);
                }
            }
        }
    }
}
