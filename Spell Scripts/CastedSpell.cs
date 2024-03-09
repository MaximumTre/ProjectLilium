using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class CastedSpell : MonoBehaviour, IStartSpell
    {
        [SerializeField] bool debug, testCast;
        [SerializeField] EntityStatus CasterStatus;
        [SerializeField] Spell Spell;

        Animator animator;
        public Raycaster[] Raycasters;
        public GameObject[] Effects;

        public float delayTime, destroyTime = 10f;

        public List<GameObject> CurrentHitObjects;
        public List<string> DamageTags;
        public LayerMask DamageMask;

        public Damage SpellDamage, StatusDamage;
        public bool generateImpulse = false;

        Vector3 randomTimeOffset;

        private void Awake()
        {
            animator = GetComponent<Animator>();            
        }

        private void Start()
        {
            animator.speed = 0;
        }

        public void CastSpell(EntityStatus caster, Spell spell)
        {
            CurrentHitObjects.Clear();
            SpellDamage = spell.SpellDamage;
            SpellDamage.myEntityStats = caster;

            if(generateImpulse)
            {
                transform.SendMessage("GenerateImpulse");
            }

            switch (spell.SpellType)
            {
                case SpellType.ForwardDamage:
                    Destroy(gameObject, destroyTime);
                    foreach (GameObject go in Effects)
                    {
                        go.SetActive(true);
                    }

                    for (int i = 0; i < Raycasters.Length; i++)
                    {
                        RaycastHit[] hits = Raycasters[i].CastBeam(DamageMask);
                        for (int ii = 0; ii < hits.Length; ii++)
                        {
                            if (DamageTags.Contains(hits[ii].collider.tag))
                            {
                                if (!CurrentHitObjects.Contains(hits[ii].collider.gameObject))
                                {
                                    CurrentHitObjects.Add(hits[ii].collider.gameObject);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < CurrentHitObjects.Count; i++)
                    {
                        EntityStatus es = CurrentHitObjects[i].GetComponent<DamageReceiver>().MyStatus;
                        int result = CalculateDamage.instance.TakeDamage(SpellDamage, es);
                        es.TakeDamage(result, caster.transform.root);
                    }
                    break;
            }

            animator.speed = 1;
            Destroy(gameObject, destroyTime);
            testCast = false;
        }


        public void StartSpell(EntityStatus caster, Spell spell, List<string> DamageTags, LayerMask DamageMask)
        {
            this.DamageTags = DamageTags;
            this.DamageMask = DamageMask;
            CurrentHitObjects.Clear();
            Spell = spell;
            SpellDamage = Spell.SpellDamage;
            SpellDamage.myEntityStats = caster;
            StatusDamage = Spell.SpellDamage;
            StatusDamage.myEntityStats = caster;
            CasterStatus = caster;            
        }

        private void Update()
        {
            if (debug)
            {
                if (testCast)
                {
                    CastSpell(CasterStatus, Spell);
                    testCast = false;
                }
            }
        }
    }
}
