using FortBlast.Common;
using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Enemy.Helpers
{
    [RequireComponent(typeof(HealthSetter))]
    public class DroidDamageDisplay : MonoBehaviour
    {
        private const float MaxSmokeParticles = 7;
        private const float MinSmokeParticles = 2;

        public Transform instancePoint;
        public GameObject smokeEffect;

        private HealthSetter _healthSetter;
        private float _maxHealth;
        private float _oneThirdHealth;

        private ParticleSystem.EmissionModule _smokeEmission;
        private bool _smokeInstantiated;

        private void Start()
        {
            _healthSetter = GetComponent<HealthSetter>();
            _maxHealth = _healthSetter.maxHealthAmount;

            _smokeInstantiated = false;
            _oneThirdHealth = _maxHealth * 0.75f;

            DisplayDamageEffect();
            _healthSetter.healthChanged += DisplayDamageEffect;
        }

        private void DisplayDamageEffect()
        {
            var currentHealth = _healthSetter.GetCurrentHealth();

            if (currentHealth <= _oneThirdHealth && !_smokeInstantiated)
            {
                var smokeInstance = Instantiate(smokeEffect, instancePoint.position,
                    smokeEffect.transform.rotation);
                smokeInstance.transform.SetParent(instancePoint);
                _smokeEmission = smokeInstance.GetComponent<ParticleSystem>().emission;
                _smokeInstantiated = true;
            }

            if (_smokeInstantiated)
            {
                var emissionRate = ExtensionFunctions.Map(currentHealth, 0, _oneThirdHealth,
                    MaxSmokeParticles, MinSmokeParticles);
                _smokeEmission.rateOverTime = emissionRate;
            }
        }
    }
}