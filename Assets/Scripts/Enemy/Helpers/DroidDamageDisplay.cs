using FortBlast.Common;
using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Enemy.Helpers
{
    [RequireComponent(typeof(HealthSetter))]
    public class DroidDamageDisplay : MonoBehaviour
    {
        private HealthSetter _healthSetter;
        private float _maxHealth;
        private readonly float _maxSmokeParticles = 7;

        private readonly float _minSmokeParticles = 2;
        private float _oneThirdHealth;

        private ParticleSystem.EmissionModule _smokeEmission;
        public Transform instancePoint;
        public GameObject smokeEffect;

        private bool smokeInstantiated;

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            _healthSetter = GetComponent<HealthSetter>();
            _maxHealth = _healthSetter.maxHealthAmount;

            smokeInstantiated = false;
            _oneThirdHealth = _maxHealth * 0.75f;
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            var currentHealth = _healthSetter.GetCurrentHealth();

            if (currentHealth <= _oneThirdHealth && !smokeInstantiated)
            {
                var smokeInstance = Instantiate(smokeEffect, instancePoint.position,
                    smokeEffect.transform.rotation);
                smokeInstance.transform.SetParent(instancePoint);
                _smokeEmission = smokeInstance.GetComponent<ParticleSystem>().emission;
                smokeInstantiated = true;
            }

            if (smokeInstantiated)
            {
                var emissionRate = ExtensionFunctions.Map(currentHealth, 0, _oneThirdHealth,
                    _maxSmokeParticles, _minSmokeParticles);
                _smokeEmission.rateOverTime = emissionRate;
            }
        }
    }
}