using FortBlast.Extras;
using UnityEngine;

namespace FortBlast.Player.Shooter
{
    public class PlayerShooterAbsorbDamage : MonoBehaviour
    {
        [Header("Absorber System")]
        public GameObject absorber;
        public ParticleSystem absorberParticles;

        [Header("Shield Affecter")]
        public float minParticleCount;
        public float maxParticleCount;
        public float maxShieldSize;
        public float sizeIncreaseRate;

        [Header("Health Affecter")]
        public float minHealthDecreaseRate;
        public float minDamageAmount;

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
            _minSize = absorber.transform.localScale.x;
            _currentSize = _minSize;

            _emissionSystem = absorberParticles.emission;
            _shapeSystem = absorberParticles.shape;
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagManager.Player))
                return;

            _currentSize = _currentSize + sizeIncreaseRate <= maxShieldSize ?
                _currentSize + sizeIncreaseRate : maxShieldSize;
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
            {
                _emissionSystem.rateOverTime = ExtensionFunctions
                    .Map(_currentSize, _minSize, maxShieldSize, minParticleCount, maxParticleCount);
                _shapeSystem.radius = _currentSize;
            }
        }
    }
}