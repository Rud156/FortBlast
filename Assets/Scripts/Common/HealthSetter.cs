using System;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Common
{
    public class HealthSetter : MonoBehaviour
    {
        public delegate void HealthZero();
        public HealthZero healthZero;

        public float maxHealthAmount;
        public GameObject deathEffect;
        public bool useSkinnedMesh;

        private float _currentHealthAmount;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _currentHealthAmount = maxHealthAmount;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() => CheckIfPlayerDead();

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            DamageAmountSetter damageAmountSetter = other.GetComponent<DamageAmountSetter>();
            if (damageAmountSetter != null)
                ReduceHealth(damageAmountSetter.damageAmount);
        }

        public float GetCurrentHealth() => _currentHealthAmount;

        public void AddHealth(float healthAmount) => _currentHealthAmount =
            _currentHealthAmount + healthAmount > maxHealthAmount ?
                maxHealthAmount : _currentHealthAmount + healthAmount;

        public void ReduceHealth(float healthAmount) => _currentHealthAmount -= healthAmount;

        private void CheckIfPlayerDead()
        {
            if (_currentHealthAmount <= 0)
            {
                if (useSkinnedMesh)
                    SpawnSkinnedMeshEffect();
                else
                    SpawnNormalEffect();

                healthZero?.Invoke();
                Destroy(gameObject);
            }
        }

        private void SpawnSkinnedMeshEffect()
        {
            SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
            GameObject particleEffect = Instantiate(deathEffect, transform.position, Quaternion.identity);

            ParticleSystem.ShapeModule shape = particleEffect.GetComponent<ParticleSystem>().shape;
            shape.skinnedMeshRenderer = skinnedMesh;
        }

        private void SpawnNormalEffect() =>
            Instantiate(deathEffect, transform.position, Quaternion.identity);
    }
}
