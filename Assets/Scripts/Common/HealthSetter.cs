using UnityEngine;

namespace FortBlast.Common
{
    [RequireComponent(typeof(Collider))]
    public class HealthSetter : MonoBehaviour
    {
        public GameObject deathEffect;
        public bool destroyWithoutEffect;
        public Transform effectInstantiatePoint;
        public float maxHealthAmount;
        public bool useSkinnedMesh;

        public delegate void HealthZero();

        public HealthZero healthZero;

        private float _currentHealthAmount;


        /// <summary>
        ///     Start is called on the frame when a script is enabled just before
        ///     any of the Update methods is called the first time.
        /// </summary>
        private void Start() => _currentHealthAmount = maxHealthAmount;

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update() => CheckIfHealthZero();

        /// <summary>
        ///     OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        private void OnTriggerEnter(Collider other)
        {
            var damageAmountSetter = other.GetComponent<DamageAmountSetter>();
            if (damageAmountSetter != null)
            {
                Destroy(other.gameObject);
                ReduceHealth(damageAmountSetter.damageAmount);
            }
        }

        public float GetCurrentHealth()
        {
            return _currentHealthAmount;
        }

        public void AddHealth(float healthAmount)
        {
            _currentHealthAmount =
                _currentHealthAmount + healthAmount > maxHealthAmount
                    ? maxHealthAmount
                    : _currentHealthAmount + healthAmount;
        }

        public void ReduceHealth(float healthAmount)
        {
            _currentHealthAmount -= healthAmount;
        }

        private void CheckIfHealthZero()
        {
            if (!(_currentHealthAmount <= 0))
                return;

            if (!destroyWithoutEffect)
            {
                if (useSkinnedMesh)
                    SpawnSkinnedMeshEffect();
                else
                    SpawnNormalEffect();
            }

            healthZero?.Invoke();
            Destroy(gameObject);
        }

        private void SpawnSkinnedMeshEffect()
        {
            var skinnedMesh = GetComponent<SkinnedMeshRenderer>();
            var particleEffect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            particleEffect.transform.position = effectInstantiatePoint.position;

            var shape = particleEffect.GetComponent<ParticleSystem>().shape;
            shape.skinnedMeshRenderer = skinnedMesh;
        }

        private void SpawnNormalEffect()
        {
            var particleEffect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            particleEffect.transform.position = effectInstantiatePoint.position;
        }
    }
}