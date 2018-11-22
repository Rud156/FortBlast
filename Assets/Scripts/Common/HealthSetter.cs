﻿using System;
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

        private float _currentHealthAmount;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() => _currentHealthAmount = maxHealthAmount;

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

        private void CheckIfPlayerDead()
        {
            if (_currentHealthAmount <= 0)
            {
                healthZero?.Invoke();
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        public float GetCurrentHealth() => _currentHealthAmount;

        public void AddHealth(float healthAmount) => _currentHealthAmount =
            _currentHealthAmount + healthAmount > maxHealthAmount ?
                maxHealthAmount : _currentHealthAmount + healthAmount;

        public void ReduceHealth(float healthAmount) => _currentHealthAmount -= healthAmount;
    }
}
