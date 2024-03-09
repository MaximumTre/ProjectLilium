using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SolarFalcon
{
    public class DamageNumberPrefab : MonoBehaviour
    {
        public TextMeshProUGUI DamageNum, HealingNum, EnduranceHeal, EnduranceDmg;
        [ColorUsage(true, true)]
        public Color DmgStartColor, HealingStartColor, ENDHealStartColor, ENDDmgStartColor;
        Transform PlayerVirtualCamera;
        public Transform CachedParent;
        public float fadeTime, moveTime;
        private void OnEnable()
        {
            DamageNum.color = DmgStartColor;
            HealingNum.color = HealingStartColor;
            EnduranceDmg.color = ENDDmgStartColor;
            EnduranceHeal.color = ENDHealStartColor;

            transform.DOMoveY(transform.position.y + 1, moveTime);
            DamageNum.DOColor(Color.clear, fadeTime);
            HealingNum.DOColor(Color.clear, fadeTime);
            EnduranceHeal.DOColor(Color.clear, fadeTime);
            EnduranceDmg.DOColor(Color.clear, fadeTime);
            
            if(PlayerVirtualCamera == null)
            {
                PlayerVirtualCamera = GameObject.Find("Player Virtual Camera").transform;
            }
        }

        private void LateUpdate()
        {
            if (PlayerVirtualCamera != null)
            {
                transform.LookAt(transform.position + PlayerVirtualCamera.transform.rotation * Vector3.forward, PlayerVirtualCamera.transform.rotation * Vector3.up);
            }
        }

        public void SetNumber(int amount, bool damage, bool endurance)
        {
            if (damage && !endurance)
            {
                DamageNum.text = amount.ToString();
                DamageNum.gameObject.SetActive(true);
                return;
            }

            if (damage && endurance)
            {
                EnduranceDmg.text = amount.ToString();
                EnduranceDmg.gameObject.SetActive(true);
                return;
            }

            if (!damage && !endurance)
            {
                HealingNum.text = amount.ToString();
                HealingNum.gameObject.SetActive(true);
                return;
            }

            if (!damage && endurance)
            {
                EnduranceHeal.text = amount.ToString();
                EnduranceHeal.gameObject.SetActive(true);
                return;
            }
        }
    }
}
