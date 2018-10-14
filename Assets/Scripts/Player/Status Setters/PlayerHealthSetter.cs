using System;
using UnityEngine;
using UnityEngine.UI;

namespace FortBlast.Player.Status_Setters
{
    public class PlayerHealthSetter : MonoBehaviour
    {
        [Header("Required Objects")]
        public GameObject deathEffect;
        public Transform mainCameraHolder;

        [Header("UI Display")]
        public Slider healthSlider;
        public Image healthFiller;

        [Header("Health Stats")]
        public float maxHealthAmount;
        public Color minHealthColor = Color.red;
        public Color halfHealthColor = Color.yellow;
        public Color maxHealthColor = Color.green;

        private float _currentHealthAmount;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _currentHealthAmount = maxHealthAmount;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            DisplayHealthToUI();
            CheckIfPlayerDead();
        }

        private void DisplayHealthToUI()
        {
            float healthRatio = _currentHealthAmount / maxHealthAmount;
            if (healthRatio <= 0.5f)
                healthFiller.color = Color.Lerp(minHealthColor, halfHealthColor, healthRatio * 2);
            else
                healthFiller.color = Color.Lerp(halfHealthColor, maxHealthColor, (healthRatio - 0.5f) * 2);

            healthSlider.value = healthRatio;
        }

        private void CheckIfPlayerDead()
        {
            if (_currentHealthAmount <= 0)
            {
                mainCameraHolder.SetParent(null);
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        public void TakeDamage(float damageAmount) => _currentHealthAmount -= damageAmount;

        public void AddHealth(float healthAmount) => _currentHealthAmount =
            _currentHealthAmount + healthAmount > maxHealthAmount ?
                maxHealthAmount : _currentHealthAmount + healthAmount;
    }
}
