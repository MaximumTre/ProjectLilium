using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class MultiPointSpell : MonoBehaviour, IStartSpell
    {
        public GameObject[] SpellPrefabs;
        public List<PointList> PointList;
        public float delayTime, timeBetweenCasts, timeBetweenLists;

        public Damage SpellDamage, StatusDamage;
        Spell Spell;
        public List<string> DamageTags;
        public LayerMask DamageMask;
        EntityStatus Caster;

        bool start = false;
        void Update()
        {
            if(start)
            {
                delayTime -= Time.deltaTime;
                if(delayTime <= 0)
                {
                    StartCoroutine(Cast());
                    start = false;
                }
            }
        }

        IEnumerator Cast()
        {
            for(int i = 0; i < PointList.Count; i++)
            {
                for (int ii = 0; ii < PointList[i].Points.Count; ii++)
                {
                    foreach(GameObject obj in SpellPrefabs)
                    {
                        GameObject o = Instantiate(obj, PointList[i].Points[ii].transform.position, PointList[i].Points[ii].transform.rotation);
                        o.GetComponent<IStartSpell>().StartSpell(Caster, Spell, DamageTags, DamageMask);
                    }
                    yield return new WaitForSeconds(timeBetweenCasts);
                }

                yield return new WaitForSeconds(timeBetweenLists);
            }
        }

        public void StartSpell(EntityStatus caster, Spell spell, List<string> damageTags, LayerMask damageMask)
        {
            Caster = caster;
            Spell = spell;
            DamageTags = damageTags;
            DamageMask = damageMask;
            start = true;
        }
    }
}
