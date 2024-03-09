using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarFalcon
{
    public class Afterimage : MonoBehaviour
    {
        public Animator Target;
        Animator Animator;
        public bool on = false;
        public float activeTime = 1f;
        float currentTime, frameNumber;
        Transform cachedParent;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            cachedParent = transform.parent;
            Target = cachedParent.GetComponent<Animator>();
        }

        private void Update()
        {
            if(on)
            {
                currentTime += Time.deltaTime;
                if(currentTime >= activeTime)
                {
                    currentTime = 0f;
                    on = false;
                    transform.SetParent(cachedParent);
                    gameObject.SetActive(false);
                }
            }
        }

        public void Activate()
        {
            AnimatorClipInfo[] currentClipInfo = Target.GetCurrentAnimatorClipInfo(0);
            frameNumber = Target.GetCurrentAnimatorStateInfo(0).normalizedTime;
            Animator.Play(currentClipInfo[0].clip.name, 0, frameNumber);
            Animator.speed = 0;
            transform.SetParent(null);
            on = true;
        }

        void OnEnable()
        {
            Activate();
        }
    }
}
