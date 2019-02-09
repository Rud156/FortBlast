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
        public delegate void HealthChanged();

        public HealthZero healthZero;
        public HealthChanged healthChanged;

        private float _currentHealthAmount;

        private void Start() => _currentHealthAmount = maxHealthAmount;

        private void OnTriggerEnter(Collider other)
        {
            var damageAmountSetter = other.GetComponent<DamageAmountSetter>();
            if (damageAmountSetter != null)
            {
                Destroy(other.gameObject);
                ReduceHealth(damageAmountSetter.damageAmount);
            }
        }

        public float GetCurrentHealth() => _currentHealthAmount;

        public void AddHealth(float healthAmount)
        {
            _currentHealthAmount =
                _currentHealthAmount + healthAmount > maxHealthAmount
                    ? maxHealthAmount
                    : _currentHealthAmount + healthAmount;

            healthChanged?.Invoke();
        }

        public void ReduceHealth(float healthAmount)
        {
            _currentHealthAmount -= healthAmount;
            healthChanged?.Invoke();

            CheckIfHealthZero();
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