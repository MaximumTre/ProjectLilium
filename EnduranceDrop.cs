using System.Collections;
using UnityEngine;

namespace SolarFalcon
{
    public class EnduranceDrop : MonoBehaviour
    {
        public float SmallDropChance, MedDropChance, LargeDropChance;
        public GameObject SmallPrefab, MedPrefab, LargePrefab;

        EntityStatus EntityStatus;
        public float explosiveForceOnDrop = 100f;
        public Transform point;

        private void Awake()
        {
            EntityStatus = GetComponent<EntityStatus>();
        }

        private void OnEnable()
        {
            EntityStatus.OnDamaged += OnDamaged;
            EntityStatus.OnMyDeath += OnDeath;
        }

        private void OnDisable()
        {
            EntityStatus.OnDamaged -= OnDamaged;
            EntityStatus.OnMyDeath -= OnDeath;
        }

        void OnDamaged(Transform entity, int amount)
        {
            if (Random.Range(0f, 1f) <= SmallDropChance)
            {
                GameObject s = Instantiate(SmallPrefab, point.position, Quaternion.identity);
                s.GetComponent<Rigidbody>().AddForce(Vector3.up * explosiveForceOnDrop);
            }

            if (Random.Range(0f, 1f) <= MedDropChance)
            {
                GameObject s = Instantiate(MedPrefab, point.position, Quaternion.identity);
                s.GetComponent<Rigidbody>().AddForce(Vector3.up * explosiveForceOnDrop);
            }
        }

        void OnDeath()
        {
            if (Random.Range(0f, 1f) <= MedDropChance)
            {
                GameObject s = Instantiate(MedPrefab, transform.position, Quaternion.identity);
                s.GetComponent<Rigidbody>().AddForce(Vector3.up * explosiveForceOnDrop);
            }

            if (Random.Range(0f, 1f) <= LargeDropChance)
            {
                GameObject s = Instantiate(LargePrefab, transform.position, Quaternion.identity);
                s.GetComponent<Rigidbody>().AddForce(Vector3.up * explosiveForceOnDrop);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point.position, 0.1f);
        }
    }
}