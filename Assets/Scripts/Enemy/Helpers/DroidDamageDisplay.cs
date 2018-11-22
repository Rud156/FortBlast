using System.Collections;
using System.Collections.Generic;
using FortBlast.Common;
using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Enemy.Helpers
{
    [RequireComponent(typeof(HealthSetter))]
    public class DroidDamageDisplay : MonoBehaviour
    {
        public GameObject smokeEffect;
        public Transform instancePoint;

        private HealthSetter _healthSetter;
        private float _maxHealth;

        private bool smokeInstantiated;
        private float _oneThirdHealth;

        private ParticleSystem.EmissionModule _smokeEmission;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _healthSetter = GetComponent<HealthSetter>();
            _maxHealth = _healthSetter.maxHealthAmount;

            smokeInstantiated = false;
            _oneThirdHealth = _maxHealth * 0.75f;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            float currentHealth = _healthSetter.GetCurrentHealth();

            if (currentHealth <= _oneThirdHealth && !smokeInstantiated)
            {
                GameObject smokeInstance = Instantiate(smokeEffect, instancePoint.position,
                    smokeEffect.transform.rotation);
                smokeInstance.transform.SetParent(instancePoint);
                _smokeEmission = smokeInstance.GetComponent<ParticleSystem>().emission;
            }

            if (smokeInstantiated)
            {
                float emissionRate = ExtensionFunctions.Map(currentHealth, 0, _oneThirdHealth, 10, 2);
                _smokeEmission.rateOverTime = emissionRate;
            }
        }

    }
}