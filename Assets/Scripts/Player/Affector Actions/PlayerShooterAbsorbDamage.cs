using System;
using FortBlast.Player.Status_Setters;
using FortBlast.Extras;
using UnityEngine;
using FortBlast.Player.Data;

namespace FortBlast.Player.Affecter_Actions
{
    [RequireComponent(typeof(PlayerHealthSetter))]
    [RequireComponent(typeof(Animator))]
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
        private Animator _playerAnimator;

        private float _minSize;
        private float _currentSize;

        private ParticleSystem.EmissionModule _emissionSystem;
        private ParticleSystem.ShapeModule _shapeSystem;

        private bool _absorberActive;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            _playerHealthSetter = GetComponent<PlayerHealthSetter>();
            _playerAnimator = GetComponent<Animator>();

            _minSize = absorber.transform.localScale.x;
            _currentSize = _minSize;

            _emissionSystem = absorberParticles.emission;
            _shapeSystem = absorberParticles.shape;

            _absorberActive = true;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            SetObjectsBasedOnSize();
            DecreaseHealthBasedOnShieldSize();

            DisplayAbsorberOnInput();
        }

        public void DamagePlayerAndDecreaseHealth(float damageAmount)
        {
            if (Input.GetMouseButton(0) && _absorberActive)
                _currentSize = _currentSize + sizeIncreaseRate <= maxShieldSize ?
                    _currentSize + sizeIncreaseRate : maxShieldSize;
            else
                _playerHealthSetter.TakeDamage(damageAmount);
        }

        public void ActivateAbsorber() => _absorberActive = true;
        public void DeActivateAbsorber() => _absorberActive = false;

        private void DisplayAbsorberOnInput()
        {
            bool mousePressed = Input.GetMouseButton(0);

            _playerAnimator.SetBool(PlayerData.PlayerShooting, mousePressed && _absorberActive);
            absorber.SetActive(mousePressed && _absorberActive);
        }

        private void DecreaseHealthBasedOnShieldSize()
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