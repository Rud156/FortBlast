using System;
using FortBlast.Player.Status_Setters;
using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Player.Shooter
{
    public class PlayerShooterAbsorbDamage : MonoBehaviour
    {
        [Header("Absorber System")]
        public GameObject absorber;
        public ParticleSystem absorberParticles;
        public Light absorberLight;

        [Header("Shield Affecter")]
        public float minParticleCount;
        public float maxParticleCount;
        public float minLightIntensity;
        public float maxLightIntensity;
        public float maxShieldSize;
        public float sizeIncreaseRate;

        [Header("Health Affecter")]
        public float maxHealthDecreaseRate;

        private PlayerHealthSetter _playerHealthSetter;
        private float _minSize;
        private float _currentSize;
        private ParticleSystem.EmissionModule _emissionSystem;
        private ParticleSystem.ShapeModule _shapeSystem;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerHealthSetter = GetComponent<PlayerHealthSetter>();

            _minSize = absorber.transform.localScale.x;
            _currentSize = _minSize;

            _emissionSystem = absorberParticles.emission;
            _shapeSystem = absorberParticles.shape;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            bool mousePressed = Input.GetMouseButton(0);

            if (!mousePressed)
                _currentSize = _minSize;
            else
                SetObjectsBasedOnSize();

            ChangeHealthBasedOnShieldSize();
        }

        public void DamagePlayerAndDecreaseHealth(float damageAmount)
        {
            _currentSize = _currentSize + sizeIncreaseRate <= maxShieldSize ?
                _currentSize + sizeIncreaseRate : maxShieldSize;
            _playerHealthSetter.TakeDamage(damageAmount);
        }

        private void ChangeHealthBasedOnShieldSize()
        {
            float damageAmount = ExtensionFunctions.Map(_currentSize, _minSize, maxShieldSize,
                0, maxHealthDecreaseRate);
            _playerHealthSetter.TakeDamage(damageAmount);
        }

        private void SetObjectsBasedOnSize()
        {
            _emissionSystem.rateOverTime = ExtensionFunctions
                            .Map(_currentSize, _minSize, maxShieldSize, minParticleCount, maxParticleCount);
            _shapeSystem.radius = _currentSize;
            absorberLight.intensity = ExtensionFunctions
                .Map(_currentSize, _minSize, maxShieldSize, minLightIntensity, maxLightIntensity);
        }
    }
}