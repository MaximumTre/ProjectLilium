
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolarFalcon
{
    public class HealthBarShrink : MonoBehaviour
    {

        private const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = .6f;

        public Image lifebarImage, enduranceBarImage;
        public Image damagedLifebarImage, damagedEnduranceBarImage;
        private float damagedHealthShrinkTimer = 0, damagedEnduranceShrinkTimer = 0;
        public EntityStatus EntityStatus;

        private void Awake()
        {
        }

        private void Start()
        {
            PlayerStatus.Instance.OnStatusUpdate += UpdateStats;
        }

        void UpdateStats()
        {
            SetHealth(EntityStatus.GetCurrentLifePointsNormalized());
            damagedLifebarImage.fillAmount = lifebarImage.fillAmount;

            SetEndurance(EntityStatus.GetCurrentEndurancePointsNormalized());
            damagedEnduranceBarImage.fillAmount = enduranceBarImage.fillAmount;

            EntityStatus.OnDamaged += HealthSystem_OnDamaged;
            EntityStatus.OnHealed += HealthSystem_OnHealed;

            EntityStatus.OnEnduranceUsed += EnduranceSystem_OnDamaged;
            EntityStatus.OnEnduranceGained += EnduranceSystem_OnHealed;
        }

        private void Update()
        {
            SetHealth(EntityStatus.GetCurrentLifePointsNormalized());
            SetEndurance(EntityStatus.GetCurrentEndurancePointsNormalized());

            damagedHealthShrinkTimer -= Time.deltaTime;
            if (damagedHealthShrinkTimer < 0)
            {
                if (lifebarImage.fillAmount < damagedLifebarImage.fillAmount)
                {
                    float shrinkSpeed = 1f;
                    damagedLifebarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
                }
            }

            damagedEnduranceShrinkTimer -= Time.deltaTime;
            if (damagedEnduranceShrinkTimer < 0)
            {
                if (enduranceBarImage.fillAmount < damagedEnduranceBarImage.fillAmount)
                {
                    float shrinkSpeed = 1f;
                    damagedEnduranceBarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
                }
            }
        }

        private void HealthSystem_OnHealed(Transform sender, int amount)
        {
            SetHealth(EntityStatus.GetCurrentLifePointsNormalized());
            damagedLifebarImage.fillAmount = lifebarImage.fillAmount;
        }

        private void HealthSystem_OnDamaged(Transform sender, int amount)
        {
            damagedHealthShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
            SetHealth(EntityStatus.GetCurrentLifePointsNormalized());
        }

        private void EnduranceSystem_OnHealed(Transform sender, int amount)
        {
            SetEndurance(EntityStatus.GetCurrentEndurancePointsNormalized());
            damagedEnduranceBarImage.fillAmount = enduranceBarImage.fillAmount;
        }

        private void EnduranceSystem_OnDamaged(Transform sender, int amount)
        {
            damagedEnduranceShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
            SetEndurance(EntityStatus.GetCurrentEndurancePointsNormalized());
        }

        private void SetHealth(float healthNormalized)
        {
            lifebarImage.fillAmount = healthNormalized;
        }

        private void SetEndurance(float endNormalized)
        {
            enduranceBarImage.fillAmount = endNormalized;
        }
    }
}
